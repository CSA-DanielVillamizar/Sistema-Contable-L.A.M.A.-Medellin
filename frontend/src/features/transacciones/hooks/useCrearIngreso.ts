import { useMutation } from '@tanstack/react-query';
import apiClient, { mensajeDeError } from '@/lib/apiClient';

export type CrearIngresoRequest = {
    MontoCOP: number;
    CentroCostoId: string;
    BancoId: string;
    MedioPago: number;
    Descripcion: string;
    MonedaOrigen?: string;
    MontoMonedaOrigen?: number;
    TasaCambioUsada?: number;
    FechaTasaCambio?: string;
    FuenteTasaCambio?: number;
};

type CrearIngresoResponse = {
    id: string;
};

export const useCrearIngreso = () => {
    return useMutation<CrearIngresoResponse, Error, CrearIngresoRequest>({
        mutationFn: async (request) => {
            if (!request) {
                throw new Error('No se recibieron datos del formulario de ingreso.');
            }

            try {
                const response = await apiClient.post<CrearIngresoResponse>('/api/transacciones/ingreso', request);
                return response.data;
            } catch (error) {
                throw new Error(mensajeDeError(error, 'No fue posible registrar el ingreso.'));
            }
        },
    });
};
