'use client';

import { mensajeDeError } from '@/lib/apiClient';
import { asignarRol, type AsignarRolPayload } from '@/features/seguridad/services/usuariosService';
import { useMutation, useQueryClient } from '@tanstack/react-query';

function mapError(error: unknown, fallbackMessage: string): Error {
    return new Error(mensajeDeError(error, fallbackMessage));
}

export function useAsignarRol() {
    const queryClient = useQueryClient();

    return useMutation<void, Error, { id: string; payload: AsignarRolPayload }>({
        mutationFn: async ({ id, payload }) => {
            try {
                await asignarRol(id, payload);
            } catch (error) {
                throw mapError(error, 'No fue posible actualizar el rol del usuario.');
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['seguridad', 'usuarios'] });
        },
    });
}
