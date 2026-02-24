import { useQuery } from '@tanstack/react-query';
import apiClient from '@/lib/apiClient';

export type CuentaContableItem = {
    id: string;
    codigo: string;
    descripcion: string;
    nivel: number;
    nivelNombre: string;
    naturalezaNombre: string;
    permiteMovimiento: boolean;
    exigeTercero: boolean;
    cuentaPadreId: string | null;
};

function mapCuenta(item: any): CuentaContableItem {
    return {
        id: String(item?.id ?? item?.Id ?? ''),
        codigo: String(item?.codigo ?? item?.Codigo ?? ''),
        descripcion: String(item?.descripcion ?? item?.Descripcion ?? ''),
        nivel: Number(item?.nivel ?? item?.Nivel ?? 0),
        nivelNombre: String(item?.nivelNombre ?? item?.NivelNombre ?? ''),
        naturalezaNombre: String(item?.naturalezaNombre ?? item?.NaturalezaNombre ?? ''),
        permiteMovimiento: Boolean(item?.permiteMovimiento ?? item?.PermiteMovimiento ?? false),
        exigeTercero: Boolean(item?.exigeTercero ?? item?.ExigeTercero ?? false),
        cuentaPadreId: item?.cuentaPadreId ?? item?.CuentaPadreId ?? null,
    };
}

export function useCuentasContables() {
    return useQuery<CuentaContableItem[]>({
        queryKey: ['contabilidad', 'cuentas'],
        queryFn: async () => {
            const response = await apiClient.get<any[]>('/api/cuentas-contables');
            return (response.data ?? []).map(mapCuenta).filter((c) => c.codigo.length > 0);
        },
    });
}

export function useCuentasAsentables() {
    return useQuery<CuentaContableItem[]>({
        queryKey: ['contabilidad', 'cuentas', 'asentables'],
        queryFn: async () => {
            const response = await apiClient.get<any[]>('/api/cuentas-contables/asentables');
            return (response.data ?? []).map(mapCuenta).filter((c) => c.codigo.length > 0);
        },
    });
}
