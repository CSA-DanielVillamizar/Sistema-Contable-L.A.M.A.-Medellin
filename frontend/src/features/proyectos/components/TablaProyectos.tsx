'use client';

import { useProyectos } from '@/features/proyectos/hooks/useProyectos';

function formatCOP(value: number): string {
    return new Intl.NumberFormat('es-CO', {
        style: 'currency',
        currency: 'COP',
        maximumFractionDigits: 0,
    }).format(value);
}

function formatFecha(value?: string | null): string {
    if (!value) {
        return '—';
    }

    const parsed = new Date(value);
    return Number.isNaN(parsed.getTime()) ? value : parsed.toLocaleDateString('es-CO');
}

export default function TablaProyectos() {
    const { data, isLoading, isError, error } = useProyectos();

    if (isLoading) {
        return <p className="text-sm text-slate-600">Cargando proyectos...</p>;
    }

    if (isError) {
        return <p className="text-sm text-red-600">{(error as Error).message}</p>;
    }

    if (!data?.length) {
        return <p className="text-sm text-slate-600">No hay proyectos registrados.</p>;
    }

    return (
        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
            <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-slate-200">
                    <thead className="bg-slate-50">
                        <tr>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Proyecto</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Cronograma</th>
                            <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-600">Presupuesto</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Estado</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-200 bg-white">
                        {data.map((item) => (
                            <tr key={item.id} className="hover:bg-slate-50">
                                <td className="px-4 py-3 text-sm text-slate-900">
                                    <p className="font-medium">{item.nombre}</p>
                                    <p className="text-xs text-slate-500">{item.descripcion}</p>
                                </td>
                                <td className="px-4 py-3 text-sm text-slate-700">
                                    {formatFecha(item.fechaInicio)} — {formatFecha(item.fechaFin)}
                                </td>
                                <td className="px-4 py-3 text-right text-sm font-semibold text-slate-900">{formatCOP(item.presupuestoEstimado)}</td>
                                <td className="px-4 py-3 text-sm text-slate-700">{item.estado}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
