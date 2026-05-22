'use client';

import apiClient from '@/lib/apiClient';
import { useQuery } from '@tanstack/react-query';
import { useEffect, useState } from 'react';

type SaldoBanco = {
    nombre: string;
    saldo: number | null;
};

type ResumenCartera = {
    totalPendienteCOP: number;
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

export default function Home() {
    const [authReady, setAuthReady] = useState(false);
    const [hasToken, setHasToken] = useState(false);
    const [authError, setAuthError] = useState<string | null>(null);

    useEffect(() => {
        const syncAuthState = () => {
            const token = localStorage.getItem('token');
            const authWasResolved = sessionStorage.getItem('auth_ready') === '1';
            const lastAuthError = sessionStorage.getItem('msal_auth_last_error');

            setHasToken(Boolean(token));
            setAuthReady(Boolean(token) || authWasResolved);
            setAuthError(lastAuthError);
        };

        syncAuthState();

        window.addEventListener('auth-token-updated', syncAuthState);
        window.addEventListener('auth-status-updated', syncAuthState);
        window.addEventListener('storage', syncAuthState);

        return () => {
            window.removeEventListener('auth-token-updated', syncAuthState);
            window.removeEventListener('auth-status-updated', syncAuthState);
            window.removeEventListener('storage', syncAuthState);
        };
    }, []);

    const bancosQuery = useQuery({
        queryKey: ['dashboard', 'bancos'],
        queryFn: async () => {
            const response = await apiClient.get<Record<string, unknown>[]>('/api/dashboard/bancos');

            return (response.data ?? []).map((item) => {
                const saldoRaw = item?.saldo ?? item?.Saldo;
                const saldoParsed = typeof saldoRaw === 'number' ? saldoRaw : Number(saldoRaw);

                return {
                    nombre: String(item?.nombre ?? item?.Nombre ?? ''),
                    saldo: Number.isFinite(saldoParsed) ? saldoParsed : null,
                } satisfies SaldoBanco;
            });
        },
        enabled: hasToken,
    });

    const carteraQuery = useQuery({
        queryKey: ['dashboard', 'cartera'],
        queryFn: async () => {
            const response = await apiClient.get<Record<string, unknown>>('/api/dashboard/cartera');

            return {
                totalPendienteCOP: toNumber(response.data?.totalPendienteCOP ?? response.data?.TotalPendienteCOP),
            } satisfies ResumenCartera;
        },
        enabled: hasToken,
    });

    const saldoTotalBancos = (bancosQuery.data ?? []).reduce((sum, banco) => sum + (banco.saldo ?? 0), 0);
    const tieneSaldosBancariosValidos = (bancosQuery.data ?? []).some((banco) => banco.saldo !== null);
    const cantidadSaldosInvalidos = (bancosQuery.data ?? []).filter((banco) => banco.saldo === null).length;
    const totalPendienteCartera = carteraQuery.data?.totalPendienteCOP ?? 0;

    if (!authReady) {
        return (
            <div className="px-6 py-10">
                <div className="rounded-xl border border-slate-200 bg-white p-6 text-slate-600">
                    Validando autenticación...
                </div>
            </div>
        );
    }

    if (!hasToken) {
        return (
            <div className="px-6 py-10">
                <div className="flex flex-col gap-4 rounded-xl border border-amber-200 bg-white p-6">
                    <h1 className="text-xl font-semibold text-slate-900">Sesión no autenticada</h1>
                    <p className="text-slate-600">
                        No fue posible completar el inicio de sesión automáticamente. Intenta recargar la página para reintentar la autenticación.
                    </p>
                    {authError ? (
                        <p className="rounded-md bg-amber-50 p-3 text-sm text-amber-900">Detalle técnico: {authError}</p>
                    ) : null}
                    <button
                        type="button"
                        onClick={() => window.dispatchEvent(new Event('auth-login-request'))}
                        className="w-fit rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white"
                    >
                        Iniciar sesión nuevamente
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="px-6 py-8">
            <div className="flex flex-col gap-6">
                <header>
                    <h1 className="text-2xl font-bold text-slate-900">Resumen financiero</h1>
                    <p className="mt-1 text-sm text-slate-500">Estado actual de bancos y cartera por cobrar</p>
                </header>

                {/* ── KPIs ── */}
                <section aria-labelledby="kpi-heading">
                    <h2 id="kpi-heading" className="sr-only">Indicadores clave</h2>
                    <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
                        <article className="rounded-xl border border-slate-200 bg-white p-6 lg:col-span-2">
                            <p className="text-sm font-medium text-slate-500">Saldo total en bancos</p>
                            <p className="mt-2 text-4xl font-bold text-slate-900">
                                {bancosQuery.isLoading || tieneSaldosBancariosValidos
                                    ? formatCOP(saldoTotalBancos)
                                    : 'Dato no disponible'}
                            </p>
                            <p className="mt-2 text-sm text-slate-400">
                                {bancosQuery.isLoading
                                    ? 'Cargando bancos...'
                                    : `${bancosQuery.data?.length ?? 0} banco(s) registrado(s)`}
                            </p>
                            {!bancosQuery.isLoading && cantidadSaldosInvalidos > 0 ? (
                                <p className="mt-0.5 text-xs text-slate-400">
                                    {cantidadSaldosInvalidos} banco(s) sin saldo válido no se incluyen en el total.
                                </p>
                            ) : null}
                        </article>

                        <article className="rounded-xl border border-slate-200 bg-white p-6">
                            <p className="text-sm font-medium text-slate-500">Cartera por cobrar</p>
                            <p className="mt-2 text-3xl font-bold text-slate-900">{formatCOP(totalPendienteCartera)}</p>
                            <p className="mt-2 text-sm text-slate-400">
                                {carteraQuery.isLoading ? 'Cargando cartera...' : 'Saldo pendiente de cobro'}
                            </p>
                        </article>
                    </div>
                </section>

                {/* ── Detalle de bancos ── */}
                {!bancosQuery.isLoading && (bancosQuery.data?.length ?? 0) > 0 ? (
                    <section aria-labelledby="bancos-heading">
                        <h2 id="bancos-heading" className="mb-3 text-sm font-semibold text-slate-700">Bancos</h2>
                        <div className="grid grid-cols-1 gap-3 sm:grid-cols-2 lg:grid-cols-3">
                            {(bancosQuery.data ?? []).map((banco) => (
                                <div
                                    key={banco.nombre}
                                    className="rounded-lg border border-slate-200 bg-white px-4 py-3"
                                >
                                    <p className="text-xs text-slate-500">{banco.nombre}</p>
                                    <p className="mt-1 text-lg font-semibold text-slate-900">
                                        {banco.saldo !== null ? formatCOP(banco.saldo) : 'Sin dato'}
                                    </p>
                                </div>
                            ))}
                        </div>
                    </section>
                ) : null}
            </div>
        </div>
    );
}

