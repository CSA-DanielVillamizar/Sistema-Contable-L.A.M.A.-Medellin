'use client';

import { useCrearProyecto } from '@/features/proyectos/hooks/useProyectos';
import {
    estadosProyectoOptions,
    proyectoSchema,
    type ProyectoFormInput,
    type ProyectoFormValues,
} from '@/features/proyectos/schemas/proyectoSchema';
import apiClient from '@/lib/apiClient';
import { zodResolver } from '@hookform/resolvers/zod';
import { useQuery } from '@tanstack/react-query';
import { useEffect } from 'react';
import { useForm } from 'react-hook-form';

type ModalNuevoProyectoProps = {
    open: boolean;
    onClose: () => void;
};

type CentroCostoCatalogo = {
    id: string;
    nombre: string;
};

const defaultValues: ProyectoFormInput = {
    CentroCostoId: '',
    Nombre: '',
    Descripcion: '',
    FechaInicio: '',
    FechaFin: '',
    PresupuestoEstimado: 0,
    Estado: 1,
};

export default function ModalNuevoProyecto({ open, onClose }: ModalNuevoProyectoProps) {
    const crearProyecto = useCrearProyecto();

    const centrosQuery = useQuery<CentroCostoCatalogo[]>({
        queryKey: ['transacciones', 'catalogo', 'centros-costo'],
        queryFn: async () => {
            const response = await apiClient.get<any[]>('/api/transacciones/centros-costo');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? ''),
                nombre: String(item?.nombre ?? ''),
            }));
        },
        enabled: open,
    });

    const {
        register,
        handleSubmit,
        reset,
        setValue,
        formState: { errors },
    } = useForm<ProyectoFormInput, unknown, ProyectoFormValues>({
        resolver: zodResolver(proyectoSchema),
        defaultValues,
    });

    useEffect(() => {
        if (open) {
            reset({
                ...defaultValues,
                CentroCostoId: centrosQuery.data?.[0]?.id ?? '',
            });
        }
    }, [open, centrosQuery.data, reset]);

    useEffect(() => {
        if (open && centrosQuery.data?.[0]?.id) {
            setValue('CentroCostoId', centrosQuery.data[0].id, { shouldValidate: true });
        }
    }, [open, centrosQuery.data, setValue]);

    const onSubmit = async (values: ProyectoFormValues) => {
        await crearProyecto.mutateAsync(values);
        onClose();
    };

    if (!open) {
        return null;
    }

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
            <div className="w-full max-w-xl rounded-xl bg-white p-6 shadow-xl">
                <h2 className="text-lg font-semibold text-slate-900">Nuevo Proyecto Social</h2>

                <form className="mt-4 space-y-4" onSubmit={handleSubmit(onSubmit)}>
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

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Nombre</label>
                        <input {...register('Nombre')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900" />
                        {errors.Nombre ? <p className="mt-1 text-xs text-red-600">{errors.Nombre.message}</p> : null}
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Descripción</label>
                        <textarea rows={3} {...register('Descripcion')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900" />
                        {errors.Descripcion ? <p className="mt-1 text-xs text-red-600">{errors.Descripcion.message}</p> : null}
                    </div>

                    <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Fecha inicio</label>
                            <input type="date" {...register('FechaInicio')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900" />
                            {errors.FechaInicio ? <p className="mt-1 text-xs text-red-600">{errors.FechaInicio.message}</p> : null}
                        </div>
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Fecha fin (opcional)</label>
                            <input type="date" {...register('FechaFin')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900" />
                            {errors.FechaFin ? <p className="mt-1 text-xs text-red-600">{errors.FechaFin.message}</p> : null}
                        </div>
                    </div>

                    <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Presupuesto estimado (COP)</label>
                            <input
                                type="number"
                                step="0.01"
                                {...register('PresupuestoEstimado')}
                                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                            />
                            {errors.PresupuestoEstimado ? <p className="mt-1 text-xs text-red-600">{errors.PresupuestoEstimado.message}</p> : null}
                        </div>
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Estado</label>
                            <select {...register('Estado')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900">
                                {estadosProyectoOptions.map((estado) => (
                                    <option key={estado.value} value={estado.value}>
                                        {estado.label}
                                    </option>
                                ))}
                            </select>
                        </div>
                    </div>

                    {crearProyecto.error ? <p className="text-sm text-red-600">{crearProyecto.error.message}</p> : null}

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
                            disabled={crearProyecto.isPending}
                            className="rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white disabled:opacity-60"
                        >
                            {crearProyecto.isPending ? 'Guardando...' : 'Guardar'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
