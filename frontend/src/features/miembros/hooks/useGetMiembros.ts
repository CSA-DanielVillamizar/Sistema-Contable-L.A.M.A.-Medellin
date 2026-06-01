'use client';

import { getMiembros, type Miembro } from '@/features/miembros/services/miembrosService';
import { useQuery } from '@tanstack/react-query';

export function useGetMiembros() {
    return useQuery<Miembro[]>({
        queryKey: ['miembros'],
        queryFn: getMiembros,
        staleTime: 1000 * 60,
    });
}
