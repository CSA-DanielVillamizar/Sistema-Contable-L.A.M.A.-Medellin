'use client';

import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { ingresoSchema, type IngresoFormValues } from '@/features/transacciones/schemas/ingresoSchema';
import { useCrearIngreso } from '@/features/transacciones/hooks/useCrearIngreso';

const defaultValues: IngresoFormValues = {
    MontoCOP: 0,
    CentroCostoId: '',
    MedioPago: '',
    EsMonedaOrigenUSD: false,
};

export default function RegistroIngresoForm() {
    const [mensajeExito, setMensajeExito] = useState<string>('');
    const { mutateAsync, isPending, error } = useCrearIngreso();

    const {
        register,
        watch,
        handleSubmit,
        reset,
        formState: { errors },
    } = useForm<IngresoFormValues>({
        resolver: zodResolver(ingresoSchema),
        defaultValues,
    });

    const esMonedaOrigenUSD = watch('EsMonedaOrigenUSD');

    const onSubmit = async (values: IngresoFormValues) => {
        setMensajeExito('');

        await mutateAsync({
            MontoCOP: values.MontoCOP,
            CentroCostoId: values.CentroCostoId,
            MedioPago: Number(values.MedioPago),
            MonedaOrigen: values.EsMonedaOrigenUSD ? 'USD' : undefined,
            MontoMonedaOrigen: values.EsMonedaOrigenUSD ? values.MontoMonedaOrigen : undefined,
            TasaCambioUsada: values.EsMonedaOrigenUSD ? values.TasaCambioUsada : undefined,
            FuenteTasaCambio: values.EsMonedaOrigenUSD ? values.FuenteTasaCambio : undefined,
        });

        setMensajeExito('Ingreso registrado correctamente.');
        reset(defaultValues);
    };

    return (
        <section className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
                <div>
                    <label className="mb-1 block text-sm font-medium text-slate-700">Monto COP</label>
                    <input
                        type="number"
                        step="0.01"
                        {...register('MontoCOP')}
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
                    />
                    {errors.MontoCOP && <p className="mt-1 text-sm text-red-600">{errors.MontoCOP.message}</p>}
                </div>

                <div>
                    <label className="mb-1 block text-sm font-medium text-slate-700">CentroCostoId</label>
                    <input
                        type="text"
                        {...register('CentroCostoId')}
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
                    />
                    {errors.CentroCostoId && <p className="mt-1 text-sm text-red-600">{errors.CentroCostoId.message}</p>}
                </div>

                <div>
                    <label className="mb-1 block text-sm font-medium text-slate-700">MedioPago</label>
                    <select
                        {...register('MedioPago')}
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
                    >
                        <option value="">Seleccione...</option>
                        <option value="1">Transferencia</option>
                        <option value="2">Consignación</option>
                        <option value="3">Corresponsal</option>
                        <option value="4">QR</option>
                    </select>
                    {errors.MedioPago && <p className="mt-1 text-sm text-red-600">{errors.MedioPago.message}</p>}
                </div>

                <label className="flex items-center gap-2 rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-sm text-slate-700">
                    <input type="checkbox" {...register('EsMonedaOrigenUSD')} />
                    Moneda origen USD
                </label>

                {esMonedaOrigenUSD && (
                    <div className="grid grid-cols-1 gap-4 rounded-lg border border-blue-100 bg-blue-50/40 p-4 md:grid-cols-3">
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">MontoMonedaOrigen</label>
                            <input
                                type="number"
                                step="0.01"
                                {...register('MontoMonedaOrigen')}
                                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
                            />
                            {errors.MontoMonedaOrigen && <p className="mt-1 text-sm text-red-600">{errors.MontoMonedaOrigen.message}</p>}
                        </div>

                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">TasaCambioUsada</label>
                            <input
                                type="number"
                                step="0.0001"
                                {...register('TasaCambioUsada')}
                                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
                            />
                            {errors.TasaCambioUsada && <p className="mt-1 text-sm text-red-600">{errors.TasaCambioUsada.message}</p>}
                        </div>

                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">FuenteTasaCambio</label>
                            <select
                                {...register('FuenteTasaCambio')}
                                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
                            >
                                <option value="">Seleccione...</option>
                                <option value="1">TRM SFC</option>
                                <option value="2">Tasa Banco</option>
                                <option value="3">Manual con Soporte</option>
                            </select>
                            {errors.FuenteTasaCambio && <p className="mt-1 text-sm text-red-600">{errors.FuenteTasaCambio.message}</p>}
                        </div>
                    </div>
                )}

                {error && (
                    <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">{error.message}</div>
                )}

                {mensajeExito && (
                    <div className="rounded-lg border border-emerald-200 bg-emerald-50 px-3 py-2 text-sm text-emerald-700">{mensajeExito}</div>
                )}

                <button
                    type="submit"
                    disabled={isPending}
                    className="inline-flex items-center rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white transition hover:bg-blue-800 disabled:cursor-not-allowed disabled:opacity-60"
                >
                    {isPending ? 'Guardando...' : 'Registrar ingreso'}
                </button>
            </form>
        </section>
    );
}
