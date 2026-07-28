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
    nombre?: string;
    tipoCaja?: number;
    cuentaContable?: string;
    saldoActual?: number;
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
    cajaId?: string;
    cajaNombre?: string;
    comprobanteContableId?: string | null;
};

export async function getCajas(): Promise<CajaTesoreria[]> {
    const response = await apiClient.get<CajaApiDto[]>('/api/tesoreria/cajas');

    return (response.data ?? []).map((item) => ({
        id: String(item?.id ?? ''),
        nombre: String(item?.nombre ?? ''),
        tipoCaja: Number(item?.tipoCaja ?? 0),
        cuentaContable: String(item?.cuentaContable ?? ''),
        saldoActual: Number(item?.saldoActual ?? 0),
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
        cajaId: String(item?.cajaId ?? ''),
        cajaNombre: String(item?.cajaNombre ?? ''),
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
