'use client';

import type { BeneficiarioFinalItem } from '@/features/tributario/hooks/useReporteBeneficiariosFinales';

type TablaBeneficiariosFinalesProps = {
    rows: BeneficiarioFinalItem[];
    isLoading: boolean;
    errorMessage?: string;
};

export default function TablaBeneficiariosFinales({ rows, isLoading, errorMessage }: TablaBeneficiariosFinalesProps) {
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
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Tipo documento</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Número documento</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Nombres</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Apellidos</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">País residencia</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Cargo o rol</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-100 bg-white">
                        {isLoading ? (
                            <tr>
                                <td colSpan={6} className="px-4 py-8 text-center text-sm text-slate-500">
                                    Cargando reporte de beneficiarios finales...
                                </td>
                            </tr>
                        ) : rows.length === 0 ? (
                            <tr>
                                <td colSpan={6} className="px-4 py-8 text-center text-sm text-slate-500">
                                    No hay beneficiarios finales para mostrar.
                                </td>
                            </tr>
                        ) : (
                            rows.map((row, index) => (
                                <tr key={`${row.numeroDocumento}-${index}`} className="hover:bg-slate-50">
                                    <td className="px-4 py-3 text-sm text-slate-800">{row.tipoDocumento}</td>
                                    <td className="px-4 py-3 text-sm text-slate-800">{row.numeroDocumento}</td>
                                    <td className="px-4 py-3 text-sm text-slate-800">{row.nombres}</td>
                                    <td className="px-4 py-3 text-sm text-slate-800">{row.apellidos}</td>
                                    <td className="px-4 py-3 text-sm text-slate-800">{row.paisResidencia}</td>
                                    <td className="px-4 py-3 text-sm text-slate-800">{row.cargoORol}</td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
