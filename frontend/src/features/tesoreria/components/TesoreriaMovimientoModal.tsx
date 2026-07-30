'use client';

import { MEDIOS_PAGO, MEDIO_PAGO_POR_DEFECTO } from '@/lib/mediosPago';
import { useState } from 'react';

export type TesoreriaCatalogItem = {
    id: string;
    nombre: string;
};

export type TesoreriaMovimientoFormValues = {
    fecha: string;
    monto: string;
    concepto: string;
    bancoId: string;
    medioPago: string;
    cuentaContableId: string;
    centroCostoId: string;
};

type TesoreriaMovimientoModalProps = {
    modo: 'ingreso' | 'egreso';
    cuentasBancarias: TesoreriaCatalogItem[];
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

function buildInitialValues(): TesoreriaMovimientoFormValues {
    return {
        fecha: getFechaActual(),
        monto: '',
        concepto: '',
        bancoId: '',
        medioPago: String(MEDIO_PAGO_POR_DEFECTO),
        cuentaContableId: '',
        centroCostoId: '',
    };
}

/**
 * Valor efectivo de un desplegable alimentado por catalogo.
 *
 * Los catalogos llegan por red y pueden hacerlo despues de abrir el modal, o
 * refrescarse mientras esta abierto. Antes la preseleccion se guardaba en el
 * estado desde un efecto que dependia de los tres catalogos: cada refresco lo
 * volvia a ejecutar y borraba lo que el usuario llevara escrito. Resolviendola
 * en el render, la preseleccion aparece cuando el catalogo carga y la eleccion
 * del usuario, una vez hecha, ya no se pisa.
 */
function valorEfectivo(seleccionado: string, catalogo: TesoreriaCatalogItem[]): string {
    return seleccionado || catalogo[0]?.id || '';
}

export default function TesoreriaMovimientoModal({
    modo,
    cuentasBancarias,
    cuentasContables,
    centrosCosto,
    enviando,
    error,
    onCerrar,
    onEnviar,
}: TesoreriaMovimientoModalProps) {
    const [values, setValues] = useState<TesoreriaMovimientoFormValues>(buildInitialValues);
    const [validationError, setValidationError] = useState<string | null>(null);

    // Lo que se muestra y lo que se envia: el estado manda, y el catalogo solo
    // aporta la preseleccion mientras el usuario no haya elegido.
    const valoresEfectivos: TesoreriaMovimientoFormValues = {
        ...values,
        bancoId: valorEfectivo(values.bancoId, cuentasBancarias),
        cuentaContableId: valorEfectivo(values.cuentaContableId, cuentasContables),
        centroCostoId: valorEfectivo(values.centroCostoId, centrosCosto),
    };

    const titulo = modo === 'ingreso' ? 'Registrar Ingreso' : 'Registrar Egreso';
    const accion = modo === 'ingreso' ? 'Registrar ingreso' : 'Registrar egreso';
    const colorPrincipal = modo === 'ingreso' ? 'bg-emerald-700 hover:bg-emerald-800' : 'bg-rose-700 hover:bg-rose-800';

    const onChange = (field: keyof TesoreriaMovimientoFormValues, value: string) => {
        setValues((previous) => ({ ...previous, [field]: value }));
    };

    const onSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        const v = valoresEfectivos;

        if (!v.fecha || !v.monto || !v.concepto.trim() || !v.bancoId || !v.cuentaContableId || !v.centroCostoId || !v.medioPago) {
            setValidationError('Todos los campos son obligatorios para registrar el movimiento.');
            return;
        }

        const montoNumero = Number(v.monto);
        if (!Number.isFinite(montoNumero) || montoNumero <= 0) {
            setValidationError('El monto debe ser un valor numérico mayor a cero.');
            return;
        }

        setValidationError(null);
        await onEnviar(v);
    };

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/55 p-4">
            <div className="w-full max-w-2xl rounded-2xl border border-slate-200 bg-white p-6 shadow-2xl">
                <div className="flex items-start justify-between gap-4">
                    <div>
                        <h2 className="text-xl font-bold text-slate-900">{titulo}</h2>
                        <p className="mt-1 text-sm text-slate-600">Movimientos en cuentaBancaria con comprobante y partida doble.</p>
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
                        <label className="mb-1 block text-sm font-medium text-slate-700">Cuenta bancaria</label>
                        <select
                            value={valoresEfectivos.bancoId}
                            onChange={(event) => onChange('bancoId', event.target.value)}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        >
                            <option value="">Seleccione...</option>
                            {cuentasBancarias.map((cuentaBancaria) => (
                                <option key={cuentaBancaria.id} value={cuentaBancaria.id}>
                                    {cuentaBancaria.nombre}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Cuenta contable</label>
                        <select
                            value={valoresEfectivos.cuentaContableId}
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

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Medio de pago</label>
                        <select
                            value={values.medioPago}
                            onChange={(event) => onChange('medioPago', event.target.value)}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        >
                            {MEDIOS_PAGO.map((medio) => (
                                <option key={medio.value} value={medio.value}>
                                    {medio.label}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="md:col-span-2">
                        <label className="mb-1 block text-sm font-medium text-slate-700">Centro de costo</label>
                        <select
                            value={valoresEfectivos.centroCostoId}
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
