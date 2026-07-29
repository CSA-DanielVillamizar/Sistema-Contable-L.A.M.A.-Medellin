'use client';

import apiClient, { type RespuestaApi, mensajeDeError } from '@/lib/apiClient';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

export type CarteraPendienteItem = {
    id: string;
    miembroId: string;
    nombreMiembro: string;
    periodo: string;
    valorEsperadoCOP: number;
    saldoPendienteCOP: number;
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
    return mensajeDeError(error, fallback);
}

export function useCarteraPendiente() {
    return useQuery<CarteraPendienteItem[]>({
        queryKey: ['cartera', 'pendiente'],
        queryFn: async () => {
            const response = await apiClient.get<RespuestaApi[]>('/api/cartera/pendiente');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? ''),
                miembroId: String(item?.miembroId ?? ''),
                nombreMiembro: String(item?.nombreMiembro ?? ''),
                periodo: String(item?.periodo ?? ''),
                valorEsperadoCOP: Number(item?.valorEsperadoCOP ?? 0),
                saldoPendienteCOP: Number(item?.saldoPendienteCOP ?? 0),
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
                await apiClient.post(`/api/cartera/cuentas-por-cobrar/${id}/pagos`, {
                    monto: payload.MontoPagadoCOP,
                    bancoId: payload.BancoId,
                });
            } catch (error) {
                throw new Error(getErrorMessage(error, 'No fue posible registrar el pago.'));
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['cartera', 'pendiente'] });
        },
    });
}
