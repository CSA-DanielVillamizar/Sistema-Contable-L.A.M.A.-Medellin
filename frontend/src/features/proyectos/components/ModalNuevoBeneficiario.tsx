'use client';

import { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import {
    beneficiarioSchema,
    tiposDocumentoBeneficiarioOptions,
    type BeneficiarioFormInput,
    type BeneficiarioFormValues,
} from '@/features/proyectos/schemas/beneficiarioSchema';
import { useCrearBeneficiario } from '@/features/proyectos/hooks/useBeneficiarios';

type ModalNuevoBeneficiarioProps = {
    open: boolean;
    onClose: () => void;
};

const defaultValues: BeneficiarioFormInput = {
    NombreCompleto: '',
    TipoDocumento: 'CC',
    NumeroDocumento: '',
    Email: '',
    Telefono: '',
    TieneConsentimientoHabeasData: false,
};

export default function ModalNuevoBeneficiario({ open, onClose }: ModalNuevoBeneficiarioProps) {
    const crearBeneficiario = useCrearBeneficiario();

    const {
        register,
        handleSubmit,
        reset,
        formState: { errors },
    } = useForm<BeneficiarioFormInput, unknown, BeneficiarioFormValues>({
        resolver: zodResolver(beneficiarioSchema),
        defaultValues,
    });

    useEffect(() => {
        if (open) {
            reset(defaultValues);
        }
    }, [open, reset]);

    const onSubmit = async (values: BeneficiarioFormValues) => {
        await crearBeneficiario.mutateAsync(values);
        onClose();
    };

    if (!open) {
        return null;
    }

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
            <div className="w-full max-w-xl rounded-xl bg-white p-6 shadow-xl">
                <h2 className="text-lg font-semibold text-slate-900">Nuevo Beneficiario</h2>

                <form className="mt-4 space-y-4" onSubmit={handleSubmit(onSubmit)}>
                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Nombre completo</label>
                        <input {...register('NombreCompleto')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900" />
                        {errors.NombreCompleto ? <p className="mt-1 text-xs text-red-600">{errors.NombreCompleto.message}</p> : null}
                    </div>

                    <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Tipo documento</label>
                            <select {...register('TipoDocumento')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900">
                                {tiposDocumentoBeneficiarioOptions.map((tipo) => (
                                    <option key={tipo.value} value={tipo.value}>
                                        {tipo.label}
                                    </option>
                                ))}
                            </select>
                            {errors.TipoDocumento ? <p className="mt-1 text-xs text-red-600">{errors.TipoDocumento.message}</p> : null}
                        </div>

                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Número documento</label>
                            <input {...register('NumeroDocumento')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900" />
                            {errors.NumeroDocumento ? <p className="mt-1 text-xs text-red-600">{errors.NumeroDocumento.message}</p> : null}
                        </div>
                    </div>

                    <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Correo</label>
                            <input type="email" {...register('Email')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900" />
                            {errors.Email ? <p className="mt-1 text-xs text-red-600">{errors.Email.message}</p> : null}
                        </div>

                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Teléfono</label>
                            <input {...register('Telefono')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900" />
                            {errors.Telefono ? <p className="mt-1 text-xs text-red-600">{errors.Telefono.message}</p> : null}
                        </div>
                    </div>

                    <label className="flex items-start gap-2 rounded-lg border border-slate-200 bg-slate-50 p-3 text-sm text-slate-700">
                        <input type="checkbox" {...register('TieneConsentimientoHabeasData')} className="mt-1" />
                        <span>Confirmo que cuento con consentimiento expreso de Habeas Data para el tratamiento de datos personales.</span>
                    </label>
                    {errors.TieneConsentimientoHabeasData ? (
                        <p className="text-xs text-red-600">{errors.TieneConsentimientoHabeasData.message}</p>
                    ) : null}

                    {crearBeneficiario.error ? <p className="text-sm text-red-600">{crearBeneficiario.error.message}</p> : null}

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
                            disabled={crearBeneficiario.isPending}
                            className="rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white disabled:opacity-60"
                        >
                            {crearBeneficiario.isPending ? 'Guardando...' : 'Guardar'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
