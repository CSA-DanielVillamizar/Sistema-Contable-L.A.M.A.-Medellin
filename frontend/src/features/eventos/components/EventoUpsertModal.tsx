'use client';

import { useActualizarEvento } from '@/features/eventos/hooks/useActualizarEvento';
import { useCrearEvento } from '@/features/eventos/hooks/useCrearEvento';
import type { EventoDetalleDto } from '@/features/eventos/services/eventosService';
import { useState, type FormEvent } from 'react';

type EventoUpsertMode = 'create' | 'edit';

type EventoUpsertModalProps = {
    mode: EventoUpsertMode;
    onClose: () => void;
    evento?: EventoDetalleDto | null;
};

type FormState = {
    nombre: string;
    descripcion: string;
    fechaProgramada: string;
    lugarEncuentro: string;
    destino: string;
    tipoEvento: number;
};

const tipoEventoOptions = [
    { value: 1, label: 'Rodada' },
    { value: 2, label: 'Social' },
    { value: 3, label: 'Reunión' },
    { value: 4, label: 'Benéfico' },
    { value: 5, label: 'Otro' },
] as const;

function getDefaultFechaProgramada(): string {
    const date = new Date();
    date.setMinutes(date.getMinutes() - date.getTimezoneOffset());
    return date.toISOString().slice(0, 16);
}

function toUtcIsoFromLocalInput(localDateTime: string): string {
    return new Date(localDateTime).toISOString();
}

function toLocalDateTimeInput(value: string): string {
    const parsed = new Date(value);
    if (Number.isNaN(parsed.getTime())) {
        return getDefaultFechaProgramada();
    }

    parsed.setMinutes(parsed.getMinutes() - parsed.getTimezoneOffset());
    return parsed.toISOString().slice(0, 16);
}

function tipoEventoFromLabel(label: string): number {
    const normalized = label.trim().toLowerCase();
    const option = tipoEventoOptions.find((item) => item.label.toLowerCase() === normalized);
    return option?.value ?? 5;
}

const defaultFormState: FormState = {
    nombre: '',
    descripcion: '',
    fechaProgramada: getDefaultFechaProgramada(),
    lugarEncuentro: '',
    destino: '',
    tipoEvento: 1,
};

function getInitialFormState(mode: EventoUpsertMode, evento: EventoDetalleDto | null): FormState {
    if (mode === 'edit' && evento) {
        return {
            nombre: evento.nombre ?? '',
            descripcion: evento.descripcion ?? '',
            fechaProgramada: toLocalDateTimeInput(evento.fechaProgramada),
            lugarEncuentro: evento.lugarEncuentro ?? '',
            destino: evento.destino ?? '',
            tipoEvento: tipoEventoFromLabel(evento.tipoEvento),
        };
    }

    return { ...defaultFormState, fechaProgramada: getDefaultFechaProgramada() };
}

function labelClassName(): string {
    return 'mb-1 block text-sm font-medium text-slate-700';
}

function inputClassName(): string {
    return 'w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-red-100 focus:border-red-500 focus:ring-2';
}

/**
 * El formulario arranca con el evento recibido, resuelto en el montaje. Quien
 * abre el modal lo monta y lo desmonta; sincronizar el estado con un efecto
 * obligaba a un render intermedio con los datos del evento anterior.
 */
export default function EventoUpsertModal({ mode, onClose, evento = null }: EventoUpsertModalProps) {
    const crearEvento = useCrearEvento();
    const actualizarEvento = useActualizarEvento();
    const [error, setError] = useState<string>('');
    const [formData, setFormData] = useState<FormState>(() => getInitialFormState(mode, evento));

    const isPending = crearEvento.isPending || actualizarEvento.isPending;

    const handleFieldChange = (field: keyof FormState, value: string | number) => {
        setFormData((prev) => ({ ...prev, [field]: value }));
    };

    const validate = (): string | null => {
        if (!formData.nombre.trim()) {
            return 'Nombre es obligatorio.';
        }

        if (!formData.descripcion.trim()) {
            return 'Descripción es obligatoria.';
        }

        if (!formData.fechaProgramada) {
            return 'Fecha programada es obligatoria.';
        }

        if (!formData.lugarEncuentro.trim()) {
            return 'Lugar de encuentro es obligatorio.';
        }

        return null;
    };

    const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setError('');

        const validationError = validate();
        if (validationError) {
            setError(validationError);
            return;
        }

        const payload = {
            nombre: formData.nombre.trim(),
            descripcion: formData.descripcion.trim(),
            fechaProgramada: toUtcIsoFromLocalInput(formData.fechaProgramada),
            lugarEncuentro: formData.lugarEncuentro.trim(),
            destino: formData.destino.trim() || null,
            tipoEvento: Number(formData.tipoEvento),
        };

        try {
            if (mode === 'create') {
                await crearEvento.mutateAsync(payload);
            } else {
                if (!evento?.id) {
                    setError('No se pudo identificar el evento a editar.');
                    return;
                }

                await actualizarEvento.mutateAsync({ id: evento.id, payload });
            }

            onClose();
        } catch (mutationError) {
            setError((mutationError as Error).message || 'No fue posible guardar el evento.');
        }
    };

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/55 px-4 py-6">
            <div className="max-h-[95vh] w-full max-w-2xl overflow-y-auto rounded-2xl bg-white p-6 shadow-2xl">
                <div className="mb-4 flex items-center justify-between">
                    <h2 className="text-xl font-semibold text-slate-900">
                        {mode === 'create' ? 'Nuevo Evento' : 'Editar Evento'}
                    </h2>
                    <button
                        type="button"
                        onClick={onClose}
                        className="rounded-md border border-slate-300 px-3 py-1.5 text-sm text-slate-700 hover:bg-slate-50"
                    >
                        Cerrar
                    </button>
                </div>

                <form onSubmit={handleSubmit} className="space-y-5">
                    <section className="rounded-xl border border-slate-200 p-4">
                        <h3 className="mb-3 text-sm font-semibold uppercase tracking-wide text-slate-600">Datos del Evento</h3>
                        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                            <div className="md:col-span-2">
                                <label htmlFor="nombre" className={labelClassName()}>Nombre</label>
                                <input
                                    id="nombre"
                                    name="nombre"
                                    value={formData.nombre}
                                    onChange={(e) => handleFieldChange('nombre', e.target.value)}
                                    className={inputClassName()}
                                />
                            </div>

                            <div className="md:col-span-2">
                                <label htmlFor="descripcion" className={labelClassName()}>Descripción</label>
                                <textarea
                                    id="descripcion"
                                    name="descripcion"
                                    rows={3}
                                    value={formData.descripcion}
                                    onChange={(e) => handleFieldChange('descripcion', e.target.value)}
                                    className={inputClassName()}
                                />
                            </div>

                            <div>
                                <label htmlFor="fechaProgramada" className={labelClassName()}>Fecha y hora</label>
                                <input
                                    id="fechaProgramada"
                                    name="fechaProgramada"
                                    type="datetime-local"
                                    value={formData.fechaProgramada}
                                    onChange={(e) => handleFieldChange('fechaProgramada', e.target.value)}
                                    className={inputClassName()}
                                />
                            </div>

                            <div>
                                <label htmlFor="tipoEvento" className={labelClassName()}>Tipo</label>
                                <select
                                    id="tipoEvento"
                                    name="tipoEvento"
                                    value={formData.tipoEvento}
                                    onChange={(e) => handleFieldChange('tipoEvento', Number(e.target.value))}
                                    className={inputClassName()}
                                >
                                    {tipoEventoOptions.map((option) => (
                                        <option key={option.value} value={option.value}>{option.label}</option>
                                    ))}
                                </select>
                            </div>

                            <div>
                                <label htmlFor="lugarEncuentro" className={labelClassName()}>Lugar de encuentro</label>
                                <input
                                    id="lugarEncuentro"
                                    name="lugarEncuentro"
                                    value={formData.lugarEncuentro}
                                    onChange={(e) => handleFieldChange('lugarEncuentro', e.target.value)}
                                    className={inputClassName()}
                                />
                            </div>

                            <div>
                                <label htmlFor="destino" className={labelClassName()}>Destino</label>
                                <input
                                    id="destino"
                                    name="destino"
                                    value={formData.destino}
                                    onChange={(e) => handleFieldChange('destino', e.target.value)}
                                    className={inputClassName()}
                                    placeholder="Opcional"
                                />
                            </div>
                        </div>
                    </section>

                    {error && (
                        <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
                            {error}
                        </div>
                    )}

                    <div className="flex items-center justify-end gap-3">
                        <button
                            type="button"
                            onClick={onClose}
                            className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
                        >
                            Cancelar
                        </button>
                        <button
                            type="submit"
                            disabled={isPending}
                            className="rounded-lg bg-red-700 px-4 py-2 text-sm font-medium text-white hover:bg-red-800 disabled:cursor-not-allowed disabled:opacity-60"
                        >
                            {isPending ? 'Guardando...' : mode === 'create' ? 'Crear evento' : 'Guardar cambios'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
