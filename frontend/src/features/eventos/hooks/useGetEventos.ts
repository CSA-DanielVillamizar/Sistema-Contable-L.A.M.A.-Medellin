'use client';

import { getEventos, type EventoDto } from '@/features/eventos/services/eventosService';
import { useQuery } from '@tanstack/react-query';

export function useGetEventos() {
    return useQuery<EventoDto[]>({
        queryKey: ['eventos'],
        queryFn: getEventos,
        staleTime: 1000 * 60,
    });
}
