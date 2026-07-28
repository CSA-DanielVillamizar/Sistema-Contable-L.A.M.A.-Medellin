'use client';

import { useQuery } from '@tanstack/react-query';
import apiClient from '@/lib/apiClient';

export type ReporteExogenaItem = {
    terceroId: string;
    nombreTercero: string;
    cuentaContableCodigo: string;
    cuentaContableNombre: string;
    totalDebito: number;
    totalCredito: number;
    saldoMovimiento: number;
};

type UseReporteExogenaParams = {
    anio: number;
    mes?: number;
    enabled?: boolean;
};

export function useReporteExogena({ anio, mes, enabled = true }: UseReporteExogenaParams) {
    return useQuery<ReporteExogenaItem[]>({
        queryKey: ['tributario', 'exogena', anio, mes ?? 'all'],
        queryFn: async () => {
            const params: Record<string, number> = { anio };
            if (typeof mes === 'number') {
                params.mes = mes;
            }

            const response = await apiClient.get<any[]>('/api/tributario/exogena', { params });

            return (response.data ?? []).map((item) => ({
                terceroId: String(item?.terceroId ?? ''),
                nombreTercero: String(item?.nombreTercero ?? ''),
                cuentaContableCodigo: String(item?.cuentaContableCodigo ?? ''),
                cuentaContableNombre: String(item?.cuentaContableNombre ?? ''),
                totalDebito: Number(item?.totalDebito ?? 0),
                totalCredito: Number(item?.totalCredito ?? 0),
                saldoMovimiento: Number(item?.saldoMovimiento ?? 0),
            }));
        },
        enabled: enabled && Number.isInteger(anio) && anio > 0,
    });
}
