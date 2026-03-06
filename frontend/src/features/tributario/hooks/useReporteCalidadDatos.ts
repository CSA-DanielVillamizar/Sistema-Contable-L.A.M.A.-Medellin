'use client';

import { useQuery } from '@tanstack/react-query';
import apiClient from '@/lib/apiClient';

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
            const response = await apiClient.get<any[]>('/api/tributario/calidad-datos');

            return (response.data ?? []).map((item) => ({
                terceroId: String(item?.terceroId ?? item?.TerceroId ?? ''),
                nombreObtenido: String(item?.nombreObtenido ?? item?.NombreObtenido ?? ''),
                tipoRelacion: String(item?.tipoRelacion ?? item?.TipoRelacion ?? ''),
                descripcionInconsistencia: String(item?.descripcionInconsistencia ?? item?.DescripcionInconsistencia ?? ''),
            }));
        },
        enabled,
    });
}
