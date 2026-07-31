'use client';

import apiClient, { mensajeDeError, type RespuestaApi } from '@/lib/apiClient';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

/**
 * Actividades de proyecto y rendicion de cuentas (historias 3-1 y 3-4).
 *
 * Lo ejecutado lo calcula el backend desde los asientos imputados al centro de
 * costo del proyecto. Aqui no se recalcula: es la unica cifra que el libro
 * respalda y tener dos versiones seria peor que no tener ninguna.
 */
export type Actividad = {
    id: string;
    proyectoSocialId: string;
    nombre: string;
    descripcion: string;
    fechaInicioPlanificada: string;
    fechaFinPlanificada: string;
    presupuestoAsignado: number;
    estado: number;
    nombreEstado: string;
    responsable: string | null;
    estaVencida: boolean;
};

export type Rendicion = {
    proyectoSocialId: string;
    nombre: string;
    estado: string;
    presupuestoEstimado: number;
    presupuestoAsignadoAActividades: number;
    ejecutadoCOP: number;
    disponibleCOP: number;
    porcentajeEjecucion: number;
    totalActividades: number;
    actividadesCompletadas: number;
    actividadesVencidas: number;
    porcentajeAvanceActividades: number;
    totalBeneficiarios: number;
};

export type ActividadPayload = {
    nombre: string;
    descripcion: string;
    fechaInicioPlanificada: string;
    fechaFinPlanificada: string;
    presupuestoAsignado: number;
    responsable: string | null;
};

export const ESTADOS_ACTIVIDAD = [
    { valor: 1, nombre: 'Planificada' },
    { valor: 2, nombre: 'En ejecución' },
    { valor: 3, nombre: 'Completada' },
    { valor: 4, nombre: 'Cancelada' },
] as const;

export function useRendicion(proyectoSocialId?: string) {
    return useQuery<Rendicion[]>({
        queryKey: ['proyectos', 'rendicion', proyectoSocialId ?? 'todos'],
        queryFn: async () => {
            const response = await apiClient.get<RespuestaApi[]>('/api/proyectos/rendicion', {
                params: { proyectoSocialId: proyectoSocialId || undefined },
            });

            return (response.data ?? []).map((r) => ({
                proyectoSocialId: String(r?.proyectoSocialId ?? ''),
                nombre: String(r?.nombre ?? ''),
                estado: String(r?.estado ?? ''),
                presupuestoEstimado: Number(r?.presupuestoEstimado ?? 0),
                presupuestoAsignadoAActividades: Number(r?.presupuestoAsignadoAActividades ?? 0),
                ejecutadoCOP: Number(r?.ejecutadoCOP ?? 0),
                disponibleCOP: Number(r?.disponibleCOP ?? 0),
                porcentajeEjecucion: Number(r?.porcentajeEjecucion ?? 0),
                totalActividades: Number(r?.totalActividades ?? 0),
                actividadesCompletadas: Number(r?.actividadesCompletadas ?? 0),
                actividadesVencidas: Number(r?.actividadesVencidas ?? 0),
                porcentajeAvanceActividades: Number(r?.porcentajeAvanceActividades ?? 0),
                totalBeneficiarios: Number(r?.totalBeneficiarios ?? 0),
            }));
        },
    });
}

export function useActividades(proyectoSocialId: string | null) {
    return useQuery<Actividad[]>({
        queryKey: ['proyectos', 'actividades', proyectoSocialId],
        enabled: Boolean(proyectoSocialId),
        queryFn: async () => {
            const response = await apiClient.get<RespuestaApi[]>(`/api/proyectos/${proyectoSocialId}/actividades`);

            return (response.data ?? []).map((a) => ({
                id: String(a?.id ?? ''),
                proyectoSocialId: String(a?.proyectoSocialId ?? ''),
                nombre: String(a?.nombre ?? ''),
                descripcion: String(a?.descripcion ?? ''),
                fechaInicioPlanificada: String(a?.fechaInicioPlanificada ?? ''),
                fechaFinPlanificada: String(a?.fechaFinPlanificada ?? ''),
                presupuestoAsignado: Number(a?.presupuestoAsignado ?? 0),
                estado: Number(a?.estado ?? 0),
                nombreEstado: String(a?.nombreEstado ?? ''),
                responsable: a?.responsable ? String(a.responsable) : null,
                estaVencida: Boolean(a?.estaVencida),
            }));
        },
    });
}

function useInvalidar() {
    const queryClient = useQueryClient();
    return () => {
        void queryClient.invalidateQueries({ queryKey: ['proyectos'] });
    };
}

export function useCrearActividad() {
    const invalidar = useInvalidar();

    return useMutation({
        mutationFn: async ({ proyectoSocialId, ...payload }: ActividadPayload & { proyectoSocialId: string }) => {
            try {
                await apiClient.post(`/api/proyectos/${proyectoSocialId}/actividades`, payload);
            } catch (error) {
                throw new Error(mensajeDeError(error, 'No fue posible crear la actividad.'));
            }
        },
        onSuccess: invalidar,
    });
}

export function useCambiarEstadoActividad() {
    const invalidar = useInvalidar();

    return useMutation({
        mutationFn: async ({ id, estado }: { id: string; estado: number }) => {
            try {
                await apiClient.patch(`/api/proyectos/actividades/${id}/estado`, { estado });
            } catch (error) {
                throw new Error(mensajeDeError(error, 'No fue posible cambiar el estado.'));
            }
        },
        onSuccess: invalidar,
    });
}
