'use client';

import type { ReporteExogenaItem } from '@/features/tributario/hooks/useReporteExogena';

type TablaExogenaProps = {
    rows: ReporteExogenaItem[];
    isLoading: boolean;
    errorMessage?: string;
};

function formatCOP(value: number): string {
    return new Intl.NumberFormat('es-CO', {
        style: 'currency',
        currency: 'COP',
        maximumFractionDigits: 0,
    }).format(value);
}

export default function TablaExogena({ rows, isLoading, errorMessage }: TablaExogenaProps) {
    if (errorMessage) {
        return (
            <section className="rounded-xl border border-red-200 bg-red-50 p-4 text-sm text-red-700 shadow-sm">
                {errorMessage}
            </section>
        );
    }

    return (
        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
            <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-slate-200">
                    <thead className="bg-slate-50">
                        <tr>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Documento</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Nombre tercero</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Cuenta</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Nombre cuenta</th>
                            <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-600">Total débito</th>
                            <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-600">Total crédito</th>
                            <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-600">Saldo</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-100 bg-white">
                        {isLoading ? (
                            <tr>
                                <td colSpan={7} className="px-4 py-8 text-center text-sm text-slate-500">
                                    Cargando reporte exógena...
                                </td>
                            </tr>
                        ) : rows.length === 0 ? (
                            <tr>
                                <td colSpan={7} className="px-4 py-8 text-center text-sm text-slate-500">
                                    No hay movimientos para los filtros seleccionados.
                                </td>
                            </tr>
                        ) : (
                            rows.map((row, index) => (
                                <tr key={`${row.terceroId}-${row.cuentaContableCodigo}-${index}`} className="hover:bg-slate-50">
                                    <td className="px-4 py-3 text-sm text-slate-800">{row.terceroId}</td>
                                    <td className="px-4 py-3 text-sm text-slate-800">{row.nombreTercero}</td>
                                    <td className="px-4 py-3 text-sm text-slate-800">{row.cuentaContableCodigo}</td>
                                    <td className="px-4 py-3 text-sm text-slate-800">{row.cuentaContableNombre}</td>
                                    <td className="px-4 py-3 text-right text-sm text-slate-800">{formatCOP(row.totalDebito)}</td>
                                    <td className="px-4 py-3 text-right text-sm text-slate-800">{formatCOP(row.totalCredito)}</td>
                                    <td className="px-4 py-3 text-right text-sm font-medium text-slate-900">{formatCOP(row.saldoMovimiento)}</td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
