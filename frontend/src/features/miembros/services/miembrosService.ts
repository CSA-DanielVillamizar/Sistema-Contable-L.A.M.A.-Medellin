import apiClient from '@/lib/apiClient';

export type MiembroMoto = {
    marca: string;
    modelo: string;
    cilindraje: number;
    placa: string;
};

export type Miembro = {
    id: string;
    documentoIdentidad: string;
    nombres: string;
    apellidos: string;
    apodo: string;
    fechaIngreso: string;
    rango: string;
    esActivo: boolean;
    tipoSangre: string;
    contactoEmergenciaNombre: string;
    contactoEmergenciaTelefono: string;
    moto: MiembroMoto;
};

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
    esActivo: boolean;
};

export type ActualizarMiembroPayload = {
    tipoSangre: number;
    nombreContactoEmergencia: string;
    telefonoContactoEmergencia: string;
    marcaMoto: string;
    modeloMoto: string;
    cilindraje: number;
    placa: string;
    rango: number;
    esActivo: boolean;
};

type IdResponseDto = {
    id?: string;
};

function toStringValue(value: unknown): string {
    return typeof value === 'string' ? value : String(value ?? '');
}

function toNumberValue(value: unknown): number {
    return typeof value === 'number' ? value : Number(value ?? 0);
}

function toBooleanValue(value: unknown): boolean {
    return typeof value === 'boolean' ? value : Boolean(value ?? false);
}

function toRecord(value: unknown): Record<string, unknown> {
    return value && typeof value === 'object' ? (value as Record<string, unknown>) : {};
}

function mapMiembro(item: unknown): Miembro {
    const dto = (item ?? {}) as Record<string, unknown>;
    const contactoEmergencia = toRecord(dto.contactoEmergencia);
    const moto = toRecord(dto.moto);

    return {
        id: toStringValue(dto.id),
        documentoIdentidad: toStringValue(dto.documentoIdentidad),
        nombres: toStringValue(dto.nombres),
        apellidos: toStringValue(dto.apellidos),
        apodo: toStringValue(dto.apodo),
        fechaIngreso: toStringValue(dto.fechaIngreso),
        rango: toStringValue(dto.rango),
        esActivo: toBooleanValue(dto.esActivo),
        tipoSangre: toStringValue(dto.tipoSangre),
        contactoEmergenciaNombre: toStringValue(
            dto.contactoEmergenciaNombre
            ?? dto.nombreContactoEmergencia
            ?? contactoEmergencia.nombre,
        ),
        contactoEmergenciaTelefono: toStringValue(
            dto.contactoEmergenciaTelefono
            ?? dto.telefonoContactoEmergencia
            ?? contactoEmergencia.telefono,
        ),
        moto: {
            marca: toStringValue(moto.marca ?? dto.marcaMoto),
            modelo: toStringValue(moto.modelo ?? dto.modeloMoto),
            cilindraje: toNumberValue(moto.cilindraje ?? dto.cilindraje),
            placa: toStringValue(moto.placa ?? dto.placa),
        },
    };
}

export async function getMiembros(): Promise<Miembro[]> {
    const response = await apiClient.get<unknown[]>('/api/miembros');
    return (response.data ?? []).map(mapMiembro).filter((item) => item.id.length > 0);
}

export async function crearMiembro(payload: CrearMiembroPayload): Promise<{ id: string }> {
    const response = await apiClient.post<IdResponseDto>('/api/miembros', payload);
    return { id: toStringValue(response.data?.id) };
}

export async function actualizarMiembro(id: string, payload: ActualizarMiembroPayload): Promise<void> {
    await apiClient.put(`/api/miembros/${id}`, payload);
}
