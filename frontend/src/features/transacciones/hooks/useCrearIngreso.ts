import axios from 'axios';
import { useMutation } from '@tanstack/react-query';
import apiClient from '@/lib/apiClient';

export type CrearIngresoRequest = {
    MontoCOP: number;
    CentroCostoId: string;
    MedioPago: number;
    MonedaOrigen?: string;
    MontoMonedaOrigen?: number;
    TasaCambioUsada?: number;
    FuenteTasaCambio?: number;
};

type CrearIngresoResponse = {
    id: string;
};

type ProblemDetails = {
    title?: string;
    detail?: string;
};

export const useCrearIngreso = () => {
    return useMutation<CrearIngresoResponse, Error, CrearIngresoRequest>({
        mutationFn: async (request) => {
            try {
                const response = await apiClient.post<CrearIngresoResponse>('/api/transacciones/ingreso', request);
                return response.data;
            } catch (error) {
                if (axios.isAxiosError<ProblemDetails>(error)) {
                    const mensaje = error.response?.data?.detail ?? error.response?.data?.title ?? 'No fue posible registrar el ingreso.';
                    throw new Error(mensaje);
                }

                throw new Error('No fue posible registrar el ingreso.');
            }
        },
    });
};
