import apiClient, { type RespuestaApi } from '@/lib/apiClient';
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
            const response = await apiClient.get<RespuestaApi[]>('/api/transacciones');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? ''),
                fecha: String(item?.fecha ?? ''),
                tipo: String(item?.tipo ?? ''),
                montoCOP: Number(item?.montoCOP ?? 0),
                descripcion: String(item?.descripcion ?? ''),
                centroCosto: String(item?.centroCosto ?? ''),
                banco: String(item?.banco ?? ''),
            }));
        },
    });
};
