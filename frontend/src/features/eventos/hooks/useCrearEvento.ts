'use client';

import { mensajeDeError } from '@/lib/apiClient';
import { crearEvento, type CreateEventoPayload } from '@/features/eventos/services/eventosService';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { toast } from 'react-hot-toast';

function mapError(error: unknown, fallbackMessage: string): Error {
    return new Error(mensajeDeError(error, fallbackMessage));
}

export function useCrearEvento() {
    const queryClient = useQueryClient();

    return useMutation<{ id: string }, Error, CreateEventoPayload>({
        mutationFn: async (payload) => {
            try {
                return await crearEvento(payload);
            } catch (error) {
                throw mapError(error, 'No fue posible crear el evento.');
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['eventos'] });
            toast.success('Evento registrado exitosamente.');
        },
    });
}
