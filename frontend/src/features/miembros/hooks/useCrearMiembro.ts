'use client';

import { mensajeDeError } from '@/lib/apiClient';
import { crearMiembro, type CrearMiembroPayload } from '@/features/miembros/services/miembrosService';
import { useMutation, useQueryClient } from '@tanstack/react-query';

function mapError(error: unknown, fallbackMessage: string): Error {
    return new Error(mensajeDeError(error, fallbackMessage));
}

export function useCrearMiembro() {
    const queryClient = useQueryClient();

    return useMutation<{ id: string }, Error, CrearMiembroPayload>({
        mutationFn: async (payload) => {
            try {
                return await crearMiembro(payload);
            } catch (error) {
                throw mapError(error, 'No fue posible crear el miembro.');
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['miembros'] });
        },
    });
}
