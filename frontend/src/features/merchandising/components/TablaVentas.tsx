'use client';

import type { VentaHistorialItem } from '@/features/merchandising/hooks/useVentas';

type TablaVentasProps = {
    ventas: VentaHistorialItem[];
    isLoading: boolean;
    isError: boolean;
    error: Error | null;
};

const formatoMoneda = new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0,
});

const formatoFecha = new Intl.DateTimeFormat('es-CO', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
});

export default function TablaVentas({ ventas, isLoading, isError, error }: TablaVentasProps) {
    if (isLoading) {
        return <section className="rounded-xl border border-slate-200 bg-white p-4 text-sm text-slate-600 shadow-sm">Cargando ventas...</section>;
    }

    if (isError) {
        return (
            <section className="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700 shadow-sm">
                {error?.message ?? 'No fue posible cargar el historial de ventas.'}
            </section>
        );
    }

    if (ventas.length === 0) {
        return <section className="rounded-xl border border-slate-200 bg-white p-4 text-sm text-slate-600 shadow-sm">No hay ventas registradas.</section>;
    }

    return (
        <section className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
            <table className="min-w-full divide-y divide-slate-200">
                <thead className="bg-slate-50">
                    <tr>
                        <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-600">Fecha</th>
                        <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-600">Número de Factura</th>
                        <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-600">Cliente</th>
                        <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wide text-slate-600">Método de Pago</th>
                        <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wide text-slate-600">Total</th>
                    </tr>
                </thead>
                <tbody className="divide-y divide-slate-100 bg-white">
                    {ventas.map((venta) => (
                        <tr key={venta.id}>
                            <td className="px-4 py-3 text-sm text-slate-700">{formatoFecha.format(new Date(venta.fecha))}</td>
                            <td className="px-4 py-3 text-sm font-medium text-slate-900">{venta.numeroFacturaInterna}</td>
                            <td className="px-4 py-3 text-sm text-slate-700">{venta.cliente}</td>
                            <td className="px-4 py-3 text-sm text-slate-700">{venta.metodoPago}</td>
                            <td className="px-4 py-3 text-right text-sm font-semibold text-slate-900">{formatoMoneda.format(venta.total)}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </section>
    );
}
