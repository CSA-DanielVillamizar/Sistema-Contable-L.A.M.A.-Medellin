'use client';

import {
    useActualizarMapeoContable,
    useMapeoContable,
} from '@/features/administracion/hooks/useMapeoContable';
import { useCuentasContables } from '@/features/contabilidad/hooks/useCuentasContables';
import { useRoleAccess } from '@/lib/useRoleAccess';
import { useState } from 'react';

/**
 * Mapeo de cuentas por operacion (historia 1-2).
 *
 * Saca del codigo una decision que es del contador. Cada cambio queda auditado
 * por la pista de BaseEntity, que es lo que pide el criterio de la historia.
 */
const ROLES_LECTURA = ['Admin', 'Operador', 'Tesorero', 'Contador'] as const;
const ROLES_ESCRITURA = ['Admin'] as const;

export default function MapeoContablePage() {
    const { canAccess, isRoleReady } = useRoleAccess(ROLES_LECTURA);
    const { canAccess: puedeEditar } = useRoleAccess(ROLES_ESCRITURA);

    const mapeoQuery = useMapeoContable();
    const cuentasQuery = useCuentasContables();
    const guardar = useActualizarMapeoContable();

    const [mensaje, setMensaje] = useState<{ tipo: 'ok' | 'error'; texto: string } | null>(null);
    const [guardando, setGuardando] = useState<number | null>(null);

    if (!isRoleReady) {
        return <p className="p-8 text-sm text-slate-500">Verificando permisos...</p>;
    }

    if (!canAccess) {
        return (
            <div className="p-8">
                <h1 className="text-xl font-bold text-slate-900">Sin permiso</h1>
                <p className="mt-2 text-sm text-slate-600">
                    El mapeo contable requiere un rol con acceso a configuración.
                </p>
            </div>
        );
    }

    // Solo cuentas que admiten movimiento: mapear a una de agrupación haría
    // fallar todo asiento que la usara. El backend impone la misma regla.
    const cuentasAsentables = (cuentasQuery.data ?? []).filter((c) => c.permiteMovimiento);

    const onCambiar = async (tipoOperacion: number, cuentaContableId: string) => {
        if (!cuentaContableId) return;

        setMensaje(null);
        setGuardando(tipoOperacion);

        try {
            await guardar.mutateAsync({ tipoOperacion, cuentaContableId });
            setMensaje({ tipo: 'ok', texto: 'Mapeo actualizado. El cambio queda registrado con su autor.' });
        } catch (error) {
            setMensaje({ tipo: 'error', texto: error instanceof Error ? error.message : 'Error al guardar.' });
        } finally {
            setGuardando(null);
        }
    };

    const mapeos = mapeoQuery.data ?? [];
    const pendientes = mapeos.filter((m) => !m.cuentaContableId).length;

    return (
        <div className="p-8">
            <h1 className="text-2xl font-bold text-slate-900">Mapeo contable</h1>
            <p className="mt-1 text-sm text-slate-600">
                Qué cuenta usa cada operación del negocio. Cambiar un mapeo afecta a los movimientos que se
                registren después, no a los ya asentados.
            </p>

            {pendientes > 0 ? (
                <div className="mt-4 rounded-lg border border-amber-300 bg-amber-50 px-3 py-2 text-sm text-amber-800">
                    {pendientes === 1
                        ? 'Hay 1 operación sin cuenta asignada.'
                        : `Hay ${pendientes} operaciones sin cuenta asignada.`}{' '}
                    Los movimientos que las usen fallarán hasta configurarlas.
                </div>
            ) : null}

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
                            <th className="px-4 py-3 font-medium">Operación</th>
                            <th className="px-4 py-3 font-medium">Cuenta asignada</th>
                            <th className="px-4 py-3 font-medium">Estado</th>
                        </tr>
                    </thead>
                    <tbody>
                        {mapeoQuery.isLoading ? (
                            <tr>
                                <td colSpan={3} className="px-4 py-6 text-center text-slate-500">Cargando...</td>
                            </tr>
                        ) : null}

                        {mapeoQuery.isError ? (
                            <tr>
                                <td colSpan={3} className="px-4 py-6 text-center text-rose-700">
                                    No fue posible cargar el mapeo.
                                </td>
                            </tr>
                        ) : null}

                        {mapeos.map((m) => (
                            <tr key={m.tipoOperacion} className="border-b border-slate-100 last:border-0">
                                <td className="px-4 py-3 font-medium text-slate-900">{m.nombreOperacion}</td>
                                <td className="px-4 py-3">
                                    <select
                                        value={m.cuentaContableId ?? ''}
                                        onChange={(e) => void onCambiar(m.tipoOperacion, e.target.value)}
                                        disabled={!puedeEditar || guardando === m.tipoOperacion}
                                        className={`w-full max-w-md rounded-lg border px-3 py-2 text-sm text-slate-900 disabled:bg-slate-50 ${
                                            m.cuentaContableId ? 'border-slate-300' : 'border-amber-400 bg-amber-50'
                                        }`}
                                    >
                                        <option value="">Sin asignar...</option>
                                        {cuentasAsentables.map((c) => (
                                            <option key={c.id} value={c.id}>
                                                {c.codigo} - {c.descripcion}
                                            </option>
                                        ))}
                                    </select>
                                </td>
                                <td className="px-4 py-3">
                                    {guardando === m.tipoOperacion ? (
                                        <span className="text-xs text-slate-500">Guardando...</span>
                                    ) : m.cuentaContableId ? (
                                        <span className="rounded-full bg-emerald-100 px-2.5 py-0.5 text-xs font-medium text-emerald-800">
                                            Configurada
                                        </span>
                                    ) : (
                                        <span className="rounded-full bg-amber-100 px-2.5 py-0.5 text-xs font-medium text-amber-800">
                                            Pendiente
                                        </span>
                                    )}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            {!puedeEditar ? (
                <p className="mt-4 text-xs text-slate-500">Solo un Admin puede modificar el mapeo.</p>
            ) : null}
        </div>
    );
}
