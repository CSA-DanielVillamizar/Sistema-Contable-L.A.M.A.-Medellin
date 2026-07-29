'use client';

import { useRegistrarVenta } from '@/features/merchandising/hooks/useRegistrarVenta';
import { useGetCuentasBancarias } from '@/features/tesoreria/hooks/useGetCuentasBancarias';
import { useState } from 'react';

type ModalVentaProps = {
    productoId: string | null;
    onCerrar: () => void;
};

export default function ModalVenta({
    productoId,
    onCerrar,
}: ModalVentaProps) {
    const registrarVentaMutation = useRegistrarVenta();
    const cuentasBancariasQuery = useGetCuentasBancarias();
    const [values, setValues] = useState({
        cantidad: '',
        bancoId: '',
        concepto: '',
    });
    const [validationError, setValidationError] = useState<string | null>(null);

    if (!productoId) {
        return null;
    }

    const onChange = (field: 'cantidad' | 'bancoId' | 'concepto', value: string) => {
        setValues((previous) => ({ ...previous, [field]: value }));
    };

    const onSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        if (!values.cantidad || !values.bancoId || !values.concepto.trim()) {
            setValidationError('Cantidad, cuentaBancaria destino y concepto son obligatorios.');
            return;
        }

        const cantidad = Number(values.cantidad);
        if (!Number.isFinite(cantidad) || cantidad <= 0) {
            setValidationError('La cantidad debe ser numerica y mayor a cero.');
            return;
        }

        setValidationError(null);
        await registrarVentaMutation.mutateAsync({
            productoId,
            payload: {
                cantidad,
                bancoId: values.bancoId,
                concepto: values.concepto.trim(),
            },
        });

        setValues({ cantidad: '', bancoId: '', concepto: '' });
        onCerrar();
    };

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/55 p-4">
            <div className="w-full max-w-xl rounded-2xl border border-slate-200 bg-white p-6 shadow-2xl">
                <div className="flex items-start justify-between gap-4">
                    <div>
                        <h2 className="text-xl font-bold text-slate-900">Vender producto</h2>
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
                            placeholder="2"
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Cuenta bancaria Destino</label>
                        <select
                            value={values.bancoId}
                            onChange={(event) => onChange('bancoId', event.target.value)}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        >
                            <option value="">Seleccione una cuentaBancaria...</option>
                            {(cuentasBancariasQuery.data ?? []).map((cuentaBancaria) => (
                                <option key={cuentaBancaria.id} value={cuentaBancaria.id}>
                                    {cuentaBancaria.nombre}
                                </option>
                            ))}
                        </select>
                        {cuentasBancariasQuery.isLoading ? (
                            <p className="mt-1 text-xs text-slate-500">Cargando cuentasBancarias...</p>
                        ) : null}
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Concepto</label>
                        <textarea
                            rows={2}
                            value={values.concepto}
                            onChange={(event) => onChange('concepto', event.target.value)}
                            placeholder="Venta en evento mensual"
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                    </div>

                    {(validationError || registrarVentaMutation.isError) ? (
                        <div className="rounded-lg border border-rose-200 bg-rose-50 px-3 py-2 text-sm text-rose-700">
                            {validationError ?? 'No fue posible registrar la venta.'}
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
                            disabled={registrarVentaMutation.isPending || cuentasBancariasQuery.isLoading}
                            className="rounded-lg bg-rose-700 px-4 py-2 text-sm font-medium text-white transition hover:bg-rose-800 disabled:cursor-not-allowed disabled:opacity-60"
                        >
                            {registrarVentaMutation.isPending ? 'Guardando...' : 'Registrar venta'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
