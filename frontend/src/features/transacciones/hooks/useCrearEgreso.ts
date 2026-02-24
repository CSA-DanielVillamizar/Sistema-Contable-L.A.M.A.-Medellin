import axios from 'axios';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import apiClient from '@/lib/apiClient';

export type CrearEgresoRequest = {
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

type CrearEgresoResponse = {
  id: string;
};

type ProblemDetails = {
  title?: string;
  detail?: string;
  errors?: Record<string, string[]>;
};

export const useCrearEgreso = () => {
  const queryClient = useQueryClient();

  return useMutation<CrearEgresoResponse, Error, CrearEgresoRequest>({
    mutationFn: async (request) => {
      if (!request) {
        throw new Error('No se recibieron datos del formulario de egreso.');
      }

      try {
        const response = await apiClient.post<CrearEgresoResponse>('/api/transacciones/egreso', request);
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
            'No fue posible registrar el egreso.';
          throw new Error(mensaje);
        }

        throw new Error('No fue posible registrar el egreso.');
      }
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    },
  });
};
