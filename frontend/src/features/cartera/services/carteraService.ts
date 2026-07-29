import apiClient, { type RespuestaApi } from '@/lib/apiClient';

/** Espeja CrearMiembroRequest en CarteraController.cs. Todos los campos son obligatorios. */
export type CrearMiembroPayload = {
    documentoIdentidad: string;
    nombres: string;
    apellidos: string;
    apodo: string;
    fechaIngreso: string;
    tipoSangre: number;
    nombreContactoEmergencia: string;
    telefonoContactoEmergencia: string;
    marcaMoto: string;
    modeloMoto: string;
    cilindraje: number;
    placa: string;
    rango: number;
};

export type CrearConceptoCobroPayload = {
    nombre: string;
    valorCOP: number;
    periodicidadMensual: number;
    cuentaContableIngresoId: string;
};

export type CrearCuentaPorCobrarPayload = {
    miembroId: string;
    conceptoCobroId: string;
    /** YYYY-MM. Obligatorio: es lo que identifica el mes que cubre la obligacion. */
    periodo: string;
    fechaEmision: string;
    fechaVencimiento: string;
    valorTotal: number;
};

export type MiembroLookupItem = {
    id: string;
    nombreCompleto: string;
};

export type ConceptoCobroLookupItem = {
    id: string;
    nombre: string;
};

export type CuentaPorCobrarItem = {
    id: string;
    nombreCompletoMiembro: string;
    nombreConcepto: string;
    fechaEmision: string;
    fechaVencimiento: string;
    valorTotal: number;
    saldoPendiente: number;
    estado: number;
};

export type GetCuentasPorCobrarParams = {
    estado?: number;
    miembroId?: string;
};

export type RegistrarPagoCarteraPayload = {
    cuentaPorCobrarId: string;
    monto: number;
    bancoId: string;
    medioPago: number;
};

type IdResponseDto = {
    id?: string;
};

function toId(response: IdResponseDto | undefined): string {
    return String(response?.id ?? '');
}

export async function crearMiembro(payload: CrearMiembroPayload): Promise<{ id: string }> {
    const response = await apiClient.post<IdResponseDto>('/api/cartera/miembros', payload);
    return { id: toId(response.data) };
}

export async function crearConceptoCobro(payload: CrearConceptoCobroPayload): Promise<{ id: string }> {
    const response = await apiClient.post<IdResponseDto>('/api/cartera/conceptos-cobro', payload);
    return { id: toId(response.data) };
}

export async function crearCuentaPorCobrar(payload: CrearCuentaPorCobrarPayload): Promise<{ id: string }> {
    const response = await apiClient.post<IdResponseDto>('/api/cartera/cuentas-por-cobrar', payload);
    return { id: toId(response.data) };
}

export async function getMiembrosLookup(): Promise<MiembroLookupItem[]> {
    const response = await apiClient.get<RespuestaApi[]>('/api/cartera/miembros/lookup');

    return (response.data ?? []).map((item) => ({
        id: String(item?.id ?? ''),
        nombreCompleto: String(item?.nombreCompleto ?? ''),
    }));
}

export async function getConceptosCobroLookup(): Promise<ConceptoCobroLookupItem[]> {
    const response = await apiClient.get<RespuestaApi[]>('/api/cartera/conceptos-cobro/lookup');

    return (response.data ?? []).map((item) => ({
        id: String(item?.id ?? ''),
        nombre: String(item?.nombre ?? ''),
    }));
}

export async function getCuentasPorCobrar(params?: GetCuentasPorCobrarParams): Promise<CuentaPorCobrarItem[]> {
    const response = await apiClient.get<RespuestaApi[]>('/api/cartera/cuentas-por-cobrar', {
        params: {
            estado: params?.estado,
            miembroId: params?.miembroId,
        },
    });

    return (response.data ?? []).map((item) => ({
        id: String(item?.id ?? ''),
        nombreCompletoMiembro: String(item?.nombreCompletoMiembro ?? ''),
        nombreConcepto: String(item?.nombreConcepto ?? ''),
        fechaEmision: String(item?.fechaEmision ?? ''),
        fechaVencimiento: String(item?.fechaVencimiento ?? ''),
        valorTotal: Number(item?.valorTotal ?? 0),
        saldoPendiente: Number(item?.saldoPendiente ?? 0),
        estado: Number(item?.estado ?? 0),
    }));
}

export async function registrarPagoCartera(payload: RegistrarPagoCarteraPayload): Promise<void> {
    await apiClient.post(`/api/cartera/cuentas-por-cobrar/${payload.cuentaPorCobrarId}/pagos`, {
        monto: payload.monto,
        bancoId: payload.bancoId,
        medioPago: payload.medioPago,
    });
}
