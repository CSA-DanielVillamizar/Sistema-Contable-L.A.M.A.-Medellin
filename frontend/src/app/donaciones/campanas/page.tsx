'use client';

import {
    useActualizarCampana,
    useCambiarEstadoCampana,
    useCampanas,
    useCrearCampana,
    type Campana,
} from '@/features/donaciones/hooks/useCampanas';
import { useRoleAccess } from '@/lib/useRoleAccess';
import { useState } from 'react';

/**
 * Campanas de donacion (historias 2-1 y 2-2).
 *
 * Agrupan donaciones bajo un proposito y una ventana de tiempo. La barra de
 * avance es lo que permite decir cuanto se recaudo de lo que se pretendia.
 */
const ROLES_LECTURA = ['Admin', 'Operador', 'Tesorero', 'Contador', 'Junta'] as const;
const ROLES_ESCRITURA = ['Operador', 'Tesorero', 'Admin'] as const;

const formatoCOP = new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0,
});

const hoy = () => new Date().toISOString().slice(0, 10);
const enMeses = (n: number) => new Date(Date.now() + n * 30 * 86400000).toISOString().slice(0, 10);

const VACIO = { nombre: '', descripcion: '', metaCOP: '', fechaInicio: hoy(), fechaFin: enMeses(3) };

export default function CampanasPage() {
    const { canAccess, isRoleReady } = useRoleAccess(ROLES_LECTURA);
    const { canAccess: puedeEscribir } = useRoleAccess(ROLES_ESCRITURA);

    const campanasQuery = useCampanas();
    const crear = useCrearCampana();
    const actualizar = useActualizarCampana();
    const cambiarEstado = useCambiarEstadoCampana();

    const [form, setForm] = useState(VACIO);
    const [editando, setEditando] = useState<Campana | null>(null);
    const [mensaje, setMensaje] = useState<{ tipo: 'ok' | 'error'; texto: string } | null>(null);

    if (!isRoleReady) {
        return <p className="p-8 text-sm text-slate-500">Verificando permisos...</p>;
    }

    if (!canAccess) {
        return (
            <div className="p-8">
                <h1 className="text-xl font-bold text-slate-900">Sin permiso</h1>
                <p className="mt-2 text-sm text-slate-600">Las campañas requieren un rol con acceso a donaciones.</p>
            </div>
        );
    }

    const campo = (k: keyof typeof VACIO, v: string) => setForm((p) => ({ ...p, [k]: v }));

    const onEnviar = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        setMensaje(null);

        const meta = Number(form.metaCOP);
        if (!Number.isFinite(meta) || meta <= 0) {
            setMensaje({ tipo: 'error', texto: 'La meta debe ser mayor a cero.' });
            return;
        }

        const payload = { ...form, metaCOP: meta };

        try {
            if (editando) {
                await actualizar.mutateAsync({ id: editando.id, ...payload });
                setMensaje({ tipo: 'ok', texto: 'Campaña actualizada.' });
                setEditando(null);
            } else {
                await crear.mutateAsync(payload);
                setMensaje({ tipo: 'ok', texto: 'Campaña creada.' });
            }
            setForm(VACIO);
        } catch (error) {
            setMensaje({ tipo: 'error', texto: error instanceof Error ? error.message : 'Error al guardar.' });
        }
    };

    const editar = (c: Campana) => {
        setEditando(c);
        setForm({
            nombre: c.nombre,
            descripcion: c.descripcion,
            metaCOP: String(c.metaCOP),
            fechaInicio: c.fechaInicio.slice(0, 10),
            fechaFin: c.fechaFin.slice(0, 10),
        });
    };

    const alternar = async (c: Campana) => {
        setMensaje(null);
        try {
            await cambiarEstado.mutateAsync({ id: c.id, activa: !c.estaActiva });
        } catch (error) {
            setMensaje({ tipo: 'error', texto: error instanceof Error ? error.message : 'Error.' });
        }
    };

    const campanas = campanasQuery.data ?? [];
    const claseInput = 'w-full rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-900';

    return (
        <div className="p-8">
            <h1 className="text-2xl font-bold text-slate-900">Campañas de donación</h1>
            <p className="mt-1 text-sm text-slate-600">
                Agrupan donaciones bajo un propósito y una ventana de tiempo. La meta es una referencia:
                recaudar por encima no es un error.
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

            {puedeEscribir ? (
                <form onSubmit={onEnviar} className="mt-6 rounded-xl border border-slate-200 bg-white p-5">
                    <h2 className="text-lg font-semibold text-slate-900">
                        {editando ? `Editar "${editando.nombre}"` : 'Nueva campaña'}
                    </h2>

                    <div className="mt-4 grid grid-cols-1 gap-4 md:grid-cols-2">
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Nombre</label>
                            <input value={form.nombre} onChange={(e) => campo('nombre', e.target.value)} className={claseInput} required />
                        </div>
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Meta (COP)</label>
                            <input type="number" min="0" step="10000" value={form.metaCOP} onChange={(e) => campo('metaCOP', e.target.value)} className={claseInput} required />
                        </div>
                        <div className="md:col-span-2">
                            <label className="mb-1 block text-sm font-medium text-slate-700">Descripción</label>
                            <input value={form.descripcion} onChange={(e) => campo('descripcion', e.target.value)} className={claseInput} required />
                        </div>
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Desde</label>
                            <input type="date" value={form.fechaInicio} onChange={(e) => campo('fechaInicio', e.target.value)} className={claseInput} required />
                        </div>
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Hasta</label>
                            <input type="date" value={form.fechaFin} onChange={(e) => campo('fechaFin', e.target.value)} className={claseInput} required />
                        </div>
                    </div>

                    <div className="mt-4 flex gap-2">
                        <button type="submit" disabled={crear.isPending || actualizar.isPending} className="rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:opacity-60">
                            {crear.isPending || actualizar.isPending ? 'Guardando...' : editando ? 'Guardar cambios' : 'Crear campaña'}
                        </button>
                        {editando ? (
                            <button type="button" onClick={() => { setEditando(null); setForm(VACIO); }} className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700">
                                Cancelar
                            </button>
                        ) : null}
                    </div>
                </form>
            ) : null}

            <div className="mt-6 grid grid-cols-1 gap-4 lg:grid-cols-2">
                {campanasQuery.isLoading ? <p className="text-sm text-slate-500">Cargando campañas...</p> : null}
                {!campanasQuery.isLoading && campanas.length === 0 ? (
                    <p className="text-sm text-slate-500">Todavía no hay campañas.</p>
                ) : null}

                {campanas.map((c) => {
                    const avance = Math.min(c.porcentajeAvance, 100);

                    return (
                        <article key={c.id} className="rounded-xl border border-slate-200 bg-white p-5">
                            <div className="flex items-start justify-between gap-3">
                                <div>
                                    <h3 className="font-semibold text-slate-900">{c.nombre}</h3>
                                    <p className="mt-0.5 text-sm text-slate-600">{c.descripcion}</p>
                                </div>
                                <span
                                    className={`shrink-0 rounded-full px-2.5 py-0.5 text-xs font-medium ${
                                        !c.estaActiva
                                            ? 'bg-slate-200 text-slate-700'
                                            : c.estaVigente
                                              ? 'bg-emerald-100 text-emerald-800'
                                              : 'bg-amber-100 text-amber-800'
                                    }`}
                                >
                                    {!c.estaActiva ? 'Cerrada' : c.estaVigente ? 'Vigente' : 'Fuera de fecha'}
                                </span>
                            </div>

                            <div className="mt-4">
                                <div className="flex items-baseline justify-between text-sm">
                                    <span className="font-medium tabular-nums text-slate-900">{formatoCOP.format(c.recaudadoCOP)}</span>
                                    <span className="text-slate-500">de {formatoCOP.format(c.metaCOP)}</span>
                                </div>

                                <div className="mt-2 h-2 overflow-hidden rounded-full bg-slate-100">
                                    <div
                                        className={`h-full rounded-full ${c.porcentajeAvance >= 100 ? 'bg-emerald-600' : 'bg-slate-700'}`}
                                        style={{ width: `${avance}%` }}
                                    />
                                </div>

                                <div className="mt-2 flex justify-between text-xs text-slate-500">
                                    <span>{c.porcentajeAvance}% · {c.cantidadDonaciones} donación(es)</span>
                                    <span>{c.fechaInicio.slice(0, 10)} → {c.fechaFin.slice(0, 10)}</span>
                                </div>
                            </div>

                            {puedeEscribir ? (
                                <div className="mt-4 flex gap-2">
                                    <button type="button" onClick={() => editar(c)} className="rounded-lg border border-slate-300 px-3 py-1.5 text-xs font-medium text-slate-700 transition hover:bg-slate-50">
                                        Editar
                                    </button>
                                    <button type="button" onClick={() => void alternar(c)} disabled={cambiarEstado.isPending} className="rounded-lg border border-slate-300 px-3 py-1.5 text-xs font-medium text-slate-700 transition hover:bg-slate-50 disabled:opacity-60">
                                        {c.estaActiva ? 'Cerrar' : 'Reabrir'}
                                    </button>
                                </div>
                            ) : null}
                        </article>
                    );
                })}
            </div>
        </div>
    );
}
