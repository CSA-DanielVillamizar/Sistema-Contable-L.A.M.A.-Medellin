import apiClient from '@/lib/apiClient';

export type EventoDto = {
    id: string;
    nombre: string;
    fechaProgramada: string;
    tipoEvento: string;
    estado: string;
};

export type CreateEventoPayload = {
    nombre: string;
    descripcion: string;
    fechaProgramada: string;
    lugarEncuentro: string;
    destino?: string | null;
    tipoEvento: number;
};

export type UpdateEventoPayload = {
    nombre: string;
    descripcion: string;
    fechaProgramada: string;
    lugarEncuentro: string;
    destino?: string | null;
    tipoEvento: number;
};

export type AsistenciaEventoDto = {
    miembroId: string;
    nombreMiembro: string;
    asistio: boolean;
};

export type EventoDetalleDto = {
    id: string;
    nombre: string;
    descripcion: string;
    fechaProgramada: string;
    lugarEncuentro: string;
    destino: string;
    tipoEvento: string;
    estado: string;
    asistencias: AsistenciaEventoDto[];
};

export type MarcarAsistenciaPayload = {
    miembroId: string;
    asistio: boolean;
    observaciones?: string | null;
};

type IdResponseDto = {
    id?: string;
    Id?: string;
};

function toStringValue(value: unknown): string {
    return typeof value === 'string' ? value : String(value ?? '');
}

function toBooleanValue(value: unknown): boolean {
    return typeof value === 'boolean' ? value : Boolean(value ?? false);
}

function mapEvento(item: unknown): EventoDto {
    const dto = (item ?? {}) as Record<string, unknown>;

    return {
        id: toStringValue(dto.id ?? dto.Id),
        nombre: toStringValue(dto.nombre ?? dto.Nombre),
        fechaProgramada: toStringValue(dto.fechaProgramada ?? dto.FechaProgramada),
        tipoEvento: toStringValue(dto.tipoEvento ?? dto.TipoEvento),
        estado: toStringValue(dto.estado ?? dto.Estado),
    };
}

function mapAsistencia(item: unknown): AsistenciaEventoDto {
    const dto = (item ?? {}) as Record<string, unknown>;

    return {
        miembroId: toStringValue(dto.miembroId ?? dto.MiembroId),
        nombreMiembro: toStringValue(dto.nombreMiembro ?? dto.NombreMiembro),
        asistio: toBooleanValue(dto.asistio ?? dto.Asistio),
    };
}

function mapEventoDetalle(item: unknown): EventoDetalleDto {
    const dto = (item ?? {}) as Record<string, unknown>;
    const asistenciasRaw = (dto.asistencias ?? dto.Asistencias ?? []) as unknown[];

    return {
        id: toStringValue(dto.id ?? dto.Id),
        nombre: toStringValue(dto.nombre ?? dto.Nombre),
        descripcion: toStringValue(dto.descripcion ?? dto.Descripcion),
        fechaProgramada: toStringValue(dto.fechaProgramada ?? dto.FechaProgramada),
        lugarEncuentro: toStringValue(dto.lugarEncuentro ?? dto.LugarEncuentro),
        destino: toStringValue(dto.destino ?? dto.Destino),
        tipoEvento: toStringValue(dto.tipoEvento ?? dto.TipoEvento),
        estado: toStringValue(dto.estado ?? dto.Estado),
        asistencias: asistenciasRaw.map(mapAsistencia),
    };
}

export async function getEventos(): Promise<EventoDto[]> {
    const response = await apiClient.get<unknown[]>('/api/eventos');
    return (response.data ?? []).map(mapEvento).filter((evento) => evento.id.length > 0);
}

export async function crearEvento(payload: CreateEventoPayload): Promise<{ id: string }> {
    const response = await apiClient.post<IdResponseDto>('/api/eventos', payload);
    return { id: toStringValue(response.data?.id ?? response.data?.Id) };
}

export async function actualizarEvento(id: string, payload: UpdateEventoPayload): Promise<void> {
    await apiClient.put(`/api/eventos/${id}`, payload);
}

export async function getEventoById(id: string): Promise<EventoDetalleDto> {
    const response = await apiClient.get<unknown>(`/api/eventos/${id}`);
    return mapEventoDetalle(response.data);
}

export async function marcarAsistencia(id: string, payload: MarcarAsistenciaPayload): Promise<void> {
    await apiClient.post(`/api/eventos/${id}/asistencia`, payload);
}
