'use client';

import {
    NOMBRE_ESTADO_ANULACION,
    useResolverAnulacion,
    useSolicitarAnulacion,
    useSolicitudesAnulacion,
    type SolicitudAnulacion,
} from '@/features/contabilidad/hooks/useAnulaciones';
import { useComprobantes } from '@/features/contabilidad/hooks/useComprobantes';
import { useRoleAccess } from '@/lib/useRoleAccess';
import { useState } from 'react';

/**
 * Anulacion de comprobantes con aprobacion (historia 1-8).
 *
 * Solo dentro del mismo mes contable y solo con aprobacion. Pasado el mes lo
 * que corresponde es un ajuste contable, y el backend lo dice con ese mensaje.
 */
const ROLES_LECTURA = ['Admin', 'Operador', 'Tesorero', 'Contador', 'Junta'] as const;
const ROLES_SOLICITAR = ['Operador', 'Tesorero', 'Admin'] as const;
const ROLES_RESOLVER = ['Tesorero', 'Contador', 'Admin'] as const;

function claseEstado(estado: number): string {
    if (estado === 2) return 'bg-emerald-100 text-emerald-800';
    if (estado === 3) return 'bg-rose-100 text-rose-800';
    return 'bg-amber-100 text-amber-800';
}

export default function AnulacionesPage() {
    const { canAccess, isRoleReady } = useRoleAccess(ROLES_LECTURA);
    const { canAccess: puedeSolicitar } = useRoleAccess(ROLES_SOLICITAR);
    const { canAccess: puedeResolver } = useRoleAccess(ROLES_RESOLVER);

    const solicitudesQuery = useSolicitudesAnulacion();
    const comprobantesQuery = useComprobantes();
    const solicitar = useSolicitarAnulacion();
    const resolver = useResolverAnulacion();

    const [comprobanteId, setComprobanteId] = useState('');
    const [motivo, setMotivo] = useState('');
    const [resolviendo, setResolviendo] = useState<{ solicitud: SolicitudAnulacion; aprobar: boolean } | null>(null);
    const [motivoResolucion, setMotivoResolucion] = useState('');
    const [mensaje, setMensaje] = useState<{ tipo: 'ok' | 'error'; texto: string } | null>(null);

    if (!isRoleReady) {
        return <p className="p-8 text-sm text-slate-500">Verificando permisos...</p>;
    }

    if (!canAccess) {
        return (
            <div className="p-8">
                <h1 className="text-xl font-bold text-slate-900">Sin permiso</h1>
                <p className="mt-2 text-sm text-slate-600">Las anulaciones requieren un rol con acceso contable.</p>
            </div>
        );
    }

    const onSolicitar = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        setMensaje(null);

        if (!comprobanteId || !motivo.trim()) {
            setMensaje({ tipo: 'error', texto: 'Comprobante y motivo son obligatorios.' });
            return;
        }

        try {
            await solicitar.mutateAsync({ comprobanteId, motivo: motivo.trim() });
            setComprobanteId('');
            setMotivo('');
            setMensaje({ tipo: 'ok', texto: 'Solicitud registrada. Queda a la espera de aprobación.' });
        } catch (error) {
            setMensaje({ tipo: 'error', texto: error instanceof Error ? error.message : 'Error al solicitar.' });
        }
    };

    const onResolver = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        if (!resolviendo) return;
        setMensaje(null);

        // Un rechazo sin motivo deja al solicitante sin saber qué corregir.
        if (!resolviendo.aprobar && !motivoResolucion.trim()) {
            setMensaje({ tipo: 'error', texto: 'Rechazar exige indicar el motivo.' });
            return;
        }

        try {
            await resolver.mutateAsync({
                id: resolviendo.solicitud.id,
                aprobar: resolviendo.aprobar,
                motivo: motivoResolucion.trim() || null,
            });

            setMensaje({
                tipo: 'ok',
                texto: resolviendo.aprobar
                    ? `Comprobante ${resolviendo.solicitud.numeroConsecutivo} anulado.`
                    : 'Solicitud rechazada.',
            });
            setResolviendo(null);
            setMotivoResolucion('');
        } catch (error) {
            setMensaje({ tipo: 'error', texto: error instanceof Error ? error.message : 'Error al resolver.' });
        }
    };

    const solicitudes = solicitudesQuery.data ?? [];
    const pendientes = solicitudes.filter((s) => s.estado === 1).length;
    const claseInput = 'w-full rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-900';

    return (
        <div className="p-8">
            <h1 className="text-2xl font-bold text-slate-900">Anulaciones</h1>
            <p className="mt-1 text-sm text-slate-600">
                Solo se anula dentro del mismo mes contable y con aprobación. Para un mes anterior corresponde
                registrar un ajuste contable.
            </p>

            {pendientes > 0 && puedeResolver ? (
                <div className="mt-4 rounded-lg border border-amber-300 bg-amber-50 px-3 py-2 text-sm text-amber-800">
                    {pendientes === 1
                        ? 'Hay 1 solicitud esperando su resolución.'
                        : `Hay ${pendientes} solicitudes esperando resolución.`}
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

            {puedeSolicitar ? (
                <form onSubmit={onSolicitar} className="mt-6 rounded-xl border border-slate-200 bg-white p-5">
                    <h2 className="text-lg font-semibold text-slate-900">Solicitar anulación</h2>

                    <div className="mt-4 grid grid-cols-1 gap-4 md:grid-cols-2">
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Comprobante</label>
                            <select
                                value={comprobanteId}
                                onChange={(e) => setComprobanteId(e.target.value)}
                                className={claseInput}
                            >
                                <option value="">Seleccione...</option>
                                {(comprobantesQuery.data ?? []).map((c) => (
                                    <option key={c.id} value={c.id}>
                                        {c.numeroConsecutivo} · {c.descripcion}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Motivo</label>
                            <input
                                value={motivo}
                                onChange={(e) => setMotivo(e.target.value)}
                                placeholder="Por qué debe anularse"
                                className={claseInput}
                            />
                        </div>
                    </div>

                    <button
                        type="submit"
                        disabled={solicitar.isPending}
                        className="mt-4 rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:opacity-60"
                    >
                        {solicitar.isPending ? 'Enviando...' : 'Solicitar anulación'}
                    </button>
                </form>
            ) : null}

            <div className="mt-6 overflow-x-auto rounded-xl border border-slate-200 bg-white">
                <table className="w-full text-sm">
                    <thead className="border-b border-slate-200 bg-slate-50 text-left text-slate-600">
                        <tr>
                            <th className="px-4 py-3 font-medium">Comprobante</th>
                            <th className="px-4 py-3 font-medium">Motivo</th>
                            <th className="px-4 py-3 font-medium">Solicitó</th>
                            <th className="px-4 py-3 font-medium">Estado</th>
                            <th className="px-4 py-3 font-medium">Resolución</th>
                            <th className="px-4 py-3 text-right font-medium">Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        {solicitudesQuery.isLoading ? (
                            <tr><td colSpan={6} className="px-4 py-6 text-center text-slate-500">Cargando...</td></tr>
                        ) : null}

                        {!solicitudesQuery.isLoading && solicitudes.length === 0 ? (
                            <tr><td colSpan={6} className="px-4 py-6 text-center text-slate-500">No hay solicitudes.</td></tr>
                        ) : null}

                        {solicitudes.map((s) => (
                            <tr key={s.id} className="border-b border-slate-100 last:border-0">
                                <td className="px-4 py-3">
                                    <span className="font-medium text-slate-900">{s.numeroConsecutivo}</span>
                                    <span className="block text-xs text-slate-400">{s.descripcionComprobante}</span>
                                </td>
                                <td className="px-4 py-3 text-slate-600">{s.motivoSolicitud}</td>
                                <td className="px-4 py-3 text-slate-600">
                                    {s.solicitadaPor ?? '—'}
                                    {s.fechaSolicitud ? (
                                        <span className="block text-xs text-slate-400">{s.fechaSolicitud.slice(0, 10)}</span>
                                    ) : null}
                                </td>
                                <td className="px-4 py-3">
                                    <span className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${claseEstado(s.estado)}`}>
                                        {NOMBRE_ESTADO_ANULACION[s.estado] ?? s.estado}
                                    </span>
                                </td>
                                <td className="px-4 py-3 text-slate-600">
                                    {s.resueltaPor ? (
                                        <>
                                            {s.resueltaPor}
                                            {s.motivoResolucion ? (
                                                <span className="block text-xs text-slate-400">{s.motivoResolucion}</span>
                                            ) : null}
                                        </>
                                    ) : (
                                        '—'
                                    )}
                                </td>
                                <td className="px-4 py-3">
                                    {s.estado === 1 && puedeResolver ? (
                                        <div className="flex items-center justify-end gap-2">
                                            <button
                                                type="button"
                                                onClick={() => {
                                                    setResolviendo({ solicitud: s, aprobar: true });
                                                    setMotivoResolucion('');
                                                }}
                                                className="rounded-lg bg-slate-900 px-3 py-1.5 text-xs font-medium text-white transition hover:bg-slate-800"
                                            >
                                                Aprobar
                                            </button>
                                            <button
                                                type="button"
                                                onClick={() => {
                                                    setResolviendo({ solicitud: s, aprobar: false });
                                                    setMotivoResolucion('');
                                                }}
                                                className="rounded-lg border border-slate-300 px-3 py-1.5 text-xs font-medium text-slate-700 transition hover:bg-slate-50"
                                            >
                                                Rechazar
                                            </button>
                                        </div>
                                    ) : null}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            {resolviendo ? (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/55 p-4">
                    <form onSubmit={onResolver} className="w-full max-w-md rounded-2xl border border-slate-200 bg-white p-6 shadow-2xl">
                        <h2 className="text-xl font-bold text-slate-900">
                            {resolviendo.aprobar ? 'Aprobar anulación' : 'Rechazar solicitud'}
                        </h2>
                        <p className="mt-1 text-sm text-slate-600">
                            Comprobante {resolviendo.solicitud.numeroConsecutivo}
                        </p>

                        {resolviendo.aprobar ? (
                            <div className="mt-3 rounded-lg border border-amber-300 bg-amber-50 px-3 py-2 text-sm text-amber-800">
                                Al aprobar, el comprobante queda anulado. No se puede deshacer.
                            </div>
                        ) : null}

                        <div className="mt-4">
                            <label className="mb-1 block text-sm font-medium text-slate-700">
                                Motivo {resolviendo.aprobar ? '(opcional)' : ''}
                            </label>
                            <textarea
                                rows={3}
                                value={motivoResolucion}
                                onChange={(e) => setMotivoResolucion(e.target.value)}
                                className={claseInput}
                            />
                        </div>

                        <div className="mt-5 flex justify-end gap-2">
                            <button
                                type="button"
                                onClick={() => setResolviendo(null)}
                                className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700"
                            >
                                Cancelar
                            </button>
                            <button
                                type="submit"
                                disabled={resolver.isPending}
                                className={`rounded-lg px-4 py-2 text-sm font-medium text-white disabled:opacity-60 ${
                                    resolviendo.aprobar ? 'bg-slate-900 hover:bg-slate-800' : 'bg-rose-700 hover:bg-rose-800'
                                }`}
                            >
                                {resolver.isPending ? 'Procesando...' : resolviendo.aprobar ? 'Aprobar y anular' : 'Rechazar'}
                            </button>
                        </div>
                    </form>
                </div>
            ) : null}
        </div>
    );
}
