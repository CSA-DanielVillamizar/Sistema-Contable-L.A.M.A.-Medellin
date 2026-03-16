import apiClient from '@/lib/apiClient';

export type CajaTesoreria = {
    id: string;
    nombre: string;
    tipoCaja: number;
    saldoActual: number;
};

export type RegistrarMovimientoTesoreriaPayload = {
    fecha: string;
    monto: number;
    concepto: string;
    terceroId?: string | null;
    cuentaContableId: string;
    cajaId: string;
    centroCostoId: string;
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
    saldoActual?: number;
    SaldoActual?: number;
};

type RegistrarMovimientoTesoreriaResponseDto = {
    id?: string;
    Id?: string;
};

export async function getCajas(): Promise<CajaTesoreria[]> {
    const response = await apiClient.get<CajaApiDto[]>('/api/tesoreria/cajas');

    return (response.data ?? []).map((item) => ({
        id: String(item?.id ?? item?.Id ?? ''),
        nombre: String(item?.nombre ?? item?.Nombre ?? ''),
        tipoCaja: Number(item?.tipoCaja ?? item?.TipoCaja ?? 0),
        saldoActual: Number(item?.saldoActual ?? item?.SaldoActual ?? 0),
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
