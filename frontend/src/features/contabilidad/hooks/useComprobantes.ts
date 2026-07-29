'use client';

import apiClient, { mensajeDeError } from '@/lib/apiClient';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import type { ComprobanteFormValues } from '@/features/contabilidad/schemas/comprobanteSchema';

type CrearComprobanteResponse = {
    id?: string;
    Id?: string;
};

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
