import axios, { AxiosError } from 'axios';

const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL ?? 'http://localhost:5006';

// ─── ProblemDetails (RFC 7807) ───────────────────────────────────────────────
export type ProblemDetails = {
    type?: string;
    title?: string;
    status?: number;
    detail?: string;
    instance?: string;
    errors?: Record<string, string[]>;
};

/**
 * Error normalizado que encapsula un ProblemDetails del backend.
 * Los hooks de React Query pueden hacer `instanceof ApiError` para discriminar.
 */
export class ApiError extends Error {
    readonly status: number;
    readonly detail: string;
    readonly validationErrors: Record<string, string[]>;
    readonly raw: ProblemDetails;

    constructor(status: number, problem: ProblemDetails) {
        // Mensaje principal: primer error de validación → detail → title → fallback
        const firstValidation =
            problem.errors ? Object.values(problem.errors).flat()[0] : undefined;
        const message = firstValidation ?? problem.detail ?? problem.title ?? 'Error inesperado del servidor';

        super(message);
        this.name = 'ApiError';
        this.status = status;
        this.detail = problem.detail ?? message;
        this.validationErrors = problem.errors ?? {};
        this.raw = problem;
    }
}

// ─── Axios instance ──────────────────────────────────────────────────────────
const apiClient = axios.create({
    baseURL: apiBaseUrl,
    timeout: 30000,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Interceptor de REQUEST: adjunta el JWT de la sesión activa
apiClient.interceptors.request.use(
    (config) => {
        const accessToken = typeof window !== 'undefined' ? localStorage.getItem('token') : null;

        if (accessToken) {
            config.headers.Authorization = `Bearer ${accessToken}`;
        }

        return config;
    },
    (error: unknown) => Promise.reject(error),
);

// Interceptor de RESPONSE: normaliza errores ProblemDetails en ApiError
apiClient.interceptors.response.use(
    (response) => response,
    (error: unknown) => {
        if (!axios.isAxiosError(error)) {
            return Promise.reject(error);
        }

        const axiosError = error as AxiosError<ProblemDetails>;
        const status = axiosError.response?.status ?? 0;
        const data = axiosError.response?.data;

        // Solo normalizar respuestas de error del servidor con cuerpo ProblemDetails
        if (status >= 400 && data && (data.title !== undefined || data.detail !== undefined || data.errors !== undefined)) {
            return Promise.reject(new ApiError(status, data));
        }

        // Para errores de red, timeout u otros sin cuerpo ProblemDetails, dejar pasar el AxiosError original
        return Promise.reject(error);
    },
);

export default apiClient;
