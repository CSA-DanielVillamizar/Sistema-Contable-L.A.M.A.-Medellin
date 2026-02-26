'use client';

import { useEffect, useMemo } from 'react';
import { useFieldArray, useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useArticulos } from '@/features/merchandising/hooks/useArticulos';
import { useProcesarVenta } from '@/features/merchandising/hooks/useVentas';
import {
    metodosPagoVentaOptions,
    ventaSchema,
    type VentaFormInput,
    type VentaFormValues,
} from '@/features/merchandising/schemas/ventaSchema';

type ModalNuevaVentaProps = {
    open: boolean;
    onClose: () => void;
};

const defaultDetalle: VentaFormInput['Detalles'][number] = {
    ArticuloId: '',
    Cantidad: 1,
    PrecioUnitario: 1,
};

const defaultValues: VentaFormInput = {
    CompradorId: '',
    MetodoPago: 1,
    Detalles: [{ ...defaultDetalle }],
};

function formatCOP(value: number): string {
    return new Intl.NumberFormat('es-CO', {
        style: 'currency',
        currency: 'COP',
        maximumFractionDigits: 0,
    }).format(value);
}

export default function ModalNuevaVenta({ open, onClose }: ModalNuevaVentaProps) {
    const articulosQuery = useArticulos();
    const procesarVenta = useProcesarVenta();

    const {
        control,
        register,
        reset,
        watch,
        setValue,
        handleSubmit,
        formState: { errors },
    } = useForm<VentaFormInput, unknown, VentaFormValues>({
        resolver: zodResolver(ventaSchema),
        defaultValues,
    });

    const { fields, append, remove } = useFieldArray({
        control,
        name: 'Detalles',
    });

    useEffect(() => {
        if (open) {
            reset(defaultValues);
        }
    }, [open, reset]);

    const detalles = watch('Detalles');

    const granTotal = useMemo(
        () =>
            (detalles ?? []).reduce(
                (sum, detalle) => sum + Number(detalle.Cantidad ?? 0) * Number(detalle.PrecioUnitario ?? 0),
                0,
            ),
        [detalles],
    );

    const onSelectArticulo = (index: number, articuloId: string) => {
        const articulo = (articulosQuery.data ?? []).find((item) => item.id === articuloId);
        setValue(`Detalles.${index}.ArticuloId`, articuloId, { shouldValidate: true });
        if (articulo) {
            setValue(`Detalles.${index}.PrecioUnitario`, articulo.precioVenta, { shouldValidate: true });
        }
    };

    const onSubmit = async (values: VentaFormValues) => {
        await procesarVenta.mutateAsync(values);
        onClose();
    };

    if (!open) {
        return null;
    }

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
            <div className="w-full max-w-6xl rounded-xl bg-white p-6 shadow-xl">
                <h2 className="text-lg font-semibold text-slate-900">Registrar Venta</h2>

                <form className="mt-4 space-y-4" onSubmit={handleSubmit(onSubmit)}>
                    <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Método de pago</label>
                            <select {...register('MetodoPago')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900">
                                {metodosPagoVentaOptions.map((metodo) => (
                                    <option key={metodo.value} value={metodo.value}>
                                        {metodo.label}
                                    </option>
                                ))}
                            </select>
                        </div>

                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">CompradorId (opcional)</label>
                            <input
                                {...register('CompradorId')}
                                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                                placeholder="GUID comprador"
                            />
                            {errors.CompradorId ? <p className="mt-1 text-xs text-red-600">{errors.CompradorId.message}</p> : null}
                        </div>
                    </div>

                    <div className="overflow-x-auto rounded-xl border border-slate-200">
                        <table className="min-w-full divide-y divide-slate-200">
                            <thead className="bg-slate-50">
                                <tr>
                                    <th className="px-3 py-2 text-left text-xs font-semibold uppercase">Artículo</th>
                                    <th className="px-3 py-2 text-right text-xs font-semibold uppercase">Cantidad</th>
                                    <th className="px-3 py-2 text-right text-xs font-semibold uppercase">Precio Unitario</th>
                                    <th className="px-3 py-2 text-right text-xs font-semibold uppercase">Subtotal</th>
                                    <th className="px-3 py-2 text-right text-xs font-semibold uppercase">Acción</th>
                                </tr>
                            </thead>

                            <tbody className="divide-y divide-slate-200 bg-white">
                                {fields.map((field, index) => {
                                    const cantidad = Number(detalles?.[index]?.Cantidad ?? 0);
                                    const precio = Number(detalles?.[index]?.PrecioUnitario ?? 0);
                                    const subtotal = cantidad * precio;

                                    return (
                                        <tr key={field.id}>
                                            <td className="px-3 py-2">
                                                <select
                                                    value={detalles?.[index]?.ArticuloId ?? ''}
                                                    onChange={(event) => onSelectArticulo(index, event.target.value)}
                                                    className="w-full rounded border border-slate-300 px-2 py-1 text-sm text-slate-900"
                                                >
                                                    <option value="">Seleccione...</option>
                                                    {(articulosQuery.data ?? []).map((articulo) => (
                                                        <option key={articulo.id} value={articulo.id}>
                                                            {articulo.nombre} ({articulo.sku})
                                                        </option>
                                                    ))}
                                                </select>
                                            </td>

                                            <td className="px-3 py-2">
                                                <input
                                                    type="number"
                                                    min={1}
                                                    {...register(`Detalles.${index}.Cantidad`)}
                                                    className="w-full rounded border border-slate-300 px-2 py-1 text-right text-sm text-slate-900"
                                                />
                                            </td>

                                            <td className="px-3 py-2 text-right text-sm text-slate-900">{formatCOP(precio)}</td>
                                            <td className="px-3 py-2 text-right text-sm font-semibold text-slate-900">{formatCOP(subtotal)}</td>

                                            <td className="px-3 py-2 text-right">
                                                <button
                                                    type="button"
                                                    onClick={() => remove(index)}
                                                    disabled={fields.length <= 1}
                                                    className="rounded border border-slate-300 px-2 py-1 text-xs text-slate-700 disabled:opacity-50"
                                                >
                                                    Quitar
                                                </button>
                                            </td>
                                        </tr>
                                    );
                                })}
                            </tbody>

                            <tfoot className="bg-slate-50">
                                <tr>
                                    <td colSpan={3} className="px-3 py-2 text-right text-sm font-semibold text-slate-700">Gran Total</td>
                                    <td className="px-3 py-2 text-right text-sm font-bold text-slate-900">{formatCOP(granTotal)}</td>
                                    <td />
                                </tr>
                            </tfoot>
                        </table>
                    </div>

                    {errors.Detalles?.message ? <p className="text-sm text-red-600">{errors.Detalles.message}</p> : null}
                    {procesarVenta.error ? <p className="text-sm text-red-600">{procesarVenta.error.message}</p> : null}

                    <div className="flex items-center justify-between">
                        <button
                            type="button"
                            onClick={() => append({ ...defaultDetalle })}
                            className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700"
                        >
                            + Agregar artículo
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
                                disabled={procesarVenta.isPending}
                                className="rounded-lg bg-emerald-700 px-4 py-2 text-sm font-medium text-white disabled:opacity-60"
                            >
                                {procesarVenta.isPending ? 'Procesando...' : 'Registrar Venta'}
                            </button>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    );
}
