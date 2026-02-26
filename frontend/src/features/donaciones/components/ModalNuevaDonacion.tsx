'use client';

import { useEffect } from 'react';
import { useQuery } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import apiClient from '@/lib/apiClient';
import {
    donacionSchema,
    formasDonacionOptions,
    mediosPagoOptions,
    type DonacionFormInput,
    type DonacionFormValues,
} from '@/features/donaciones/schemas/donacionSchema';
import { useDonantes, useRegistrarDonacion } from '@/features/donaciones/hooks/useDonaciones';

type ModalNuevaDonacionProps = {
    open: boolean;
    onClose: () => void;
};

type BancoCatalogo = {
    id: string;
    numeroCuenta: string;
};

type CentroCostoCatalogo = {
    id: string;
    nombre: string;
};

const defaultValues: DonacionFormInput = {
    DonanteId: '',
    MontoCOP: 0,
    BancoId: '',
    CentroCostoId: '',
    FormaDonacion: 1,
    MedioPago: 1,
    MedioPagoODescripcion: '',
    Descripcion: '',
};

export default function ModalNuevaDonacion({ open, onClose }: ModalNuevaDonacionProps) {
    const registrarDonacion = useRegistrarDonacion();
    const donantesQuery = useDonantes();

    const bancosQuery = useQuery<BancoCatalogo[]>({
        queryKey: ['transacciones', 'catalogo', 'bancos'],
        queryFn: async () => {
            const response = await apiClient.get<any[]>('/api/transacciones/bancos');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? item?.Id ?? ''),
                numeroCuenta: String(item?.numeroCuenta ?? item?.NumeroCuenta ?? ''),
            }));
        },
        enabled: open,
    });

    const centrosQuery = useQuery<CentroCostoCatalogo[]>({
        queryKey: ['transacciones', 'catalogo', 'centros-costo'],
        queryFn: async () => {
            const response = await apiClient.get<any[]>('/api/transacciones/centros-costo');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? item?.Id ?? ''),
                nombre: String(item?.nombre ?? item?.Nombre ?? ''),
            }));
        },
        enabled: open,
    });

    const {
        register,
        watch,
        handleSubmit,
        reset,
        setValue,
        formState: { errors },
    } = useForm<DonacionFormInput, unknown, DonacionFormValues>({
        resolver: zodResolver(donacionSchema),
        defaultValues,
    });

    const formaDonacion = watch('FormaDonacion');

    useEffect(() => {
        if (!open) {
            return;
        }

        reset({
            ...defaultValues,
            DonanteId: donantesQuery.data?.[0]?.id ?? '',
            BancoId: bancosQuery.data?.[0]?.id ?? '',
            CentroCostoId: centrosQuery.data?.[0]?.id ?? '',
        });
    }, [open, donantesQuery.data, bancosQuery.data, centrosQuery.data, reset]);

    useEffect(() => {
        if (open && donantesQuery.data?.[0]?.id) {
            setValue('DonanteId', donantesQuery.data[0].id, { shouldValidate: true });
        }
    }, [open, donantesQuery.data, setValue]);

    useEffect(() => {
        if (open && bancosQuery.data?.[0]?.id) {
            setValue('BancoId', bancosQuery.data[0].id, { shouldValidate: true });
        }
    }, [open, bancosQuery.data, setValue]);

    useEffect(() => {
        if (open && centrosQuery.data?.[0]?.id) {
            setValue('CentroCostoId', centrosQuery.data[0].id, { shouldValidate: true });
        }
    }, [open, centrosQuery.data, setValue]);

    const onSubmit = async (values: DonacionFormValues) => {
        const medioPagoSeleccionado = mediosPagoOptions.find((item) => item.value === values.MedioPago)?.label ?? '';

        await registrarDonacion.mutateAsync({
            ...values,
            MedioPago: values.FormaDonacion === 1 ? (values.MedioPago ?? 1) : 1,
            MedioPagoODescripcion:
                values.FormaDonacion === 1
                    ? medioPagoSeleccionado
                    : (values.MedioPagoODescripcion ?? '').trim(),
        });

        onClose();
    };

    if (!open) {
        return null;
    }

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
            <div className="w-full max-w-lg rounded-xl bg-white p-6 shadow-xl">
                <h2 className="text-lg font-semibold text-slate-900">Registrar Donación</h2>

                <form className="mt-4 space-y-4" onSubmit={handleSubmit(onSubmit)}>
                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Donante</label>
                        <select {...register('DonanteId')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900">
                            <option value="">Seleccione...</option>
                            {(donantesQuery.data ?? []).map((donante) => (
                                <option key={donante.id} value={donante.id}>
                                    {donante.nombreORazonSocial}
                                </option>
                            ))}
                        </select>
                        {errors.DonanteId ? <p className="mt-1 text-xs text-red-600">{errors.DonanteId.message}</p> : null}
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Monto (COP)</label>
                        <input
                            type="number"
                            step="0.01"
                            {...register('MontoCOP')}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                        {errors.MontoCOP ? <p className="mt-1 text-xs text-red-600">{errors.MontoCOP.message}</p> : null}
                    </div>

                    <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Banco</label>
                            <select {...register('BancoId')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900">
                                <option value="">Seleccione...</option>
                                {(bancosQuery.data ?? []).map((banco) => (
                                    <option key={banco.id} value={banco.id}>
                                        {banco.numeroCuenta}
                                    </option>
                                ))}
                            </select>
                            {errors.BancoId ? <p className="mt-1 text-xs text-red-600">{errors.BancoId.message}</p> : null}
                        </div>

                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Centro de costo</label>
                            <select {...register('CentroCostoId')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900">
                                <option value="">Seleccione...</option>
                                {(centrosQuery.data ?? []).map((centro) => (
                                    <option key={centro.id} value={centro.id}>
                                        {centro.nombre}
                                    </option>
                                ))}
                            </select>
                            {errors.CentroCostoId ? <p className="mt-1 text-xs text-red-600">{errors.CentroCostoId.message}</p> : null}
                        </div>
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Tipo de Donación</label>
                        <select {...register('FormaDonacion')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900">
                            {formasDonacionOptions.map((forma) => (
                                <option key={forma.value} value={forma.value}>
                                    {forma.label}
                                </option>
                            ))}
                        </select>
                    </div>

                    {formaDonacion === 1 ? (
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Medio de pago bancarizado</label>
                            <select {...register('MedioPago')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900">
                                <option value="">Seleccione...</option>
                                {mediosPagoOptions.map((medio) => (
                                    <option key={medio.value} value={medio.value}>
                                        {medio.label}
                                    </option>
                                ))}
                            </select>
                            {errors.MedioPago ? <p className="mt-1 text-xs text-red-600">{errors.MedioPago.message}</p> : null}
                        </div>
                    ) : (
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Descripción y Valoración del Bien</label>
                            <textarea
                                rows={3}
                                {...register('MedioPagoODescripcion')}
                                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                            />
                            {errors.MedioPagoODescripcion ? (
                                <p className="mt-1 text-xs text-red-600">{errors.MedioPagoODescripcion.message}</p>
                            ) : null}
                        </div>
                    )}

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Descripción (opcional)</label>
                        <input {...register('Descripcion')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900" />
                    </div>

                    {registrarDonacion.error ? (
                        <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">{registrarDonacion.error.message}</div>
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
                            disabled={registrarDonacion.isPending}
                            className="rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white disabled:opacity-60"
                        >
                            {registrarDonacion.isPending ? 'Guardando...' : 'Guardar'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
