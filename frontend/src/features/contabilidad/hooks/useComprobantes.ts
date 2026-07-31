'use client';

import apiClient, { mensajeDeError, type RespuestaApi } from '@/lib/apiClient';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import type { ComprobanteFormValues } from '@/features/contabilidad/schemas/comprobanteSchema';

type CrearComprobanteResponse = {
    id?: string;
    Id?: string;
};

export type ComprobanteResumen = {
    id: string;
    numeroConsecutivo: string;
    fecha: string;
    tipoComprobante: string;
    descripcion: string;
    estado: string;
    total: number;
};

/**
 * Listado de comprobantes. No existia endpoint para consultarlos: se podian
 * crear pero ninguna pantalla podia ofrecerlos para elegir ni descargar su
 * recibo.
 */
export function useComprobantes() {
    return useQuery<ComprobanteResumen[]>({
        queryKey: ['contabilidad', 'comprobantes'],
        queryFn: async () => {
            const response = await apiClient.get<RespuestaApi[]>('/api/comprobantes');

            return (response.data ?? []).map((c) => ({
                id: String(c?.id ?? ''),
                numeroConsecutivo: String(c?.numeroConsecutivo ?? ''),
                fecha: String(c?.fecha ?? ''),
                tipoComprobante: String(c?.tipoComprobante ?? ''),
                descripcion: String(c?.descripcion ?? ''),
                estado: String(c?.estado ?? ''),
                total: Number(c?.total ?? 0),
            }));
        },
    });
}

export function useRegistrarComprobante() {
    const queryClient = useQueryClient();

    return useMutation<CrearComprobanteResponse, Error, ComprobanteFormValues>({
        mutationFn: async (payload) => {
            try {
                const response = await apiClient.post<CrearComprobanteResponse>('/api/comprobantes', payload);
                return response.data;
            } catch (error) {
                throw new Error(mensajeDeError(error, 'No fue posible registrar el comprobante.'));
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['contabilidad', 'comprobantes'] });
        },
    });
}
