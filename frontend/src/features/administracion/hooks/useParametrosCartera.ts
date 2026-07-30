'use client';

import apiClient, { mensajeDeError, type RespuestaApi } from '@/lib/apiClient';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

/**
 * Parametros de cartera: tarifas por tipo de afiliacion (historia 0-7) y la
 * cuota de renovacion de membresia internacional (historia 1-9).
 *
 * El backend existia pero no habia pantalla, asi que el valor de la cuota solo
 * se podia cambiar tocando el API a mano. El cliente pidio expresamente que el
 * Tesorero confirme o actualice el valor antes de que el sistema genere la
 * cartera masiva del mes.
 */
export const TIPOS_AFILIACION = [
    { valor: 1, nombre: 'Full Member Color' },
    { valor: 2, nombre: 'Rockets Prospect' },
    { valor: 3, nombre: 'Prospect' },
    { valor: 4, nombre: 'Spousal (esposa)', exento: true },
    { valor: 5, nombre: 'Asociado' },
    { valor: 6, nombre: 'Lady L.A.M.A.' },
] as const;

export type TarifaCuota = {
    tipoAfiliacion: number;
    valorMensualCOP: number;
};

const CLAVE = ['administracion', 'tarifas'] as const;

export function useTarifasCuota() {
    return useQuery<TarifaCuota[]>({
        queryKey: CLAVE,
        queryFn: async () => {
            const response = await apiClient.get<RespuestaApi[]>('/api/configuracion/tarifas');

            return (response.data ?? []).map((t) => ({
                tipoAfiliacion: Number(t?.tipoAfiliacion ?? 0),
                valorMensualCOP: Number(t?.valorMensualCOP ?? 0),
            }));
        },
    });
}

export function useActualizarTarifas() {
    const queryClient = useQueryClient();

    return useMutation({
        mutationFn: async (tarifas: TarifaCuota[]) => {
            try {
                await apiClient.put('/api/configuracion/tarifas', { tarifas });
            } catch (error) {
                throw new Error(mensajeDeError(error, 'No fue posible guardar las tarifas.'));
            }
        },
        onSuccess: () => {
            void queryClient.invalidateQueries({ queryKey: CLAVE });
            void queryClient.invalidateQueries({ queryKey: ['cartera'] });
        },
    });
}

export function useActualizarRenovacionMembresia() {
    return useMutation({
        mutationFn: async ({ anio, valorUSD }: { anio: number; valorUSD: number | null }) => {
            try {
                await apiClient.patch(`/api/configuracion/cuotas-asamblea/${anio}/renovacion-membresia`, {
                    renovacionMembresiaUSD: valorUSD,
                });
            } catch (error) {
                throw new Error(mensajeDeError(error, 'No fue posible guardar la cuota de renovacion.'));
            }
        },
    });
}
