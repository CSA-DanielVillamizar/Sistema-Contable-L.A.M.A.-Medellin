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
};

export function useReporteExogena({ anio, mes }: UseReporteExogenaParams) {
    return useQuery<ReporteExogenaItem[]>({
        queryKey: ['tributario', 'exogena', anio, mes ?? 'all'],
        queryFn: async () => {
            const params: Record<string, number> = { anio };
            if (typeof mes === 'number') {
                params.mes = mes;
            }

            const response = await apiClient.get<any[]>('/api/tributario/exogena', { params });

            return (response.data ?? []).map((item) => ({
                terceroId: String(item?.terceroId ?? item?.TerceroId ?? ''),
                nombreTercero: String(item?.nombreTercero ?? item?.NombreTercero ?? ''),
                cuentaContableCodigo: String(item?.cuentaContableCodigo ?? item?.CuentaContableCodigo ?? ''),
                cuentaContableNombre: String(item?.cuentaContableNombre ?? item?.CuentaContableNombre ?? ''),
                totalDebito: Number(item?.totalDebito ?? item?.TotalDebito ?? 0),
                totalCredito: Number(item?.totalCredito ?? item?.TotalCredito ?? 0),
                saldoMovimiento: Number(item?.saldoMovimiento ?? item?.SaldoMovimiento ?? 0),
            }));
        },
        enabled: Number.isInteger(anio) && anio > 0,
    });
}
