'use client';

import { ApiError } from '@/lib/apiClient';
import { Lock, ServerCrash, SearchX } from 'lucide-react';

/**
 * Estado de pantalla cuando algo no se puede mostrar.
 *
 * Existe para que un fallo se vea como una explicacion y no como el codigo
 * crudo que devolvio el servidor. Un "HTTP 403" no le dice nada a quien lo
 * lee; "no tienes permiso, pideselo a un administrador" si.
 *
 * Cada pantalla repetia su propio bloque de "sin permiso", con textos
 * distintos entre si. Este centraliza el mensaje y el aspecto.
 */
type EstadoDeErrorProps = {
    error?: unknown;
    /** Que estaba intentando hacer el usuario. Ayuda a que el mensaje sea concreto. */
    contexto?: string;
    /** Se muestra en lugar del mensaje deducido del error. */
    mensaje?: string;
};

type Presentacion = {
    icono: React.ReactNode;
    titulo: string;
    detalle: string;
    tono: string;
};

function presentar(error: unknown, contexto?: string, mensaje?: string): Presentacion {
    const status = error instanceof ApiError ? error.status : undefined;
    const sufijo = contexto ? ` ${contexto}.` : '';

    if (status === 403) {
        return {
            icono: <Lock size={22} strokeWidth={2} />,
            titulo: 'No tienes permiso',
            detalle:
                mensaje ??
                `Tu rol actual no permite${sufijo || ' esta operación.'} Si crees que deberías tenerlo, pídeselo a un administrador.`,
            tono: 'bg-amber-50 text-amber-800 border-amber-200',
        };
    }

    if (status === 401) {
        return {
            icono: <Lock size={22} strokeWidth={2} />,
            titulo: 'Tu sesión expiró',
            detalle: mensaje ?? 'Vuelve a iniciar sesión para continuar.',
            tono: 'bg-slate-50 text-slate-700 border-slate-200',
        };
    }

    if (status === 404) {
        return {
            icono: <SearchX size={22} strokeWidth={2} />,
            titulo: 'No encontramos esto',
            detalle: mensaje ?? 'El recurso que buscabas no existe o fue eliminado.',
            tono: 'bg-slate-50 text-slate-700 border-slate-200',
        };
    }

    return {
        icono: <ServerCrash size={22} strokeWidth={2} />,
        titulo: 'No se pudo cargar',
        detalle:
            mensaje ??
            (error instanceof Error && error.message
                ? error.message
                : 'Ocurrió un error inesperado. Intenta de nuevo en unos minutos.'),
        tono: 'bg-rose-50 text-rose-800 border-rose-200',
    };
}

export default function EstadoDeError({ error, contexto, mensaje }: EstadoDeErrorProps) {
    const { icono, titulo, detalle, tono } = presentar(error, contexto, mensaje);

    return (
        <div className={`flex items-start gap-3 rounded-xl border px-4 py-4 ${tono}`}>
            <span className="mt-0.5 shrink-0">{icono}</span>
            <div>
                <p className="font-semibold">{titulo}</p>
                <p className="mt-1 text-sm leading-relaxed">{detalle}</p>
            </div>
        </div>
    );
}
