'use client';

import { useState } from 'react';

export type CatalogItem = {
    id: string;
    nombre: string;
};

export type NuevoProductoFormValues = {
    nombre: string;
    sku: string;
    precioVentaCOP: string;
    cuentaContableIngresoId: string;
};

type ModalNuevoProductoProps = {
    abierto: boolean;
    cuentasIngreso: CatalogItem[];
    enviando: boolean;
    error: string | null;
    onCerrar: () => void;
    onEnviar: (values: NuevoProductoFormValues) => Promise<void>;
};

export default function ModalNuevoProducto({
    abierto,
    cuentasIngreso,
    enviando,
    error,
    onCerrar,
    onEnviar,
}: ModalNuevoProductoProps) {
    const [values, setValues] = useState<NuevoProductoFormValues>({
        nombre: '',
        sku: '',
        precioVentaCOP: '',
        cuentaContableIngresoId: '',
    });
    const [validationError, setValidationError] = useState<string | null>(null);

    if (!abierto) {
        return null;
    }

    const onChange = (field: keyof NuevoProductoFormValues, value: string) => {
        setValues((previous) => ({ ...previous, [field]: value }));
    };

    const onSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        if (!values.nombre.trim() || !values.sku.trim() || !values.precioVentaCOP || !values.cuentaContableIngresoId) {
            setValidationError('Nombre, SKU, precio y cuenta contable son obligatorios.');
            return;
        }

        const precio = Number(values.precioVentaCOP);
        if (!Number.isFinite(precio) || precio <= 0) {
            setValidationError('El precio debe ser numerico y mayor a cero.');
            return;
        }

        setValidationError(null);
        await onEnviar(values);
    };

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/55 p-4">
            <div className="w-full max-w-xl rounded-2xl border border-slate-200 bg-white p-6 shadow-2xl">
                <div className="flex items-start justify-between gap-4">
                    <div>
                        <h2 className="text-xl font-bold text-slate-900">Nuevo producto</h2>
                        <p className="mt-1 text-sm text-slate-600">Crear item de merchandising para inventario y ventas.</p>
                    </div>

                    <button
                        type="button"
                        onClick={onCerrar}
                        className="rounded-lg border border-slate-300 px-3 py-1.5 text-sm font-medium text-slate-700"
                    >
                        Cerrar
                    </button>
                </div>

                <form className="mt-5 grid grid-cols-1 gap-4" onSubmit={onSubmit}>
                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Nombre</label>
                        <input
                            type="text"
                            value={values.nombre}
                            onChange={(event) => onChange('nombre', event.target.value)}
                            placeholder="Parche L.A.M.A. Oficial"
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">SKU</label>
                        <input
                            type="text"
                            value={values.sku}
                            onChange={(event) => onChange('sku', event.target.value)}
                            placeholder="P-OFC-01"
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Precio venta (COP)</label>
                        <input
                            type="number"
                            min="0"
                            step="0.01"
                            value={values.precioVentaCOP}
                            onChange={(event) => onChange('precioVentaCOP', event.target.value)}
                            placeholder="35000"
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Cuenta contable ingreso</label>
                        <select
                            value={values.cuentaContableIngresoId}
                            onChange={(event) => onChange('cuentaContableIngresoId', event.target.value)}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        >
                            <option value="">Seleccione...</option>
                            {cuentasIngreso.map((cuenta) => (
                                <option key={cuenta.id} value={cuenta.id}>
                                    {cuenta.nombre}
                                </option>
                            ))}
                        </select>
                    </div>

                    {(validationError || error) ? (
                        <div className="rounded-lg border border-rose-200 bg-rose-50 px-3 py-2 text-sm text-rose-700">
                            {validationError ?? error}
                        </div>
                    ) : null}

                    <div className="flex items-center justify-end gap-2 pt-1">
                        <button
                            type="button"
                            onClick={onCerrar}
                            className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700"
                        >
                            Cancelar
                        </button>

                        <button
                            type="submit"
                            disabled={enviando}
                            className="rounded-lg bg-indigo-700 px-4 py-2 text-sm font-medium text-white transition hover:bg-indigo-800 disabled:cursor-not-allowed disabled:opacity-60"
                        >
                            {enviando ? 'Guardando...' : 'Crear producto'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
