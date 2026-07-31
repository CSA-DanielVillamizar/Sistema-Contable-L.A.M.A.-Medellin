'use client';

import apiClient, { mensajeDeError, type RespuestaApi } from '@/lib/apiClient';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

/**
 * Cierre de periodo (historia 1-5).
 *
 * El backend estaba construido y verificado contra SQL Server, pero no tenia
 * pantalla: cerrar un mes solo era posible llamando al API a mano. Son dos
 * pasos con responsables distintos, y la matriz del BRD los separa: el Tesorero
 * valida y el Contador ejecuta.
 */
export type EstadoPeriodo = 0 | 1 | 2;

export type PeriodoContable = {
    anio: number;
    mes: number;
    estado: EstadoPeriodo;
    fechaValidacionTesoreria: string | null;
    validadoPor: string | null;
    fechaCierre: string | null;
    cerradoPor: string | null;
};

export const NOMBRE_ESTADO: Record<number, string> = {
    0: 'Abierto',
    1: 'Validado por tesorería',
    2: 'Cerrado',
};

const CLAVE = ['contabilidad', 'periodos'] as const;

export function usePeriodosContables() {
    return useQuery<PeriodoContable[]>({
        queryKey: CLAVE,
        queryFn: async () => {
            const response = await apiClient.get<RespuestaApi[]>('/api/periodos-contables');

            return (response.data ?? []).map((p) => ({
                anio: Number(p?.anio ?? 0),
                mes: Number(p?.mes ?? 0),
                estado: Number(p?.estado ?? 0) as EstadoPeriodo,
                fechaValidacionTesoreria: p?.fechaValidacionTesoreria ? String(p.fechaValidacionTesoreria) : null,
                validadoPor: p?.validadoPor ? String(p.validadoPor) : null,
                fechaCierre: p?.fechaCierre ? String(p.fechaCierre) : null,
                cerradoPor: p?.cerradoPor ? String(p.cerradoPor) : null,
            }));
        },
    });
}

function useAccionPeriodo(accion: 'validar' | 'cerrar', respaldo: string) {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async ({ anio, mes }: { anio: number; mes: number }) => {
            try {
                await apiClient.post(`/api/periodos-contables/${anio}/${mes}/${accion}`);
            } catch (error) {
                throw new Error(mensajeDeError(error, respaldo));
            }
        },
        onSuccess: () => {
            void queryClient.invalidateQueries({ queryKey: CLAVE });
        },
    });
}

export function useValidarPeriodo() {
    return useAccionPeriodo('validar', 'No fue posible validar el periodo.');
}

export function useCerrarPeriodo() {
    return useAccionPeriodo('cerrar', 'No fue posible cerrar el periodo.');
}
