import apiClient from '@/lib/apiClient';

/** Espeja CuentaBancariaDto del backend. */
export type CuentaBancariaTesoreria = {
    id: string;
    nombre: string;
    numeroCuenta: string;
    saldoActual: number;
    esActivo: boolean;
};

export type RegistrarMovimientoTesoreriaPayload = {
    monto: number;
    concepto: string;
    cuentaContableId: string;
    bancoId: string;
    centroCostoId: string;
    fecha?: string;
    terceroId?: string | null;
};

export type EgresoTesoreria = {
    id: string;
    fecha: string;
    monto: number;
    concepto: string;
    terceroId: string | null;
    cuentaContableId: string;
    cuentaContableNombre: string;
    bancoId: string;
    cuentaBancariaNombre: string;
    comprobanteContableId: string | null;
};

type RegistrarMovimientoTesoreriaResponse = {
    id: string;
};

type CuentaBancariaApiDto = {
    id?: string;
    nombre?: string;
    numeroCuenta?: string;
    saldoActual?: number;
    esActivo?: boolean;
};

type RegistrarMovimientoTesoreriaResponseDto = {
    id?: string;
};

type EgresoApiDto = {
    id?: string;
    fecha?: string;
    monto?: number;
    concepto?: string;
    terceroId?: string | null;
    cuentaContableId?: string;
    cuentaContableNombre?: string;
    bancoId?: string;
    cuentaBancariaNombre?: string;
    comprobanteContableId?: string | null;
};

export async function getCuentasBancarias(): Promise<CuentaBancariaTesoreria[]> {
    const response = await apiClient.get<CuentaBancariaApiDto[]>('/api/tesoreria/cuentas-bancarias');

    return (response.data ?? []).map((item) => ({
        id: String(item?.id ?? ''),
        nombre: String(item?.nombre ?? ''),
        numeroCuenta: String(item?.numeroCuenta ?? ''),
        saldoActual: Number(item?.saldoActual ?? 0),
        esActivo: Boolean(item?.esActivo ?? true),
    }));
}

export async function getEgresos(): Promise<EgresoTesoreria[]> {
    const response = await apiClient.get<EgresoApiDto[]>('/api/tesoreria/egresos');

    return (response.data ?? []).map((item) => ({
        id: String(item?.id ?? ''),
        fecha: String(item?.fecha ?? ''),
        monto: Number(item?.monto ?? 0),
        concepto: String(item?.concepto ?? ''),
        terceroId: item?.terceroId ?? null,
        cuentaContableId: String(item?.cuentaContableId ?? ''),
        cuentaContableNombre: String(item?.cuentaContableNombre ?? ''),
        bancoId: String(item?.bancoId ?? ''),
        cuentaBancariaNombre: String(item?.cuentaBancariaNombre ?? ''),
        comprobanteContableId: item?.comprobanteContableId ?? null,
    }));
}

export async function registrarIngreso(payload: RegistrarMovimientoTesoreriaPayload): Promise<RegistrarMovimientoTesoreriaResponse> {
    const response = await apiClient.post<RegistrarMovimientoTesoreriaResponseDto>('/api/tesoreria/ingresos', payload);

    return {
        id: String(response.data?.id ?? ''),
    };
}

export async function registrarEgreso(payload: RegistrarMovimientoTesoreriaPayload): Promise<RegistrarMovimientoTesoreriaResponse> {
    const response = await apiClient.post<RegistrarMovimientoTesoreriaResponseDto>('/api/tesoreria/egresos', payload);

    return {
        id: String(response.data?.id ?? ''),
    };
}
