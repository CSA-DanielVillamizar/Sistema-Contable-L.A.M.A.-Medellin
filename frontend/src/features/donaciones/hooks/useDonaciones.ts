'use client';

import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import apiClient, { type RespuestaApi, mensajeDeError } from '@/lib/apiClient';
import type { DonacionFormValues, DonanteFormValues } from '@/features/donaciones/schemas/donacionSchema';

function mapError(error: unknown, fallback: string): Error {
    return new Error(mensajeDeError(error, fallback));
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
            const response = await apiClient.get<RespuestaApi[]>('/api/donaciones/donantes');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? ''),
                nombreORazonSocial: String(item?.nombreORazonSocial ?? ''),
                tipoDocumento: String(item?.tipoDocumento ?? ''),
                numeroDocumento: String(item?.numeroDocumento ?? ''),
                email: String(item?.email ?? ''),
                tipoPersona: String(item?.tipoPersona ?? ''),
            }));
        },
    });
}

/**
 * Filtros de la consulta de donaciones (historia 2-4).
 *
 * Todos son opcionales y se combinan entre si. Las fechas van en formato
 * ISO (aaaa-mm-dd), que es lo que produce un <input type="date"> y lo que
 * espera el DateOnly del backend.
 */
export type FiltrosDonaciones = {
    desde?: string;
    hasta?: string;
    donanteId?: string;
    centroCostoId?: string;
    certificadoEmitido?: boolean;
};

/** Deja fuera los filtros vacios para no mandar parametros sin valor. */
function aParametros(filtros: FiltrosDonaciones): Record<string, string> {
    return Object.fromEntries(
        Object.entries(filtros)
            .filter(([, valor]) => valor !== undefined && valor !== '')
            .map(([clave, valor]) => [clave, String(valor)]),
    );
}

export function useDonaciones(filtros: FiltrosDonaciones = {}) {
    const parametros = aParametros(filtros);

    return useQuery<DonacionItem[]>({
        // Los filtros forman parte de la clave: sin ellos, React Query servia
        // el resultado de un filtro anterior desde la cache al cambiar de
        // criterio, y la tabla mostraba datos que no correspondian.
        queryKey: ['donaciones', 'listado', parametros],
        queryFn: async () => {
            const response = await apiClient.get<RespuestaApi[]>('/api/donaciones', {
                params: parametros,
            });

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? ''),
                donanteId: String(item?.donanteId ?? ''),
                nombreDonante: String(item?.nombreDonante ?? ''),
                montoCOP: Number(item?.montoCOP ?? 0),
                fecha: String(item?.fecha ?? ''),
                banco: String(item?.banco ?? ''),
                centroCosto: String(item?.centroCosto ?? ''),
                certificadoEmitido: Boolean(item?.certificadoEmitido ?? false),
                codigoVerificacion: String(item?.codigoVerificacion ?? ''),
                formaDonacion: String(item?.formaDonacion ?? ''),
                medioPagoODescripcion: String(item?.medioPagoODescripcion ?? ''),
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

/**
 * Cuerpo crudo del certificado. Es anidado, asi que no basta con RespuestaApi:
 * se declaran los objetos internos para poder navegarlos con seguridad.
 */
type CertificadoApiDto = {
    fundacion?: RespuestaApi;
    donante?: RespuestaApi;
    monto?: RespuestaApi;
    formaDonacion?: unknown;
    medioPagoODescripcion?: unknown;
    anioGravable?: unknown;
    fecha?: unknown;
    codigoVerificacion?: unknown;
};

export function useCertificadoDonacion(id?: string) {
    return useQuery<CertificadoDonacionItem | null>({
        queryKey: ['donaciones', 'certificado', id],
        enabled: Boolean(id),
        queryFn: async () => {
            if (!id) {
                return null;
            }

            const response = await apiClient.get<CertificadoApiDto>(`/api/donaciones/${id}/certificado`);
            const item = response.data ?? {};

            return {
                fundacion: {
                    nombre: String(item?.fundacion?.nombre ?? ''),
                    nit: String(item?.fundacion?.nit ?? ''),
                    direccion: String(item?.fundacion?.direccion ?? ''),
                    ciudad: String(item?.fundacion?.ciudad ?? ''),
                },
                donante: {
                    donanteId: String(item?.donante?.donanteId ?? ''),
                    nombreDonante: String(item?.donante?.nombreDonante ?? ''),
                    tipoDocumento: String(item?.donante?.tipoDocumento ?? ''),
                    numeroDocumento: String(item?.donante?.numeroDocumento ?? ''),
                    email: String(item?.donante?.email ?? ''),
                },
                monto: {
                    valorCOP: Number(item?.monto?.valorCOP ?? 0),
                    enLetras: String(item?.monto?.enLetras ?? ''),
                },
                formaDonacion: String(item?.formaDonacion ?? ''),
                medioPagoODescripcion: String(item?.medioPagoODescripcion ?? ''),
                anioGravable: Number(item?.anioGravable ?? 2026),
                fecha: String(item?.fecha ?? ''),
                codigoVerificacion: String(item?.codigoVerificacion ?? ''),
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
