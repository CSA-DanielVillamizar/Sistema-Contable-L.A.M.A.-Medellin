export const TRIBUTARIO_ALLOWED_ROLES = ['Contador', 'Admin'] as const;

/**
 * Administra el sistema completo. Aparece ademas en cada lista de roles
 * permitidos, pero no se depende de eso: ver `tieneAcceso`.
 */
export const ROL_ADMIN = 'Admin';

type JwtPayload = {
    roles?: string[];
    role?: string | string[];
};

function decodeJwtPayload(token: string): JwtPayload | null {
    try {
        const parts = token.split('.');
        if (parts.length < 2) {
            return null;
        }

        const base64 = parts[1].replace(/-/g, '+').replace(/_/g, '/');
        const padding = '='.repeat((4 - (base64.length % 4)) % 4);
        const normalized = `${base64}${padding}`;
        const json = atob(normalized);

        return JSON.parse(json) as JwtPayload;
    } catch {
        return null;
    }
}

export function getUserRolesFromToken(token: string | null): string[] {
    if (!token) {
        return [];
    }

    const payload = decodeJwtPayload(token);
    if (!payload) {
        return [];
    }

    const roles = new Set<string>();

    if (Array.isArray(payload.roles)) {
        payload.roles.forEach((role) => roles.add(String(role)));
    }

    if (Array.isArray(payload.role)) {
        payload.role.forEach((role) => roles.add(String(role)));
    } else if (typeof payload.role === 'string') {
        roles.add(payload.role);
    }

    return Array.from(roles);
}

export function hasAnyAllowedRole(userRoles: string[], allowedRoles: readonly string[]): boolean {
    return userRoles.some((role) => allowedRoles.includes(role));
}

/**
 * Decide si la sesion puede ver una pantalla que exige alguno de `permitidos`.
 *
 * Tres reglas, en este orden:
 *
 * 1. Si no se pudo averiguar el rol (`null`), se deja pasar. La autorizacion de
 *    verdad la impone el backend, que responde 403 con un mensaje explicado;
 *    esconder la pantalla por no haber podido preguntar dejaria al usuario sin
 *    nada que hacer y sin saber por que.
 * 2. Admin satisface cualquier exigencia. Es la misma regla que aplica el
 *    backend en AdminSiempreAutorizadoHandler, y esta aqui por el mismo motivo:
 *    una pantalla nueva que olvide incluir a Admin en su lista dejaria al
 *    administrador fuera de su propio sistema.
 * 3. En los demas casos, basta con tener uno de los roles permitidos.
 */
export function tieneAcceso(roles: string[] | null, permitidos: readonly string[]): boolean {
    if (roles === null) {
        return true;
    }

    if (roles.includes(ROL_ADMIN)) {
        return true;
    }

    return hasAnyAllowedRole(roles, permitidos);
}
