'use client';

import { actualizarEvento, type UpdateEventoPayload } from '@/features/eventos/services/eventosService';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import axios from 'axios';

type ProblemDetails = {
    title?: string;
    detail?: string;
    errors?: Record<string, string[]>;
};

function mapError(error: unknown, fallbackMessage: string): Error {
    if (axios.isAxiosError<ProblemDetails>(error)) {
        const validationErrors = error.response?.data?.errors;
        const firstValidationError = validationErrors
            ? Object.values(validationErrors).flat().find((message) => message)
            : undefined;

        return new Error(
            firstValidationError
            ?? error.response?.data?.detail
            ?? error.response?.data?.title
            ?? fallbackMessage,
        );
    }

    return new Error(fallbackMessage);
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
