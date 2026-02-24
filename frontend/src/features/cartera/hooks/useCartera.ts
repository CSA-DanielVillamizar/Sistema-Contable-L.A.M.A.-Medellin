'use client';

import axios from 'axios';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import apiClient from '@/lib/apiClient';

export type CarteraPendienteItem = {
    id: string;
    miembroId: string;
    nombreMiembro: string;
    periodo: string;
    valorEsperadoCOP: number;
    saldoPendienteCOP: number;
};

type ProblemDetails = {
    title?: string;
    detail?: string;
    errors?: Record<string, string[]>;
};

type RegistrarPagoPayload = {
    id: string;
    MontoPagadoCOP: number;
    BancoId: string;
    CentroCostoId: string;
    Descripcion?: string;
};

type GenerarCarteraPayload = {
    Periodo: string;
};

function getErrorMessage(error: unknown, fallback: string): string {
    if (!axios.isAxiosError<ProblemDetails>(error)) {
        return fallback;
    }

    const validationMessage = error.response?.data?.errors
        ? Object.values(error.response.data.errors).flat()[0]
        : undefined;

    return validationMessage ?? error.response?.data?.detail ?? error.response?.data?.title ?? fallback;
}

export function useCarteraPendiente() {
    return useQuery<CarteraPendienteItem[]>({
        queryKey: ['cartera', 'pendiente'],
        queryFn: async () => {
            const response = await apiClient.get<any[]>('/api/cartera/pendiente');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? item?.Id ?? ''),
                miembroId: String(item?.miembroId ?? item?.MiembroId ?? ''),
                nombreMiembro: String(item?.nombreMiembro ?? item?.NombreMiembro ?? ''),
                periodo: String(item?.periodo ?? item?.Periodo ?? ''),
                valorEsperadoCOP: Number(item?.valorEsperadoCOP ?? item?.ValorEsperadoCOP ?? 0),
                saldoPendienteCOP: Number(item?.saldoPendienteCOP ?? item?.SaldoPendienteCOP ?? 0),
            }));
        },
    });
}

export function useGenerarCarteraMensual() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async (payload: GenerarCarteraPayload) => {
            try {
                return await apiClient.post('/api/cartera/generar-mensual', payload);
            } catch (error) {
                throw new Error(getErrorMessage(error, 'No fue posible generar la cartera mensual.'));
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['cartera', 'pendiente'] });
        },
    });
}

export function useRegistrarPagoCartera() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async ({ id, ...payload }: RegistrarPagoPayload) => {
            try {
                await apiClient.post(`/api/cartera/${id}/pago`, payload);
            } catch (error) {
                throw new Error(getErrorMessage(error, 'No fue posible registrar el pago.'));
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['cartera', 'pendiente'] });
        },
    });
}
