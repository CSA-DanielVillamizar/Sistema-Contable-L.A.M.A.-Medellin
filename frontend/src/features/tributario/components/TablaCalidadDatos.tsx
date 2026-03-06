'use client';

import type { InconsistenciaTributariaItem } from '@/features/tributario/hooks/useReporteCalidadDatos';

type TablaCalidadDatosProps = {
    rows: InconsistenciaTributariaItem[];
    isLoading: boolean;
    errorMessage?: string;
};

export default function TablaCalidadDatos({ rows, isLoading, errorMessage }: TablaCalidadDatosProps) {
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
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">TerceroId</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Nombre obtenido</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Tipo relación</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Descripción inconsistencia</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Acción</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-100 bg-white">
                        {isLoading ? (
                            <tr>
                                <td colSpan={5} className="px-4 py-8 text-center text-sm text-slate-500">
                                    Cargando auditoría de datos...
                                </td>
                            </tr>
                        ) : rows.length === 0 ? (
                            <tr>
                                <td colSpan={5} className="px-4 py-8 text-center text-sm text-slate-500">
                                    No se encontraron inconsistencias.
                                </td>
                            </tr>
                        ) : (
                            rows.map((row, index) => {
                                const href = row.tipoRelacion === 'Miembro' ? '/miembros' : '/donaciones';

                                return (
                                    <tr key={`${row.terceroId}-${index}`} className="hover:bg-slate-50">
                                        <td className="px-4 py-3 text-sm text-slate-800">{row.terceroId}</td>
                                        <td className="px-4 py-3 text-sm text-slate-800">{row.nombreObtenido}</td>
                                        <td className="px-4 py-3 text-sm text-slate-800">{row.tipoRelacion}</td>
                                        <td className="px-4 py-3 text-sm text-slate-800">{row.descripcionInconsistencia}</td>
                                        <td className="px-4 py-3 text-sm text-slate-800">
                                            <a href={href} className="text-blue-700 hover:text-blue-900 hover:underline">
                                                Editar registro
                                            </a>
                                        </td>
                                    </tr>
                                );
                            })
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
