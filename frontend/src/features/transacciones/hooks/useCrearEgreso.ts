import { useMutation, useQueryClient } from '@tanstack/react-query';
import apiClient, { mensajeDeError } from '@/lib/apiClient';

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
        throw new Error(mensajeDeError(error, 'No fue posible registrar el egreso.'));
      }
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    },
  });
};
