import apiClient from '@/lib/apiClient';
import { useQuery } from '@tanstack/react-query';

export type CuentaContableItem = {
    id: string;
    codigo: string;
    descripcion: string;
    naturaleza: string;
    permiteMovimiento: boolean;
    exigeTercero: boolean;
};

export function useCuentasContables() {
    return useQuery<CuentaContableItem[]>({
        queryKey: ['contabilidad', 'cuentas-contables'],
        queryFn: async () => {
            const response = await apiClient.get<any[]>('/api/cuentas-contables');
            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? item?.Id ?? ''),
                codigo: String(item?.codigo ?? item?.Codigo ?? ''),
                descripcion: String(item?.descripcion ?? item?.Descripcion ?? ''),
                naturaleza: String(item?.naturaleza ?? item?.Naturaleza ?? ''),
                permiteMovimiento: Boolean(item?.permiteMovimiento ?? item?.PermiteMovimiento ?? false),
                exigeTercero: Boolean(item?.exigeTercero ?? item?.ExigeTercero ?? false),
            }));
        },
    });
}
