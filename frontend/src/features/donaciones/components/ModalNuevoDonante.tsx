'use client';

import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import {
    donanteSchema,
    tiposDocumentoOptions,
    tiposPersonaOptions,
    type DonanteFormInput,
    type DonanteFormValues,
} from '@/features/donaciones/schemas/donacionSchema';
import { useCrearDonante } from '@/features/donaciones/hooks/useDonaciones';

type ModalNuevoDonanteProps = {
    open: boolean;
    onClose: () => void;
};

const defaultValues: DonanteFormInput = {
    NombreORazonSocial: '',
    TipoDocumento: 1,
    NumeroDocumento: '',
    Email: '',
    TipoPersona: 1,
};

export default function ModalNuevoDonante({ open, onClose }: ModalNuevoDonanteProps) {
    const crearDonante = useCrearDonante();

    const {
        register,
        handleSubmit,
        reset,
        formState: { errors },
    } = useForm<DonanteFormInput, unknown, DonanteFormValues>({
        resolver: zodResolver(donanteSchema),
        defaultValues,
    });

    useEffect(() => {
        if (!open) {
            return;
        }

        reset(defaultValues);
    }, [open, reset]);

    const onSubmit = async (values: DonanteFormValues) => {
        await crearDonante.mutateAsync(values);
        onClose();
    };

    if (!open) {
        return null;
    }

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
            <div className="w-full max-w-lg rounded-xl bg-white p-6 shadow-xl">
                <h2 className="text-lg font-semibold text-slate-900">Registrar Donante</h2>

                <form className="mt-4 space-y-4" onSubmit={handleSubmit(onSubmit)}>
                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Nombre o Razón Social</label>
                        <input
                            {...register('NombreORazonSocial')}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                        {errors.NombreORazonSocial ? <p className="mt-1 text-xs text-red-600">{errors.NombreORazonSocial.message}</p> : null}
                    </div>

                    <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Tipo de Documento</label>
                            <select
                                {...register('TipoDocumento')}
                                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                            >
                                {tiposDocumentoOptions.map((opcion) => (
                                    <option key={opcion.value} value={opcion.value}>
                                        {opcion.label}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Tipo de Persona</label>
                            <select
                                {...register('TipoPersona')}
                                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                            >
                                {tiposPersonaOptions.map((opcion) => (
                                    <option key={opcion.value} value={opcion.value}>
                                        {opcion.label}
                                    </option>
                                ))}
                            </select>
                        </div>
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Número de Documento</label>
                        <input
                            {...register('NumeroDocumento')}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                        {errors.NumeroDocumento ? <p className="mt-1 text-xs text-red-600">{errors.NumeroDocumento.message}</p> : null}
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Correo</label>
                        <input
                            type="email"
                            {...register('Email')}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                        {errors.Email ? <p className="mt-1 text-xs text-red-600">{errors.Email.message}</p> : null}
                    </div>

                    {crearDonante.error ? (
                        <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">{crearDonante.error.message}</div>
                    ) : null}

                    <div className="flex justify-end gap-2">
                        <button
                            type="button"
                            onClick={onClose}
                            className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700"
                        >
                            Cancelar
                        </button>

                        <button
                            type="submit"
                            disabled={crearDonante.isPending}
                            className="rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white disabled:opacity-60"
                        >
                            {crearDonante.isPending ? 'Guardando...' : 'Guardar'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
