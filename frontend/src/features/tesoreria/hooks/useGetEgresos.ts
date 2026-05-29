'use client';

import { getEgresos, type EgresoTesoreria } from '@/features/tesoreria/services/tesoreriaService';
import { useQuery } from '@tanstack/react-query';

export function useGetEgresos() {
    return useQuery<EgresoTesoreria[]>({
        queryKey: ['tesoreria', 'egresos'],
        queryFn: getEgresos,
    });
}
