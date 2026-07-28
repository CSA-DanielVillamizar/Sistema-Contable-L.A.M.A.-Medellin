'use client';

import type { ProyectoFormValues } from '@/features/proyectos/schemas/proyectoSchema';
import apiClient, { type RespuestaApi } from '@/lib/apiClient';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import axios from 'axios';

type ProblemDetails = {
    title?: string;
    detail?: string;
    errors?: Record<string, string[]>;
};

type CrearResponse = {
    id: string;
};

export type ProyectoItem = {
    id: string;
    centroCostoId: string;
    centroCosto: string;
    nombre: string;
    descripcion: string;
    fechaInicio: string;
    fechaFin?: string | null;
    presupuestoEstimado: number;
    estado: string;
};

function mapError(error: unknown, fallback: string): Error {
    if (axios.isAxiosError<ProblemDetails>(error)) {
        const firstValidationError = error.response?.data?.errors
            ? Object.values(error.response.data.errors).flat()[0]
            : undefined;

        return new Error(firstValidationError ?? error.response?.data?.detail ?? error.response?.data?.title ?? fallback);
    }

    return new Error(fallback);
}

export function useProyectos() {
    return useQuery<ProyectoItem[]>({
        queryKey: ['proyectos', 'listado'],
        queryFn: async () => {
            const response = await apiClient.get<RespuestaApi[]>('/api/proyectos');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? ''),
                centroCostoId: String(item?.centroCostoId ?? ''),
                centroCosto: String(item?.centroCosto ?? ''),
                nombre: String(item?.nombre ?? ''),
                descripcion: String(item?.descripcion ?? ''),
                fechaInicio: String(item?.fechaInicio ?? ''),
                fechaFin: item?.fechaFin ? String(item.fechaFin) : null,
                presupuestoEstimado: Number(item?.presupuestoEstimado ?? 0),
                estado: String(item?.estado ?? ''),
            }));
        },
    });
}

export function useCrearProyecto() {
    const queryClient = useQueryClient();

    return useMutation<CrearResponse, Error, ProyectoFormValues>({
        mutationFn: async (payload) => {
            try {
                const response = await apiClient.post<CrearResponse>('/api/proyectos', payload);
                return response.data;
            } catch (error) {
                throw mapError(error, 'No fue posible registrar el proyecto.');
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['proyectos', 'listado'] });
        },
    });
}
