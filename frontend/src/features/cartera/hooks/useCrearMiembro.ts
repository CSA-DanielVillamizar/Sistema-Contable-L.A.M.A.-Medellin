'use client';

import { mensajeDeError } from '@/lib/apiClient';
import type { CrearMiembroPayload } from '@/features/cartera/services/carteraService';
import { crearMiembro } from '@/features/cartera/services/carteraService';
import { useMutation } from '@tanstack/react-query';

type UseCrearMiembroOptions = {
    onSuccessNotification?: (message: string) => void;
    onErrorNotification?: (message: string) => void;
};

function getErrorMessage(error: unknown): string {
    return mensajeDeError(error, 'No fue posible crear el miembro.');
}

export function useCrearMiembro(options?: UseCrearMiembroOptions) {
    return useMutation<{ id: string }, Error, CrearMiembroPayload>({
        mutationFn: async (payload) => {
            try {
                return await crearMiembro(payload);
            } catch (error) {
                throw new Error(getErrorMessage(error));
            }
        },
        onSuccess: () => {
            options?.onSuccessNotification?.('Miembro creado correctamente.');
        },
        onError: (error) => {
            options?.onErrorNotification?.(error.message);
        },
    });
}
