'use client';

import { useState } from 'react';
import VistaCertificado from '@/features/donaciones/components/VistaCertificado';
import { useCertificadoDonacion, useDonaciones } from '@/features/donaciones/hooks/useDonaciones';

function formatCOP(value: number): string {
    return new Intl.NumberFormat('es-CO', {
        style: 'currency',
        currency: 'COP',
        maximumFractionDigits: 0,
    }).format(value);
}

function formatFecha(value: string): string {
    const parsed = new Date(value);
    return Number.isNaN(parsed.getTime()) ? value : parsed.toLocaleDateString('es-CO');
}

export default function TablaDonaciones() {
    const { data, isLoading, isError, error } = useDonaciones();
    const [donacionSeleccionadaId, setDonacionSeleccionadaId] = useState<string | null>(null);
    const certificadoQuery = useCertificadoDonacion(donacionSeleccionadaId ?? undefined);

    if (isLoading) {
        return <p className="text-sm text-slate-600">Cargando donaciones...</p>;
    }

    if (isError) {
        return <p className="text-sm text-red-600">{(error as Error).message}</p>;
    }

    if (!data?.length) {
        return <p className="text-sm text-slate-600">No hay donaciones registradas.</p>;
    }

    return (
        <>
            <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
                <div className="overflow-x-auto">
                    <table className="min-w-full divide-y divide-slate-200">
                        <thead className="bg-slate-50">
                            <tr>
                                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Donante</th>
                                <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-600">Monto</th>
                                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Fecha</th>
                                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Certificado</th>
                                <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-600">Acciones</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-slate-200 bg-white">
                            {data.map((item) => (
                                <tr key={item.id} className="hover:bg-slate-50">
                                    <td className="px-4 py-3 text-sm text-slate-900">{item.nombreDonante}</td>
                                    <td className="px-4 py-3 text-right text-sm font-semibold text-slate-900">{formatCOP(item.montoCOP)}</td>
                                    <td className="px-4 py-3 text-sm text-slate-700">{formatFecha(item.fecha)}</td>
                                    <td className="px-4 py-3 text-sm">
                                        <span
                                            className={`rounded-full px-2 py-1 text-xs font-medium ${
                                                item.certificadoEmitido ? 'bg-emerald-100 text-emerald-700' : 'bg-amber-100 text-amber-700'
                                            }`}
                                        >
                                            {item.certificadoEmitido ? 'Emitido' : 'Pendiente'}
                                        </span>
                                    </td>
                                    <td className="px-4 py-3 text-right text-sm">
                                        <button
                                            type="button"
                                            onClick={() => setDonacionSeleccionadaId(item.id)}
                                            className="rounded-md border border-blue-200 bg-blue-50 px-3 py-1.5 text-blue-700 hover:bg-blue-100"
                                        >
                                            Ver Certificado
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>

            {donacionSeleccionadaId ? (
                <div className="fixed inset-0 z-50 overflow-auto bg-black/40 p-4">
                    {certificadoQuery.isLoading ? (
                        <div className="mx-auto mt-20 max-w-xl rounded-lg bg-white p-6 text-sm text-slate-700">Cargando certificado...</div>
                    ) : certificadoQuery.isError ? (
                        <div className="mx-auto mt-20 max-w-xl rounded-lg bg-white p-6 text-sm text-red-700">
                            {(certificadoQuery.error as Error).message}
                            <div className="mt-4">
                                <button
                                    type="button"
                                    className="rounded-lg border border-slate-300 px-4 py-2"
                                    onClick={() => setDonacionSeleccionadaId(null)}
                                >
                                    Cerrar
                                </button>
                            </div>
                        </div>
                    ) : certificadoQuery.data ? (
                        <VistaCertificado data={certificadoQuery.data} onClose={() => setDonacionSeleccionadaId(null)} />
                    ) : null}
                </div>
            ) : null}
        </>
    );
}
