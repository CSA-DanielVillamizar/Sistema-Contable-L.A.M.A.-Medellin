'use client';

import { useSyncExternalStore } from 'react';
import { tieneAcceso } from '@/lib/authRoles';
import { leerSesion, leerSesionEnServidor, suscribirseASesion } from '@/lib/sesionInterna';

/**
 * Resuelve si la sesion actual tiene alguno de los roles indicados.
 *
 * Lee los roles que AuthProvider publico al resolver la sesion. Antes decodaba
 * el access token de Entra en cada componente, y eso no podia funcionar: el rol
 * de la aplicacion no viaja en ese token, se guarda en la base de datos. Todo
 * usuario salia sin roles y cada pantalla con exigencia de rol mostraba "no
 * tienes permiso", incluido el administrador.
 *
 * `isRoleReady` sigue existiendo porque la sesion se resuelve despues del primer
 * render; sin esperarla se pintaria "sin permiso" durante un instante.
 *
 * Esto es solo control de interfaz. La autorizacion real la impone el backend
 * con [Authorize(Roles = ...)]; aqui unicamente se evita ofrecer pantallas que
 * el usuario no va a poder usar.
 */
export function useRoleAccess(allowedRoles: readonly string[]) {
    const { roles, resuelta } = useSyncExternalStore(
        suscribirseASesion,
        leerSesion,
        leerSesionEnServidor,
    );

    return {
        canAccess: resuelta && tieneAcceso(roles, allowedRoles),
        isRoleReady: resuelta,
    };
}
