'use client';

import EventoUpsertModal from '@/features/eventos/components/EventoUpsertModal';
import EstadoDeError from '@/components/layout/EstadoDeError';
import { useGetEventos } from '@/features/eventos/hooks/useGetEventos';
import type { EventoDto } from '@/features/eventos/services/eventosService';
import Link from 'next/link';
import { useState } from 'react';

type VistaEventos = 'proximas-rodadas' | 'eventos-pasados';

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

function sortByFechaDesc(a: EventoDto, b: EventoDto): number {
    return new Date(b.fechaProgramada).getTime() - new Date(a.fechaProgramada).getTime();
}

function sortByFechaAsc(a: EventoDto, b: EventoDto): number {
    return new Date(a.fechaProgramada).getTime() - new Date(b.fechaProgramada).getTime();
}

function isRodada(tipoEvento: string): boolean {
    return tipoEvento.trim().toLowerCase() === 'rodada';
}

function getVistaEventos(data: EventoDto[], vista: VistaEventos): EventoDto[] {
    const now = Date.now();

    if (vista === 'proximas-rodadas') {
        return data
            .filter((evento) => isRodada(evento.tipoEvento) && new Date(evento.fechaProgramada).getTime() >= now)
            .sort(sortByFechaAsc);
    }

    return data
        .filter((evento) => new Date(evento.fechaProgramada).getTime() < now)
        .sort(sortByFechaDesc);
}

function tabClassName(isActive: boolean): string {
    return isActive
        ? 'rounded-lg border border-red-700 bg-red-700 px-3 py-2 text-sm font-semibold text-white'
        : 'rounded-lg border border-slate-300 bg-white px-3 py-2 text-sm font-semibold text-slate-700 hover:border-slate-400 hover:bg-slate-50';
}

export default function ListaEventos() {
    const { data, isLoading, isError, error } = useGetEventos();
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [vistaActiva, setVistaActiva] = useState<VistaEventos>('proximas-rodadas');

    if (isLoading) {
        return <p className="text-sm text-slate-600">Cargando agenda de eventos...</p>;
    }

    if (isError) {
        return <EstadoDeError error={error} contexto="ver los eventos" />;
    }

    const eventosFiltrados = getVistaEventos(data ?? [], vistaActiva);
    const totalRodadasProximas = getVistaEventos(data ?? [], 'proximas-rodadas').length;
    const totalEventosPasados = getVistaEventos(data ?? [], 'eventos-pasados').length;

    return (
        <>
            <div className="mb-6 flex flex-col gap-4 md:flex-row md:items-center md:justify-between">
                <div className="inline-flex flex-wrap gap-2 rounded-xl border border-slate-200 bg-slate-50 p-2">
                    <button
                        type="button"
                        onClick={() => setVistaActiva('proximas-rodadas')}
                        className={tabClassName(vistaActiva === 'proximas-rodadas')}
                    >
                        Próximas Rodadas ({totalRodadasProximas})
                    </button>
                    <button
                        type="button"
                        onClick={() => setVistaActiva('eventos-pasados')}
                        className={tabClassName(vistaActiva === 'eventos-pasados')}
                    >
                        Eventos Pasados ({totalEventosPasados})
                    </button>
                </div>

                <button
                    type="button"
                    onClick={() => setIsModalOpen(true)}
                    className="rounded-lg bg-red-700 px-4 py-2 text-sm font-medium text-white hover:bg-red-800"
                >
                    + Nuevo Evento
                </button>
            </div>

            {eventosFiltrados.length === 0 ? (
                <p className="text-sm text-slate-600">
                    {vistaActiva === 'proximas-rodadas'
                        ? 'No hay rodadas programadas próximamente.'
                        : 'No hay eventos pasados registrados.'}
                </p>
            ) : (
                <div className="grid grid-cols-1 gap-5 md:grid-cols-2 xl:grid-cols-3">
                    {eventosFiltrados.map((evento) => (
                        <article
                            key={evento.id}
                            className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm transition hover:-translate-y-0.5 hover:shadow-md"
                        >
                            <header className="mb-4 border-b border-slate-100 pb-3">
                                <div className="mb-2 flex flex-wrap gap-2">
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
                                <h3 className="text-base font-semibold text-slate-900">{evento.nombre}</h3>
                            </header>

                            <section>
                                <h4 className="mb-1 text-xs font-semibold uppercase tracking-wide text-slate-500">Fecha Programada</h4>
                                <p className="text-sm text-slate-800">{formatFecha(evento.fechaProgramada)}</p>
                            </section>

                            <div className="mt-4 flex justify-end">
                                <Link
                                    href={`/eventos/${evento.id}`}
                                    className="rounded-lg border border-slate-300 bg-white px-3 py-1.5 text-xs font-semibold text-slate-700 hover:bg-slate-50"
                                >
                                    Ver / Asistencia
                                </Link>
                            </div>
                        </article>
                    ))}
                </div>
            )}

            {isModalOpen ? (
                <EventoUpsertModal mode="create" onClose={() => setIsModalOpen(false)} />
            ) : null}
        </>
    );
}
