'use client';

import { mensajeDeError } from '@/lib/apiClient';
import {
    registrarPagoCartera,
    type RegistrarPagoCarteraPayload,
} from '@/features/cartera/services/carteraService';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-hot-toast';

function getErrorMessage(error: unknown): string {
    return mensajeDeError(error, 'No fue posible registrar el pago.');
}

export function useRegistrarPago() {
    const queryClient = useQueryClient();

    return useMutation<void, Error, RegistrarPagoCarteraPayload>({
        mutationFn: async (payload) => {
            try {
                await registrarPagoCartera(payload);
            } catch (error) {
                throw new Error(getErrorMessage(error));
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['cartera', 'cuentas-por-cobrar'] });
            toast.success('Pago registrado exitosamente.');
        },
    });
}
