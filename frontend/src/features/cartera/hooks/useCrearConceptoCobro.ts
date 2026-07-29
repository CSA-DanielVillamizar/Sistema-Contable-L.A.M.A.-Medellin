'use client';

import { mensajeDeError } from '@/lib/apiClient';
import type { CrearConceptoCobroPayload } from '@/features/cartera/services/carteraService';
import { crearConceptoCobro } from '@/features/cartera/services/carteraService';
import { useMutation } from '@tanstack/react-query';

type UseCrearConceptoCobroOptions = {
    onSuccessNotification?: (message: string) => void;
    onErrorNotification?: (message: string) => void;
};

function getErrorMessage(error: unknown): string {
    return mensajeDeError(error, 'No fue posible crear el concepto de cobro.');
}

export function useCrearConceptoCobro(options?: UseCrearConceptoCobroOptions) {
    return useMutation<{ id: string }, Error, CrearConceptoCobroPayload>({
        mutationFn: async (payload) => {
            try {
                return await crearConceptoCobro(payload);
            } catch (error) {
                throw new Error(getErrorMessage(error));
            }
        },
        onSuccess: () => {
            options?.onSuccessNotification?.('Concepto de cobro creado correctamente.');
        },
        onError: (error) => {
            options?.onErrorNotification?.(error.message);
        },
    });
}
