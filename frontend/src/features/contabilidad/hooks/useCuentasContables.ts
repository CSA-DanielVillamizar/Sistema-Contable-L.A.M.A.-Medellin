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
                id: String(item?.id ?? ''),
                codigo: String(item?.codigo ?? ''),
                descripcion: String(item?.descripcion ?? ''),
                naturaleza: String(item?.naturaleza ?? ''),
                permiteMovimiento: Boolean(item?.permiteMovimiento ?? false),
                exigeTercero: Boolean(item?.exigeTercero ?? false),
            }));
        },
    });
}
