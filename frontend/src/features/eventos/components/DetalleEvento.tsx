'use client';

import EventoUpsertModal from '@/features/eventos/components/EventoUpsertModal';
import { useGetEventoById } from '@/features/eventos/hooks/useGetEventoById';
import { useMarcarAsistencia } from '@/features/eventos/hooks/useMarcarAsistencia';
import { useGetMiembros } from '@/features/miembros/hooks/useGetMiembros';
import Link from 'next/link';
import { useMemo, useState } from 'react';

type DetalleEventoProps = {
    eventoId: string;
};

function formatFecha(value: string): string {
    const parsed = new Date(value);

    if (Number.isNaN(parsed.getTime())) {
        return 'Fecha inválida';
    }

    return parsed.toLocaleString('es-CO', {
        year: 'numeric',
        month: 'long',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit',
    });
}

function estadoBadgeClass(estado: string): string {
    const normalized = estado.toLowerCase();

    if (normalized === 'programado') {
        return 'border-amber-300 bg-amber-50 text-amber-800';
    }

    if (normalized === 'enprogreso' || normalized === 'en progreso') {
        return 'border-blue-300 bg-blue-50 text-blue-800';
    }

    if (normalized === 'finalizado') {
        return 'border-emerald-300 bg-emerald-50 text-emerald-800';
    }

    if (normalized === 'cancelado') {
        return 'border-red-300 bg-red-50 text-red-800';
    }

    return 'border-slate-300 bg-slate-100 text-slate-700';
}

function tipoBadgeClass(tipo: string): string {
    const normalized = tipo.toLowerCase();

    if (normalized === 'rodada') {
        return 'border-indigo-300 bg-indigo-50 text-indigo-800';
    }

    if (normalized === 'social') {
        return 'border-fuchsia-300 bg-fuchsia-50 text-fuchsia-800';
    }

    if (normalized === 'reunion' || normalized === 'reunión') {
        return 'border-cyan-300 bg-cyan-50 text-cyan-800';
    }

    if (normalized === 'benefico' || normalized === 'benéfico') {
        return 'border-emerald-300 bg-emerald-50 text-emerald-800';
    }

    return 'border-slate-300 bg-slate-100 text-slate-700';
}

export default function DetalleEvento({ eventoId }: DetalleEventoProps) {
    const eventoQuery = useGetEventoById(eventoId);
    const miembrosQuery = useGetMiembros();
    const marcarAsistenciaMutation = useMarcarAsistencia(eventoId);
    const [updatingMiembroId, setUpdatingMiembroId] = useState<string | null>(null);
    const [isEditModalOpen, setIsEditModalOpen] = useState(false);

    const asistenciasPorMiembro = useMemo(() => {
        const source = eventoQuery.data?.asistencias ?? [];
        return new Map(source.map((item) => [item.miembroId, item]));
    }, [eventoQuery.data?.asistencias]);

    if (eventoQuery.isLoading || miembrosQuery.isLoading) {
        return <p className="text-sm text-slate-600">Cargando detalle del evento...</p>;
    }

    if (eventoQuery.isError) {
        return <p className="text-sm text-red-600">{(eventoQuery.error as Error).message}</p>;
    }

    if (miembrosQuery.isError) {
        return <p className="text-sm text-red-600">{(miembrosQuery.error as Error).message}</p>;
    }

    if (!eventoQuery.data) {
        return <p className="text-sm text-slate-600">No se encontró el evento.</p>;
    }

    const evento = eventoQuery.data;
    const miembros = miembrosQuery.data ?? [];

    const onToggleAsistencia = async (miembroId: string, asistioActual: boolean) => {
        try {
            setUpdatingMiembroId(miembroId);
            await marcarAsistenciaMutation.mutateAsync({
                miembroId,
                asistio: !asistioActual,
                observaciones: null,
            });
        } finally {
            setUpdatingMiembroId(null);
        }
    };

    return (
        <div className="space-y-6">
            <section className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
                <div className="mb-4 flex items-center justify-between">
                    <div>
                        <h1 className="text-2xl font-semibold text-slate-900">{evento.nombre}</h1>
                        <p className="mt-1 text-sm text-slate-600">{evento.descripcion}</p>
                    </div>
                    <div className="flex items-center gap-2">
                        <button
                            type="button"
                            onClick={() => setIsEditModalOpen(true)}
                            className="rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
                        >
                            Editar Evento
                        </button>
                        <Link href="/eventos" className="text-sm text-slate-600 hover:text-slate-900">
                            ← Volver a eventos
                        </Link>
                    </div>
                </div>

                <div className="mb-4 flex flex-wrap gap-2">
                    <span
                        className={`inline-flex rounded-full border px-2.5 py-1 text-xs font-medium ${tipoBadgeClass(evento.tipoEvento)}`}
                    >
                        {evento.tipoEvento}
                    </span>
                    <span
                        className={`inline-flex rounded-full border px-2.5 py-1 text-xs font-medium ${estadoBadgeClass(evento.estado)}`}
                    >
                        {evento.estado}
                    </span>
                </div>

                <div className="grid grid-cols-1 gap-4 text-sm text-slate-700 md:grid-cols-3">
                    <div>
                        <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Fecha Programada</p>
                        <p>{formatFecha(evento.fechaProgramada)}</p>
                    </div>
                    <div>
                        <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Lugar Encuentro</p>
                        <p>{evento.lugarEncuentro}</p>
                    </div>
                    <div>
                        <p className="text-xs font-semibold uppercase tracking-wide text-slate-500">Destino</p>
                        <p>{evento.destino || 'No definido'}</p>
                    </div>
                </div>
            </section>

            <section className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
                <h2 className="mb-4 text-lg font-semibold text-slate-900">Control de asistencia</h2>

                {marcarAsistenciaMutation.error && (
                    <div className="mb-4 rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
                        {marcarAsistenciaMutation.error.message}
                    </div>
                )}

                {miembros.length === 0 ? (
                    <p className="text-sm text-slate-600">No hay miembros registrados para marcar asistencia.</p>
                ) : (
                    <div className="overflow-x-auto rounded-xl border border-slate-200">
                        <div className="min-w-[640px] bg-slate-50 px-4 py-3">
                            <div className="grid grid-cols-[2fr_1fr_1fr] items-center gap-4 text-xs font-semibold uppercase tracking-wide text-slate-500">
                                <p>Miembro</p>
                                <p>Estado</p>
                                <p className="text-right">Acción</p>
                            </div>
                        </div>

                        <div className="divide-y divide-slate-200 bg-white">
                            {miembros.map((miembro) => {
                                const asistencia = asistenciasPorMiembro.get(miembro.id);
                                const asistio = asistencia?.asistio ?? false;
                                const isUpdating = updatingMiembroId === miembro.id && marcarAsistenciaMutation.isPending;

                                return (
                                    <article
                                        key={miembro.id}
                                        className="grid grid-cols-[2fr_1fr_1fr] items-center gap-4 px-4 py-3"
                                    >
                                        <div>
                                            <h3 className="text-sm font-semibold text-slate-900">
                                                {miembro.nombres} {miembro.apellidos}
                                            </h3>
                                            <p className="text-xs text-slate-500">{miembro.apodo || 'Sin apodo'}</p>
                                        </div>

                                        <div>
                                            <span
                                                className={[
                                                    'inline-flex rounded-full border px-2.5 py-1 text-xs font-semibold',
                                                    asistio
                                                        ? 'border-emerald-300 bg-emerald-50 text-emerald-700'
                                                        : 'border-rose-300 bg-rose-50 text-rose-700',
                                                ].join(' ')}
                                            >
                                                {asistio ? 'Asistió' : 'Pendiente'}
                                            </span>
                                        </div>

                                        <button
                                            type="button"
                                            onClick={() => onToggleAsistencia(miembro.id, asistio)}
                                            disabled={isUpdating}
                                            className={[
                                                'justify-self-end rounded-lg px-4 py-2 text-sm font-semibold text-white transition-colors disabled:cursor-not-allowed disabled:opacity-70',
                                                asistio ? 'bg-emerald-600 hover:bg-emerald-700' : 'bg-rose-600 hover:bg-rose-700',
                                            ].join(' ')}
                                        >
                                            {isUpdating ? 'Guardando...' : asistio ? 'Marcar no asistió' : 'Marcar asistió'}
                                        </button>
                                    </article>
                                );
                            })}
                        </div>
                    </div>
                )}
            </section>

            <EventoUpsertModal
                mode="edit"
                isOpen={isEditModalOpen}
                onClose={() => setIsEditModalOpen(false)}
                evento={evento}
            />
        </div>
    );
}
