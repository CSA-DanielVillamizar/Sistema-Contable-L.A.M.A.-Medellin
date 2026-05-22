import apiClient from '@/lib/apiClient';
import { useQuery } from '@tanstack/react-query';

export type TransaccionItem = {
    id: string;
    fecha: string;
    tipo: 'Ingreso' | 'Egreso' | string;
    montoCOP: number;
    descripcion: string;
    centroCosto: string;
    banco: string;
};

export const useTransacciones = () => {
    return useQuery<TransaccionItem[]>({
        queryKey: ['transacciones', 'listado'],
        queryFn: async () => {
            const response = await apiClient.get<Record<string, unknown>[]>('/api/transacciones');
            // Mapear los campos con mayúsculas del backend a minúsculas para el frontend
            return response.data.map((item: Record<string, unknown>): TransaccionItem => ({
                id: String(item.Id ?? item.id ?? ''),
                fecha: String(item.Fecha ?? item.fecha ?? ''),
                tipo: String(item.Tipo ?? item.tipo ?? ''),
                montoCOP: Number(item.MontoCOP ?? item.montoCOP ?? 0),
                descripcion: String(item.Descripcion ?? item.descripcion ?? ''),
                centroCosto: String(item.CentroCosto ?? item.centroCosto ?? ''),
                banco: String(item.Banco ?? item.banco ?? ''),
            }));
        },
    });
};
