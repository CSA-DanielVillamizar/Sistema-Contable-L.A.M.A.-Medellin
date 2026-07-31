'use client';

import { useQuery } from '@tanstack/react-query';
import apiClient, { type RespuestaApi } from '@/lib/apiClient';

export type CentroCostoCatalogo = {
    id: string;
    nombre: string;
};

/**
 * Centros de costo para poblar un desplegable.
 *
 * Es el catalogo, no la administracion: lo puede consultar cualquiera que
 * registre un movimiento. Para crearlos o editarlos esta useCentrosCostoAdmin,
 * que exige rol.
 *
 * La misma consulta esta repetida a mano en siete pantallas mas. Comparten la
 * clave de cache, asi que no generan trafico de mas, pero conviene irlas
 * pasando por aqui.
 */
export function useCentrosCostoCatalogo(habilitado = true) {
    return useQuery<CentroCostoCatalogo[]>({
        queryKey: ['transacciones', 'catalogo', 'centros-costo'],
        enabled: habilitado,
        queryFn: async () => {
            const response = await apiClient.get<RespuestaApi[]>('/api/transacciones/centros-costo');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? ''),
                nombre: String(item?.nombre ?? ''),
            }));
        },
    });
}
