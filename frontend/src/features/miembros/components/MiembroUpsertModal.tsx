'use client';

import { useActualizarMiembro } from '@/features/miembros/hooks/useActualizarMiembro';
import { useCrearMiembro } from '@/features/miembros/hooks/useCrearMiembro';
import { gruposSanguineosOptions, rangosClubOptions } from '@/features/miembros/schemas/miembroSchema';
import type { ActualizarMiembroPayload, CrearMiembroPayload, Miembro } from '@/features/miembros/services/miembrosService';
import { ChangeEvent, FormEvent, useState } from 'react';

type MiembroUpsertModalProps = {
    mode: 'create' | 'edit';
    isOpen: boolean;
    miembro: Miembro | null;
    onClose: () => void;
};

const labelClassName = 'mb-1 block text-sm font-medium text-slate-700';
const inputClassName = 'w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-red-100 focus:border-red-500 focus:ring-2 disabled:cursor-not-allowed disabled:bg-slate-50';

interface FormState {
    documentoIdentidad: string;
    nombres: string;
    apellidos: string;
    apodo: string;
    fechaIngreso: string;
    tipoSangre: number;
    nombreContactoEmergencia: string;
    telefonoContactoEmergencia: string;
    marcaMoto: string;
    modeloMoto: string;
    cilindraje: number;
    placa: string;
    rango: number;
    esActivo: boolean;
}

const defaultFormState: FormState = {
    documentoIdentidad: '',
    nombres: '',
    apellidos: '',
    apodo: '',
    fechaIngreso: new Date().toISOString().slice(0, 10),
    tipoSangre: 1,
    nombreContactoEmergencia: '',
    telefonoContactoEmergencia: '',
    marcaMoto: '',
    modeloMoto: '',
    cilindraje: 150,
    placa: '',
    rango: 1,
    esActivo: true,
};

function toFormState(miembro: Miembro): FormState {
    return {
        documentoIdentidad: miembro.documentoIdentidad,
        nombres: miembro.nombres,
        apellidos: miembro.apellidos,
        apodo: miembro.apodo,
        fechaIngreso: miembro.fechaIngreso,
        tipoSangre: typeof miembro.tipoSangre === 'number' ? miembro.tipoSangre : parseInt(miembro.tipoSangre, 10),
        nombreContactoEmergencia: miembro.nombreContactoEmergencia,
        telefonoContactoEmergencia: miembro.telefonoContactoEmergencia,
        marcaMoto: miembro.marcaMoto,
        modeloMoto: miembro.modeloMoto,
        cilindraje: miembro.cilindraje,
        placa: miembro.placa,
        rango: typeof miembro.rango === 'number' ? miembro.rango : parseInt(miembro.rango, 10),
        esActivo: miembro.esActivo,
    };
}

export default function MiembroUpsertModal({ mode, isOpen, miembro, onClose }: MiembroUpsertModalProps) {
    const crearMiembro = useCrearMiembro();
    const actualizarMiembro = useActualizarMiembro();
    const [formData, setFormData] = useState<FormState>(defaultFormState);

    if (!isOpen) {
        return null;
    }

    if (mode === 'edit' && miembro && formData.documentoIdentidad === '') {
        setFormData(toFormState(miembro));
    }

    const handleChange = (e: ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
        const { name, value, type } = e.target as HTMLInputElement;
        const boolFields = ['esActivo'];
        const numFields = ['tipoSangre', 'cilindraje', 'rango'];

        setFormData((prev) => ({
            ...prev,
            [name]:
                type === 'checkbox'
                    ? (e.target as HTMLInputElement).checked
                    : numFields.includes(name)
                        ? parseInt(value, 10)
                        : value,
        }));
    };

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();

        try {
            if (mode === 'create') {
                const payload: CrearMiembroPayload = {
                    documentoIdentidad: formData.documentoIdentidad.trim(),
                    nombres: formData.nombres.trim(),
                    apellidos: formData.apellidos.trim(),
                    apodo: formData.apodo.trim(),
                    fechaIngreso: formData.fechaIngreso,
                    tipoSangre: formData.tipoSangre,
                    nombreContactoEmergencia: formData.nombreContactoEmergencia.trim(),
                    telefonoContactoEmergencia: formData.telefonoContactoEmergencia.trim(),
                    marcaMoto: formData.marcaMoto.trim(),
                    modeloMoto: formData.modeloMoto.trim(),
                    cilindraje: formData.cilindraje,
                    placa: formData.placa.trim().toUpperCase(),
                    rango: formData.rango,
                    esActivo: true,
                };
                await crearMiembro.mutateAsync(payload);
                setFormData(defaultFormState);
                onClose();
            } else if (miembro) {
                const payload: ActualizarMiembroPayload = {
                    tipoSangre: formData.tipoSangre,
                    nombreContactoEmergencia: formData.nombreContactoEmergencia.trim(),
                    telefonoContactoEmergencia: formData.telefonoContactoEmergencia.trim(),
                    marcaMoto: formData.marcaMoto.trim(),
                    modeloMoto: formData.modeloMoto.trim(),
                    cilindraje: formData.cilindraje,
                    placa: formData.placa.trim().toUpperCase(),
                    rango: formData.rango,
                    esActivo: formData.esActivo,
                };
                await actualizarMiembro.mutateAsync({ id: miembro.id, payload });
                setFormData(defaultFormState);
                onClose();
            }
        } catch (error) {
            console.error('Form submission error:', error);
        }
    };

    const isPending = crearMiembro.isPending || actualizarMiembro.isPending;
    const mutationError = crearMiembro.error ?? actualizarMiembro.error;

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-900/60 px-4 py-6">
            <div className="max-h-[95vh] w-full max-w-4xl overflow-y-auto rounded-2xl bg-white p-6 shadow-2xl">
                <div className="mb-5 flex items-center justify-between">
                    <div>
                        <h2 className="text-xl font-semibold text-slate-900">
                            {mode === 'create' ? 'Nuevo Miembro' : 'Editar Miembro'}
                        </h2>
                        <p className="mt-1 text-sm text-slate-600">
                            {mode === 'create'
                                ? 'Registro completo de un nuevo integrante del capítulo.'
                                : 'Actualiza los campos operativos permitidos por el contrato del backend.'}
                        </p>
                    </div>
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
                        <h3 className="mb-3 text-sm font-semibold uppercase tracking-wide text-slate-600">
                            Informacion Personal
                        </h3>
                        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                            <div>
                                <label className={labelClassName}>Nombres</label>
                                <input
                                    type="text"
                                    name="nombres"
                                    value={formData.nombres}
                                    onChange={handleChange}
                                    disabled={mode === 'edit'}
                                    className={inputClassName}
                                />
                            </div>
                            <div>
                                <label className={labelClassName}>Apellidos</label>
                                <input
                                    type="text"
                                    name="apellidos"
                                    value={formData.apellidos}
                                    onChange={handleChange}
                                    disabled={mode === 'edit'}
                                    className={inputClassName}
                                />
                            </div>
                            <div>
                                <label className={labelClassName}>Documento</label>
                                <input
                                    type="text"
                                    name="documentoIdentidad"
                                    value={formData.documentoIdentidad}
                                    onChange={handleChange}
                                    disabled={mode === 'edit'}
                                    className={inputClassName}
                                />
                            </div>
                            <div>
                                <label className={labelClassName}>Apodo</label>
                                <input
                                    type="text"
                                    name="apodo"
                                    value={formData.apodo}
                                    onChange={handleChange}
                                    disabled={mode === 'edit'}
                                    className={inputClassName}
                                />
                            </div>
                            <div>
                                <label className={labelClassName}>Fecha de ingreso</label>
                                <input
                                    type="date"
                                    name="fechaIngreso"
                                    value={formData.fechaIngreso}
                                    onChange={handleChange}
                                    disabled={mode === 'edit'}
                                    className={inputClassName}
                                />
                            </div>
                            <div>
                                <label className={labelClassName}>Rango</label>
                                <select
                                    name="rango"
                                    value={formData.rango}
                                    onChange={handleChange}
                                    className={inputClassName}
                                >
                                    {rangosClubOptions.map((opt) => (
                                        <option key={opt.value} value={opt.value}>
                                            {opt.label}
                                        </option>
                                    ))}
                                </select>
                            </div>
                            <div className="md:col-span-2">
                                <label className="inline-flex items-center gap-2 text-sm text-slate-700">
                                    <input
                                        type="checkbox"
                                        name="esActivo"
                                        checked={formData.esActivo}
                                        onChange={handleChange}
                                        className="h-4 w-4 rounded border-slate-300"
                                    />
                                    Miembro activo
                                </label>
                            </div>
                        </div>
                    </section>

                    <section className="rounded-xl border border-rose-200 bg-rose-50/40 p-4">
                        <h3 className="mb-3 text-sm font-semibold uppercase tracking-wide text-slate-600">
                            Contacto y Emergencia
                        </h3>
                        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                            <div>
                                <label className={labelClassName}>Tipo de sangre</label>
                                <select
                                    name="tipoSangre"
                                    value={formData.tipoSangre}
                                    onChange={handleChange}
                                    className={inputClassName}
                                >
                                    {gruposSanguineosOptions.map((opt) => (
                                        <option key={opt.value} value={opt.value}>
                                            {opt.label}
                                        </option>
                                    ))}
                                </select>
                            </div>
                            <div>
                                <label className={labelClassName}>Contacto de emergencia</label>
                                <input
                                    type="text"
                                    name="nombreContactoEmergencia"
                                    value={formData.nombreContactoEmergencia}
                                    onChange={handleChange}
                                    className={inputClassName}
                                />
                            </div>
                            <div>
                                <label className={labelClassName}>Telefono de emergencia</label>
                                <input
                                    type="tel"
                                    name="telefonoContactoEmergencia"
                                    value={formData.telefonoContactoEmergencia}
                                    onChange={handleChange}
                                    className={inputClassName}
                                />
                            </div>
                        </div>
                    </section>

                    <section className="rounded-xl border border-blue-200 bg-blue-50/40 p-4">
                        <h3 className="mb-3 text-sm font-semibold uppercase tracking-wide text-slate-600">
                            Motocicleta
                        </h3>
                        <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                            <div>
                                <label className={labelClassName}>Marca</label>
                                <input
                                    type="text"
                                    name="marcaMoto"
                                    value={formData.marcaMoto}
                                    onChange={handleChange}
                                    className={inputClassName}
                                />
                            </div>
                            <div>
                                <label className={labelClassName}>Modelo</label>
                                <input
                                    type="text"
                                    name="modeloMoto"
                                    value={formData.modeloMoto}
                                    onChange={handleChange}
                                    className={inputClassName}
                                />
                            </div>
                            <div>
                                <label className={labelClassName}>Cilindraje</label>
                                <input
                                    type="number"
                                    name="cilindraje"
                                    value={formData.cilindraje}
                                    onChange={handleChange}
                                    className={inputClassName}
                                />
                            </div>
                            <div>
                                <label className={labelClassName}>Placa</label>
                                <input
                                    type="text"
                                    name="placa"
                                    value={formData.placa}
                                    onChange={handleChange}
                                    className={inputClassName}
                                />
                            </div>
                        </div>
                    </section>

                    {mutationError && (
                        <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
                            {mutationError.message}
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
                            {isPending ? 'Guardando...' : mode === 'create' ? 'Crear miembro' : 'Guardar cambios'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
