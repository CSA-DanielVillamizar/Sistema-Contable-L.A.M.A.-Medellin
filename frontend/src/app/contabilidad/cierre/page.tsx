'use client';

import {
    NOMBRE_ESTADO,
    useCerrarPeriodo,
    usePeriodosContables,
    useValidarPeriodo,
    type PeriodoContable,
} from '@/features/contabilidad/hooks/usePeriodosContables';
import { useRoleAccess } from '@/lib/useRoleAccess';
import { useState } from 'react';

/**
 * Cierre mensual (historia 1-5).
 *
 * Son dos pasos con responsables distintos, tal como los separa la matriz del
 * BRD: el Tesorero da por revisado el mes y el Contador lo cierra. Una vez
 * cerrado, el periodo queda bloqueado para movimientos nuevos.
 */
const ROLES_PERMITIDOS = ['Contador', 'Tesorero', 'Junta', 'Admin'] as const;
const ROLES_VALIDAR = ['Tesorero', 'Contador', 'Admin'] as const;
const ROLES_CERRAR = ['Contador', 'Admin'] as const;

const MESES = [
    'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
    'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre',
];

function claseEstado(estado: number): string {
    if (estado === 2) return 'bg-slate-200 text-slate-700';
    if (estado === 1) return 'bg-amber-100 text-amber-800';
    return 'bg-emerald-100 text-emerald-800';
}

export default function CierrePeriodoPage() {
    const { canAccess, isRoleReady } = useRoleAccess(ROLES_PERMITIDOS);
    const { canAccess: puedeValidar } = useRoleAccess(ROLES_VALIDAR);
    const { canAccess: puedeCerrar } = useRoleAccess(ROLES_CERRAR);

    const periodosQuery = usePeriodosContables();
    const validar = useValidarPeriodo();
    const cerrar = useCerrarPeriodo();

    const [mensaje, setMensaje] = useState<{ tipo: 'ok' | 'error'; texto: string } | null>(null);

    if (!isRoleReady) {
        return <p className="p-8 text-sm text-slate-500">Verificando permisos...</p>;
    }

    if (!canAccess) {
        return (
            <div className="p-8">
                <h1 className="text-xl font-bold text-slate-900">Sin permiso</h1>
                <p className="mt-2 text-sm text-slate-600">
                    El cierre de periodo requiere rol Contador, Tesorero o Junta.
                </p>
            </div>
        );
    }

    const ejecutar = async (
        accion: typeof validar | typeof cerrar,
        periodo: PeriodoContable,
        exito: string,
    ) => {
        setMensaje(null);

        try {
            await accion.mutateAsync({ anio: periodo.anio, mes: periodo.mes });
            setMensaje({ tipo: 'ok', texto: exito });
        } catch (error) {
            setMensaje({ tipo: 'error', texto: error instanceof Error ? error.message : 'Ocurrió un error.' });
        }
    };

    const periodos = periodosQuery.data ?? [];

    return (
        <div className="p-8">
            <h1 className="text-2xl font-bold text-slate-900">Cierre de periodo</h1>
            <p className="mt-1 text-sm text-slate-600">
                Dos pasos: el Tesorero da por revisado el mes y el Contador lo cierra. Un periodo cerrado
                no admite movimientos nuevos.
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

            <div className="mt-6 overflow-x-auto rounded-xl border border-slate-200 bg-white">
                <table className="w-full text-sm">
                    <thead className="border-b border-slate-200 bg-slate-50 text-left text-slate-600">
                        <tr>
                            <th className="px-4 py-3 font-medium">Periodo</th>
                            <th className="px-4 py-3 font-medium">Estado</th>
                            <th className="px-4 py-3 font-medium">Validado por</th>
                            <th className="px-4 py-3 font-medium">Cerrado por</th>
                            <th className="px-4 py-3 text-right font-medium">Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        {periodosQuery.isLoading ? (
                            <tr>
                                <td colSpan={5} className="px-4 py-6 text-center text-slate-500">Cargando periodos...</td>
                            </tr>
                        ) : null}

                        {periodosQuery.isError ? (
                            <tr>
                                <td colSpan={5} className="px-4 py-6 text-center text-rose-700">
                                    No fue posible cargar los periodos.
                                </td>
                            </tr>
                        ) : null}

                        {!periodosQuery.isLoading && !periodosQuery.isError && periodos.length === 0 ? (
                            <tr>
                                <td colSpan={5} className="px-4 py-6 text-center text-slate-500">
                                    Todavía no hay periodos registrados. Se crean al registrar el primer movimiento del mes.
                                </td>
                            </tr>
                        ) : null}

                        {periodos.map((p) => (
                            <tr key={`${p.anio}-${p.mes}`} className="border-b border-slate-100 last:border-0">
                                <td className="px-4 py-3 font-medium text-slate-900">
                                    {MESES[p.mes - 1] ?? p.mes} {p.anio}
                                </td>
                                <td className="px-4 py-3">
                                    <span className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${claseEstado(p.estado)}`}>
                                        {NOMBRE_ESTADO[p.estado] ?? p.estado}
                                    </span>
                                </td>
                                <td className="px-4 py-3 text-slate-600">
                                    {p.validadoPor ?? '—'}
                                    {p.fechaValidacionTesoreria ? (
                                        <span className="block text-xs text-slate-400">
                                            {p.fechaValidacionTesoreria.slice(0, 10)}
                                        </span>
                                    ) : null}
                                </td>
                                <td className="px-4 py-3 text-slate-600">
                                    {p.cerradoPor ?? '—'}
                                    {p.fechaCierre ? (
                                        <span className="block text-xs text-slate-400">{p.fechaCierre.slice(0, 10)}</span>
                                    ) : null}
                                </td>
                                <td className="px-4 py-3">
                                    <div className="flex items-center justify-end gap-2">
                                        {p.estado === 0 && puedeValidar ? (
                                            <button
                                                type="button"
                                                onClick={() => void ejecutar(validar, p, `Periodo ${p.anio}-${String(p.mes).padStart(2, '0')} validado.`)}
                                                disabled={validar.isPending}
                                                className="rounded-lg border border-slate-300 px-3 py-1.5 text-xs font-medium text-slate-700 transition hover:bg-slate-50 disabled:opacity-60"
                                            >
                                                Validar
                                            </button>
                                        ) : null}

                                        {p.estado === 1 && puedeCerrar ? (
                                            <button
                                                type="button"
                                                onClick={() => void ejecutar(cerrar, p, `Periodo ${p.anio}-${String(p.mes).padStart(2, '0')} cerrado.`)}
                                                disabled={cerrar.isPending}
                                                className="rounded-lg bg-slate-900 px-3 py-1.5 text-xs font-medium text-white transition hover:bg-slate-800 disabled:opacity-60"
                                            >
                                                Cerrar
                                            </button>
                                        ) : null}

                                        {p.estado === 2 ? <span className="text-xs text-slate-400">Bloqueado</span> : null}
                                    </div>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
