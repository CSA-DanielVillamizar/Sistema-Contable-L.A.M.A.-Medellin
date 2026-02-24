import axios from 'axios';
import { useMutation } from '@tanstack/react-query';
import apiClient from '@/lib/apiClient';

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

type ProblemDetails = {
    title?: string;
    detail?: string;
    errors?: Record<string, string[]>;
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
                if (axios.isAxiosError<ProblemDetails>(error)) {
                    const validationErrors = error.response?.data?.errors;
                    const firstValidationError = validationErrors
                        ? Object.values(validationErrors).flat().find((message) => message)
                        : undefined;

                    const mensaje =
                        firstValidationError ??
                        error.response?.data?.detail ??
                        error.response?.data?.title ??
                        'No fue posible registrar el ingreso.';
                    throw new Error(mensaje);
                }

                throw new Error('No fue posible registrar el ingreso.');
            }
        },
    });
};
