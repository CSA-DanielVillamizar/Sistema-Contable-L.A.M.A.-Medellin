'use client';

import {
    registrarIngreso,
    type RegistrarMovimientoTesoreriaPayload,
} from '@/features/tesoreria/services/tesoreriaService';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import axios from 'axios';

type ProblemDetails = {
    title?: string;
    detail?: string;
    errors?: Record<string, string[]>;
};

function getErrorMessage(error: unknown): string {
    if (!axios.isAxiosError<ProblemDetails>(error)) {
        return 'No fue posible registrar el ingreso.';
    }

    const validationErrors = error.response?.data?.errors;
    const firstValidationError = validationErrors
        ? Object.values(validationErrors).flat().find((message) => message)
        : undefined;

    return firstValidationError
        ?? error.response?.data?.detail
        ?? error.response?.data?.title
        ?? 'No fue posible registrar el ingreso.';
}

export function useRegistrarIngreso() {
    const queryClient = useQueryClient();

    return useMutation<{ id: string }, Error, RegistrarMovimientoTesoreriaPayload>({
        mutationFn: async (payload) => {
            try {
                return await registrarIngreso(payload);
            } catch (error) {
                throw new Error(getErrorMessage(error));
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['tesoreria', 'cajas'] });
            await queryClient.invalidateQueries({ queryKey: ['tesoreria', 'egresos'] });
            await queryClient.invalidateQueries({ queryKey: ['transacciones', 'listado'] });
        },
    });
}
