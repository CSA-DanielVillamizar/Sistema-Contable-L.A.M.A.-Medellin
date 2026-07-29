'use client';

import { mensajeDeError } from '@/lib/apiClient';
import type { CrearCuentaPorCobrarPayload } from '@/features/cartera/services/carteraService';
import { crearCuentaPorCobrar } from '@/features/cartera/services/carteraService';
import { useMutation } from '@tanstack/react-query';

type UseCrearCuentaPorCobrarOptions = {
    onSuccessNotification?: (message: string) => void;
    onErrorNotification?: (message: string) => void;
};

function getErrorMessage(error: unknown): string {
    return mensajeDeError(error, 'No fue posible crear la cuenta por cobrar.');
}

export function useCrearCuentaPorCobrar(options?: UseCrearCuentaPorCobrarOptions) {
    return useMutation<{ id: string }, Error, CrearCuentaPorCobrarPayload>({
        mutationFn: async (payload) => {
            try {
                return await crearCuentaPorCobrar(payload);
            } catch (error) {
                throw new Error(getErrorMessage(error));
            }
        },
        onSuccess: () => {
            options?.onSuccessNotification?.('Cuenta por cobrar creada correctamente.');
        },
        onError: (error) => {
            options?.onErrorNotification?.(error.message);
        },
    });
}
