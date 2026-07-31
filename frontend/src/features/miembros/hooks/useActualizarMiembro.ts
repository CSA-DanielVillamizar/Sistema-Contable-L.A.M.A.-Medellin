'use client';

import { mensajeDeError } from '@/lib/apiClient';
import { actualizarMiembro, type ActualizarMiembroPayload } from '@/features/miembros/services/miembrosService';
import { useMutation, useQueryClient } from '@tanstack/react-query';

function mapError(error: unknown, fallbackMessage: string): Error {
    return new Error(mensajeDeError(error, fallbackMessage));
}

export function useActualizarMiembro() {
    const queryClient = useQueryClient();

    return useMutation<void, Error, { id: string; payload: ActualizarMiembroPayload }>({
        mutationFn: async ({ id, payload }) => {
            try {
                await actualizarMiembro(id, payload);
            } catch (error) {
                throw mapError(error, 'No fue posible actualizar el miembro.');
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['miembros'] });
        },
    });
}
