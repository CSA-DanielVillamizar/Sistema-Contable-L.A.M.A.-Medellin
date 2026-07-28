'use client';

import { useEffect, useState } from 'react';
import { getUserRolesFromToken, hasAnyAllowedRole } from '@/lib/authRoles';
import { getAccessToken } from '@/lib/msalClient';

/**
 * Resuelve si la sesion actual tiene alguno de los roles indicados.
 *
 * Obtener el token es asincrono desde que dejo de guardarse en localStorage, asi
 * que el consumidor debe esperar a `isRoleReady` antes de decidir que renderiza;
 * de lo contrario pintaria "sin permiso" durante el primer frame.
 *
 * Esto es solo control de UI. La autorizacion real la impone el backend con
 * [Authorize(Roles = ...)]; aqui unicamente se evita mostrar pantallas inutiles.
 */
export function useRoleAccess(allowedRoles: readonly string[]) {
    const [canAccess, setCanAccess] = useState(false);
    const [isRoleReady, setIsRoleReady] = useState(false);

    useEffect(() => {
        let cancelado = false;

        const resolverAcceso = async () => {
            const token = await getAccessToken();
            const roles = getUserRolesFromToken(token);

            if (cancelado) {
                return;
            }

            setCanAccess(hasAnyAllowedRole(roles, allowedRoles));
            setIsRoleReady(true);
        };

        void resolverAcceso();

        return () => {
            cancelado = true;
        };
        // allowedRoles se espera como constante a nivel de modulo (referencia estable).
    }, [allowedRoles]);

    return { canAccess, isRoleReady };
}
