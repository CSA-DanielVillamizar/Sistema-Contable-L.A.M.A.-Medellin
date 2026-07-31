'use client';

import { useGetResumenDashboard } from '@/features/dashboard/hooks/useGetResumenDashboard';
import EstadoDeError from '@/components/layout/EstadoDeError';
import { Banknote, CalendarDays, Users } from 'lucide-react';

function formatCOP(value: number): string {
    return new Intl.NumberFormat('es-CO', {
        style: 'currency',
        currency: 'COP',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
    }).format(value);
}

function formatFecha(value: string | null): string {
    if (!value) {
        return 'Sin fecha programada';
    }

    const parsed = new Date(value);

    if (Number.isNaN(parsed.getTime())) {
        return 'Fecha no disponible';
    }

    return parsed.toLocaleString('es-CO', {
        year: 'numeric',
        month: 'long',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
    });
}

export default function ResumenKpis() {
    const { data, isLoading, isError, error } = useGetResumenDashboard();

    if (isLoading) {
        return <p className="text-sm text-slate-600">Cargando KPIs ejecutivos...</p>;
    }

    if (isError) {
        return <EstadoDeError error={error} contexto="ver el resumen" />;
    }

    const resumen = data;

    return (
        <section className="grid grid-cols-1 gap-4 md:grid-cols-3">
            <article className="rounded-2xl border border-blue-200 bg-blue-50 p-5 shadow-sm">
                <div className="flex items-center gap-2 text-blue-700">
                    <Users size={24} className="text-blue-600" />
                    <p className="text-xs font-semibold uppercase tracking-wide">Miembros Activos</p>
                </div>
                <p className="mt-2 text-4xl font-bold text-blue-900">{resumen?.totalMiembrosActivos ?? 0}</p>
                <p className="mt-1 text-sm text-blue-700">Miembros vigentes del capítulo</p>
            </article>

            <article className="rounded-2xl border border-emerald-200 bg-emerald-50 p-5 shadow-sm">
                <div className="flex items-center gap-2 text-emerald-700">
                    <Banknote size={24} className="text-emerald-600" />
                    <p className="text-xs font-semibold uppercase tracking-wide">Total en CuentasBancarias</p>
                </div>
                <p className="mt-2 text-4xl font-bold text-emerald-900">{formatCOP(resumen?.totalDineroCuentasBancarias ?? 0)}</p>
                <p className="mt-1 text-sm text-emerald-700">Liquidez disponible en tesorería</p>
            </article>

            <article className="rounded-2xl border border-orange-200 bg-orange-50 p-5 shadow-sm">
                <div className="flex items-center gap-2 text-orange-700">
                    <CalendarDays size={24} className="text-orange-600" />
                    <p className="text-xs font-semibold uppercase tracking-wide">Próxima Rodada</p>
                </div>
                <p className="mt-2 text-lg font-bold text-orange-900">{resumen?.proximoEventoNombre ?? 'Sin evento programado'}</p>
                <p className="mt-1 text-sm text-orange-700">{formatFecha(resumen?.proximaFechaEvento ?? null)}</p>
            </article>
        </section>
    );
}
