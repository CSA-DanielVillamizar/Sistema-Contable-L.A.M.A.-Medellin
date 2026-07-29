'use client';

import {
    actualizarCuentaBancaria,
    cambiarEstadoCuentaBancaria,
    crearCuentaBancaria,
    getCuentasBancarias,
    type ActualizarCuentaBancariaPayload,
    type CrearCuentaBancariaPayload,
    type CuentaBancaria,
} from '@/features/administracion/services/cuentasBancariasService';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';

const CLAVE_ADMIN = ['administracion', 'cuentas-bancarias'] as const;

/**
 * Se invalidan tambien las claves de tesoreria y del catalogo de transacciones:
 * los desplegables de ingresos, egresos y donaciones leen la misma lista desde
 * otro endpoint, y sin esto seguirian mostrando el estado anterior.
 */
function clavesAfectadas(): readonly (readonly string[])[] {
    return [
        CLAVE_ADMIN,
        ['tesoreria', 'cuentas-bancarias'],
        ['transacciones', 'catalogo', 'bancos'],
        ['dashboard', 'bancos'],
    ];
}

export function useCuentasBancariasAdmin() {
    return useQuery<CuentaBancaria[]>({
        queryKey: CLAVE_ADMIN,
        // Incluye las inactivas: es la unica pantalla desde donde se reactivan.
        queryFn: () => getCuentasBancarias(true),
    });
}

function useInvalidar() {
    const queryClient = useQueryClient();

    return () => {
        clavesAfectadas().forEach((clave) => {
            void queryClient.invalidateQueries({ queryKey: clave });
        });
    };
}

export function useCrearCuentaBancaria() {
    const invalidar = useInvalidar();

    return useMutation({
        mutationFn: (payload: CrearCuentaBancariaPayload) => crearCuentaBancaria(payload),
        onSuccess: invalidar,
    });
}

export function useActualizarCuentaBancaria() {
    const invalidar = useInvalidar();

    return useMutation({
        mutationFn: (payload: ActualizarCuentaBancariaPayload) => actualizarCuentaBancaria(payload),
        onSuccess: invalidar,
    });
}

export function useCambiarEstadoCuentaBancaria() {
    const invalidar = useInvalidar();

    return useMutation({
        mutationFn: ({ id, esActivo }: { id: string; esActivo: boolean }) =>
            cambiarEstadoCuentaBancaria(id, esActivo),
        onSuccess: invalidar,
    });
}
