'use client';

import apiClient, { mensajeDeError, type RespuestaApi } from '@/lib/apiClient';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

/**
 * Solicitudes de anulacion (historia 1-8).
 *
 * Son dos papeles distintos: el Operador solicita con motivo y el Tesorero
 * resuelve. Que la misma persona hiciera ambas cosas anularia el control, asi
 * que la pantalla los separa igual que lo hace el backend.
 */
export type SolicitudAnulacion = {
    id: string;
    comprobanteId: string;
    numeroConsecutivo: string;
    fechaComprobante: string;
    descripcionComprobante: string;
    motivoSolicitud: string;
    estado: number;
    solicitadaPor: string | null;
    fechaSolicitud: string | null;
    resueltaPor: string | null;
    fechaResolucion: string | null;
    motivoResolucion: string | null;
};

export const NOMBRE_ESTADO_ANULACION: Record<number, string> = {
    1: 'Pendiente',
    2: 'Aprobada',
    3: 'Rechazada',
};

const CLAVE = ['contabilidad', 'anulaciones'] as const;

export function useSolicitudesAnulacion() {
    return useQuery<SolicitudAnulacion[]>({
        queryKey: CLAVE,
        queryFn: async () => {
            const response = await apiClient.get<RespuestaApi[]>('/api/anulaciones');

            return (response.data ?? []).map((s) => ({
                id: String(s?.id ?? ''),
                comprobanteId: String(s?.comprobanteId ?? ''),
                numeroConsecutivo: String(s?.numeroConsecutivo ?? ''),
                fechaComprobante: String(s?.fechaComprobante ?? ''),
                descripcionComprobante: String(s?.descripcionComprobante ?? ''),
                motivoSolicitud: String(s?.motivoSolicitud ?? ''),
                estado: Number(s?.estado ?? 0),
                solicitadaPor: s?.solicitadaPor ? String(s.solicitadaPor) : null,
                fechaSolicitud: s?.fechaSolicitud ? String(s.fechaSolicitud) : null,
                resueltaPor: s?.resueltaPor ? String(s.resueltaPor) : null,
                fechaResolucion: s?.fechaResolucion ? String(s.fechaResolucion) : null,
                motivoResolucion: s?.motivoResolucion ? String(s.motivoResolucion) : null,
            }));
        },
    });
}

function useInvalidar() {
    const queryClient = useQueryClient();

    return () => {
        void queryClient.invalidateQueries({ queryKey: CLAVE });
        // Aprobar anula el comprobante, asi que el listado tambien cambia.
        void queryClient.invalidateQueries({ queryKey: ['contabilidad', 'comprobantes'] });
    };
}

export function useSolicitarAnulacion() {
    const invalidar = useInvalidar();

    return useMutation({
        mutationFn: async ({ comprobanteId, motivo }: { comprobanteId: string; motivo: string }) => {
            try {
                await apiClient.post('/api/anulaciones', { comprobanteId, motivo });
            } catch (error) {
                throw new Error(mensajeDeError(error, 'No fue posible solicitar la anulacion.'));
            }
        },
        onSuccess: invalidar,
    });
}

export function useResolverAnulacion() {
    const invalidar = useInvalidar();

    return useMutation({
        mutationFn: async ({
            id,
            aprobar,
            motivo,
        }: {
            id: string;
            aprobar: boolean;
            motivo: string | null;
        }) => {
            try {
                await apiClient.post(`/api/anulaciones/${id}/resolver`, { aprobar, motivo });
            } catch (error) {
                throw new Error(mensajeDeError(error, 'No fue posible resolver la solicitud.'));
            }
        },
        onSuccess: invalidar,
    });
}
