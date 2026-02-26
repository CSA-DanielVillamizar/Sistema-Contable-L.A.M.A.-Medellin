'use client';

import { useMemo, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useCuentasContables } from '@/features/contabilidad/hooks/useCuentasContables';
import { useCrearArticulo } from '@/features/merchandising/hooks/useArticulos';
import {
    articuloSchema,
    categoriasArticuloOptions,
    type ArticuloFormInput,
    type ArticuloFormValues,
} from '@/features/merchandising/schemas/articuloSchema';

type ModalNuevoArticuloProps = {
    open: boolean;
    onClose: () => void;
};

const defaultValues: ArticuloFormInput = {
    Nombre: '',
    SKU: '',
    Descripcion: '',
    Categoria: 1,
    PrecioVenta: 0,
    CostoPromedio: 0,
    StockActual: 0,
    CuentaContableIngresoId: '',
};

export default function ModalNuevoArticulo({ open, onClose }: ModalNuevoArticuloProps) {
    const crearArticulo = useCrearArticulo();
    const cuentasQuery = useCuentasContables();

    const cuentasIngresoAsentables = useMemo(
        () =>
            (cuentasQuery.data ?? []).filter(
                (cuenta) => cuenta.permiteMovimiento && cuenta.codigo.startsWith('4'),
            ),
        [cuentasQuery.data],
    );

    const {
        register,
        handleSubmit,
        reset,
        setValue,
        formState: { errors },
    } = useForm<ArticuloFormInput, unknown, ArticuloFormValues>({
        resolver: zodResolver(articuloSchema),
        defaultValues,
    });

    useEffect(() => {
        if (!open) {
            return;
        }

        const cuentaDefault = cuentasIngresoAsentables[0]?.id ?? '';
        reset({ ...defaultValues, CuentaContableIngresoId: cuentaDefault });
    }, [open, reset, cuentasIngresoAsentables]);

    useEffect(() => {
        if (!open || !cuentasIngresoAsentables.length) {
            return;
        }

        setValue('CuentaContableIngresoId', cuentasIngresoAsentables[0].id);
    }, [cuentasIngresoAsentables, open, setValue]);

    const onSubmit = async (values: ArticuloFormValues) => {
        await crearArticulo.mutateAsync(values);
        onClose();
    };

    if (!open) {
        return null;
    }

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
            <div className="w-full max-w-2xl rounded-xl bg-white p-6 shadow-xl">
                <h2 className="text-lg font-semibold text-slate-900">Nuevo Artículo</h2>

                <form className="mt-4 space-y-4" onSubmit={handleSubmit(onSubmit)}>
                    <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Nombre</label>
                            <input {...register('Nombre')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900" />
                            {errors.Nombre ? <p className="mt-1 text-xs text-red-600">{errors.Nombre.message}</p> : null}
                        </div>
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">SKU</label>
                            <input {...register('SKU')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900" />
                            {errors.SKU ? <p className="mt-1 text-xs text-red-600">{errors.SKU.message}</p> : null}
                        </div>
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Descripción</label>
                        <textarea rows={3} {...register('Descripcion')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900" />
                        {errors.Descripcion ? <p className="mt-1 text-xs text-red-600">{errors.Descripcion.message}</p> : null}
                    </div>

                    <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Categoría</label>
                            <select {...register('Categoria')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900">
                                {categoriasArticuloOptions.map((categoria) => (
                                    <option key={categoria.value} value={categoria.value}>
                                        {categoria.label}
                                    </option>
                                ))}
                            </select>
                        </div>
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Stock inicial</label>
                            <input type="number" {...register('StockActual')} className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900" />
                            {errors.StockActual ? <p className="mt-1 text-xs text-red-600">{errors.StockActual.message}</p> : null}
                        </div>
                    </div>

                    <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Precio de venta (COP)</label>
                            <input
                                type="number"
                                step="0.01"
                                {...register('PrecioVenta')}
                                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                            />
                            {errors.PrecioVenta ? <p className="mt-1 text-xs text-red-600">{errors.PrecioVenta.message}</p> : null}
                        </div>
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Costo promedio (COP)</label>
                            <input
                                type="number"
                                step="0.01"
                                {...register('CostoPromedio')}
                                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                            />
                            {errors.CostoPromedio ? <p className="mt-1 text-xs text-red-600">{errors.CostoPromedio.message}</p> : null}
                        </div>
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Cuenta contable de ingreso</label>
                        <select
                            {...register('CuentaContableIngresoId')}
                            disabled={cuentasQuery.isLoading || !cuentasIngresoAsentables.length}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        >
                            {cuentasIngresoAsentables.map((cuenta) => (
                                <option key={cuenta.id} value={cuenta.id}>
                                    {cuenta.codigo} — {cuenta.descripcion}
                                </option>
                            ))}
                        </select>
                        {errors.CuentaContableIngresoId ? (
                            <p className="mt-1 text-xs text-red-600">{errors.CuentaContableIngresoId.message}</p>
                        ) : null}
                        {!cuentasQuery.isLoading && !cuentasIngresoAsentables.length ? (
                            <p className="mt-1 text-xs text-red-600">No hay cuentas de ingresos asentables disponibles.</p>
                        ) : null}
                    </div>

                    {crearArticulo.error ? <p className="text-sm text-red-600">{crearArticulo.error.message}</p> : null}

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
                            disabled={crearArticulo.isPending || !cuentasIngresoAsentables.length}
                            className="rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white disabled:opacity-60"
                        >
                            {crearArticulo.isPending ? 'Guardando...' : 'Guardar'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
