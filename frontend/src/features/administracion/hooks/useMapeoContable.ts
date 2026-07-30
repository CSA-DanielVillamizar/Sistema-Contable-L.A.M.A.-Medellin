'use client';

import apiClient, { mensajeDeError, type RespuestaApi } from '@/lib/apiClient';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

/**
 * Mapeo de cuentas por operacion (historia 1-2).
 *
 * El backend devuelve todas las operaciones, incluidas las que aun no tienen
 * cuenta: la pantalla las marca como pendientes en vez de esconderlas.
 */
export type MapeoContable = {
    tipoOperacion: number;
    nombreOperacion: string;
    cuentaContableId: string | null;
    codigoCuenta: string | null;
    descripcionCuenta: string | null;
};

const CLAVE = ['administracion', 'mapeo-contable'] as const;

export function useMapeoContable() {
    return useQuery<MapeoContable[]>({
        queryKey: CLAVE,
        queryFn: async () => {
            const response = await apiClient.get<RespuestaApi[]>('/api/configuracion/mapeo-contable');

            return (response.data ?? []).map((m) => ({
                tipoOperacion: Number(m?.tipoOperacion ?? 0),
                nombreOperacion: String(m?.nombreOperacion ?? ''),
                cuentaContableId: m?.cuentaContableId ? String(m.cuentaContableId) : null,
                codigoCuenta: m?.codigoCuenta ? String(m.codigoCuenta) : null,
                descripcionCuenta: m?.descripcionCuenta ? String(m.descripcionCuenta) : null,
            }));
        },
    });
}

export function useActualizarMapeoContable() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async ({
            tipoOperacion,
            cuentaContableId,
        }: {
            tipoOperacion: number;
            cuentaContableId: string;
        }) => {
            try {
                await apiClient.put(`/api/configuracion/mapeo-contable/${tipoOperacion}`, { cuentaContableId });
            } catch (error) {
                throw new Error(mensajeDeError(error, 'No fue posible guardar el mapeo.'));
            }
        },
        onSuccess: () => {
            void queryClient.invalidateQueries({ queryKey: CLAVE });
        },
    });
}
