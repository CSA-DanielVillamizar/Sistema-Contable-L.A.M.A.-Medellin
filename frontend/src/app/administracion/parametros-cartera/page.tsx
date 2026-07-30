'use client';

import {
    TIPOS_AFILIACION,
    useActualizarRenovacionMembresia,
    useActualizarTarifas,
    useTarifasCuota,
} from '@/features/administracion/hooks/useParametrosCartera';
import { useRoleAccess } from '@/lib/useRoleAccess';
import { useState } from 'react';

/**
 * Parametros de cartera (historias 0-7 y 1-9).
 *
 * El cliente lo pidio explicitamente: antes de que el sistema genere la cartera
 * masiva del mes, el Tesorero entra aqui y confirma o actualiza el valor. Hasta
 * ahora el backend existia pero no habia pantalla, asi que el valor solo se
 * cambiaba llamando al API a mano.
 */
const ROLES_LECTURA = ['Admin', 'Tesorero', 'Contador', 'Operador'] as const;
const ROLES_ESCRITURA = ['Admin'] as const;

const formatoCOP = new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0,
});

export default function ParametrosCarteraPage() {
    const { canAccess, isRoleReady } = useRoleAccess(ROLES_LECTURA);
    const { canAccess: puedeEditar } = useRoleAccess(ROLES_ESCRITURA);

    const tarifasQuery = useTarifasCuota();
    const guardarTarifas = useActualizarTarifas();
    const guardarRenovacion = useActualizarRenovacionMembresia();

    const [editadas, setEditadas] = useState<Record<number, string>>({});
    const [anio, setAnio] = useState(new Date().getFullYear());
    const [renovacionUSD, setRenovacionUSD] = useState('20');
    const [mensaje, setMensaje] = useState<{ tipo: 'ok' | 'error'; texto: string } | null>(null);

    if (!isRoleReady) {
        return <p className="p-8 text-sm text-slate-500">Verificando permisos...</p>;
    }

    if (!canAccess) {
        return (
            <div className="p-8">
                <h1 className="text-xl font-bold text-slate-900">Sin permiso</h1>
                <p className="mt-2 text-sm text-slate-600">
                    Los parámetros de cartera requieren un rol con acceso a configuración.
                </p>
            </div>
        );
    }

    const tarifas = tarifasQuery.data ?? [];

    const valorDe = (tipo: number): string => {
        if (editadas[tipo] !== undefined) return editadas[tipo];
        return String(tarifas.find((t) => t.tipoAfiliacion === tipo)?.valorMensualCOP ?? 0);
    };

    const onGuardarTarifas = async () => {
        setMensaje(null);

        const payload = TIPOS_AFILIACION.map((t) => ({
            tipoAfiliacion: t.valor,
            valorMensualCOP: Number(valorDe(t.valor)) || 0,
        }));

        if (payload.some((t) => t.valorMensualCOP < 0)) {
            setMensaje({ tipo: 'error', texto: 'Ninguna tarifa puede ser negativa.' });
            return;
        }

        try {
            await guardarTarifas.mutateAsync(payload);
            setEditadas({});
            setMensaje({ tipo: 'ok', texto: 'Tarifas actualizadas. Aplican a la próxima generación de cartera.' });
        } catch (error) {
            setMensaje({ tipo: 'error', texto: error instanceof Error ? error.message : 'Error al guardar.' });
        }
    };

    const onGuardarRenovacion = async () => {
        setMensaje(null);
        const valor = Number(renovacionUSD);

        if (!Number.isFinite(valor) || valor < 0) {
            setMensaje({ tipo: 'error', texto: 'El valor en USD debe ser un número no negativo.' });
            return;
        }

        try {
            await guardarRenovacion.mutateAsync({ anio, valorUSD: valor });
            setMensaje({ tipo: 'ok', texto: `Renovación de membresía ${anio} fijada en ${valor} USD.` });
        } catch (error) {
            setMensaje({ tipo: 'error', texto: error instanceof Error ? error.message : 'Error al guardar.' });
        }
    };

    return (
        <div className="p-8">
            <h1 className="text-2xl font-bold text-slate-900">Parámetros de cartera</h1>
            <p className="mt-1 text-sm text-slate-600">
                Confirme estos valores antes de generar la cartera del mes. La cuota se define en la primera
                asamblea de cada mes.
            </p>

            {mensaje ? (
                <div
                    className={`mt-4 rounded-lg border px-3 py-2 text-sm ${
                        mensaje.tipo === 'ok'
                            ? 'border-emerald-200 bg-emerald-50 text-emerald-800'
                            : 'border-rose-200 bg-rose-50 text-rose-700'
                    }`}
                >
                    {mensaje.texto}
                </div>
            ) : null}

            <section className="mt-6 rounded-xl border border-slate-200 bg-white p-5">
                <h2 className="text-lg font-semibold text-slate-900">Cuota mensual por tipo de afiliación</h2>

                {tarifasQuery.isLoading ? <p className="mt-3 text-sm text-slate-500">Cargando tarifas...</p> : null}

                <div className="mt-4 space-y-3">
                    {TIPOS_AFILIACION.map((tipo) => (
                        <div key={tipo.valor} className="flex flex-wrap items-center gap-4">
                            <label className="w-52 text-sm font-medium text-slate-700">{tipo.nombre}</label>

                            {'exento' in tipo && tipo.exento ? (
                                <span className="rounded-lg bg-slate-100 px-3 py-2 text-sm text-slate-600">
                                    Exento de cuota mensual
                                </span>
                            ) : (
                                <>
                                    <input
                                        type="number"
                                        min="0"
                                        step="1000"
                                        value={valorDe(tipo.valor)}
                                        onChange={(e) =>
                                            setEditadas((prev) => ({ ...prev, [tipo.valor]: e.target.value }))
                                        }
                                        disabled={!puedeEditar}
                                        className="w-40 rounded-lg border border-slate-300 px-3 py-2 text-slate-900 disabled:bg-slate-50"
                                    />
                                    <span className="text-sm text-slate-500">
                                        {formatoCOP.format(Number(valorDe(tipo.valor)) || 0)}
                                    </span>
                                </>
                            )}
                        </div>
                    ))}
                </div>

                {puedeEditar ? (
                    <button
                        type="button"
                        onClick={() => void onGuardarTarifas()}
                        disabled={guardarTarifas.isPending}
                        className="mt-5 rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:opacity-60"
                    >
                        {guardarTarifas.isPending ? 'Guardando...' : 'Guardar tarifas'}
                    </button>
                ) : (
                    <p className="mt-5 text-xs text-slate-500">Solo un Admin puede modificar estos valores.</p>
                )}
            </section>

            <section className="mt-6 rounded-xl border border-slate-200 bg-white p-5">
                <h2 className="text-lg font-semibold text-slate-900">Renovación de membresía internacional</h2>
                <p className="mt-1 text-sm text-slate-600">
                    Se recauda en diciembre. El capítulo solo hace de puente hacia el comité internacional, así
                    que no es ingreso propio: se registra contra la cuenta 281505, Ingresos Recibidos para
                    Terceros.
                </p>

                <div className="mt-4 flex flex-wrap items-end gap-4">
                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Año</label>
                        <input
                            type="number"
                            value={anio}
                            onChange={(e) => setAnio(Number(e.target.value))}
                            disabled={!puedeEditar}
                            className="w-32 rounded-lg border border-slate-300 px-3 py-2 text-slate-900 disabled:bg-slate-50"
                        />
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Valor en USD</label>
                        <input
                            type="number"
                            min="0"
                            step="1"
                            value={renovacionUSD}
                            onChange={(e) => setRenovacionUSD(e.target.value)}
                            disabled={!puedeEditar}
                            className="w-32 rounded-lg border border-slate-300 px-3 py-2 text-slate-900 disabled:bg-slate-50"
                        />
                    </div>

                    {puedeEditar ? (
                        <button
                            type="button"
                            onClick={() => void onGuardarRenovacion()}
                            disabled={guardarRenovacion.isPending}
                            className="rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:opacity-60"
                        >
                            {guardarRenovacion.isPending ? 'Guardando...' : 'Guardar'}
                        </button>
                    ) : null}
                </div>
            </section>
        </div>
    );
}
