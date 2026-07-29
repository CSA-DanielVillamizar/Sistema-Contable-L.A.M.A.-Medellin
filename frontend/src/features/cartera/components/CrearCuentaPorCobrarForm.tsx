'use client';

import { useCrearCuentaPorCobrar } from '@/features/cartera/hooks/useCrearCuentaPorCobrar';
import { useGetConceptosCobroLookup } from '@/features/cartera/hooks/useGetConceptosCobroLookup';
import { useGetMiembrosLookup } from '@/features/cartera/hooks/useGetMiembrosLookup';
import {
    crearCuentaPorCobrarSchema,
    type CrearCuentaPorCobrarFormInput,
    type CrearCuentaPorCobrarFormValues,
} from '@/features/cartera/schemas/carteraSchemas';
import { zodResolver } from '@hookform/resolvers/zod';
import { useState } from 'react';
import { useForm } from 'react-hook-form';

const defaultValues: CrearCuentaPorCobrarFormInput = {
    miembroId: '',
    conceptoCobroId: '',
    periodo: new Date().toISOString().slice(0, 7),
    fechaEmision: '',
    fechaVencimiento: '',
    valorTotal: 0,
};

export default function CrearCuentaPorCobrarForm() {
    const [mensajeExito, setMensajeExito] = useState('');
    const [mensajeError, setMensajeError] = useState('');
    const { data: miembrosLookup = [], isLoading: isLoadingMiembros } = useGetMiembrosLookup();
    const { data: conceptosLookup = [], isLoading: isLoadingConceptos } = useGetConceptosCobroLookup();

    const { mutateAsync, isPending } = useCrearCuentaPorCobrar({
        onSuccessNotification: (message) => {
            setMensajeError('');
            setMensajeExito(message);
        },
        onErrorNotification: (message) => {
            setMensajeExito('');
            setMensajeError(message);
        },
    });

    const {
        register,
        handleSubmit,
        reset,
        formState: { errors },
    } = useForm<CrearCuentaPorCobrarFormInput, unknown, CrearCuentaPorCobrarFormValues>({
        resolver: zodResolver(crearCuentaPorCobrarSchema),
        defaultValues,
    });

    const onSubmit = async (values: CrearCuentaPorCobrarFormValues) => {
        await mutateAsync({
            miembroId: values.miembroId,
            conceptoCobroId: values.conceptoCobroId,
            periodo: values.periodo,
            fechaEmision: values.fechaEmision,
            fechaVencimiento: values.fechaVencimiento,
            valorTotal: values.valorTotal,
        });

        reset(defaultValues);
    };

    return (
        <section className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
            <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">

                {/* Miembro */}
                <div>
                    <label className="mb-1 block text-sm font-medium text-slate-700">
                        Miembro
                    </label>
                    <select
                        {...register('miembroId')}
                        disabled={isLoadingMiembros}
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2 disabled:bg-slate-100"
                    >
                        <option value="">
                            {isLoadingMiembros ? 'Cargando...' : 'Seleccione un miembro'}
                        </option>
                        {miembrosLookup.map((miembro) => (
                            <option key={miembro.id} value={miembro.id}>
                                {miembro.nombreCompleto}
                            </option>
                        ))}
                    </select>
                    {errors.miembroId && (
                        <p className="mt-1 text-sm text-red-600">{errors.miembroId.message}</p>
                    )}
                </div>

                {/* Concepto de cobro */}
                <div>
                    <label className="mb-1 block text-sm font-medium text-slate-700">
                        Concepto de cobro
                    </label>
                    <select
                        {...register('conceptoCobroId')}
                        disabled={isLoadingConceptos}
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2 disabled:bg-slate-100"
                    >
                        <option value="">
                            {isLoadingConceptos ? 'Cargando...' : 'Seleccione un concepto de cobro'}
                        </option>
                        {conceptosLookup.map((concepto) => (
                            <option key={concepto.id} value={concepto.id}>
                                {concepto.nombre}
                            </option>
                        ))}
                    </select>
                    {errors.conceptoCobroId && (
                        <p className="mt-1 text-sm text-red-600">{errors.conceptoCobroId.message}</p>
                    )}
                </div>

                {/* Fechas en fila */}
                <div className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Periodo</label>
                        <input
                            type="month"
                            {...register('periodo')}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                        <p className="mt-1 text-xs text-slate-500">
                            Mes que cubre la obligacion. Es lo que evita cobrar dos veces el mismo periodo.
                        </p>
                        {errors.periodo && (
                            <p className="mt-1 text-sm text-red-600">{errors.periodo.message}</p>
                        )}
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Fecha de emisión</label>
                        <input
                            type="date"
                            {...register('fechaEmision')}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
                        />
                        {errors.fechaEmision && (
                            <p className="mt-1 text-sm text-red-600">{errors.fechaEmision.message}</p>
                        )}
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Fecha de vencimiento</label>
                        <input
                            type="date"
                            {...register('fechaVencimiento')}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
                        />
                        {errors.fechaVencimiento && (
                            <p className="mt-1 text-sm text-red-600">{errors.fechaVencimiento.message}</p>
                        )}
                    </div>
                </div>

                {/* Valor total */}
                <div>
                    <label className="mb-1 block text-sm font-medium text-slate-700">Valor total (COP)</label>
                    <input
                        type="number"
                        min={1}
                        step={1}
                        {...register('valorTotal')}
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
                        placeholder="Ej: 50000"
                    />
                    {errors.valorTotal && (
                        <p className="mt-1 text-sm text-red-600">{errors.valorTotal.message}</p>
                    )}
                </div>

                {/* Mensajes de estado */}
                {mensajeExito && (
                    <div className="rounded-lg bg-green-50 px-4 py-3 text-sm text-green-800">
                        {mensajeExito}
                    </div>
                )}
                {mensajeError && (
                    <div className="rounded-lg bg-red-50 px-4 py-3 text-sm text-red-800">
                        {mensajeError}
                    </div>
                )}

                <button
                    type="submit"
                    disabled={isPending}
                    className="w-full rounded-lg bg-blue-600 px-4 py-2 font-semibold text-white transition hover:bg-blue-700 disabled:opacity-50"
                >
                    {isPending ? 'Guardando…' : 'Crear cuenta por cobrar'}
                </button>
            </form>
        </section>
    );
}
