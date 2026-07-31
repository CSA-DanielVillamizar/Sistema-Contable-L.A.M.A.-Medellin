'use client';

import { mensajeDeError } from '@/lib/apiClient';
import { marcarAsistencia, type MarcarAsistenciaPayload } from '@/features/eventos/services/eventosService';
import { useMutation, useQueryClient } from '@tanstack/react-query';

function mapError(error: unknown, fallbackMessage: string): Error {
    return new Error(mensajeDeError(error, fallbackMessage));
}

export function useMarcarAsistencia(eventoId: string) {
    const queryClient = useQueryClient();

    return useMutation<void, Error, MarcarAsistenciaPayload>({
        mutationFn: async (payload) => {
            try {
                await marcarAsistencia(eventoId, payload);
            } catch (error) {
                throw mapError(error, 'No fue posible marcar la asistencia.');
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['evento', eventoId] });
            await queryClient.invalidateQueries({ queryKey: ['eventos'] });
        },
    });
}
