'use client';

import axios from 'axios';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import apiClient from '@/lib/apiClient';
import type { ArticuloFormValues } from '@/features/merchandising/schemas/articuloSchema';

type ProblemDetails = {
    title?: string;
    detail?: string;
    errors?: Record<string, string[]>;
};

type CrearArticuloResponse = {
    id: string;
};

export type ArticuloItem = {
    id: string;
    nombre: string;
    sku: string;
    descripcion: string;
    categoria: string;
    precioVenta: number;
    costoPromedio: number;
    stockActual: number;
    cuentaContableIngresoId: string;
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

export function useArticulos() {
    return useQuery<ArticuloItem[]>({
        queryKey: ['merchandising', 'articulos'],
        queryFn: async () => {
            const response = await apiClient.get<any[]>('/api/articulos');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? item?.Id ?? ''),
                nombre: String(item?.nombre ?? item?.Nombre ?? ''),
                sku: String(item?.sku ?? item?.SKU ?? item?.Sku ?? ''),
                descripcion: String(item?.descripcion ?? item?.Descripcion ?? ''),
                categoria: String(item?.categoria ?? item?.Categoria ?? ''),
                precioVenta: Number(item?.precioVenta ?? item?.PrecioVenta ?? 0),
                costoPromedio: Number(item?.costoPromedio ?? item?.CostoPromedio ?? 0),
                stockActual: Number(item?.stockActual ?? item?.StockActual ?? 0),
                cuentaContableIngresoId: String(item?.cuentaContableIngresoId ?? item?.CuentaContableIngresoId ?? ''),
            }));
        },
    });
}

export function useCrearArticulo() {
    const queryClient = useQueryClient();

    return useMutation<CrearArticuloResponse, Error, ArticuloFormValues>({
        mutationFn: async (payload) => {
            try {
                const response = await apiClient.post<CrearArticuloResponse>('/api/articulos', payload);
                return response.data;
            } catch (error) {
                throw mapError(error, 'No fue posible registrar el artículo.');
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['merchandising', 'articulos'] });
        },
    });
}
