'use client';

import type { ArticuloItem } from '@/features/merchandising/hooks/useArticulos';

type TablaArticulosProps = {
    articulos: ArticuloItem[];
    isLoading: boolean;
    isError: boolean;
    error: Error | null;
};

function formatCOP(value: number): string {
    return new Intl.NumberFormat('es-CO', {
        style: 'currency',
        currency: 'COP',
        maximumFractionDigits: 0,
    }).format(value);
}

export default function TablaArticulos({ articulos, isLoading, isError, error }: TablaArticulosProps) {
    if (isLoading) {
        return <p className="text-sm text-slate-600">Cargando artículos...</p>;
    }

    if (isError) {
        return <p className="text-sm text-red-600">{error?.message ?? 'No fue posible cargar los artículos.'}</p>;
    }

    if (!articulos.length) {
        return <p className="text-sm text-slate-600">No hay artículos registrados.</p>;
    }

    return (
        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
            <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-slate-200">
                    <thead className="bg-slate-50">
                        <tr>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Nombre</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">SKU</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Categoría</th>
                            <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-600">Precio</th>
                            <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-600">Costo</th>
                            <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-600">Stock Actual</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-200 bg-white">
                        {articulos.map((item) => {
                            const stockBajo = item.stockActual <= 5;

                            return (
                                <tr key={item.id} className="hover:bg-slate-50">
                                    <td className="px-4 py-3 text-sm text-slate-900">{item.nombre}</td>
                                    <td className="px-4 py-3 text-sm text-slate-700">{item.sku}</td>
                                    <td className="px-4 py-3 text-sm text-slate-700">{item.categoria}</td>
                                    <td className="px-4 py-3 text-right text-sm text-slate-900">{formatCOP(item.precioVenta)}</td>
                                    <td className="px-4 py-3 text-right text-sm text-slate-700">{formatCOP(item.costoPromedio)}</td>
                                    <td className="px-4 py-3 text-right text-sm font-semibold">
                                        {stockBajo ? (
                                            <span className="inline-flex items-center rounded-full bg-red-100 px-2.5 py-0.5 text-xs font-medium text-red-700">
                                                Stock Bajo: {item.stockActual}
                                            </span>
                                        ) : (
                                            <span className="text-slate-900">{item.stockActual}</span>
                                        )}
                                    </td>
                                </tr>
                            );
                        })}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
