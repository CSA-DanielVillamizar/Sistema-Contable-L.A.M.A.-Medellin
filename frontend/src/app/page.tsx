'use client';

import Link from 'next/link';
import { useQuery } from '@tanstack/react-query';
import apiClient from '@/lib/apiClient';

type SaldoBanco = {
    nombre: string;
    saldo: number;
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

export default function Home() {
    const bancosQuery = useQuery({
        queryKey: ['dashboard', 'bancos'],
        queryFn: async () => {
            const response = await apiClient.get<SaldoBanco[]>('/api/dashboard/bancos');
            return response.data;
        },
    });

    const carteraQuery = useQuery({
        queryKey: ['dashboard', 'cartera'],
        queryFn: async () => {
            const response = await apiClient.get<ResumenCartera>('/api/dashboard/cartera');
            return response.data;
        },
    });

    const saldoTotalBancos = (bancosQuery.data ?? []).reduce((sum, banco) => sum + banco.saldo, 0);
    const totalPendienteCartera = carteraQuery.data?.totalPendienteCOP ?? 0;

    return (
        <main className="min-h-screen bg-slate-50 px-6 py-10">
            <div className="mx-auto flex w-full max-w-6xl flex-col gap-6">
                <header>
                    <h1 className="text-3xl font-bold text-slate-900">Dashboard Financiero</h1>
                    <p className="mt-1 text-slate-600">Resumen de bancos y cartera por cobrar</p>
                </header>

                <section className="grid grid-cols-1 gap-6 lg:grid-cols-3">
                    <article className="rounded-xl border border-slate-200 bg-white p-6 lg:col-span-2">
                        <p className="text-sm text-slate-600">Saldo Total en Bancos</p>
                        <p className="mt-2 text-4xl font-bold text-slate-900">{formatCOP(saldoTotalBancos)}</p>
                        <p className="mt-2 text-sm text-slate-500">
                            {bancosQuery.isLoading ? 'Cargando bancos...' : `${bancosQuery.data?.length ?? 0} banco(s)`}
                        </p>
                    </article>

                    <article className="rounded-xl border border-slate-200 bg-white p-6">
                        <p className="text-sm text-slate-600">Total de Cartera por Cobrar</p>
                        <p className="mt-2 text-3xl font-bold text-slate-900">{formatCOP(totalPendienteCartera)}</p>
                        <p className="mt-2 text-sm text-slate-500">
                            {carteraQuery.isLoading ? 'Cargando cartera...' : 'Saldo pendiente en estado Pendiente'}
                        </p>
                    </article>
                </section>

                <section className="grid grid-cols-1 gap-4 md:grid-cols-3">
                    <Link
                        href="/transacciones/ingreso"
                        className="rounded-xl border border-slate-300 bg-white px-6 py-8 text-center text-lg font-semibold text-slate-800"
                    >
                        Registrar Ingreso
                    </Link>

                    <Link
                        href="/cartera/generar"
                        className="rounded-xl border border-slate-300 bg-white px-6 py-8 text-center text-lg font-semibold text-slate-800"
                    >
                        Generar Cartera
                    </Link>

                    <Link
                        href="/cartera/listado"
                        className="rounded-xl border border-slate-300 bg-white px-6 py-8 text-center text-lg font-semibold text-slate-800"
                    >
                        Ver Cartera Pendiente
                    </Link>
                </section>
            </div>
        </main>
    );
}
