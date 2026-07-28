import {
    InteractionRequiredAuthError,
    PublicClientApplication,
    type AccountInfo,
} from '@azure/msal-browser';

// ---------------------------------------------------------------------------
// Dueño unico del token de acceso.
//
// El access token NO se persiste en localStorage. Antes se guardaba en
// localStorage['token'] y se leia desde el cliente HTTP y varias paginas: eso
// deja el token legible para cualquier script inyectado en la pagina, y ademas
// sobrevive al cierre del navegador.
//
// Ahora MSAL es la unica fuente del token y lo entrega bajo demanda:
//  - su cache vive en sessionStorage (se limpia al cerrar la pestana)
//  - acquireTokenSilent renueva solo cuando hace falta
//  - nada guarda el token crudo en disco ni lo expone en `window`
// ---------------------------------------------------------------------------

const tenantId = process.env.NEXT_PUBLIC_AZURE_AD_TENANT_ID ?? '95bb5dd0-a2fa-4336-9db4-fee9c5cbe8ae';
const clientId = process.env.NEXT_PUBLIC_AZURE_AD_CLIENT_ID ?? '3805c7ed-4245-4578-9ee1-85d48a2232fd';

export const apiScope =
    process.env.NEXT_PUBLIC_API_SCOPE ?? 'api://b81ee2ee-5417-4aa0-8000-e470aec5543e/user_impersonation';

const fallbackRedirectUri = typeof window !== 'undefined' ? window.location.origin : 'http://localhost:3000';
export const redirectUri = process.env.NEXT_PUBLIC_AZURE_AD_REDIRECT_URI ?? fallbackRedirectUri;

export const msalInstance = new PublicClientApplication({
    auth: {
        clientId,
        authority: `https://login.microsoftonline.com/${tenantId}`,
        redirectUri,
    },
    cache: {
        // sessionStorage en vez de localStorage: la sesion no sobrevive al cierre
        // de la pestana y reduce la ventana de exposicion ante un XSS.
        cacheLocation: 'sessionStorage',
    },
});

let initializacionEnCurso: Promise<void> | null = null;

/** MSAL v5 exige initialize() antes de cualquier operacion. Idempotente. */
export function ensureMsalInitialized(): Promise<void> {
    initializacionEnCurso ??= msalInstance.initialize();
    return initializacionEnCurso;
}

function resolverCuenta(): AccountInfo | null {
    return msalInstance.getActiveAccount() ?? msalInstance.getAllAccounts()[0] ?? null;
}

/**
 * Devuelve un access token vigente, o null si hace falta interaccion del usuario.
 * `forceRefresh` omite la cache de MSAL: se usa al reintentar tras un 401.
 */
export async function getAccessToken(options?: { forceRefresh?: boolean }): Promise<string | null> {
    if (typeof window === 'undefined') {
        return null;
    }

    await ensureMsalInitialized();

    const account = resolverCuenta();
    if (!account) {
        return null;
    }

    try {
        const resultado = await msalInstance.acquireTokenSilent({
            account,
            scopes: [apiScope],
            forceRefresh: options?.forceRefresh ?? false,
        });

        return resultado.accessToken;
    } catch (error) {
        if (error instanceof InteractionRequiredAuthError) {
            // El llamador decide si dispara login interactivo; aqui no redirigimos.
            return null;
        }

        return null;
    }
}

/** True si hay sesion con token utilizable, sin exponer el token. */
export async function hasValidSession(): Promise<boolean> {
    return (await getAccessToken()) !== null;
}
