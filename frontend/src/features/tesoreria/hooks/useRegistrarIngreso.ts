'use client';

import { mensajeDeError } from '@/lib/apiClient';
import {
    registrarIngreso,
    type RegistrarMovimientoTesoreriaPayload,
} from '@/features/tesoreria/services/tesoreriaService';
import { useMutation, useQueryClient } from '@tanstack/react-query';

function getErrorMessage(error: unknown): string {
    return mensajeDeError(error, 'No fue posible registrar el ingreso.');
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
