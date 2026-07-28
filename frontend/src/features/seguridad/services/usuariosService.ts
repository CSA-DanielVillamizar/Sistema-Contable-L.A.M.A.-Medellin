import apiClient from '@/lib/apiClient';

export enum RolSistema {
    Administrador = 1,
    Tesorero = 2,
    Logistica = 3,
    CapitanRuta = 4,
}

export type UsuarioDto = {
    id: string;
    email: string;
    rol: RolSistema;
    esActivo: boolean;
};

type UsuarioApiDto = {
    id?: string;
    email?: string;
    rol?: string | number;
    esActivo?: boolean;
};

export type AsignarRolPayload = {
    nuevoRol: RolSistema;
};

function toStringValue(value: unknown): string {
    return typeof value === 'string' ? value : String(value ?? '');
}

function toBooleanValue(value: unknown): boolean {
    return typeof value === 'boolean' ? value : Boolean(value ?? false);
}

function mapRolSistema(value: unknown): RolSistema {
    if (typeof value === 'number' && value >= 1 && value <= 4) {
        return value as RolSistema;
    }

    const normalized = String(value ?? '').toLowerCase();

    if (normalized === 'administrador') {
        return RolSistema.Administrador;
    }

    if (normalized === 'tesorero') {
        return RolSistema.Tesorero;
    }

    if (normalized === 'logistica') {
        return RolSistema.Logistica;
    }

    if (normalized === 'capitanruta') {
        return RolSistema.CapitanRuta;
    }

    return RolSistema.Logistica;
}

function mapUsuario(item: unknown): UsuarioDto {
    const dto = (item ?? {}) as UsuarioApiDto;

    return {
        id: toStringValue(dto.id),
        email: toStringValue(dto.email),
        rol: mapRolSistema(dto.rol),
        esActivo: toBooleanValue(dto.esActivo),
    };
}

export async function getUsuarios(): Promise<UsuarioDto[]> {
    const response = await apiClient.get<unknown[]>('/api/usuarios');
    return (response.data ?? []).map(mapUsuario).filter((usuario) => usuario.id.length > 0);
}

export async function asignarRol(id: string, payload: AsignarRolPayload): Promise<void> {
    await apiClient.put(`/api/usuarios/${id}/rol`, payload);
}
