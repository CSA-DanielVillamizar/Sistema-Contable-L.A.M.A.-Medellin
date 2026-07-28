'use client';

import ResumenKpis from '@/features/dashboard/components/ResumenKpis';
import apiClient from '@/lib/apiClient';
import { hasValidSession } from '@/lib/msalClient';
import { useQuery } from '@tanstack/react-query';
import { useEffect, useState } from 'react';

// ---------------------------------------------------------------------------
// Tipos locales
// ---------------------------------------------------------------------------
type SaldoBanco = {
    nombre: string;
    saldo: number | null;
};

type ResumenCartera = {
    totalPendienteCOP: number;
};

type DashboardBancoDto = {
    saldo?: number | string | null;
    nombre?: string | null;
};

type DashboardCarteraDto = {
    totalPendienteCOP?: number | string | null;
};

function formatCOP(value: number): string {
    return new Intl.NumberFormat('es-CO', {
        style: 'currency',
        currency: 'COP',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
    }).format(value);
}

function toNumber(value: unknown): number {
    const parsed = typeof value === 'number' ? value : Number(value);
    return Number.isFinite(parsed) ? parsed : 0;
}

// ---------------------------------------------------------------------------
// Dashboard Ejecutivo — página principal del ERP
// ---------------------------------------------------------------------------
export default function Home() {
    const [authReady, setAuthReady] = useState(false);
    const [hasToken, setHasToken] = useState(false);
    const [authError, setAuthError] = useState<string | null>(null);

    useEffect(() => {
        const syncAuthState = async () => {
            // El token ya no vive en localStorage: se le pregunta a MSAL si hay
            // sesion utilizable, sin traer el token a este componente.
            const sesionValida = await hasValidSession();
            const authWasResolved = sessionStorage.getItem('auth_ready') === '1';
            const lastAuthError = sessionStorage.getItem('msal_auth_last_error');

            setHasToken(sesionValida);
            setAuthReady(sesionValida || authWasResolved);
            setAuthError(lastAuthError);
        };

        const onAuthStateChanged = () => void syncAuthState();

        onAuthStateChanged();

        // Se retiro el listener de 'storage': existia para detectar cambios del
        // token en localStorage, que ya no se persiste ahi.
        window.addEventListener('auth-token-updated', onAuthStateChanged);
        window.addEventListener('auth-status-updated', onAuthStateChanged);

        return () => {
            window.removeEventListener('auth-token-updated', onAuthStateChanged);
            window.removeEventListener('auth-status-updated', onAuthStateChanged);
        };
    }, []);

    const bancosQuery = useQuery({
        queryKey: ['dashboard', 'bancos'],
        queryFn: async () => {
            const response = await apiClient.get<DashboardBancoDto[]>('/api/dashboard/bancos');
            return (response.data ?? []).map((item) => {
                const saldoRaw = item?.saldo;
                const saldoParsed = typeof saldoRaw === 'number' ? saldoRaw : Number(saldoRaw);
                return {
                    nombre: String(item?.nombre ?? ''),
                    saldo: Number.isFinite(saldoParsed) ? saldoParsed : null,
                } satisfies SaldoBanco;
            });
        },
        enabled: hasToken,
    });

    const carteraQuery = useQuery({
        queryKey: ['dashboard', 'cartera'],
        queryFn: async () => {
            const response = await apiClient.get<DashboardCarteraDto>('/api/dashboard/cartera');
            return {
                totalPendienteCOP: toNumber(
                    response.data?.totalPendienteCOP,
                ),
            } satisfies ResumenCartera;
        },
        enabled: hasToken,
    });

    const saldoTotalBancos = (bancosQuery.data ?? []).reduce((sum, b) => sum + (b.saldo ?? 0), 0);
    const tieneSaldosValidos = (bancosQuery.data ?? []).some((b) => b.saldo !== null);
    const totalCartera = carteraQuery.data?.totalPendienteCOP ?? 0;

    // --- Estado: autenticando ---
    if (!authReady) {
        return (
            <div className="flex flex-1 items-center justify-center p-10">
                <div className="rounded-xl border border-slate-200 bg-white p-8 text-slate-500">
                    Validando autenticación...
                </div>
            </div>
        );
    }

    // --- Estado: sin sesión ---
    if (!hasToken) {
        return (
            <div className="flex flex-1 items-center justify-center p-10">
                <div className="flex w-full max-w-md flex-col gap-4 rounded-xl border border-amber-200 bg-white p-8">
                    <h2 className="text-lg font-semibold text-slate-900">Sesión no autenticada</h2>
                    <p className="text-sm text-slate-600">
                        No fue posible completar el inicio de sesión. Recarga la página para reintentar.
                    </p>
                    {authError ? (
                        <p className="rounded-md bg-amber-50 p-3 text-xs text-amber-900">
                            Detalle: {authError}
                        </p>
                    ) : null}
                    <button
                        type="button"
                        onClick={() => window.dispatchEvent(new Event('auth-login-request'))}
                        className="w-fit rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white"
                    >
                        Iniciar sesión
                    </button>
                </div>
            </div>
        );
    }

    // --- Dashboard Ejecutivo ---
    return (
        <div className="px-6 py-8">
            <div className="mx-auto flex max-w-6xl flex-col gap-8">

                {/* KPIs principales: miembros activos, total en cajas, próxima rodada */}
                <ResumenKpis />

                {/* Indicadores financieros */}
                <section>
                    <h2 className="mb-4 text-sm font-semibold uppercase tracking-wider text-slate-500">
                        Posición Financiera
                    </h2>
                    <div className="grid grid-cols-1 gap-5 lg:grid-cols-3">
                        <article className="rounded-xl border border-slate-200 bg-white p-6 lg:col-span-2">
                            <p className="text-xs font-medium uppercase tracking-wider text-slate-400">
                                Saldo total en bancos
                            </p>
                            <p className="mt-3 text-4xl font-bold text-slate-900">
                                {bancosQuery.isLoading
                                    ? '—'
                                    : tieneSaldosValidos
                                        ? formatCOP(saldoTotalBancos)
                                        : 'Sin datos'}
                            </p>
                            <p className="mt-2 text-sm text-slate-400">
                                {bancosQuery.isLoading
                                    ? 'Cargando...'
                                    : `${bancosQuery.data?.length ?? 0} cuenta(s) bancaria(s)`}
                            </p>
                        </article>

                        <article className="rounded-xl border border-slate-200 bg-white p-6">
                            <p className="text-xs font-medium uppercase tracking-wider text-slate-400">
                                Cartera por cobrar
                            </p>
                            <p className="mt-3 text-3xl font-bold text-slate-900">
                                {carteraQuery.isLoading ? '—' : formatCOP(totalCartera)}
                            </p>
                            <p className="mt-2 text-sm text-slate-400">
                                {carteraQuery.isLoading ? 'Cargando...' : 'Saldo pendiente de cobro'}
                            </p>
                        </article>
                    </div>
                </section>
            </div>
        </div>
    );
}
