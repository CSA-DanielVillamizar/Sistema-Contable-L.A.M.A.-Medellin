'use client';

import { useBeneficiarios } from '@/features/proyectos/hooks/useBeneficiarios';

export default function TablaBeneficiarios() {
    const { data, isLoading, isError, error } = useBeneficiarios();

    if (isLoading) {
        return <p className="text-sm text-slate-600">Cargando beneficiarios...</p>;
    }

    if (isError) {
        return <p className="text-sm text-red-600">{(error as Error).message}</p>;
    }

    if (!data?.length) {
        return <p className="text-sm text-slate-600">No hay beneficiarios registrados.</p>;
    }

    return (
        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
            <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-slate-200">
                    <thead className="bg-slate-50">
                        <tr>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Nombre</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Documento</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Contacto</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Habeas Data</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-200 bg-white">
                        {data.map((item) => (
                            <tr key={item.id} className="hover:bg-slate-50">
                                <td className="px-4 py-3 text-sm text-slate-900">{item.nombreCompleto}</td>
                                <td className="px-4 py-3 text-sm text-slate-700">{item.tipoDocumento} {item.numeroDocumento}</td>
                                <td className="px-4 py-3 text-sm text-slate-700">
                                    <p>{item.email}</p>
                                    <p className="text-xs text-slate-500">{item.telefono}</p>
                                </td>
                                <td className="px-4 py-3 text-sm">
                                    <span className="rounded-full bg-emerald-100 px-2 py-1 text-xs font-medium text-emerald-700">
                                        {item.tieneConsentimientoHabeasData ? 'Consentido' : 'No'}
                                    </span>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
