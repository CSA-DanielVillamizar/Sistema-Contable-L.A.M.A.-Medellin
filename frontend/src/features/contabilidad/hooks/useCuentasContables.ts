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

function toNullableString(value: unknown): string | null {
    return typeof value === 'string' ? value : null;
}

function mapCuenta(item: Record<string, unknown>): CuentaContableItem {
    return {
        id: String(item?.id ?? item?.Id ?? ''),
        codigo: String(item?.codigo ?? item?.Codigo ?? ''),
        descripcion: String(item?.descripcion ?? item?.Descripcion ?? ''),
        nivel: Number(item?.nivel ?? item?.Nivel ?? 0),
        nivelNombre: String(item?.nivelNombre ?? item?.NivelNombre ?? ''),
        naturalezaNombre: String(item?.naturalezaNombre ?? item?.NaturalezaNombre ?? ''),
        permiteMovimiento: Boolean(item?.permiteMovimiento ?? item?.PermiteMovimiento ?? false),
        exigeTercero: Boolean(item?.exigeTercero ?? item?.ExigeTercero ?? false),
        cuentaPadreId: toNullableString(item?.cuentaPadreId) ?? toNullableString(item?.CuentaPadreId),
    };
}

export function useCuentasContables() {
    return useQuery<CuentaContableItem[]>({
        queryKey: ['contabilidad', 'cuentas'],
        queryFn: async () => {
            const response = await apiClient.get<Record<string, unknown>[]>('/api/cuentas-contables');
            return (response.data ?? []).map(mapCuenta).filter((c) => c.codigo.length > 0);
        },
    });
}

export function useCuentasAsentables() {
    return useQuery<CuentaContableItem[]>({
        queryKey: ['contabilidad', 'cuentas', 'asentables'],
        queryFn: async () => {
            const response = await apiClient.get<Record<string, unknown>[]>('/api/cuentas-contables/asentables');
            return (response.data ?? []).map(mapCuenta).filter((c) => c.codigo.length > 0);
        },
    });
}
