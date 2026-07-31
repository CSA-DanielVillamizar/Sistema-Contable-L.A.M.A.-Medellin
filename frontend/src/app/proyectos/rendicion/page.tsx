'use client';

import {
    ESTADOS_ACTIVIDAD,
    useActividades,
    useCambiarEstadoActividad,
    useCrearActividad,
    useRendicion,
} from '@/features/proyectos/hooks/useRendicion';
import { useRoleAccess } from '@/lib/useRoleAccess';
import { useState } from 'react';

/**
 * Actividades y rendicion de cuentas (historias 3-1 y 3-4).
 *
 * Lo ejecutado sale de los asientos imputados al centro de costo del proyecto,
 * no de un campo que alguien mantenga a mano. Por eso puede diferir de lo
 * asignado a actividades: uno es planeacion y el otro es lo que el libro
 * respalda.
 */
const ROLES_LECTURA = ['Admin', 'Operador', 'Tesorero', 'Contador', 'Junta'] as const;
const ROLES_ESCRITURA = ['Operador', 'Admin'] as const;

const formatoCOP = new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0,
});

const hoy = () => new Date().toISOString().slice(0, 10);
const enDias = (n: number) => new Date(Date.now() + n * 86400000).toISOString().slice(0, 10);

const VACIO = {
    nombre: '',
    descripcion: '',
    fechaInicioPlanificada: hoy(),
    fechaFinPlanificada: enDias(30),
    presupuestoAsignado: '',
    responsable: '',
};

export default function RendicionPage() {
    const { canAccess, isRoleReady } = useRoleAccess(ROLES_LECTURA);
    const { canAccess: puedeEscribir } = useRoleAccess(ROLES_ESCRITURA);

    const rendicionQuery = useRendicion();
    const [seleccionado, setSeleccionado] = useState<string | null>(null);
    const actividadesQuery = useActividades(seleccionado);
    const crear = useCrearActividad();
    const cambiarEstado = useCambiarEstadoActividad();

    const [form, setForm] = useState(VACIO);
    const [mensaje, setMensaje] = useState<{ tipo: 'ok' | 'error'; texto: string } | null>(null);

    if (!isRoleReady) {
        return <p className="p-8 text-sm text-slate-500">Verificando permisos...</p>;
    }

    if (!canAccess) {
        return (
            <div className="p-8">
                <h1 className="text-xl font-bold text-slate-900">Sin permiso</h1>
                <p className="mt-2 text-sm text-slate-600">La rendición requiere un rol con acceso a proyectos.</p>
            </div>
        );
    }

    const proyectos = rendicionQuery.data ?? [];
    const activo = proyectos.find((p) => p.proyectoSocialId === seleccionado) ?? null;
    const campo = (k: keyof typeof VACIO, v: string) => setForm((p) => ({ ...p, [k]: v }));
    const claseInput = 'w-full rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-900';

    const onCrear = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        if (!seleccionado) return;
        setMensaje(null);

        const presupuesto = Number(form.presupuestoAsignado || 0);
        if (presupuesto < 0) {
            setMensaje({ tipo: 'error', texto: 'El presupuesto no puede ser negativo.' });
            return;
        }

        try {
            await crear.mutateAsync({
                proyectoSocialId: seleccionado,
                ...form,
                presupuestoAsignado: presupuesto,
                responsable: form.responsable.trim() || null,
            });
            setForm(VACIO);
            setMensaje({ tipo: 'ok', texto: 'Actividad creada.' });
        } catch (error) {
            setMensaje({ tipo: 'error', texto: error instanceof Error ? error.message : 'Error al crear.' });
        }
    };

    const onCambiarEstado = async (id: string, estado: number) => {
        setMensaje(null);
        try {
            await cambiarEstado.mutateAsync({ id, estado });
        } catch (error) {
            setMensaje({ tipo: 'error', texto: error instanceof Error ? error.message : 'Error.' });
        }
    };

    return (
        <div className="p-8">
            <h1 className="text-2xl font-bold text-slate-900">Rendición de proyectos</h1>
            <p className="mt-1 text-sm text-slate-600">
                Lo ejecutado sale de los asientos imputados al centro de costo del proyecto. Puede diferir de lo
                asignado a actividades: uno es planeación, el otro es lo que respalda el libro.
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

            {rendicionQuery.isLoading ? <p className="mt-5 text-sm text-slate-500">Cargando proyectos...</p> : null}
            {!rendicionQuery.isLoading && proyectos.length === 0 ? (
                <p className="mt-5 text-sm text-slate-500">Todavía no hay proyectos registrados.</p>
            ) : null}

            <div className="mt-6 grid grid-cols-1 gap-4 lg:grid-cols-2">
                {proyectos.map((p) => {
                    const ejecucion = Math.min(p.porcentajeEjecucion, 100);
                    const excedido = p.porcentajeEjecucion > 100;

                    return (
                        <article
                            key={p.proyectoSocialId}
                            className={`cursor-pointer rounded-xl border bg-white p-5 transition ${
                                seleccionado === p.proyectoSocialId ? 'border-slate-900' : 'border-slate-200 hover:border-slate-400'
                            }`}
                            onClick={() => setSeleccionado(seleccionado === p.proyectoSocialId ? null : p.proyectoSocialId)}
                        >
                            <div className="flex items-start justify-between gap-3">
                                <h3 className="font-semibold text-slate-900">{p.nombre}</h3>
                                <span className="shrink-0 rounded-full bg-slate-100 px-2.5 py-0.5 text-xs font-medium text-slate-700">
                                    {p.estado}
                                </span>
                            </div>

                            <div className="mt-4">
                                <div className="flex items-baseline justify-between text-sm">
                                    <span className="font-medium tabular-nums text-slate-900">{formatoCOP.format(p.ejecutadoCOP)}</span>
                                    <span className="text-slate-500">de {formatoCOP.format(p.presupuestoEstimado)}</span>
                                </div>
                                <div className="mt-2 h-2 overflow-hidden rounded-full bg-slate-100">
                                    <div className={`h-full rounded-full ${excedido ? 'bg-rose-600' : 'bg-slate-700'}`} style={{ width: `${ejecucion}%` }} />
                                </div>
                                <p className={`mt-1 text-xs ${excedido ? 'font-medium text-rose-700' : 'text-slate-500'}`}>
                                    {p.porcentajeEjecucion}% ejecutado{excedido ? ' · presupuesto excedido' : ''}
                                </p>
                            </div>

                            <dl className="mt-4 grid grid-cols-2 gap-3 text-sm">
                                <div>
                                    <dt className="text-xs text-slate-400">Avance de actividades</dt>
                                    <dd className="font-medium text-slate-900">
                                        {p.porcentajeAvanceActividades}% ({p.actividadesCompletadas}/{p.totalActividades})
                                    </dd>
                                </div>
                                <div>
                                    <dt className="text-xs text-slate-400">Vencidas</dt>
                                    <dd className={p.actividadesVencidas > 0 ? 'font-medium text-amber-700' : 'text-slate-700'}>
                                        {p.actividadesVencidas}
                                    </dd>
                                </div>
                                <div>
                                    <dt className="text-xs text-slate-400">Asignado a actividades</dt>
                                    <dd className="tabular-nums text-slate-700">{formatoCOP.format(p.presupuestoAsignadoAActividades)}</dd>
                                </div>
                                <div>
                                    <dt className="text-xs text-slate-400">Beneficiarios</dt>
                                    <dd className="text-slate-700">{p.totalBeneficiarios}</dd>
                                </div>
                            </dl>
                        </article>
                    );
                })}
            </div>

            {activo ? (
                <section className="mt-8">
                    <h2 className="text-lg font-semibold text-slate-900">Actividades de {activo.nombre}</h2>

                    {puedeEscribir ? (
                        <form onSubmit={onCrear} className="mt-4 rounded-xl border border-slate-200 bg-white p-5">
                            <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
                                <div className="md:col-span-2">
                                    <label className="mb-1 block text-sm font-medium text-slate-700">Nombre</label>
                                    <input value={form.nombre} onChange={(e) => campo('nombre', e.target.value)} className={claseInput} required />
                                </div>
                                <div>
                                    <label className="mb-1 block text-sm font-medium text-slate-700">Presupuesto</label>
                                    <input type="number" min="0" step="10000" value={form.presupuestoAsignado} onChange={(e) => campo('presupuestoAsignado', e.target.value)} className={claseInput} />
                                </div>
                                <div className="md:col-span-3">
                                    <label className="mb-1 block text-sm font-medium text-slate-700">Descripción</label>
                                    <input value={form.descripcion} onChange={(e) => campo('descripcion', e.target.value)} className={claseInput} required />
                                </div>
                                <div>
                                    <label className="mb-1 block text-sm font-medium text-slate-700">Desde</label>
                                    <input type="date" value={form.fechaInicioPlanificada} onChange={(e) => campo('fechaInicioPlanificada', e.target.value)} className={claseInput} required />
                                </div>
                                <div>
                                    <label className="mb-1 block text-sm font-medium text-slate-700">Hasta</label>
                                    <input type="date" value={form.fechaFinPlanificada} onChange={(e) => campo('fechaFinPlanificada', e.target.value)} className={claseInput} required />
                                </div>
                                <div>
                                    <label className="mb-1 block text-sm font-medium text-slate-700">Responsable</label>
                                    <input value={form.responsable} onChange={(e) => campo('responsable', e.target.value)} className={claseInput} />
                                </div>
                            </div>

                            <button type="submit" disabled={crear.isPending} className="mt-4 rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:opacity-60">
                                {crear.isPending ? 'Creando...' : 'Agregar actividad'}
                            </button>
                        </form>
                    ) : null}

                    <div className="mt-4 overflow-x-auto rounded-xl border border-slate-200 bg-white">
                        <table className="w-full text-sm">
                            <thead className="border-b border-slate-200 bg-slate-50 text-left text-slate-600">
                                <tr>
                                    <th className="px-4 py-3 font-medium">Actividad</th>
                                    <th className="px-4 py-3 font-medium">Plazo</th>
                                    <th className="px-4 py-3 font-medium">Responsable</th>
                                    <th className="px-4 py-3 text-right font-medium">Presupuesto</th>
                                    <th className="px-4 py-3 font-medium">Estado</th>
                                </tr>
                            </thead>
                            <tbody>
                                {actividadesQuery.isLoading ? (
                                    <tr><td colSpan={5} className="px-4 py-6 text-center text-slate-500">Cargando...</td></tr>
                                ) : null}

                                {!actividadesQuery.isLoading && (actividadesQuery.data ?? []).length === 0 ? (
                                    <tr><td colSpan={5} className="px-4 py-6 text-center text-slate-500">Este proyecto no tiene actividades.</td></tr>
                                ) : null}

                                {(actividadesQuery.data ?? []).map((a) => {
                                    const cerrada = a.estado === 3 || a.estado === 4;

                                    return (
                                        <tr key={a.id} className="border-b border-slate-100 last:border-0">
                                            <td className="px-4 py-3">
                                                <span className="font-medium text-slate-900">{a.nombre}</span>
                                                <span className="block text-xs text-slate-400">{a.descripcion}</span>
                                            </td>
                                            <td className={`px-4 py-3 ${a.estaVencida ? 'font-medium text-amber-700' : 'text-slate-600'}`}>
                                                {a.fechaInicioPlanificada.slice(0, 10)} → {a.fechaFinPlanificada.slice(0, 10)}
                                                {a.estaVencida ? <span className="block text-xs">Vencida</span> : null}
                                            </td>
                                            <td className="px-4 py-3 text-slate-600">{a.responsable ?? '—'}</td>
                                            <td className="px-4 py-3 text-right tabular-nums text-slate-700">{formatoCOP.format(a.presupuestoAsignado)}</td>
                                            <td className="px-4 py-3">
                                                {puedeEscribir && !cerrada ? (
                                                    <select
                                                        value={a.estado}
                                                        onChange={(e) => void onCambiarEstado(a.id, Number(e.target.value))}
                                                        className="rounded-lg border border-slate-300 px-2 py-1 text-xs text-slate-900"
                                                    >
                                                        {ESTADOS_ACTIVIDAD.map((e) => (
                                                            <option key={e.valor} value={e.valor}>{e.nombre}</option>
                                                        ))}
                                                    </select>
                                                ) : (
                                                    <span className="rounded-full bg-slate-100 px-2.5 py-0.5 text-xs font-medium text-slate-700">
                                                        {a.nombreEstado}
                                                    </span>
                                                )}
                                            </td>
                                        </tr>
                                    );
                                })}
                            </tbody>
                        </table>
                    </div>
                </section>
            ) : proyectos.length > 0 ? (
                <p className="mt-6 text-sm text-slate-500">Seleccione un proyecto para ver y gestionar sus actividades.</p>
            ) : null}
        </div>
    );
}
