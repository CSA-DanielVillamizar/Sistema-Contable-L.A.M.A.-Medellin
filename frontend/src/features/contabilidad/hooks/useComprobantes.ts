'use client';

import axios from 'axios';
import apiClient from '@/lib/apiClient';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import type { ComprobanteFormValues } from '@/features/contabilidad/schemas/comprobanteSchema';

type ProblemDetails = {
    title?: string;
    detail?: string;
    errors?: Record<string, string[]>;
};

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
                if (axios.isAxiosError<ProblemDetails>(error)) {
                    const firstValidationError = error.response?.data?.errors
                        ? Object.values(error.response.data.errors).flat()[0]
                        : undefined;

                    throw new Error(
                        firstValidationError
                        ?? error.response?.data?.detail
                        ?? error.response?.data?.title
                        ?? 'No fue posible registrar el comprobante.');
                }

                throw new Error('No fue posible registrar el comprobante.');
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['contabilidad', 'comprobantes'] });
        },
    });
}
