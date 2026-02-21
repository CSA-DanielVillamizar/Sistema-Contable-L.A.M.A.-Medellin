'use client';

import { InteractionRequiredAuthError, PublicClientApplication } from '@azure/msal-browser';
import { MsalProvider, useMsal } from '@azure/msal-react';
import { useEffect } from 'react';

const tenantId = process.env.NEXT_PUBLIC_AZURE_AD_TENANT_ID ?? '95bb5dd0-a2fa-4336-9db4-fee9c5cbe8ae';
const clientId = process.env.NEXT_PUBLIC_AZURE_AD_CLIENT_ID ?? '3805c7ed-4245-4578-9ee1-85d48a2232fd';
const apiScope = process.env.NEXT_PUBLIC_API_SCOPE ?? 'api://b81ee2ee-5417-4aa0-8000-e470aec5543e/user_impersonation';

const msalInstance = new PublicClientApplication({
    auth: {
        clientId,
        authority: `https://login.microsoftonline.com/${tenantId}`,
        redirectUri: 'http://localhost:3000',
    },
    cache: {
        cacheLocation: 'localStorage',
        storeAuthStateInCookie: false,
    },
});

type AuthProviderProps = {
    children: React.ReactNode;
};

function TokenSync({ children }: AuthProviderProps) {
    const { instance, accounts, inProgress } = useMsal();

    useEffect(() => {
        if (inProgress !== 'none') {
            return;
        }

        const ensureToken = async () => {
            if (accounts.length === 0) {
                await instance.loginRedirect({ scopes: [apiScope] });
                return;
            }

            try {
                const tokenResponse = await instance.acquireTokenSilent({
                    account: accounts[0],
                    scopes: [apiScope],
                });

                localStorage.setItem('token', tokenResponse.accessToken);
            } catch (error) {
                if (error instanceof InteractionRequiredAuthError) {
                    await instance.acquireTokenRedirect({ scopes: [apiScope] });
                }
            }
        };

        void ensureToken();
    }, [accounts, inProgress, instance]);

    return <>{children}</>;
}

export default function AuthProvider({ children }: AuthProviderProps) {
    return (
        <MsalProvider instance={msalInstance}>
            <TokenSync>{children}</TokenSync>
        </MsalProvider>
    );
}
