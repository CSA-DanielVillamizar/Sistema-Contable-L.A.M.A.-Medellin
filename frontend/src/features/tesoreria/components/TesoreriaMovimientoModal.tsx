'use client';

import { useState } from 'react';

export type TesoreriaCatalogItem = {
    id: string;
    nombre: string;
};

export type TesoreriaMovimientoFormValues = {
    fecha: string;
    monto: string;
    concepto: string;
    cajaId: string;
    cuentaContableId: string;
    centroCostoId: string;
};

type TesoreriaMovimientoModalProps = {
    modo: 'ingreso' | 'egreso';
    abierto: boolean;
    cajas: TesoreriaCatalogItem[];
    cuentasContables: TesoreriaCatalogItem[];
    centrosCosto: TesoreriaCatalogItem[];
    enviando: boolean;
    error: string | null;
    onCerrar: () => void;
    onEnviar: (values: TesoreriaMovimientoFormValues) => Promise<void>;
};

function getFechaActual(): string {
    return new Date().toISOString().slice(0, 10);
}

function buildInitialValues(
    cajas: TesoreriaCatalogItem[],
    cuentasContables: TesoreriaCatalogItem[],
    centrosCosto: TesoreriaCatalogItem[],
): TesoreriaMovimientoFormValues {
    return {
        fecha: getFechaActual(),
        monto: '',
        concepto: '',
        cajaId: cajas[0]?.id ?? '',
        cuentaContableId: cuentasContables[0]?.id ?? '',
        centroCostoId: centrosCosto[0]?.id ?? '',
    };
}

export default function TesoreriaMovimientoModal({
    modo,
    abierto,
    cajas,
    cuentasContables,
    centrosCosto,
    enviando,
    error,
    onCerrar,
    onEnviar,
}: TesoreriaMovimientoModalProps) {
    const [values, setValues] = useState<TesoreriaMovimientoFormValues>(() =>
        buildInitialValues(cajas, cuentasContables, centrosCosto),
    );
    const [validationError, setValidationError] = useState<string | null>(null);

    if (!abierto) {
        return null;
    }

    const titulo = modo === 'ingreso' ? 'Registrar Ingreso' : 'Registrar Egreso';
    const accion = modo === 'ingreso' ? 'Registrar ingreso' : 'Registrar egreso';
    const colorPrincipal = modo === 'ingreso' ? 'bg-emerald-700 hover:bg-emerald-800' : 'bg-rose-700 hover:bg-rose-800';

    const onChange = (field: keyof TesoreriaMovimientoFormValues, value: string) => {
        setValues((previous) => ({ ...previous, [field]: value }));
    };

    const onSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        if (!values.fecha || !values.monto || !values.concepto.trim() || !values.cajaId || !values.cuentaContableId || !values.centroCostoId) {
            setValidationError('Todos los campos son obligatorios para registrar el movimiento.');
            return;
        }

        const montoNumero = Number(values.monto);
        if (!Number.isFinite(montoNumero) || montoNumero <= 0) {
            setValidationError('El monto debe ser un valor numérico mayor a cero.');
            return;
        }

        setValidationError(null);
        await onEnviar(values);
    };

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/55 p-4">
            <div className="w-full max-w-2xl rounded-2xl border border-slate-200 bg-white p-6 shadow-2xl">
                <div className="flex items-start justify-between gap-4">
                    <div>
                        <h2 className="text-xl font-bold text-slate-900">{titulo}</h2>
                        <p className="mt-1 text-sm text-slate-600">Movimientos en caja con comprobante y partida doble.</p>
                    </div>

                    <button
                        type="button"
                        onClick={onCerrar}
                        className="rounded-lg border border-slate-300 px-3 py-1.5 text-sm font-medium text-slate-700"
                    >
                        Cerrar
                    </button>
                </div>

                <form className="mt-5 grid grid-cols-1 gap-4 md:grid-cols-2" onSubmit={onSubmit}>
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
                        <label className="mb-1 block text-sm font-medium text-slate-700">Monto</label>
                        <input
                            type="number"
                            min="0"
                            step="0.01"
                            value={values.monto}
                            onChange={(event) => onChange('monto', event.target.value)}
                            placeholder="50000"
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                    </div>

                    <div className="md:col-span-2">
                        <label className="mb-1 block text-sm font-medium text-slate-700">Concepto</label>
                        <textarea
                            rows={2}
                            value={values.concepto}
                            onChange={(event) => onChange('concepto', event.target.value)}
                            placeholder="Donacion voluntaria en evento"
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Caja</label>
                        <select
                            value={values.cajaId}
                            onChange={(event) => onChange('cajaId', event.target.value)}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        >
                            <option value="">Seleccione...</option>
                            {cajas.map((caja) => (
                                <option key={caja.id} value={caja.id}>
                                    {caja.nombre}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Cuenta contable</label>
                        <select
                            value={values.cuentaContableId}
                            onChange={(event) => onChange('cuentaContableId', event.target.value)}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        >
                            <option value="">Seleccione...</option>
                            {cuentasContables.map((cuenta) => (
                                <option key={cuenta.id} value={cuenta.id}>
                                    {cuenta.nombre}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="md:col-span-2">
                        <label className="mb-1 block text-sm font-medium text-slate-700">Centro de costo</label>
                        <select
                            value={values.centroCostoId}
                            onChange={(event) => onChange('centroCostoId', event.target.value)}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        >
                            <option value="">Seleccione...</option>
                            {centrosCosto.map((centro) => (
                                <option key={centro.id} value={centro.id}>
                                    {centro.nombre}
                                </option>
                            ))}
                        </select>
                    </div>

                    {(validationError || error) ? (
                        <div className="md:col-span-2 rounded-lg border border-rose-200 bg-rose-50 px-3 py-2 text-sm text-rose-700">
                            {validationError ?? error}
                        </div>
                    ) : null}

                    <div className="md:col-span-2 flex items-center justify-end gap-2 pt-1">
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
                            className={`rounded-lg px-4 py-2 text-sm font-medium text-white transition disabled:cursor-not-allowed disabled:opacity-60 ${colorPrincipal}`}
                        >
                            {enviando ? 'Guardando...' : accion}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
