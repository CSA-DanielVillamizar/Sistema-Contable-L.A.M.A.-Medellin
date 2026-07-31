'use client';

import apiClient, { type RespuestaApi } from '@/lib/apiClient';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

/** Espeja TipoCentroCosto del dominio. Los valores los persiste el backend. */
export const TIPOS_CENTRO_COSTO = [
    { value: 1, label: 'Capítulo' },
    { value: 2, label: 'Fundación' },
    { value: 3, label: 'Proyecto' },
    { value: 4, label: 'Evento' },
] as const;

export function nombreTipoCentroCosto(valor: number): string {
    return TIPOS_CENTRO_COSTO.find((t) => t.value === valor)?.label ?? 'Sin clasificar';
}

export type CentroCosto = {
    id: string;
    nombre: string;
    tipo: number;
};

export type CentroCostoPayload = {
    nombre: string;
    tipo: number;
};

const CLAVE = ['administracion', 'centros-costo'] as const;

export function useCentrosCostoAdmin() {
    return useQuery<CentroCosto[]>({
        queryKey: CLAVE,
        queryFn: async () => {
            const response = await apiClient.get<RespuestaApi[]>('/api/transacciones/centros-costo');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? ''),
                nombre: String(item?.nombre ?? ''),
                tipo: Number(item?.tipo ?? 0),
            }));
        },
    });
}

/**
 * El catalogo lo leen el modal de comprobantes y los formularios de ingreso y
 * egreso desde otra clave; sin invalidarlas seguirian sin ver el centro nuevo.
 */
function useInvalidar() {
    const queryClient = useQueryClient();

    return () => {
        [CLAVE, ['transacciones', 'catalogo', 'centros-costo'], ['tesoreria', 'catalogos', 'centros-costo']].forEach(
            (clave) => {
                void queryClient.invalidateQueries({ queryKey: clave });
            },
        );
    };
}

export function useCrearCentroCosto() {
    const invalidar = useInvalidar();

    return useMutation({
        mutationFn: async (payload: CentroCostoPayload) => {
            await apiClient.post('/api/configuracion/centros-costo', payload);
        },
        onSuccess: invalidar,
    });
}

export function useActualizarCentroCosto() {
    const invalidar = useInvalidar();

    return useMutation({
        mutationFn: async ({ id, ...payload }: CentroCostoPayload & { id: string }) => {
            await apiClient.put(`/api/configuracion/centros-costo/${id}`, payload);
        },
        onSuccess: invalidar,
    });
}
