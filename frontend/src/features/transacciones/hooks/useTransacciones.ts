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
            const response = await apiClient.get<any[]>('/api/transacciones');
            // Mapear los campos con mayúsculas del backend a minúsculas para el frontend
            return response.data.map((item: any) => ({
                id: item.Id || item.id || '',
                fecha: item.Fecha || item.fecha || '',
                tipo: item.Tipo || item.tipo || '',
                montoCOP: item.MontoCOP || item.montoCOP || 0,
                descripcion: item.Descripcion || item.descripcion || '',
                centroCosto: item.CentroCosto || item.centroCosto || '',
                banco: item.Banco || item.banco || '',
            }));
        },
    });
};
