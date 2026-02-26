'use client';

import apiClient from '@/lib/apiClient';
import { zodResolver } from '@hookform/resolvers/zod';
import { useQuery } from '@tanstack/react-query';
import { useEffect, useMemo } from 'react';
import { useFieldArray, useForm } from 'react-hook-form';
import { useCuentasContables } from '@/features/contabilidad/hooks/useCuentasContables';
import { useRegistrarComprobante } from '@/features/contabilidad/hooks/useComprobantes';
import {
    comprobanteSchema,
    type ComprobanteFormInput,
    type ComprobanteFormValues,
} from '@/features/contabilidad/schemas/comprobanteSchema';

type ModalNuevoComprobanteProps = {
    open: boolean;
    onClose: () => void;
};

type CentroCostoItem = {
    id: string;
    nombre: string;
};

const tiposComprobante = [
    { value: 1, label: 'Ingreso' },
    { value: 2, label: 'Egreso' },
    { value: 3, label: 'Diario' },
    { value: 4, label: 'Ajuste' },
    { value: 5, label: 'Cierre' },
];

const asientoVacio: ComprobanteFormInput['Asientos'][number] = {
    CuentaContableId: '',
    CentroCostoId: '',
    TerceroId: '',
    Referencia: '',
    Debe: 0,
    Haber: 0,
};

const defaultValues: ComprobanteFormInput = {
    Fecha: new Date().toISOString().slice(0, 10),
    Tipo: 3,
    Descripcion: '',
    Asientos: [{ ...asientoVacio }, { ...asientoVacio }],
};

function formatCOP(value: number): string {
    return new Intl.NumberFormat('es-CO', {
        style: 'currency',
        currency: 'COP',
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
    }).format(value);
}

export default function ModalNuevoComprobante({ open, onClose }: ModalNuevoComprobanteProps) {
    const cuentasQuery = useCuentasContables();
    const registrarComprobante = useRegistrarComprobante();

    const centrosCostoQuery = useQuery<CentroCostoItem[]>({
        queryKey: ['transacciones', 'catalogo', 'centros-costo'],
        enabled: open,
        queryFn: async () => {
            const response = await apiClient.get<any[]>('/api/transacciones/centros-costo');
            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? item?.Id ?? ''),
                nombre: String(item?.nombre ?? item?.Nombre ?? ''),
            }));
        },
    });

    const {
        control,
        register,
        watch,
        reset,
        handleSubmit,
        formState: { errors },
    } = useForm<ComprobanteFormInput, unknown, ComprobanteFormValues>({
        resolver: zodResolver(comprobanteSchema),
        defaultValues,
    });

    const { fields, append, remove } = useFieldArray({
        control,
        name: 'Asientos',
    });

    useEffect(() => {
        if (open) {
            reset(defaultValues);
        }
    }, [open, reset]);

    const asientos = watch('Asientos');
    const totalDebe = useMemo(() => (asientos ?? []).reduce((sum, row) => sum + Number(row.Debe ?? 0), 0), [asientos]);
    const totalHaber = useMemo(() => (asientos ?? []).reduce((sum, row) => sum + Number(row.Haber ?? 0), 0), [asientos]);
    const diferencia = Number((totalDebe - totalHaber).toFixed(2));
    const comprobanteBalanceado = diferencia === 0;

    const cuentasAsentables = (cuentasQuery.data ?? []).filter((item) => item.permiteMovimiento);

    const onSubmit = async (values: ComprobanteFormValues) => {
        await registrarComprobante.mutateAsync({
            ...values,
            Asientos: values.Asientos.map((item) => ({
                ...item,
                TerceroId: item.TerceroId ? item.TerceroId : undefined,
            })),
        });

        onClose();
    };

    if (!open) {
        return null;
    }

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
            <div className="w-full max-w-7xl rounded-xl bg-white p-6 shadow-xl">
                <h2 className="text-lg font-semibold text-slate-900">Nuevo Comprobante</h2>

                <form className="mt-4 space-y-4" onSubmit={handleSubmit(onSubmit)}>
                    <div className="grid grid-cols-1 gap-4 md:grid-cols-3">
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Fecha</label>
                            <input type="date" {...register('Fecha')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900" />
                            {errors.Fecha ? <p className="mt-1 text-xs text-red-600">{errors.Fecha.message}</p> : null}
                        </div>

                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Tipo</label>
                            <select {...register('Tipo')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900">
                                {tiposComprobante.map((tipo) => (
                                    <option key={tipo.value} value={tipo.value}>{tipo.label}</option>
                                ))}
                            </select>
                        </div>

                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Descripción</label>
                            <input {...register('Descripcion')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900" />
                            {errors.Descripcion ? <p className="mt-1 text-xs text-red-600">{errors.Descripcion.message}</p> : null}
                        </div>
                    </div>

                    <div className="overflow-x-auto rounded-xl border border-slate-200">
                        <table className="min-w-full divide-y divide-slate-200">
                            <thead className="bg-slate-50">
                                <tr>
                                    <th className="px-3 py-2 text-left text-xs font-semibold uppercase">Cuenta</th>
                                    <th className="px-3 py-2 text-left text-xs font-semibold uppercase">Centro de Costo</th>
                                    <th className="px-3 py-2 text-left text-xs font-semibold uppercase">Tercero</th>
                                    <th className="px-3 py-2 text-left text-xs font-semibold uppercase">Referencia</th>
                                    <th className="px-3 py-2 text-right text-xs font-semibold uppercase">Debe</th>
                                    <th className="px-3 py-2 text-right text-xs font-semibold uppercase">Haber</th>
                                    <th className="px-3 py-2 text-right text-xs font-semibold uppercase">Acción</th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-slate-200 bg-white">
                                {fields.map((field, index) => (
                                    <tr key={field.id}>
                                        <td className="px-3 py-2">
                                            <select {...register(`Asientos.${index}.CuentaContableId`)} className="w-full rounded border border-slate-300 px-2 py-1 text-sm text-slate-900">
                                                <option value="">Seleccione...</option>
                                                {cuentasAsentables.map((cuenta) => (
                                                    <option key={cuenta.id} value={cuenta.id}>
                                                        {cuenta.codigo} - {cuenta.descripcion}
                                                    </option>
                                                ))}
                                            </select>
                                        </td>

                                        <td className="px-3 py-2">
                                            <select {...register(`Asientos.${index}.CentroCostoId`)} className="w-full rounded border border-slate-300 px-2 py-1 text-sm text-slate-900">
                                                <option value="">Seleccione...</option>
                                                {(centrosCostoQuery.data ?? []).map((centro) => (
                                                    <option key={centro.id} value={centro.id}>{centro.nombre}</option>
                                                ))}
                                            </select>
                                        </td>

                                        <td className="px-3 py-2">
                                            <input {...register(`Asientos.${index}.TerceroId`)} placeholder="GUID opcional" className="w-full rounded border border-slate-300 px-2 py-1 text-sm text-slate-900" />
                                        </td>

                                        <td className="px-3 py-2">
                                            <input {...register(`Asientos.${index}.Referencia`)} className="w-full rounded border border-slate-300 px-2 py-1 text-sm text-slate-900" />
                                        </td>

                                        <td className="px-3 py-2">
                                            <input type="number" step="0.01" {...register(`Asientos.${index}.Debe`)} className="w-full rounded border border-slate-300 px-2 py-1 text-right text-sm text-slate-900" />
                                        </td>

                                        <td className="px-3 py-2">
                                            <input type="number" step="0.01" {...register(`Asientos.${index}.Haber`)} className="w-full rounded border border-slate-300 px-2 py-1 text-right text-sm text-slate-900" />
                                        </td>

                                        <td className="px-3 py-2 text-right">
                                            <button
                                                type="button"
                                                onClick={() => remove(index)}
                                                disabled={fields.length <= 2}
                                                className="rounded border border-slate-300 px-2 py-1 text-xs text-slate-700 disabled:opacity-50"
                                            >
                                                Quitar
                                            </button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>

                            <tfoot className="bg-slate-50">
                                <tr>
                                    <td colSpan={4} className="px-3 py-2 text-right text-sm font-semibold text-slate-700">Totales</td>
                                    <td className="px-3 py-2 text-right text-sm font-semibold text-slate-900">{formatCOP(totalDebe)}</td>
                                    <td className="px-3 py-2 text-right text-sm font-semibold text-slate-900">{formatCOP(totalHaber)}</td>
                                    <td className={`px-3 py-2 text-right text-sm font-semibold ${comprobanteBalanceado ? 'text-emerald-700' : 'text-red-700'}`}>
                                        Diferencia: {formatCOP(diferencia)}
                                    </td>
                                </tr>
                            </tfoot>
                        </table>
                    </div>

                    {errors.Asientos?.message ? <p className="text-sm text-red-600">{errors.Asientos.message}</p> : null}
                    {registrarComprobante.error ? <p className="text-sm text-red-600">{registrarComprobante.error.message}</p> : null}

                    <div className="flex items-center justify-between">
                        <button
                            type="button"
                            onClick={() => append({ ...asientoVacio })}
                            className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700"
                        >
                            + Agregar línea
                        </button>

                        <div className="flex gap-2">
                            <button
                                type="button"
                                onClick={onClose}
                                className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700"
                            >
                                Cancelar
                            </button>
                            <button
                                type="submit"
                                disabled={!comprobanteBalanceado || registrarComprobante.isPending}
                                className="rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white disabled:opacity-60"
                            >
                                {registrarComprobante.isPending ? 'Guardando...' : 'Guardar'}
                            </button>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    );
}
