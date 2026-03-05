export const TRIBUTARIO_ALLOWED_ROLES = ['Contador', 'Admin'] as const;

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
