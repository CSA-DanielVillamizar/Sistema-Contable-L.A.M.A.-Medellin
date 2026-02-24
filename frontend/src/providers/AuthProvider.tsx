'use client';

import { InteractionRequiredAuthError, PublicClientApplication } from '@azure/msal-browser';
import { MsalProvider, useMsal } from '@azure/msal-react';
import { useEffect, useRef, useState } from 'react';

const tenantId = process.env.NEXT_PUBLIC_AZURE_AD_TENANT_ID ?? '95bb5dd0-a2fa-4336-9db4-fee9c5cbe8ae';
const clientId = process.env.NEXT_PUBLIC_AZURE_AD_CLIENT_ID ?? '3805c7ed-4245-4578-9ee1-85d48a2232fd';
const apiScope = process.env.NEXT_PUBLIC_API_SCOPE ?? 'api://b81ee2ee-5417-4aa0-8000-e470aec5543e/user_impersonation';
const fallbackRedirectUri = typeof window !== 'undefined' ? window.location.origin : 'http://localhost:3000';
const redirectUri = process.env.NEXT_PUBLIC_AZURE_AD_REDIRECT_URI ?? fallbackRedirectUri;
const redirectInFlightKey = 'msal_redirect_in_flight';
const authReadyKey = 'auth_ready';
const authRetryKey = 'msal_auth_auto_retry';
const authLastErrorKey = 'msal_auth_last_error';
const authManualLoginEvent = 'auth-login-request';

const msalInstance = new PublicClientApplication({
    auth: {
        clientId,
        authority: `https://login.microsoftonline.com/${tenantId}`,
        redirectUri,
    },
    cache: {
        cacheLocation: 'localStorage',
    },
});

type AuthProviderProps = {
    children: React.ReactNode;
};

function TokenSync({ children }: AuthProviderProps) {
    const { instance, accounts, inProgress } = useMsal();
    const [isAuthReady, setIsAuthReady] = useState(false);
    const hasTriggeredRedirectRef = useRef(false);

    useEffect(() => {
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
                setIsAuthReady(true);
            }
        };

        const resolveReady = () => {
            sessionStorage.removeItem(redirectInFlightKey);
            hasTriggeredRedirectRef.current = false;
            sessionStorage.setItem(authReadyKey, '1');
            sessionStorage.removeItem(authRetryKey);
            clearAuthError();
            setIsAuthReady(true);
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
                    localStorage.removeItem('token');
                    setIsAuthReady(true);
                    return;
                }

                if (sessionStorage.getItem(redirectInFlightKey) === '1' || isHandlingAuthCallback()) {
                    sessionStorage.setItem(authReadyKey, '1');
                    notifyTokenStateChanged();
                    setIsAuthReady(true);
                    return;
                }

                await startInteractiveLogin();
                return;
            }

            try {
                instance.setActiveAccount(accounts[0]);

                const tokenResponse = await instance.acquireTokenSilent({
                    account: accounts[0],
                    scopes: [apiScope],
                });

                localStorage.setItem('token', tokenResponse.accessToken);
                notifyTokenStateChanged();
                resolveReady();
            } catch (error) {
                if (error instanceof InteractionRequiredAuthError) {
                    if (sessionStorage.getItem(redirectInFlightKey) === '1' || isHandlingAuthCallback()) {
                        sessionStorage.setItem(authReadyKey, '1');
                        notifyTokenStateChanged();
                        setIsAuthReady(true);
                        return;
                    }

                    await beginRedirect(async () => {
                        await instance.acquireTokenRedirect({ scopes: [apiScope] });
                    });
                    return;
                }

                const errorMessage = error instanceof Error ? error.message : 'Error desconocido de autenticación';

                sessionStorage.removeItem(redirectInFlightKey);
                hasTriggeredRedirectRef.current = false;
                sessionStorage.setItem(authReadyKey, '1');
                localStorage.removeItem('token');
                setLastAuthError(errorMessage);
                notifyTokenStateChanged();
                setIsAuthReady(true);
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
    }, [accounts, inProgress, instance]);

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
