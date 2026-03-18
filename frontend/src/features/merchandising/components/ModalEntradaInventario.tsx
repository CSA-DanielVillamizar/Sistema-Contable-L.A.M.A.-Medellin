'use client';

import { useState } from 'react';

export type EntradaInventarioFormValues = {
    cantidad: string;
    fecha: string;
    observaciones: string;
};

type ModalEntradaInventarioProps = {
    abierto: boolean;
    productoNombre: string;
    enviando: boolean;
    error: string | null;
    onCerrar: () => void;
    onEnviar: (values: EntradaInventarioFormValues) => Promise<void>;
};

function getFechaActual(): string {
    return new Date().toISOString().slice(0, 10);
}

export default function ModalEntradaInventario({
    abierto,
    productoNombre,
    enviando,
    error,
    onCerrar,
    onEnviar,
}: ModalEntradaInventarioProps) {
    const [values, setValues] = useState<EntradaInventarioFormValues>({
        cantidad: '',
        fecha: getFechaActual(),
        observaciones: '',
    });
    const [validationError, setValidationError] = useState<string | null>(null);

    if (!abierto) {
        return null;
    }

    const onChange = (field: keyof EntradaInventarioFormValues, value: string) => {
        setValues((previous) => ({ ...previous, [field]: value }));
    };

    const onSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        if (!values.cantidad || !values.fecha) {
            setValidationError('Cantidad y fecha son obligatorias.');
            return;
        }

        const cantidad = Number(values.cantidad);
        if (!Number.isFinite(cantidad) || cantidad <= 0) {
            setValidationError('La cantidad debe ser numerica y mayor a cero.');
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
                        <h2 className="text-xl font-bold text-slate-900">Anadir stock</h2>
                        <p className="mt-1 text-sm text-slate-600">Producto: {productoNombre}</p>
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
                        <label className="mb-1 block text-sm font-medium text-slate-700">Cantidad</label>
                        <input
                            type="number"
                            min="1"
                            step="1"
                            value={values.cantidad}
                            onChange={(event) => onChange('cantidad', event.target.value)}
                            placeholder="10"
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Fecha</label>
                        <input
                            type="date"
                            value={values.fecha}
                            onChange={(event) => onChange('fecha', event.target.value)}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Observaciones</label>
                        <textarea
                            rows={2}
                            value={values.observaciones}
                            onChange={(event) => onChange('observaciones', event.target.value)}
                            placeholder="Fondeo inicial de inventario"
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
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
                            className="rounded-lg bg-emerald-700 px-4 py-2 text-sm font-medium text-white transition hover:bg-emerald-800 disabled:cursor-not-allowed disabled:opacity-60"
                        >
                            {enviando ? 'Guardando...' : 'Registrar entrada'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
