'use client';

import { mensajeDeError } from '@/lib/apiClient';
import { actualizarEvento, type UpdateEventoPayload } from '@/features/eventos/services/eventosService';
import { useMutation, useQueryClient } from '@tanstack/react-query';

function mapError(error: unknown, fallbackMessage: string): Error {
    return new Error(mensajeDeError(error, fallbackMessage));
}

export function useActualizarEvento() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async ({ id, payload }: { id: string; payload: UpdateEventoPayload }) => {
            try {
                await actualizarEvento(id, payload);
            } catch (error) {
                throw mapError(error, 'No fue posible actualizar el evento.');
            }
        },
        onSuccess: async (_, variables: { id: string; payload: UpdateEventoPayload }) => {
            await queryClient.invalidateQueries({ queryKey: ['eventos'] });
            await queryClient.invalidateQueries({ queryKey: ['evento', variables.id] });
        },
    });
}
