'use client';

import apiClient, { mensajeDeError, type RespuestaApi } from '@/lib/apiClient';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

/**
 * Cuentas por pagar a proveedores (historias 1-13 y 1-14).
 *
 * Es el reflejo de cartera: la factura se reconoce cuando llega, no cuando se
 * paga. Antes una obligacion pendiente no existia en ninguna parte hasta el
 * pago, asi que el pasivo del capitulo quedaba fuera del balance.
 */
export type CuentaPorPagar = {
    id: string;
    nombreProveedor: string;
    nitProveedor: string;
    numeroFactura: string;
    concepto: string;
    codigoCuentaGasto: string;
    nombreCentroCosto: string;
    fechaEmision: string;
    fechaVencimiento: string;
    valorTotal: number;
    saldoPendiente: number;
    estado: number;
    estaVencida: boolean;
};

export const NOMBRE_ESTADO_CXP: Record<number, string> = {
    1: 'Pendiente',
    2: 'Pago parcial',
    3: 'Pagada',
    4: 'Anulada',
};

export type RegistrarCuentaPorPagarPayload = {
    nombreProveedor: string;
    nitProveedor: string;
    numeroFactura: string;
    concepto: string;
    cuentaContableGastoId: string;
    centroCostoId: string;
    fechaEmision: string;
    fechaVencimiento: string;
    valorTotal: number;
};

const CLAVE = ['cuentas-por-pagar'] as const;

export function useCuentasPorPagar(incluirAnuladas = false) {
    return useQuery<CuentaPorPagar[]>({
        queryKey: [...CLAVE, incluirAnuladas],
        queryFn: async () => {
            const response = await apiClient.get<RespuestaApi[]>('/api/cuentas-por-pagar', {
                params: { incluirAnuladas },
            });

            return (response.data ?? []).map((c) => ({
                id: String(c?.id ?? ''),
                nombreProveedor: String(c?.nombreProveedor ?? ''),
                nitProveedor: String(c?.nitProveedor ?? ''),
                numeroFactura: String(c?.numeroFactura ?? ''),
                concepto: String(c?.concepto ?? ''),
                codigoCuentaGasto: String(c?.codigoCuentaGasto ?? ''),
                nombreCentroCosto: String(c?.nombreCentroCosto ?? ''),
                fechaEmision: String(c?.fechaEmision ?? ''),
                fechaVencimiento: String(c?.fechaVencimiento ?? ''),
                valorTotal: Number(c?.valorTotal ?? 0),
                saldoPendiente: Number(c?.saldoPendiente ?? 0),
                estado: Number(c?.estado ?? 0),
                estaVencida: Boolean(c?.estaVencida),
            }));
        },
    });
}

function useInvalidar() {
    const queryClient = useQueryClient();

    return () => {
        void queryClient.invalidateQueries({ queryKey: CLAVE });
        // El pago mueve el banco, asi que sus saldos tambien quedan viejos.
        void queryClient.invalidateQueries({ queryKey: ['tesoreria'] });
        void queryClient.invalidateQueries({ queryKey: ['dashboard'] });
    };
}

export function useRegistrarCuentaPorPagar() {
    const invalidar = useInvalidar();

    return useMutation({
        mutationFn: async (payload: RegistrarCuentaPorPagarPayload) => {
            try {
                await apiClient.post('/api/cuentas-por-pagar', payload);
            } catch (error) {
                throw new Error(mensajeDeError(error, 'No fue posible registrar la factura.'));
            }
        },
        onSuccess: invalidar,
    });
}

export function usePagarCuentaPorPagar() {
    const invalidar = useInvalidar();

    return useMutation({
        mutationFn: async ({
            id,
            monto,
            bancoId,
            medioPago,
        }: {
            id: string;
            monto: number;
            bancoId: string;
            medioPago: number;
        }) => {
            try {
                await apiClient.post(`/api/cuentas-por-pagar/${id}/pagos`, { monto, bancoId, medioPago });
            } catch (error) {
                throw new Error(mensajeDeError(error, 'No fue posible registrar el pago.'));
            }
        },
        onSuccess: invalidar,
    });
}

export function useAnularCuentaPorPagar() {
    const invalidar = useInvalidar();

    return useMutation({
        mutationFn: async (id: string) => {
            try {
                await apiClient.post(`/api/cuentas-por-pagar/${id}/anular`);
            } catch (error) {
                throw new Error(mensajeDeError(error, 'No fue posible anular la factura.'));
            }
        },
        onSuccess: invalidar,
    });
}
