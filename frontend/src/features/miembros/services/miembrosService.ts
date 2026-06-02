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
    Id?: string;
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
    const contactoEmergencia = toRecord(dto.contactoEmergencia ?? dto.ContactoEmergencia);
    const moto = toRecord(dto.moto ?? dto.Moto);

    return {
        id: toStringValue(dto.id ?? dto.Id),
        documentoIdentidad: toStringValue(dto.documentoIdentidad ?? dto.DocumentoIdentidad),
        nombres: toStringValue(dto.nombres ?? dto.Nombres),
        apellidos: toStringValue(dto.apellidos ?? dto.Apellidos),
        apodo: toStringValue(dto.apodo ?? dto.Apodo),
        fechaIngreso: toStringValue(dto.fechaIngreso ?? dto.FechaIngreso),
        rango: toStringValue(dto.rango ?? dto.Rango),
        esActivo: toBooleanValue(dto.esActivo ?? dto.EsActivo),
        tipoSangre: toStringValue(dto.tipoSangre ?? dto.TipoSangre),
        contactoEmergenciaNombre: toStringValue(
            dto.contactoEmergenciaNombre
            ?? dto.ContactoEmergenciaNombre
            ?? dto.nombreContactoEmergencia
            ?? dto.NombreContactoEmergencia
            ?? contactoEmergencia.nombre
            ?? contactoEmergencia.Nombre,
        ),
        contactoEmergenciaTelefono: toStringValue(
            dto.contactoEmergenciaTelefono
            ?? dto.ContactoEmergenciaTelefono
            ?? dto.telefonoContactoEmergencia
            ?? dto.TelefonoContactoEmergencia
            ?? contactoEmergencia.telefono
            ?? contactoEmergencia.Telefono,
        ),
        moto: {
            marca: toStringValue(moto.marca ?? moto.Marca ?? dto.marcaMoto ?? dto.MarcaMoto),
            modelo: toStringValue(moto.modelo ?? moto.Modelo ?? dto.modeloMoto ?? dto.ModeloMoto),
            cilindraje: toNumberValue(moto.cilindraje ?? moto.Cilindraje ?? dto.cilindraje ?? dto.Cilindraje),
            placa: toStringValue(moto.placa ?? moto.Placa ?? dto.placa ?? dto.Placa),
        },
    };
}

export async function getMiembros(): Promise<Miembro[]> {
    const response = await apiClient.get<unknown[]>('/api/miembros');
    return (response.data ?? []).map(mapMiembro).filter((item) => item.id.length > 0);
}

export async function crearMiembro(payload: CrearMiembroPayload): Promise<{ id: string }> {
    const response = await apiClient.post<IdResponseDto>('/api/miembros', payload);
    return { id: toStringValue(response.data?.id ?? response.data?.Id) };
}

export async function actualizarMiembro(id: string, payload: ActualizarMiembroPayload): Promise<void> {
    await apiClient.put(`/api/miembros/${id}`, payload);
}
