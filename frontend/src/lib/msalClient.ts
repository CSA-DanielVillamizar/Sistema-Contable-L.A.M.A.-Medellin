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

// Tenant CIAM (Entra External ID): worldlama.onmicrosoft.com. Usa el dominio
// ciamlogin.com, no login.microsoftonline.com (ese es para tenants workforce).
const ciamSubdomain = process.env.NEXT_PUBLIC_AZURE_AD_CIAM_SUBDOMAIN ?? 'worldlama';
const tenantId = process.env.NEXT_PUBLIC_AZURE_AD_TENANT_ID ?? 'f372e858-1f5a-4ad8-8d3e-13a3926affb2';
const clientId = process.env.NEXT_PUBLIC_AZURE_AD_CLIENT_ID ?? '8e85ca27-48fa-4a35-84d6-1df1b4861606';

export const apiScope =
    process.env.NEXT_PUBLIC_API_SCOPE ?? 'api://ca154993-2258-4814-975e-edc583caa9b7/user_impersonation';

const fallbackRedirectUri = typeof window !== 'undefined' ? window.location.origin : 'http://localhost:3000';
export const redirectUri = process.env.NEXT_PUBLIC_AZURE_AD_REDIRECT_URI ?? fallbackRedirectUri;

export const msalInstance = new PublicClientApplication({
    auth: {
        clientId,
        authority: `https://${ciamSubdomain}.ciamlogin.com/${tenantId}`,
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
