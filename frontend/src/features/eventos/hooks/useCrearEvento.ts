'use client';

import { crearEvento, type CreateEventoPayload } from '@/features/eventos/services/eventosService';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import axios from 'axios';
import { toast } from 'react-hot-toast';

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
