'use client';

import axios from 'axios';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import apiClient from '@/lib/apiClient';
import type { VentaFormValues } from '@/features/merchandising/schemas/ventaSchema';

type ProblemDetails = {
    title?: string;
    detail?: string;
    errors?: Record<string, string[]>;
};

type ProcesarVentaResponse = {
    id: string;
};

export type VentaHistorialItem = {
    id: string;
    fecha: string;
    numeroFacturaInterna: string;
    cliente: string;
    metodoPago: string;
    total: number;
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

export function useProcesarVenta() {
    const queryClient = useQueryClient();

    return useMutation<ProcesarVentaResponse, Error, VentaFormValues>({
        mutationFn: async (payload) => {
            try {
                const response = await apiClient.post<ProcesarVentaResponse>('/api/ventas', {
                    compradorId: payload.CompradorId,
                    metodoPago: payload.MetodoPago,
                    detalles: payload.Detalles.map((detalle) => ({
                        articuloId: detalle.ArticuloId,
                        cantidad: detalle.Cantidad,
                    })),
                });

                return response.data;
            } catch (error) {
                throw mapError(error, 'No fue posible registrar la venta.');
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['merchandising', 'articulos'] });
            await queryClient.invalidateQueries({ queryKey: ['merchandising', 'ventas'] });
        },
    });
}

export function useVentas() {
    return useQuery<VentaHistorialItem[]>({
        queryKey: ['merchandising', 'ventas'],
        queryFn: async () => {
            const response = await apiClient.get<any[]>('/api/ventas');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? item?.Id ?? ''),
                fecha: String(item?.fecha ?? item?.Fecha ?? ''),
                numeroFacturaInterna: String(item?.numeroFacturaInterna ?? item?.NumeroFacturaInterna ?? ''),
                cliente: String(item?.cliente ?? item?.Cliente ?? 'Consumidor final'),
                metodoPago: String(item?.metodoPago ?? item?.MetodoPago ?? ''),
                total: Number(item?.total ?? item?.Total ?? 0),
            }));
        },
    });
}
