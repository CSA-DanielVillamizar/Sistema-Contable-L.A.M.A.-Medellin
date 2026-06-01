import { getMovimientosProducto, type MovimientoProducto } from '@/features/merchandising/services/merchandisingService';
import { useQuery } from '@tanstack/react-query';

export function useGetMovimientosProducto(productoId: string | null) {
    return useQuery<MovimientoProducto[]>({
        queryKey: ['merchandising', 'productos', productoId, 'movimientos'],
        queryFn: () => getMovimientosProducto(productoId as string),
        enabled: Boolean(productoId),
    });
}
