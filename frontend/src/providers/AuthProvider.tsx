'use client';

import { InteractionRequiredAuthError } from '@azure/msal-browser';
import { MsalProvider, useMsal } from '@azure/msal-react';
import { usePathname } from 'next/navigation';
import { useEffect, useRef, useState } from 'react';
import { apiScope, msalInstance } from '@/lib/msalClient';
import apiClient from '@/lib/apiClient';
import type { AccountInfo } from '@azure/msal-browser';

/**
 * Crea el perfil interno del usuario si aun no existe.
 *
 * Sin perfil, la transformacion de claims no concede ningun rol y la matriz de
 * permisos responde 403 en toda la aplicacion. El endpoint es idempotente: si
 * el perfil ya esta, devuelve el existente sin tocarlo.
 *
 * Un fallo aqui no bloquea el arranque: el usuario vera los 403 de la API, que
 * es mas util que dejarlo mirando una pantalla en blanco sin explicacion.
 */
async function sincronizarPerfilInterno(cuenta: AccountInfo): Promise<void> {
    const entraObjectId = (cuenta.idTokenClaims as { oid?: string } | undefined)?.oid
        ?? cuenta.localAccountId;

    if (!entraObjectId) {
        return;
    }

    try {
        await apiClient.post('/api/usuarios/sync', {
            email: cuenta.username,
            entraObjectId,
            nombres: cuenta.name ?? cuenta.username,
        });
    } catch {
        // Silencioso a proposito: ver arriba.
    }
}

const redirectInFlightKey = 'msal_redirect_in_flight';
const authReadyKey = 'auth_ready';
const authRetryKey = 'msal_auth_auto_retry';
const authLastErrorKey = 'msal_auth_last_error';
const authManualLoginEvent = 'auth-login-request';

type AuthProviderProps = {
    children: React.ReactNode;
};

/**
 * Rutas que se sirven sin sesion.
 *
 * Hoy solo la verificacion de recibos: es a donde apunta el codigo QR, y quien
 * recibe un recibo impreso no tiene cuenta en el sistema. Forzarle un login
 * convertiria la verificacion en algo que solo los propios miembros pueden
 * hacer, que es justo lo contrario de para que sirve.
 */
const RUTAS_PUBLICAS = ['/verificar'];

function esRutaPublica(pathname: string | null): boolean {
    return Boolean(pathname) && RUTAS_PUBLICAS.some((r) => pathname!.startsWith(r));
}

function TokenSync({ children }: AuthProviderProps) {
    const { instance, accounts, inProgress } = useMsal();
    const [sesionResuelta, setSesionResuelta] = useState(false);
    const hasTriggeredRedirectRef = useRef(false);
    const pathname = usePathname();
    const rutaPublica = esRutaPublica(pathname);

    // Una ruta publica no espera a MSAL: no hay sesion que resolver.
    const isAuthReady = rutaPublica || sesionResuelta;

    useEffect(() => {
        if (rutaPublica) {
            return;
        }

        if (inProgress !== 'none') {
            return;
        }

        const notifyTokenStateChanged = () => {
            if (typeof window !== 'undefined') {
                window.dispatchEvent(new Event('auth-token-updated'));
                window.dispatchEvent(new Event('auth-status-updated'));
            }
        };

        const setLastAuthError = (message: string) => {
            sessionStorage.setItem(authLastErrorKey, message);
            notifyTokenStateChanged();
        };

        const clearAuthError = () => {
            sessionStorage.removeItem(authLastErrorKey);
            notifyTokenStateChanged();
        };

        const isHandlingAuthCallback = () => {
            if (typeof window === 'undefined') {
                return false;
            }

            const search = window.location.search;
            const hash = window.location.hash;
            return search.includes('code=') || hash.includes('code=') || search.includes('error=') || hash.includes('error=');
        };

        const getCallbackError = () => {
            if (typeof window === 'undefined') {
                return null;
            }

            const searchParams = new URLSearchParams(window.location.search);
            const hashSource = window.location.hash.startsWith('#') ? window.location.hash.slice(1) : window.location.hash;
            const hashParams = new URLSearchParams(hashSource);

            const error = searchParams.get('error') ?? hashParams.get('error');
            const description = searchParams.get('error_description') ?? hashParams.get('error_description');

            if (!error) {
                return null;
            }

            return `${error}${description ? `: ${decodeURIComponent(description)}` : ''}`;
        };

        const cleanAuthCallbackParams = () => {
            if (typeof window === 'undefined') {
                return;
            }

            const cleanUrl = `${window.location.origin}${window.location.pathname}`;
            window.history.replaceState({}, document.title, cleanUrl);
        };

        const beginRedirect = async (redirectAction: () => Promise<void>) => {
            if (hasTriggeredRedirectRef.current || sessionStorage.getItem(redirectInFlightKey) === '1') {
                return;
            }

            if (isHandlingAuthCallback()) {
                return;
            }

            hasTriggeredRedirectRef.current = true;
            sessionStorage.setItem(redirectInFlightKey, '1');
            sessionStorage.removeItem(authReadyKey);
            await redirectAction();
        };

        const startInteractiveLogin = async () => {
            try {
                await beginRedirect(async () => {
                    await instance.loginRedirect({ scopes: [apiScope] });
                });
            } catch (error) {
                const errorMessage = error instanceof Error ? error.message : 'No fue posible iniciar loginRedirect';
                sessionStorage.removeItem(redirectInFlightKey);
                hasTriggeredRedirectRef.current = false;
                sessionStorage.setItem(authReadyKey, '1');
                setLastAuthError(errorMessage);
                setSesionResuelta(true);
            }
        };

        const isTimeoutAuthError = (error: unknown) => {
            const message = error instanceof Error ? error.message.toLowerCase() : '';
            return message.includes('timed_out') || message.includes('monitor_window_timeout') || message.includes('token renewal operation failed');
        };

        const resolveReady = () => {
            sessionStorage.removeItem(redirectInFlightKey);
            hasTriggeredRedirectRef.current = false;
            sessionStorage.setItem(authReadyKey, '1');
            sessionStorage.removeItem(authRetryKey);
            clearAuthError();
            setSesionResuelta(true);
        };

        const ensureToken = async () => {
            if (accounts.length === 0) {
                const callbackError = getCallbackError();

                if (callbackError) {
                    const retries = Number(sessionStorage.getItem(authRetryKey) ?? '0');

                    if (retries < 1) {
                        sessionStorage.setItem(authRetryKey, String(retries + 1));
                        sessionStorage.removeItem(redirectInFlightKey);
                        hasTriggeredRedirectRef.current = false;
                        cleanAuthCallbackParams();

                        await startInteractiveLogin();
                        return;
                    }

                    sessionStorage.removeItem(redirectInFlightKey);
                    hasTriggeredRedirectRef.current = false;
                    sessionStorage.setItem(authReadyKey, '1');
                    setLastAuthError(callbackError);
                    setSesionResuelta(true);
                    return;
                }

                if (sessionStorage.getItem(redirectInFlightKey) === '1' || isHandlingAuthCallback()) {
                    sessionStorage.setItem(authReadyKey, '1');
                    notifyTokenStateChanged();
                    setSesionResuelta(true);
                    return;
                }

                await startInteractiveLogin();
                return;
            }

            try {
                instance.setActiveAccount(accounts[0]);

                // Se pide el token para confirmar que la sesion sirve, pero NO se
                // persiste: a partir de aqui MSAL es el unico que lo custodia y el
                // cliente HTTP se lo pide cuando lo necesita.
                await instance.acquireTokenSilent({
                    account: accounts[0],
                    scopes: [apiScope],
                });

                notifyTokenStateChanged();

                // El perfil interno es lo que da rol al usuario, y sin rol la
                // API responde 403 en todas las pantallas. Se sincroniza aqui
                // porque es el unico momento en que se sabe que la sesion
                // sirve y quien es el usuario.
                await sincronizarPerfilInterno(accounts[0]);

                resolveReady();
            } catch (error) {
                if (error instanceof InteractionRequiredAuthError) {
                    if (sessionStorage.getItem(redirectInFlightKey) === '1' || isHandlingAuthCallback()) {
                        sessionStorage.setItem(authReadyKey, '1');
                        notifyTokenStateChanged();
                        setSesionResuelta(true);
                        return;
                    }

                    await beginRedirect(async () => {
                        await instance.acquireTokenRedirect({ scopes: [apiScope] });
                    });
                    return;
                }

                if (isTimeoutAuthError(error)) {
                    const retries = Number(sessionStorage.getItem(authRetryKey) ?? '0');

                    if (retries < 1) {
                        sessionStorage.setItem(authRetryKey, String(retries + 1));
                        sessionStorage.removeItem(redirectInFlightKey);
                        hasTriggeredRedirectRef.current = false;

                        await beginRedirect(async () => {
                            await instance.acquireTokenRedirect({ scopes: [apiScope] });
                        });
                        return;
                    }
                }

                const errorMessage = error instanceof Error ? error.message : 'Error desconocido de autenticación';

                sessionStorage.removeItem(redirectInFlightKey);
                hasTriggeredRedirectRef.current = false;
                sessionStorage.setItem(authReadyKey, '1');
                setLastAuthError(errorMessage);
                notifyTokenStateChanged();
                setSesionResuelta(true);
            }
        };

        void ensureToken();

        const onManualLoginRequest = () => {
            sessionStorage.removeItem(authReadyKey);
            sessionStorage.removeItem(authRetryKey);
            clearAuthError();
            void startInteractiveLogin();
        };

        window.addEventListener(authManualLoginEvent, onManualLoginRequest);

        return () => {
            window.removeEventListener(authManualLoginEvent, onManualLoginRequest);
        };
    }, [accounts, inProgress, instance, rutaPublica]);

    if (!isAuthReady) {
        return <div className="p-6 text-sm text-slate-600">Autenticando sesión...</div>;
    }

    return <>{children}</>;
}

export default function AuthProvider({ children }: AuthProviderProps) {
    return (
        <MsalProvider instance={msalInstance}>
            <TokenSync>{children}</TokenSync>
        </MsalProvider>
    );
}
