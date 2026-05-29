'use client';

import { useRegistrarPago } from '@/features/cartera/hooks/useRegistrarPago';
import { registrarPagoCarteraSchema } from '@/features/cartera/schemas/carteraSchemas';
import type { CuentaPorCobrarItem } from '@/features/cartera/services/carteraService';
import { useGetCajas } from '@/features/tesoreria/hooks/useGetCajas';
import { useEffect, useMemo, useState } from 'react';

type ListaCuentasPorCobrarProps = {
    cuentas: CuentaPorCobrarItem[];
    isLoading: boolean;
    error: Error | null;
};

function formatCOP(value: number): string {
    return new Intl.NumberFormat('es-CO', {
        style: 'currency',
        currency: 'COP',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
    }).format(value);
}

function formatDate(value: string): string {
    if (!value) {
        return '-';
    }

    const date = new Date(`${value}T00:00:00`);
    if (Number.isNaN(date.getTime())) {
        return value;
    }

    return new Intl.DateTimeFormat('es-CO').format(date);
}

function estadoLabel(estado: number): string {
    switch (estado) {
        case 1:
            return 'Pendiente';
        case 2:
            return 'Pagada Parcial';
        case 3:
            return 'Pagada';
        case 4:
            return 'Anulada';
        default:
            return 'Desconocido';
    }
}

export default function ListaCuentasPorCobrar({ cuentas, isLoading, error }: ListaCuentasPorCobrarProps) {
    const registrarPagoMutation = useRegistrarPago();
    const cajasQuery = useGetCajas();
    const [cuentaActivaId, setCuentaActivaId] = useState<string | null>(null);
    const [montoPago, setMontoPago] = useState<string>('');
    const [cajaId, setCajaId] = useState<string>('');
    const [errorMonto, setErrorMonto] = useState<string | null>(null);

    const cuentaActiva = useMemo(
        () => cuentas.find((cuenta) => cuenta.id === cuentaActivaId) ?? null,
        [cuentaActivaId, cuentas],
    );

    const seleccionarCuenta = (cuenta: CuentaPorCobrarItem) => {
        setCuentaActivaId(cuenta.id);
        setMontoPago(String(cuenta.saldoPendiente));
        setCajaId(cajasQuery.data?.[0]?.id ?? '');
        setErrorMonto(null);
    };

    const cerrarPago = () => {
        setCuentaActivaId(null);
        setMontoPago('');
        setCajaId('');
        setErrorMonto(null);
    };

    useEffect(() => {
        if (!cuentaActivaId || cuentas.length === 0) {
            return;
        }

        const sigueVisible = cuentas.some((cuenta) => cuenta.id === cuentaActivaId);
        if (!sigueVisible) {
            cerrarPago();
        }
    }, [cuentaActivaId, cuentas]);

    const onSubmitPago = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        if (!cuentaActiva) {
            return;
        }

        const parsed = registrarPagoCarteraSchema.safeParse({ monto: montoPago, cajaId });
        if (!parsed.success) {
            setErrorMonto(parsed.error.issues[0]?.message ?? 'Monto invalido.');
            return;
        }

        setErrorMonto(null);

        await registrarPagoMutation.mutateAsync({
            cuentaPorCobrarId: cuentaActiva.id,
            monto: parsed.data.monto,
            cajaId: parsed.data.cajaId,
        });

        cerrarPago();
    };

    if (isLoading) {
        return <p className="text-sm text-slate-600">Cargando cuentas por cobrar...</p>;
    }

    if (error) {
        return (
            <div className="rounded-lg border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
                Error al cargar cuentas por cobrar: {error.message}
            </div>
        );
    }

    if (cuentas.length === 0) {
        return (
            <div className="rounded-lg border border-slate-200 bg-slate-50 px-4 py-6 text-center text-sm text-slate-600">
                No hay cuentas por cobrar para los filtros seleccionados.
            </div>
        );
    }

    return (
        <div className="grid gap-4 lg:grid-cols-[1.35fr_1fr]">
            <section className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
                <header className="border-b border-slate-200 bg-slate-50 px-4 py-3">
                    <h2 className="text-sm font-semibold uppercase tracking-wide text-slate-700">
                        Cuentas por cobrar
                    </h2>
                    <p className="mt-1 text-xs text-slate-500">
                        Selecciona una deuda para ver su detalle y registrar pago en contexto.
                    </p>
                </header>

                <ul className="max-h-[70vh] divide-y divide-slate-100 overflow-y-auto">
                    {cuentas.map((cuenta) => {
                        const isSelected = cuentaActivaId === cuenta.id;
                        const puedePagar = cuenta.estado === 1 || cuenta.estado === 2;

                        return (
                            <li key={cuenta.id}>
                                <button
                                    type="button"
                                    onClick={() => seleccionarCuenta(cuenta)}
                                    className={`w-full px-4 py-3 text-left transition ${isSelected ? 'bg-amber-50' : 'hover:bg-slate-50'}`}
                                >
                                    <div className="flex items-center justify-between gap-4">
                                        <div>
                                            <p className="text-sm font-semibold text-slate-900">{cuenta.nombreCompletoMiembro}</p>
                                            <p className="text-xs text-slate-600">{cuenta.nombreConcepto}</p>
                                        </div>
                                        <p className="text-sm font-semibold text-slate-900">{formatCOP(cuenta.saldoPendiente)}</p>
                                    </div>
                                    <div className="mt-2 flex items-center justify-between text-xs text-slate-600">
                                        <span>Vence: {formatDate(cuenta.fechaVencimiento)}</span>
                                        <span className={`${puedePagar ? 'text-emerald-700' : 'text-slate-500'}`}>
                                            {estadoLabel(cuenta.estado)}
                                        </span>
                                    </div>
                                </button>
                            </li>
                        );
                    })}
                </ul>
            </section>

            <section className="min-h-[70vh] rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
                {cuentaActiva ? (
                    <>
                        <header className="mb-4 border-b border-slate-200 pb-4">
                            <h3 className="text-lg font-semibold text-slate-900">Detalle de la deuda</h3>
                            <p className="text-sm text-slate-600">{cuentaActiva.nombreCompletoMiembro}</p>
                        </header>

                        <div className="mb-5 grid grid-cols-1 gap-3 rounded-lg bg-slate-50 p-3 text-sm text-slate-700 sm:grid-cols-2">
                            <p><span className="font-medium text-slate-900">Concepto:</span> {cuentaActiva.nombreConcepto}</p>
                            <p><span className="font-medium text-slate-900">Estado:</span> {estadoLabel(cuentaActiva.estado)}</p>
                            <p><span className="font-medium text-slate-900">Emision:</span> {formatDate(cuentaActiva.fechaEmision)}</p>
                            <p><span className="font-medium text-slate-900">Vencimiento:</span> {formatDate(cuentaActiva.fechaVencimiento)}</p>
                            <p><span className="font-medium text-slate-900">Valor total:</span> {formatCOP(cuentaActiva.valorTotal)}</p>
                            <p><span className="font-medium text-slate-900">Saldo pendiente:</span> {formatCOP(cuentaActiva.saldoPendiente)}</p>
                        </div>

                        {cuentaActiva.estado === 1 || cuentaActiva.estado === 2 ? (
                            <form className="space-y-3" onSubmit={onSubmitPago}>
                                <h4 className="text-sm font-semibold uppercase tracking-wide text-slate-700">Registrar pago</h4>

                                <div>
                                    <label htmlFor="caja-destino" className="mb-1 block text-sm font-medium text-slate-700">
                                        Caja Destino
                                    </label>
                                    <select
                                        id="caja-destino"
                                        className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm text-slate-900"
                                        value={cajaId}
                                        onChange={(e) => setCajaId(e.target.value)}
                                        disabled={cajasQuery.isLoading || (cajasQuery.data?.length ?? 0) === 0}
                                    >
                                        <option value="">Seleccione...</option>
                                        {(cajasQuery.data ?? []).map((caja) => (
                                            <option key={caja.id} value={caja.id}>
                                                {caja.nombre}
                                            </option>
                                        ))}
                                    </select>
                                </div>

                                <div>
                                    <label htmlFor="monto-pago" className="mb-1 block text-sm font-medium text-slate-700">
                                        Monto
                                    </label>
                                    <input
                                        id="monto-pago"
                                        type="number"
                                        step="0.01"
                                        min="0"
                                        className="w-full rounded-md border border-slate-300 px-3 py-2 text-sm text-slate-900"
                                        value={montoPago}
                                        onChange={(e) => setMontoPago(e.target.value)}
                                    />
                                    {errorMonto ? <p className="mt-1 text-xs text-red-600">{errorMonto}</p> : null}
                                </div>

                                {registrarPagoMutation.error ? (
                                    <p className="rounded-md border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
                                        {registrarPagoMutation.error.message}
                                    </p>
                                ) : null}

                                <div className="flex justify-end gap-2 pt-1">
                                    <button
                                        type="button"
                                        onClick={cerrarPago}
                                        className="rounded-md border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700"
                                    >
                                        Limpiar seleccion
                                    </button>
                                    <button
                                        type="submit"
                                        disabled={registrarPagoMutation.isPending}
                                        className="rounded-md bg-emerald-700 px-3 py-2 text-sm font-semibold text-white disabled:opacity-60"
                                    >
                                        {registrarPagoMutation.isPending ? 'Registrando...' : 'Confirmar pago'}
                                    </button>
                                </div>
                            </form>
                        ) : (
                            <p className="rounded-md border border-slate-200 bg-slate-50 px-3 py-2 text-sm text-slate-600">
                                Esta cuenta no admite nuevos pagos por su estado actual.
                            </p>
                        )}
                    </>
                ) : (
                    <div className="flex h-full min-h-[220px] items-center justify-center rounded-lg border border-dashed border-slate-300 bg-slate-50 p-6 text-center text-sm text-slate-500">
                        Selecciona una cuenta en el panel izquierdo para ver detalle y registrar pago.
                    </div>
                )}
            </section>
        </div>
    );
}
