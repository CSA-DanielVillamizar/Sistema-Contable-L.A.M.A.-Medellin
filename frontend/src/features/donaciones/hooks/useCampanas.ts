'use client';

import apiClient, { mensajeDeError, type RespuestaApi } from '@/lib/apiClient';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

/**
 * Campanas de donacion (historias 2-1 y 2-2).
 *
 * Lo recaudado y el avance los calcula el backend sumando las donaciones
 * imputadas; aqui no se recalcula nada para que no haya dos versiones de la
 * misma cifra.
 */
export type Campana = {
    id: string;
    nombre: string;
    descripcion: string;
    metaCOP: number;
    recaudadoCOP: number;
    porcentajeAvance: number;
    cantidadDonaciones: number;
    fechaInicio: string;
    fechaFin: string;
    estaActiva: boolean;
    estaVigente: boolean;
};

export type CampanaPayload = {
    nombre: string;
    descripcion: string;
    metaCOP: number;
    fechaInicio: string;
    fechaFin: string;
};

const CLAVE = ['donaciones', 'campanas'] as const;

export function useCampanas(incluirCerradas = true) {
    return useQuery<Campana[]>({
        queryKey: [...CLAVE, incluirCerradas],
        queryFn: async () => {
            const response = await apiClient.get<RespuestaApi[]>('/api/donaciones/campanas', {
                params: { incluirCerradas },
            });

            return (response.data ?? []).map((c) => ({
                id: String(c?.id ?? ''),
                nombre: String(c?.nombre ?? ''),
                descripcion: String(c?.descripcion ?? ''),
                metaCOP: Number(c?.metaCOP ?? 0),
                recaudadoCOP: Number(c?.recaudadoCOP ?? 0),
                porcentajeAvance: Number(c?.porcentajeAvance ?? 0),
                cantidadDonaciones: Number(c?.cantidadDonaciones ?? 0),
                fechaInicio: String(c?.fechaInicio ?? ''),
                fechaFin: String(c?.fechaFin ?? ''),
                estaActiva: Boolean(c?.estaActiva),
                estaVigente: Boolean(c?.estaVigente),
            }));
        },
    });
}

function useInvalidar() {
    const queryClient = useQueryClient();
    return () => {
        void queryClient.invalidateQueries({ queryKey: CLAVE });
    };
}

export function useCrearCampana() {
    const invalidar = useInvalidar();

    return useMutation({
        mutationFn: async (payload: CampanaPayload) => {
            try {
                await apiClient.post('/api/donaciones/campanas', payload);
            } catch (error) {
                throw new Error(mensajeDeError(error, 'No fue posible crear la campaña.'));
            }
        },
        onSuccess: invalidar,
    });
}

export function useActualizarCampana() {
    const invalidar = useInvalidar();

    return useMutation({
        mutationFn: async ({ id, ...payload }: CampanaPayload & { id: string }) => {
            try {
                await apiClient.put(`/api/donaciones/campanas/${id}`, payload);
            } catch (error) {
                throw new Error(mensajeDeError(error, 'No fue posible actualizar la campaña.'));
            }
        },
        onSuccess: invalidar,
    });
}

export function useCambiarEstadoCampana() {
    const invalidar = useInvalidar();

    return useMutation({
        mutationFn: async ({ id, activa }: { id: string; activa: boolean }) => {
            try {
                await apiClient.patch(`/api/donaciones/campanas/${id}/estado`, { activa });
            } catch (error) {
                throw new Error(mensajeDeError(error, 'No fue posible cambiar el estado.'));
            }
        },
        onSuccess: invalidar,
    });
}
