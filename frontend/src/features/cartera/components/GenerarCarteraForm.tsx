'use client';

import { useState } from 'react';
import axios from 'axios';
import { useMutation } from '@tanstack/react-query';
import apiClient from '@/lib/apiClient';

type GenerarObligacionesRequest = {
    Periodo: string;
};

type GenerarObligacionesResponse = {
    periodo: string;
    cuotasGeneradas: number;
    mensaje?: string;
};

type ProblemDetails = {
    title?: string;
    detail?: string;
};

export default function GenerarCarteraForm() {
    const [periodo, setPeriodo] = useState<string>(() => {
        const hoy = new Date();
        const mes = String(hoy.getMonth() + 1).padStart(2, '0');
        return `${hoy.getFullYear()}-${mes}`;
    });
    const [mensajeExito, setMensajeExito] = useState<string>('');

    const { mutateAsync, isPending, error, data } = useMutation<GenerarObligacionesResponse, Error, GenerarObligacionesRequest>({
        mutationFn: async (request) => {
            try {
                const response = await apiClient.post<GenerarObligacionesResponse>('/api/cartera/generar-obligaciones', request);
                return response.data;
            } catch (err) {
                if (axios.isAxiosError<ProblemDetails>(err)) {
                    const mensaje = err.response?.data?.detail ?? err.response?.data?.title ?? 'No fue posible generar las cuotas.';
                    throw new Error(mensaje);
                }

                throw new Error('No fue posible generar las cuotas.');
            }
        },
    });

    const handleGenerar = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        setMensajeExito('');

        try {
            const response = await mutateAsync({ Periodo: periodo });
            const cantidad = response.cuotasGeneradas;
            setMensajeExito(`Se generaron ${cantidad} cuotas para el período ${response.periodo}.`);
        } catch {
            // El estado de error ya lo maneja React Query y se renderiza en el bloque {error}
        }
    };

    return (
        <section className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
            <header className="mb-6 border-b border-slate-100 pb-4">
                <h2 className="text-lg font-semibold text-slate-900">Panel de Control - Generación Masiva</h2>
                <p className="mt-1 text-sm text-slate-600">Selecciona el período y genera de forma masiva las cuentas por cobrar de los miembros activos.</p>
            </header>

            <form onSubmit={handleGenerar} className="space-y-5">
                <div>
                    <label htmlFor="periodo" className="mb-1 block text-sm font-medium text-slate-700">
                        Período a facturar (Mes/Año)
                    </label>
                    <input
                        id="periodo"
                        type="month"
                        value={periodo}
                        onChange={(e) => setPeriodo(e.target.value)}
                        required
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
                    />
                    <p className="mt-1 text-xs text-slate-500">Formato: AAAA-MM (ejemplo: 2026-02)</p>
                </div>

                {error && <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">{error.message}</div>}

                {mensajeExito && (
                    <div className="rounded-lg border border-emerald-200 bg-emerald-50 px-3 py-2 text-sm text-emerald-700">{mensajeExito}</div>
                )}

                {data?.mensaje && (
                    <div className="rounded-lg border border-blue-200 bg-blue-50 px-3 py-2 text-sm text-blue-700">{data.mensaje}</div>
                )}

                <button
                    type="submit"
                    disabled={isPending}
                    className="inline-flex items-center rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white transition hover:bg-blue-800 disabled:cursor-not-allowed disabled:opacity-60"
                >
                    {isPending ? 'Generando cuotas...' : 'Generar cuotas'}
                </button>
            </form>
        </section>
    );
}
