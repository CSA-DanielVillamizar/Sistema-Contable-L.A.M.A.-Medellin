'use client';

import axios from 'axios';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import apiClient from '@/lib/apiClient';
import type { DonacionFormValues, DonanteFormValues } from '@/features/donaciones/schemas/donacionSchema';

type ProblemDetails = {
    title?: string;
    detail?: string;
    errors?: Record<string, string[]>;
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

export type DonanteItem = {
    id: string;
    nombreORazonSocial: string;
    tipoDocumento: string;
    numeroDocumento: string;
    email: string;
    tipoPersona: string;
};

export type DonacionItem = {
    id: string;
    donanteId: string;
    nombreDonante: string;
    montoCOP: number;
    fecha: string;
    banco: string;
    centroCosto: string;
    certificadoEmitido: boolean;
    codigoVerificacion: string;
    formaDonacion: string;
    medioPagoODescripcion: string;
};

export type CertificadoDonacionItem = {
    fundacion: {
        nombre: string;
        nit: string;
        direccion: string;
        ciudad: string;
    };
    donante: {
        donanteId: string;
        nombreDonante: string;
        tipoDocumento: string;
        numeroDocumento: string;
        email: string;
    };
    monto: {
        valorCOP: number;
        enLetras: string;
    };
    formaDonacion: string;
    medioPagoODescripcion: string;
    anioGravable: number;
    fecha: string;
    codigoVerificacion: string;
};

type CrearResponse = {
    id: string;
};

export function useDonantes() {
    return useQuery<DonanteItem[]>({
        queryKey: ['donaciones', 'donantes'],
        queryFn: async () => {
            const response = await apiClient.get<any[]>('/api/donaciones/donantes');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? item?.Id ?? ''),
                nombreORazonSocial: String(item?.nombreORazonSocial ?? item?.NombreORazonSocial ?? ''),
                tipoDocumento: String(item?.tipoDocumento ?? item?.TipoDocumento ?? ''),
                numeroDocumento: String(item?.numeroDocumento ?? item?.NumeroDocumento ?? ''),
                email: String(item?.email ?? item?.Email ?? ''),
                tipoPersona: String(item?.tipoPersona ?? item?.TipoPersona ?? ''),
            }));
        },
    });
}

export function useDonaciones() {
    return useQuery<DonacionItem[]>({
        queryKey: ['donaciones', 'listado'],
        queryFn: async () => {
            const response = await apiClient.get<any[]>('/api/donaciones');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? item?.Id ?? ''),
                donanteId: String(item?.donanteId ?? item?.DonanteId ?? ''),
                nombreDonante: String(item?.nombreDonante ?? item?.NombreDonante ?? ''),
                montoCOP: Number(item?.montoCOP ?? item?.MontoCOP ?? 0),
                fecha: String(item?.fecha ?? item?.Fecha ?? ''),
                banco: String(item?.banco ?? item?.Banco ?? ''),
                centroCosto: String(item?.centroCosto ?? item?.CentroCosto ?? ''),
                certificadoEmitido: Boolean(item?.certificadoEmitido ?? item?.CertificadoEmitido ?? false),
                codigoVerificacion: String(item?.codigoVerificacion ?? item?.CodigoVerificacion ?? ''),
                formaDonacion: String(item?.formaDonacion ?? item?.FormaDonacion ?? ''),
                medioPagoODescripcion: String(item?.medioPagoODescripcion ?? item?.MedioPagoODescripcion ?? ''),
            }));
        },
    });
}

export function useCrearDonante() {
    const queryClient = useQueryClient();

    return useMutation<CrearResponse, Error, DonanteFormValues>({
        mutationFn: async (payload) => {
            try {
                const response = await apiClient.post<CrearResponse>('/api/donaciones/donantes', payload);
                return response.data;
            } catch (error) {
                throw mapError(error, 'No fue posible registrar el donante.');
            }
        },
        onSuccess: async () => {
            await queryClient.invalidateQueries({ queryKey: ['donaciones', 'donantes'] });
        },
    });
}

export function useCertificadoDonacion(id?: string) {
    return useQuery<CertificadoDonacionItem | null>({
        queryKey: ['donaciones', 'certificado', id],
        enabled: Boolean(id),
        queryFn: async () => {
            if (!id) {
                return null;
            }

            const response = await apiClient.get<any>(`/api/donaciones/${id}/certificado`);
            const item = response.data ?? {};

            return {
                fundacion: {
                    nombre: String(item?.fundacion?.nombre ?? item?.Fundacion?.Nombre ?? ''),
                    nit: String(item?.fundacion?.nit ?? item?.Fundacion?.Nit ?? ''),
                    direccion: String(item?.fundacion?.direccion ?? item?.Fundacion?.Direccion ?? ''),
                    ciudad: String(item?.fundacion?.ciudad ?? item?.Fundacion?.Ciudad ?? ''),
                },
                donante: {
                    donanteId: String(item?.donante?.donanteId ?? item?.Donante?.DonanteId ?? ''),
                    nombreDonante: String(item?.donante?.nombreDonante ?? item?.Donante?.NombreDonante ?? ''),
                    tipoDocumento: String(item?.donante?.tipoDocumento ?? item?.Donante?.TipoDocumento ?? ''),
                    numeroDocumento: String(item?.donante?.numeroDocumento ?? item?.Donante?.NumeroDocumento ?? ''),
                    email: String(item?.donante?.email ?? item?.Donante?.Email ?? ''),
                },
                monto: {
                    valorCOP: Number(item?.monto?.valorCOP ?? item?.Monto?.ValorCOP ?? 0),
                    enLetras: String(item?.monto?.enLetras ?? item?.Monto?.EnLetras ?? ''),
                },
                formaDonacion: String(item?.formaDonacion ?? item?.FormaDonacion ?? ''),
                medioPagoODescripcion: String(item?.medioPagoODescripcion ?? item?.MedioPagoODescripcion ?? ''),
                anioGravable: Number(item?.anioGravable ?? item?.AnioGravable ?? 2026),
                fecha: String(item?.fecha ?? item?.Fecha ?? ''),
                codigoVerificacion: String(item?.codigoVerificacion ?? item?.CodigoVerificacion ?? ''),
            };
        },
    });
}

export function useRegistrarDonacion() {
    const queryClient = useQueryClient();

    return useMutation<CrearResponse, Error, DonacionFormValues>({
        mutationFn: async (payload) => {
            try {
                const response = await apiClient.post<CrearResponse>('/api/donaciones', payload);
                return response.data;
            } catch (error) {
                throw mapError(error, 'No fue posible registrar la donación.');
            }
        },
        onSuccess: async () => {
            await Promise.all([
                queryClient.invalidateQueries({ queryKey: ['donaciones', 'listado'] }),
                queryClient.invalidateQueries({ queryKey: ['dashboard', 'bancos'] }),
            ]);
        },
    });
}
