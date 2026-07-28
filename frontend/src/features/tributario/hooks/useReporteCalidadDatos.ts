'use client';

import { useQuery } from '@tanstack/react-query';
import apiClient, { type RespuestaApi } from '@/lib/apiClient';

export type InconsistenciaTributariaItem = {
    terceroId: string;
    nombreObtenido: string;
    tipoRelacion: 'Miembro' | 'Donante' | string;
    descripcionInconsistencia: string;
};

type UseReporteCalidadDatosParams = {
    enabled?: boolean;
};

export function useReporteCalidadDatos({ enabled = true }: UseReporteCalidadDatosParams = {}) {
    return useQuery<InconsistenciaTributariaItem[]>({
        queryKey: ['tributario', 'calidad-datos'],
        queryFn: async () => {
            const response = await apiClient.get<RespuestaApi[]>('/api/tributario/calidad-datos');

            return (response.data ?? []).map((item) => ({
                terceroId: String(item?.terceroId ?? ''),
                nombreObtenido: String(item?.nombreObtenido ?? ''),
                tipoRelacion: String(item?.tipoRelacion ?? ''),
                descripcionInconsistencia: String(item?.descripcionInconsistencia ?? ''),
            }));
        },
        enabled,
    });
}
