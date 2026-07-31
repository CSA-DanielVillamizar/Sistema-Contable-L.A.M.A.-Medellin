import axios, { type InternalAxiosRequestConfig } from 'axios';
import { getAccessToken } from '@/lib/msalClient';

const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL ?? 'http://localhost:5006';

const apiClient = axios.create({
    baseURL: apiBaseUrl,
    timeout: 30000,
    headers: {
        'Content-Type': 'application/json',
    },
});

// El token se pide a MSAL en cada request en vez de leerlo de localStorage.
// MSAL lo mantiene en cache y lo renueva solo cuando esta por expirar, asi que
// esto no genera trafico extra y elimina el token persistido en disco.
apiClient.interceptors.request.use(
    async (config) => {
        const accessToken = await getAccessToken();

        if (accessToken) {
            config.headers.Authorization = `Bearer ${accessToken}`;
        }

        return config;
    },
    (error) => Promise.reject(error),
);

export default apiClient;

/**
 * Cuerpo crudo de una respuesta del API, antes de normalizarlo a un tipo del
 * dominio del frontend.
 *
 * `unknown` es el tipo honesto para estos campos: vienen de la red y todavia no
 * estan validados. Los consumidores los pasan por String/Number/Boolean con un
 * valor por defecto, que es justamente lo que convierte un `unknown` en un dato
 * utilizable. Usar `any` aqui apagaba el chequeo de tipos sin ganar nada.
 */
export type RespuestaApi = Record<string, unknown>;

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

/**
 * Texto para un error que llego sin cuerpo.
 *
 * La tuberia de autorizacion de ASP.NET responde 401 y 403 vacios, de modo que
 * no hay `detail` que mostrar. Antes se caia a "HTTP 403", que no le dice nada
 * a quien lo lee.
 */
function mensajePorEstado(status: number): string {
    switch (status) {
        case 401:
            return 'Tu sesión expiró. Vuelve a iniciar sesión.';
        case 403:
            return 'No tienes permiso para esta operación. Si crees que deberías tenerlo, pídeselo a un administrador.';
        case 404:
            return 'No encontramos lo que buscabas.';
        case 408:
            return 'La operación tardó demasiado. Intenta de nuevo.';
        case 502:
        case 503:
        case 504:
            return 'El servicio no está disponible en este momento. Intenta en unos minutos.';
        default:
            return status >= 500
                ? 'Ocurrió un error en el servidor. Si se repite, avísale al equipo.'
                : 'No fue posible completar la operación.';
    }
}

/**
 * Mensaje presentable de un error salido de una llamada al API.
 *
 * El interceptor de abajo convierte toda respuesta de error en un `ApiError`,
 * asi que para cuando el error llega a un hook ya NO es un error de axios.
 * Veinte hooks comprobaban `axios.isAxiosError(error)` y, al dar siempre false,
 * caian en su mensaje generico: el `detail` del backend —que es donde viaja la
 * regla de negocio incumplida— nunca llegaba a la pantalla. Un 422 se veia como
 * "No fue posible registrar el comprobante", sin decir por que.
 *
 * Se conserva la rama de axios por si algun consumidor usa una instancia sin
 * este interceptor.
 */
export function mensajeDeError(error: unknown, respaldo: string): string {
    if (error instanceof ApiError) {
        const primerErrorDeValidacion = Object.values(error.validationErrors).flat()[0];
        return primerErrorDeValidacion ?? error.message ?? respaldo;
    }

    if (axios.isAxiosError<ProblemDetails>(error)) {
        const problem = error.response?.data;
        const primerErrorDeValidacion = problem?.errors
            ? Object.values(problem.errors).flat()[0]
            : undefined;

        return primerErrorDeValidacion ?? problem?.detail ?? problem?.title ?? respaldo;
    }

    return respaldo;
}

// ---------------------------------------------------------------------------
// Interceptor de RESPUESTA — normaliza errores ProblemDetails del backend
// ---------------------------------------------------------------------------
/** Marca interna para no reintentar en bucle una request que ya se reintento. */
type RequestConfigConReintento = InternalAxiosRequestConfig & { _reintentoAuth?: boolean };

apiClient.interceptors.response.use(
    (response) => response,
    async (error) => {
        if (!axios.isAxiosError(error) || !error.response) {
            // Error de red o timeout — relanzamos tal cual
            return Promise.reject(error);
        }

        const { status, data } = error.response;

        // Un 401 suele ser un token vencido. Se pide uno nuevo saltando la cache y
        // se reintenta la request una sola vez. Antes el usuario quedaba viendo
        // errores hasta que recargara la pagina a mano.
        const originalConfig = error.config as RequestConfigConReintento | undefined;

        if (status === 401 && originalConfig && !originalConfig._reintentoAuth) {
            originalConfig._reintentoAuth = true;

            const tokenRenovado = await getAccessToken({ forceRefresh: true });

            if (tokenRenovado) {
                originalConfig.headers.Authorization = `Bearer ${tokenRenovado}`;
                return apiClient(originalConfig);
            }
        }

        // El backend (.NET ProblemDetails) siempre envía un objeto con `title`
        // o `detail`, en camelCase.
        const isProblemDetails =
            typeof data === 'object' &&
            data !== null &&
            ('title' in data || 'detail' in data);

        if (isProblemDetails) {
            const normalized: ProblemDetails = {
                type: data.type,
                title: data.title,
                status: data.status ?? status,
                detail: data.detail,
                errors: data.errors,
            };

            return Promise.reject(new ApiError(normalized, status));
        }

        // Respuesta de error sin cuerpo. Pasa siempre con 401 y 403, que los
        // emite la tuberia de autorizacion antes de llegar al controlador, y
        // con los errores de la nube. Sin esto el usuario veia "HTTP 403".
        return Promise.reject(new ApiError({ title: mensajePorEstado(status) }, status));
    },
);
