import apiClient, { type RespuestaApi } from '@/lib/apiClient';

/**
 * Administracion de cuentas bancarias. Espeja TesoreriaController.
 *
 * El saldo no aparece en ningun payload de escritura a proposito: se deriva de
 * los movimientos registrados, y permitir editarlo desde una pantalla lo
 * desligaria del libro.
 */
export type CuentaBancaria = {
    id: string;
    nombre: string;
    numeroCuenta: string;
    saldoActual: number;
    esActivo: boolean;
    cuentaContableId: string;
};

export type CrearCuentaBancariaPayload = {
    nombre: string;
    numeroCuenta: string;
    cuentaContableId: string;
};

export type ActualizarCuentaBancariaPayload = CrearCuentaBancariaPayload & {
    id: string;
};

export async function getCuentasBancarias(incluirInactivas = false): Promise<CuentaBancaria[]> {
    const response = await apiClient.get<RespuestaApi[]>('/api/tesoreria/cuentas-bancarias', {
        params: { incluirInactivas },
    });

    return (response.data ?? []).map((item) => ({
        id: String(item?.id ?? ''),
        nombre: String(item?.nombre ?? ''),
        numeroCuenta: String(item?.numeroCuenta ?? ''),
        saldoActual: Number(item?.saldoActual ?? 0),
        esActivo: Boolean(item?.esActivo),
        cuentaContableId: String(item?.cuentaContableId ?? ''),
    }));
}

export async function crearCuentaBancaria(payload: CrearCuentaBancariaPayload): Promise<{ id: string }> {
    const response = await apiClient.post<{ id?: string }>('/api/tesoreria/cuentas-bancarias', payload);
    return { id: String(response.data?.id ?? '') };
}

export async function actualizarCuentaBancaria({
    id,
    ...payload
}: ActualizarCuentaBancariaPayload): Promise<void> {
    await apiClient.put(`/api/tesoreria/cuentas-bancarias/${id}`, payload);
}

export async function cambiarEstadoCuentaBancaria(id: string, esActivo: boolean): Promise<void> {
    await apiClient.patch(`/api/tesoreria/cuentas-bancarias/${id}/estado`, { esActivo });
}
