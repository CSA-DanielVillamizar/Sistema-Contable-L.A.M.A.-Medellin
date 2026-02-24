import { useQuery } from '@tanstack/react-query';

export type TrmActualDto = {
    tasaCambioUsada: number;
    fechaTasaCambio: string;
    fuenteTasaCambio: number;
    fuenteNombre: string;
};

export function useTrmOficial(enabled: boolean) {
    return useQuery({
        queryKey: ['transacciones', 'trm', 'actual'],
        enabled,
        queryFn: async (): Promise<TrmActualDto> => {
            const response = await fetch('/api/trm/actual', {
                method: 'GET',
                headers: {
                    Accept: 'application/json',
                },
            });

            const payload = (await response.json()) as Partial<TrmActualDto> & { message?: string };

            if (!response.ok) {
                throw new Error(payload.message ?? 'No fue posible consultar la TRM oficial.');
            }

            if (
                typeof payload.tasaCambioUsada !== 'number' ||
                typeof payload.fechaTasaCambio !== 'string' ||
                typeof payload.fuenteTasaCambio !== 'number'
            ) {
                throw new Error('La respuesta de TRM oficial no tiene el formato esperado.');
            }

            return {
                tasaCambioUsada: payload.tasaCambioUsada,
                fechaTasaCambio: payload.fechaTasaCambio,
                fuenteTasaCambio: payload.fuenteTasaCambio,
                fuenteNombre: payload.fuenteNombre ?? 'TRM SFC',
            };
        },
        staleTime: 1000 * 60 * 10,
    });
}
