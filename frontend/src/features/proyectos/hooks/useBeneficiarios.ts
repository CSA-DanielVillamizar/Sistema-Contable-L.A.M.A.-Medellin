'use client';

import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import apiClient, { type RespuestaApi, mensajeDeError } from '@/lib/apiClient';
import type { BeneficiarioFormValues } from '@/features/proyectos/schemas/beneficiarioSchema';

type CrearResponse = {
    id: string;
};

export type BeneficiarioItem = {
    id: string;
    proyectoSocialId: string;
    nombreCompleto: string;
    tipoDocumento: string;
    numeroDocumento: string;
    email: string;
    telefono: string;
    tieneConsentimientoHabeasData: boolean;
};

function mapError(error: unknown, fallback: string): Error {
    return new Error(mensajeDeError(error, fallback));
}

export function useBeneficiarios() {
    return useQuery<BeneficiarioItem[]>({
        queryKey: ['beneficiarios', 'listado'],
        queryFn: async () => {
            const response = await apiClient.get<RespuestaApi[]>('/api/beneficiarios');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? ''),
                proyectoSocialId: String(item?.proyectoSocialId ?? ''),
                nombreCompleto: String(item?.nombreCompleto ?? ''),
                tipoDocumento: String(item?.tipoDocumento ?? ''),
                numeroDocumento: String(item?.numeroDocumento ?? ''),
                email: String(item?.email ?? ''),
                telefono: String(item?.telefono ?? ''),
                tieneConsentimientoHabeasData: Boolean(item?.tieneConsentimientoHabeasData ?? false),
            }));
        },
    });
}

export function useCrearBeneficiario() {
    const queryClient = useQueryClient();

    return useMutation<CrearResponse, Error, BeneficiarioFormValues>({
        mutationFn: async (payload) => {
            try {
                const response = await apiClient.post<CrearResponse>('/api/beneficiarios', payload);
                return response.data;
            } catch (error) {
                throw mapError(error, 'No fue posible registrar el beneficiario.');
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['beneficiarios', 'listado'] });
        },
    });
}
