/**
 * Roles efectivos de la sesion, tal como los ve el backend.
 *
 * El rol de la aplicacion (Admin, Tesorero, Contador...) vive en la tabla de
 * usuarios, no en el token de Entra: es un dato que se administra desde la
 * pantalla de seguridad y que el backend proyecta como claim en cada peticion.
 * Por eso el frontend no puede deducirlo leyendo el token, que es lo que hacia
 * antes: como el token no trae ese claim, todo usuario aparecia sin ningun rol
 * y las pantallas con exigencia de rol respondian "no tienes permiso" incluso a
 * un administrador.
 *
 * La unica fuente disponible en el cliente es la respuesta de
 * /api/usuarios/sync, que devuelve el rol vigente. Se publica aqui una sola vez
 * al resolver la sesion y las pantallas lo leen sin volver a preguntar.
 */
export type EstadoSesion = {
    /**
     * Roles efectivos, o `null` cuando no se pudieron determinar (la
     * sincronizacion fallo). Son cosas distintas: "sin roles" niega el acceso,
     * "no se sabe" lo deja pasar y que decida el backend.
     */
    roles: string[] | null;
    /** La sesion ya se resolvio, con exito o no. */
    resuelta: boolean;
};

const ESTADO_INICIAL: EstadoSesion = { roles: null, resuelta: false };

let estado: EstadoSesion = ESTADO_INICIAL;

const suscriptores = new Set<() => void>();

function publicar(nuevo: EstadoSesion): void {
    estado = nuevo;
    suscriptores.forEach((notificar) => notificar());
}

/**
 * Fija los roles de la sesion.
 *
 * Se combinan las dos procedencias posibles. Hoy manda el perfil interno, pero
 * si algun dia se configuran app roles en Entra seguiran contando, y mientras
 * tanto no estorban porque el token no los trae.
 */
export function publicarSesion(rolInterno: string | null, rolesDelToken: string[]): void {
    const roles = new Set(rolesDelToken);

    if (rolInterno) {
        roles.add(rolInterno);
    }

    publicar({
        roles: roles.size > 0 ? Array.from(roles) : null,
        resuelta: true,
    });
}

/**
 * Da la sesion por resuelta sin haber podido averiguar el rol.
 *
 * Pasa cuando la autenticacion termina en error o cuando /api/usuarios/sync no
 * responde. Deja `roles` en `null` a proposito: bloquear la interfaz por no
 * haber podido preguntar dejaria al usuario sin nada que hacer, y el backend
 * sigue negando lo que corresponda con su mensaje.
 */
export function marcarSesionResuelta(): void {
    if (estado.resuelta) {
        return;
    }

    publicar({ roles: null, resuelta: true });
}

export function suscribirseASesion(alCambiar: () => void): () => void {
    suscriptores.add(alCambiar);
    return () => suscriptores.delete(alCambiar);
}

export function leerSesion(): EstadoSesion {
    return estado;
}

/** El servidor nunca tiene sesion: renderiza siempre el estado inicial. */
export function leerSesionEnServidor(): EstadoSesion {
    return ESTADO_INICIAL;
}
