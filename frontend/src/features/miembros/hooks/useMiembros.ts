import { useQuery } from '@tanstack/react-query';
import apiClient from '@/lib/apiClient';

export type MiembroItem = {
    id: string;
    nombreCompleto: string;
    documento: string;
    email: string;
    telefono: string;
    tipoAfiliacion: string;
    estado: string;
};

function mapMiembro(item: any): MiembroItem {
    return {
        id: String(item?.id ?? item?.Id ?? ''),
        nombreCompleto: String(item?.nombreCompleto ?? item?.NombreCompleto ?? ''),
        documento: String(item?.documento ?? item?.Documento ?? ''),
        email: String(item?.email ?? item?.Email ?? ''),
        telefono: String(item?.telefono ?? item?.Telefono ?? ''),
        tipoAfiliacion: String(item?.tipoAfiliacion ?? item?.TipoAfiliacion ?? ''),
        estado: String(item?.estado ?? item?.Estado ?? ''),
    };
}

export const useMiembros = () => {
    return useQuery<MiembroItem[]>({
        queryKey: ['miembros', 'listado'],
        queryFn: async () => {
            const response = await apiClient.get<any[]>('/api/miembros');
            return (response.data ?? []).map(mapMiembro).filter((item) => item.id.length > 0);
        },
    });
};

export const useMiembroById = (id?: string) => {
    return useQuery<MiembroItem | null>({
        queryKey: ['miembros', 'detalle', id],
        queryFn: async () => {
            if (!id) {
                return null;
            }

            const response = await apiClient.get<any>(`/api/miembros/${id}`);
            return mapMiembro(response.data);
        },
        enabled: Boolean(id),
    });
};
