import axios from 'axios';

const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL ?? 'http://localhost:5006';

const apiClient = axios.create({
    baseURL: apiBaseUrl,
    timeout: 30000,
    headers: {
        'Content-Type': 'application/json',
    },
});

apiClient.interceptors.request.use(
    (config) => {
        const accessToken = typeof window !== 'undefined' ? localStorage.getItem('token') : null;

        if (accessToken) {
            config.headers.Authorization = `Bearer ${accessToken}`;
        }

        return config;
    },
    (error) => Promise.reject(error),
);

export default apiClient;

// ---------------------------------------------------------------------------
// Tipos para ProblemDetails (RFC 7807) — el backend retorna este contrato
// ---------------------------------------------------------------------------
export interface ProblemDetails {
    type?: string;
    title?: string;
    status?: number;
    detail?: string;
    instance?: string;
    // FluentValidation agrega errores de validación en este campo
    errors?: Record<string, string[]>;
}

/** Error normalizado que lanzan todos los hooks de React Query. */
export class ApiError extends Error {
    public readonly status: number;
    public readonly title: string;
    public readonly validationErrors: Record<string, string[]>;

    constructor(problem: ProblemDetails, httpStatus: number) {
        // `detail` es el mensaje de negocio; `title` es el código HTTP genérico
        super(problem.detail ?? problem.title ?? `Error ${httpStatus}`);
        this.name = 'ApiError';
        this.status = httpStatus;
        this.title = problem.title ?? `HTTP ${httpStatus}`;
        this.validationErrors = problem.errors ?? {};
    }
}

// ---------------------------------------------------------------------------
// Interceptor de RESPUESTA — normaliza errores ProblemDetails del backend
// ---------------------------------------------------------------------------
apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        if (!axios.isAxiosError(error) || !error.response) {
            // Error de red o timeout — relanzamos tal cual
            return Promise.reject(error);
        }

        const { status, data } = error.response;

        // El backend (.NET ProblemDetails) siempre envía un objeto con `title`
        // o `detail`. Aceptamos tanto camelCase como PascalCase por defensividad.
        const isProblemDetails =
            typeof data === 'object' &&
            data !== null &&
            ('title' in data || 'Title' in data || 'detail' in data || 'Detail' in data);

        if (isProblemDetails) {
            // Normalizar camelCase y PascalCase en un solo objeto
            const normalized: ProblemDetails = {
                type: data.type ?? data.Type,
                title: data.title ?? data.Title,
                status: data.status ?? data.Status ?? status,
                detail: data.detail ?? data.Detail,
                errors: data.errors ?? data.Errors,
            };

            return Promise.reject(new ApiError(normalized, status));
        }

        // Respuesta de error sin cuerpo ProblemDetails (p.ej. 502 de la nube)
        return Promise.reject(new ApiError({ title: `HTTP ${status}` }, status));
    },
);
