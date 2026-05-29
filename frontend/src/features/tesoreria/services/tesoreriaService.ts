import apiClient from '@/lib/apiClient';

export type CajaTesoreria = {
    id: string;
    nombre: string;
    tipoCaja: number;
    cuentaContable: string;
    saldoActual: number;
};

export type RegistrarMovimientoTesoreriaPayload = {
    monto: number;
    concepto: string;
    cuentaContableId: string;
    cajaId: string;
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
    cajaId: string;
    cajaNombre: string;
    comprobanteContableId: string | null;
};

type RegistrarMovimientoTesoreriaResponse = {
    id: string;
};

type CajaApiDto = {
    id?: string;
    Id?: string;
    nombre?: string;
    Nombre?: string;
    tipoCaja?: number;
    TipoCaja?: number;
    cuentaContable?: string;
    CuentaContable?: string;
    saldoActual?: number;
    SaldoActual?: number;
};

type RegistrarMovimientoTesoreriaResponseDto = {
    id?: string;
    Id?: string;
};

type EgresoApiDto = {
    id?: string;
    Id?: string;
    fecha?: string;
    Fecha?: string;
    monto?: number;
    Monto?: number;
    concepto?: string;
    Concepto?: string;
    terceroId?: string | null;
    TerceroId?: string | null;
    cuentaContableId?: string;
    CuentaContableId?: string;
    cuentaContableNombre?: string;
    CuentaContableNombre?: string;
    cajaId?: string;
    CajaId?: string;
    cajaNombre?: string;
    CajaNombre?: string;
    comprobanteContableId?: string | null;
    ComprobanteContableId?: string | null;
};

export async function getCajas(): Promise<CajaTesoreria[]> {
    const response = await apiClient.get<CajaApiDto[]>('/api/tesoreria/cajas');

    return (response.data ?? []).map((item) => ({
        id: String(item?.id ?? item?.Id ?? ''),
        nombre: String(item?.nombre ?? item?.Nombre ?? ''),
        tipoCaja: Number(item?.tipoCaja ?? item?.TipoCaja ?? 0),
        cuentaContable: String(item?.cuentaContable ?? item?.CuentaContable ?? ''),
        saldoActual: Number(item?.saldoActual ?? item?.SaldoActual ?? 0),
    }));
}

export async function getEgresos(): Promise<EgresoTesoreria[]> {
    const response = await apiClient.get<EgresoApiDto[]>('/api/tesoreria/egresos');

    return (response.data ?? []).map((item) => ({
        id: String(item?.id ?? item?.Id ?? ''),
        fecha: String(item?.fecha ?? item?.Fecha ?? ''),
        monto: Number(item?.monto ?? item?.Monto ?? 0),
        concepto: String(item?.concepto ?? item?.Concepto ?? ''),
        terceroId: item?.terceroId ?? item?.TerceroId ?? null,
        cuentaContableId: String(item?.cuentaContableId ?? item?.CuentaContableId ?? ''),
        cuentaContableNombre: String(item?.cuentaContableNombre ?? item?.CuentaContableNombre ?? ''),
        cajaId: String(item?.cajaId ?? item?.CajaId ?? ''),
        cajaNombre: String(item?.cajaNombre ?? item?.CajaNombre ?? ''),
        comprobanteContableId: item?.comprobanteContableId ?? item?.ComprobanteContableId ?? null,
    }));
}

export async function registrarIngreso(payload: RegistrarMovimientoTesoreriaPayload): Promise<RegistrarMovimientoTesoreriaResponse> {
    const response = await apiClient.post<RegistrarMovimientoTesoreriaResponseDto>('/api/tesoreria/ingresos', payload);

    return {
        id: String(response.data?.id ?? response.data?.Id ?? ''),
    };
}

export async function registrarEgreso(payload: RegistrarMovimientoTesoreriaPayload): Promise<RegistrarMovimientoTesoreriaResponse> {
    const response = await apiClient.post<RegistrarMovimientoTesoreriaResponseDto>('/api/tesoreria/egresos', payload);

    return {
        id: String(response.data?.id ?? response.data?.Id ?? ''),
    };
}
