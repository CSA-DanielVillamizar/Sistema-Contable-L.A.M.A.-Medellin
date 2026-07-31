'use client';

import { use, useEffect, useState } from 'react';

/**
 * Verificacion publica de un recibo (historia 1-7).
 *
 * Es a donde apunta el codigo QR impreso. Va sin sesion a proposito: quien
 * recibe un recibo en papel no tiene cuenta en el sistema.
 *
 * Se llama al API directamente y no por el cliente compartido, porque ese
 * adjunta el token de MSAL y aqui no hay sesion de la cual sacarlo.
 */
type Estado =
    | { fase: 'cargando' }
    | { fase: 'valido'; recibo: ReciboVerificado }
    | { fase: 'anulado'; recibo: ReciboVerificado }
    | { fase: 'inexistente' }
    | { fase: 'error' };

type ReciboVerificado = {
    numeroConsecutivo: string;
    fecha: string;
    valorCOP: number;
    estado: string;
    esValido: boolean;
};

const formatoCOP = new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0,
});

const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL ?? 'http://localhost:5006';

export default function VerificarReciboPage({
    params,
}: {
    params: Promise<{ consecutivo: string }>;
}) {
    const { consecutivo } = use(params);
    const [estado, setEstado] = useState<Estado>({ fase: 'cargando' });

    useEffect(() => {
        let cancelado = false;

        const verificar = async () => {
            try {
                const respuesta = await fetch(
                    `${apiBaseUrl}/api/recibos/verificar/${encodeURIComponent(consecutivo)}`,
                );

                if (cancelado) return;

                if (respuesta.status === 404) {
                    setEstado({ fase: 'inexistente' });
                    return;
                }

                if (!respuesta.ok) {
                    setEstado({ fase: 'error' });
                    return;
                }

                const recibo = (await respuesta.json()) as ReciboVerificado;
                if (cancelado) return;

                setEstado({ fase: recibo.esValido ? 'valido' : 'anulado', recibo });
            } catch {
                if (!cancelado) setEstado({ fase: 'error' });
            }
        };

        void verificar();

        return () => {
            cancelado = true;
        };
    }, [consecutivo]);

    return (
        <main className="flex min-h-screen items-center justify-center bg-slate-50 p-6">
            <div className="w-full max-w-md rounded-2xl border border-slate-200 bg-white p-8 shadow-sm">
                <header className="text-center">
                    <h1 className="text-sm font-bold uppercase tracking-widest text-slate-500">
                        Fundación L.A.M.A. Medellín
                    </h1>
                    <p className="mt-1 text-xs text-slate-400">NIT 902.007.705-8</p>
                </header>

                <div className="mt-8">
                    {estado.fase === 'cargando' ? (
                        <p className="text-center text-sm text-slate-500">Verificando recibo...</p>
                    ) : null}

                    {estado.fase === 'inexistente' ? (
                        <div className="text-center">
                            <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-rose-100 text-2xl text-rose-700">
                                ✕
                            </div>
                            <p className="mt-4 font-semibold text-slate-900">Recibo no encontrado</p>
                            <p className="mt-1 text-sm text-slate-600">
                                No existe ningún movimiento con el número <strong>{consecutivo}</strong>.
                            </p>
                        </div>
                    ) : null}

                    {estado.fase === 'error' ? (
                        <div className="text-center">
                            <p className="font-semibold text-slate-900">No se pudo verificar</p>
                            <p className="mt-1 text-sm text-slate-600">
                                El sistema no respondió. Intente de nuevo en unos minutos.
                            </p>
                        </div>
                    ) : null}

                    {estado.fase === 'valido' || estado.fase === 'anulado' ? (
                        <>
                            <div className="text-center">
                                <div
                                    className={`mx-auto flex h-12 w-12 items-center justify-center rounded-full text-2xl ${
                                        estado.fase === 'valido'
                                            ? 'bg-emerald-100 text-emerald-700'
                                            : 'bg-amber-100 text-amber-700'
                                    }`}
                                >
                                    {estado.fase === 'valido' ? '✓' : '!'}
                                </div>
                                <p className="mt-4 font-semibold text-slate-900">
                                    {estado.fase === 'valido' ? 'Recibo válido' : 'Recibo anulado'}
                                </p>
                                {estado.fase === 'anulado' ? (
                                    <p className="mt-1 text-sm text-amber-700">
                                        El movimiento existe pero fue anulado. Este recibo ya no tiene validez.
                                    </p>
                                ) : null}
                            </div>

                            <dl className="mt-6 space-y-3 border-t border-slate-200 pt-6 text-sm">
                                <div className="flex justify-between">
                                    <dt className="text-slate-500">Número</dt>
                                    <dd className="font-medium text-slate-900">{estado.recibo.numeroConsecutivo}</dd>
                                </div>
                                <div className="flex justify-between">
                                    <dt className="text-slate-500">Fecha</dt>
                                    <dd className="text-slate-900">{estado.recibo.fecha.slice(0, 10)}</dd>
                                </div>
                                <div className="flex justify-between">
                                    <dt className="text-slate-500">Valor</dt>
                                    <dd className="font-semibold tabular-nums text-slate-900">
                                        {formatoCOP.format(estado.recibo.valorCOP)}
                                    </dd>
                                </div>
                            </dl>

                            <p className="mt-6 text-center text-xs leading-relaxed text-slate-400">
                                Esta verificación confirma que el movimiento existe en el sistema, su fecha y su
                                valor. Los demás datos del recibo solo están disponibles para el personal
                                autorizado.
                            </p>
                        </>
                    ) : null}
                </div>
            </div>
        </main>
    );
}
