'use client';

import { useRegistrarEntrada } from '@/features/merchandising/hooks/useRegistrarEntrada';
import { useState } from 'react';

type ModalEntradaInventarioProps = {
    productoId: string | null;
    onCerrar: () => void;
};

export default function ModalEntradaInventario({
    productoId,
    onCerrar,
}: ModalEntradaInventarioProps) {
    const registrarEntradaMutation = useRegistrarEntrada();
    const [values, setValues] = useState({
        cantidad: '',
        fecha: new Date().toISOString().slice(0, 10),
        concepto: '',
    });
    const [validationError, setValidationError] = useState<string | null>(null);

    if (!productoId) {
        return null;
    }

    const onChange = (field: 'cantidad' | 'fecha' | 'concepto', value: string) => {
        setValues((previous) => ({ ...previous, [field]: value }));
    };

    const onSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        if (!values.cantidad || !values.fecha || !values.concepto.trim()) {
            setValidationError('Cantidad, fecha y concepto son obligatorios.');
            return;
        }

        const cantidad = Number(values.cantidad);
        if (!Number.isFinite(cantidad) || cantidad <= 0) {
            setValidationError('La cantidad debe ser numerica y mayor a cero.');
            return;
        }

        setValidationError(null);
        await registrarEntradaMutation.mutateAsync({
            productoId,
            payload: {
                cantidad,
                fecha: values.fecha,
                observaciones: values.concepto.trim(),
            },
        });

        setValues({ cantidad: '', fecha: new Date().toISOString().slice(0, 10), concepto: '' });
        onCerrar();
    };

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/55 p-4">
            <div className="w-full max-w-xl rounded-2xl border border-slate-200 bg-white p-6 shadow-2xl">
                <div className="flex items-start justify-between gap-4">
                    <div>
                        <h2 className="text-xl font-bold text-slate-900">Registrar entrada de inventario</h2>
                        <p className="mt-1 text-sm text-slate-600">Producto: {productoId}</p>
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
                        <label className="mb-1 block text-sm font-medium text-slate-700">Concepto</label>
                        <textarea
                            rows={2}
                            value={values.concepto}
                            onChange={(event) => onChange('concepto', event.target.value)}
                            placeholder="Compra proveedor abril"
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                    </div>

                    {(validationError || registrarEntradaMutation.isError) ? (
                        <div className="rounded-lg border border-rose-200 bg-rose-50 px-3 py-2 text-sm text-rose-700">
                            {validationError ?? 'No fue posible registrar la entrada.'}
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
                            disabled={registrarEntradaMutation.isPending}
                            className="rounded-lg bg-emerald-700 px-4 py-2 text-sm font-medium text-white transition hover:bg-emerald-800 disabled:cursor-not-allowed disabled:opacity-60"
                        >
                            {registrarEntradaMutation.isPending ? 'Guardando...' : 'Registrar entrada'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
