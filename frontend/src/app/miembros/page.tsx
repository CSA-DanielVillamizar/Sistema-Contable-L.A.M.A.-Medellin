'use client';

import MiembroUpsertModal from '@/features/miembros/components/MiembroUpsertModal';
import { useGetMiembros } from '@/features/miembros/hooks/useGetMiembros';
import type { Miembro } from '@/features/miembros/services/miembrosService';
import { useMemo, useState } from 'react';

function estadoBadgeClass(esActivo: boolean): string {
    return esActivo
        ? 'border-emerald-300 bg-emerald-50 text-emerald-700'
        : 'border-slate-300 bg-slate-100 text-slate-700';
}

function getInitials(miembro: Miembro): string {
    const nombre = `${miembro.nombres} ${miembro.apellidos}`.trim();
    const words = nombre.split(' ').filter(Boolean);
    return words.slice(0, 2).map((word) => word[0]?.toUpperCase() ?? '').join('') || 'MM';
}

function formatFecha(fechaIso: string): string {
    const date = new Date(fechaIso);
    if (Number.isNaN(date.getTime())) {
        return fechaIso;
    }

    return date.toLocaleDateString('es-CO', {
        year: 'numeric',
        month: 'short',
        day: '2-digit',
    });
}

function formatNombreCompleto(miembro: Miembro): string {
    return `${miembro.nombres} ${miembro.apellidos}`.trim();
}

export default function MiembrosPage() {
    const { data: miembros = [], isLoading, isError, error } = useGetMiembros();
    const [miembroActivoId, setMiembroActivoId] = useState<string | null>(null);
    const [modoFormulario, setModoFormulario] = useState<'create' | 'edit' | null>(null);

    const miembroActivo = useMemo(() => {
        if (!miembroActivoId) {
            return miembros[0] ?? null;
        }

        return miembros.find((item) => item.id === miembroActivoId) ?? miembros[0] ?? null;
    }, [miembros, miembroActivoId]);

    const abrirNuevo = () => setModoFormulario('create');
    const abrirEdicion = () => {
        if (miembroActivo) {
            setModoFormulario('edit');
        }
    };

    const cerrarFormulario = () => setModoFormulario(null);

    return (
        <main className="mx-auto max-w-7xl px-6 py-10">
            <header className="mb-6 flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-semibold text-slate-900">Directorio de Miembros</h1>
                    <p className="mt-1 text-sm text-slate-600">
                        Vista unificada del directorio y hoja de vida del capítulo L.A.M.A. Medellín.
                    </p>
                </div>
                <button
                    type="button"
                    onClick={abrirNuevo}
                    className="inline-flex items-center rounded-lg bg-red-700 px-4 py-2 text-sm font-medium text-white hover:bg-red-800"
                >
                    Nuevo Miembro
                </button>
            </header>

            {isLoading && <p className="text-sm text-slate-600">Cargando directorio de miembros...</p>}
            {isError && <p className="text-sm text-red-600">{(error as Error).message}</p>}

            {!isLoading && !isError && (
                <section className="grid grid-cols-1 gap-6 xl:grid-cols-[1.05fr_1fr]">
                    <article className="rounded-2xl border border-slate-200 bg-white shadow-sm">
                        <div className="border-b border-slate-200 px-5 py-4">
                            <h2 className="text-lg font-semibold text-slate-900">Directorio</h2>
                            <p className="mt-1 text-sm text-slate-600">Selecciona un miembro para ver su hoja de vida.</p>
                        </div>

                        {miembros.length === 0 ? (
                            <p className="px-5 py-6 text-sm text-slate-600">No hay miembros registrados.</p>
                        ) : (
                            <div className="max-h-[70vh] space-y-3 overflow-y-auto px-4 py-4">
                                {miembros.map((miembro) => {
                                    const isSelected = miembroActivo?.id === miembro.id;

                                    return (
                                        <button
                                            key={miembro.id}
                                            type="button"
                                            onClick={() => setMiembroActivoId(miembro.id)}
                                            className={`w-full rounded-xl border p-3 text-left transition ${isSelected
                                                ? 'border-red-300 bg-red-50/60 shadow-sm'
                                                : 'border-slate-200 bg-white hover:border-slate-300 hover:bg-slate-50'
                                                }`}
                                        >
                                            <div className="flex items-center gap-3">
                                                <div className="flex h-12 w-12 items-center justify-center rounded-full bg-gradient-to-br from-red-100 to-amber-100 text-sm font-semibold text-red-800">
                                                    {getInitials(miembro)}
                                                </div>
                                                <div className="min-w-0 flex-1">
                                                    <p className="truncate text-sm font-semibold text-slate-900">
                                                        {formatNombreCompleto(miembro)}
                                                    </p>
                                                    <p className="truncate text-xs italic text-slate-500">{miembro.apodo || 'Sin apodo'}</p>
                                                    <div className="mt-2 flex flex-wrap gap-2">
                                                        <span className="inline-flex rounded-full border border-blue-200 bg-blue-50 px-2 py-0.5 text-xs font-medium text-blue-700">
                                                            {miembro.rango}
                                                        </span>
                                                        <span className={`inline-flex rounded-full border px-2 py-0.5 text-xs font-medium ${estadoBadgeClass(miembro.esActivo)}`}>
                                                            {miembro.esActivo ? 'Activo' : 'Inactivo'}
                                                        </span>
                                                    </div>
                                                </div>
                                            </div>
                                        </button>
                                    );
                                })}
                            </div>
                        )}
                    </article>

                    <article className="rounded-2xl border border-slate-200 bg-white shadow-sm">
                        <div className="flex items-center justify-between border-b border-slate-200 px-5 py-4">
                            <div>
                                <h2 className="text-lg font-semibold text-slate-900">Hoja de Vida</h2>
                                <p className="mt-1 text-sm text-slate-600">Perfil completo del integrante seleccionado.</p>
                            </div>
                            <button
                                type="button"
                                onClick={abrirEdicion}
                                disabled={!miembroActivo}
                                className="rounded-lg border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-60"
                            >
                                Editar Miembro
                            </button>
                        </div>

                        {!miembroActivo ? (
                            <p className="px-5 py-6 text-sm text-slate-600">Selecciona un miembro para abrir su perfil.</p>
                        ) : (
                            <div className="space-y-5 px-5 py-5">
                                <section className="rounded-xl border border-slate-200 p-4">
                                    <h3 className="mb-3 text-xs font-semibold uppercase tracking-wide text-slate-500">Informacion Personal</h3>
                                    <div className="grid grid-cols-1 gap-4 text-sm text-slate-700 md:grid-cols-2">
                                        <div>
                                            <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Nombre completo</p>
                                            <p>{formatNombreCompleto(miembroActivo)}</p>
                                        </div>
                                        <div>
                                            <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Apodo</p>
                                            <p>{miembroActivo.apodo || 'Sin apodo'}</p>
                                        </div>
                                        <div>
                                            <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Documento</p>
                                            <p>{miembroActivo.documentoIdentidad}</p>
                                        </div>
                                        <div>
                                            <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Fecha ingreso</p>
                                            <p>{formatFecha(miembroActivo.fechaIngreso)}</p>
                                        </div>
                                    </div>
                                </section>

                                <section className="rounded-xl border border-rose-200 bg-rose-50/40 p-4">
                                    <h3 className="mb-3 text-xs font-semibold uppercase tracking-wide text-slate-500">Rol y Estado</h3>
                                    <div className="grid grid-cols-1 gap-4 text-sm text-slate-700 md:grid-cols-2">
                                        <div>
                                            <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Rol en el club</p>
                                            <p>{miembroActivo.rango}</p>
                                        </div>
                                        <div>
                                            <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Estado</p>
                                            <p>{miembroActivo.esActivo ? 'Activo' : 'Inactivo'}</p>
                                        </div>
                                        <div>
                                            <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Tipo de sangre</p>
                                            <p>{miembroActivo.tipoSangre}</p>
                                        </div>
                                        <div>
                                            <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Contacto emergencia</p>
                                            <p>{miembroActivo.contactoEmergenciaNombre} - {miembroActivo.contactoEmergenciaTelefono}</p>
                                        </div>
                                    </div>
                                </section>

                                <section className="rounded-xl border border-blue-200 bg-blue-50/40 p-4">
                                    <h3 className="mb-3 text-xs font-semibold uppercase tracking-wide text-slate-500">Moto</h3>
                                    <div className="grid grid-cols-1 gap-4 text-sm text-slate-700 md:grid-cols-2">
                                        <div>
                                            <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Marca</p>
                                            <p>{miembroActivo.moto.marca}</p>
                                        </div>
                                        <div>
                                            <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Modelo</p>
                                            <p>{miembroActivo.moto.modelo}</p>
                                        </div>
                                        <div>
                                            <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Cilindraje</p>
                                            <p>{miembroActivo.moto.cilindraje} cc</p>
                                        </div>
                                        <div>
                                            <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Placa</p>
                                            <p>{miembroActivo.moto.placa}</p>
                                        </div>
                                    </div>
                                </section>
                            </div>
                        )}
                    </article>
                </section>
            )}

            <MiembroUpsertModal
                mode={modoFormulario === 'edit' ? 'edit' : 'create'}
                isOpen={modoFormulario !== null}
                miembro={modoFormulario === 'edit' ? miembroActivo : null}
                onClose={cerrarFormulario}
            />
        </main>
    );
}
