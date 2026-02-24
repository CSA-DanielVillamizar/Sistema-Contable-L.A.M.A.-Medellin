import axios from 'axios';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import apiClient from '@/lib/apiClient';
import type { MiembroFormValues } from '@/features/miembros/schemas/miembroSchema';

type ProblemDetails = {
    title?: string;
    detail?: string;
    errors?: Record<string, string[]>;
};

type CrearMiembroResponse = {
    id: string;
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

export const useCrearMiembro = () => {
    const queryClient = useQueryClient();

    return useMutation<CrearMiembroResponse, Error, MiembroFormValues>({
        mutationFn: async (request) => {
            try {
                const response = await apiClient.post<CrearMiembroResponse>('/api/miembros', request);
                return response.data;
            } catch (error) {
                throw mapError(error, 'No fue posible crear el miembro.');
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['miembros', 'listado'] });
        },
    });
};

export const useActualizarMiembro = () => {
    const queryClient = useQueryClient();

    return useMutation<void, Error, { id: string; request: MiembroFormValues }>({
        mutationFn: async ({ id, request }) => {
            try {
                await apiClient.put(`/api/miembros/${id}`, request);
            } catch (error) {
                throw mapError(error, 'No fue posible actualizar el miembro.');
            }
        },
        onSuccess: async (_, variables) => {
            await queryClient.invalidateQueries({ queryKey: ['miembros', 'listado'] });
            await queryClient.invalidateQueries({ queryKey: ['miembros', 'detalle', variables.id] });
        },
    });
};

export const useEliminarMiembro = () => {
    const queryClient = useQueryClient();

    return useMutation<void, Error, string>({
        mutationFn: async (id) => {
            try {
                await apiClient.delete(`/api/miembros/${id}`);
            } catch (error) {
                throw mapError(error, 'No fue posible eliminar el miembro.');
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['miembros', 'listado'] });
        },
    });
};
