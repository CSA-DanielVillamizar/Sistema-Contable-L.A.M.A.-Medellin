'use client';

import { useCuentasContables } from '@/features/contabilidad/hooks/useCuentasContables';
import {
    NOMBRE_ESTADO_CXP,
    useAnularCuentaPorPagar,
    useCuentasPorPagar,
    usePagarCuentaPorPagar,
    useRegistrarCuentaPorPagar,
    type CuentaPorPagar,
} from '@/features/cuentasPorPagar/hooks/useCuentasPorPagar';
import { useGetCuentasBancarias } from '@/features/tesoreria/hooks/useGetCuentasBancarias';
import apiClient, { type RespuestaApi } from '@/lib/apiClient';
import { MEDIOS_PAGO, MEDIO_PAGO_POR_DEFECTO } from '@/lib/mediosPago';
import { useRoleAccess } from '@/lib/useRoleAccess';
import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';

/**
 * Cuentas por pagar (historias 1-13 y 1-14).
 *
 * La factura se registra cuando llega y se cruza cuando se paga. Lo vencido
 * aparece marcado, que es lo primero que el tesorero necesita ver.
 */
const ROLES_LECTURA = ['Admin', 'Operador', 'Tesorero', 'Contador', 'Junta'] as const;
const ROLES_ESCRITURA = ['Operador', 'Tesorero', 'Admin'] as const;

const formatoCOP = new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0,
});

const VACIO = {
    nombreProveedor: '',
    nitProveedor: '',
    numeroFactura: '',
    concepto: '',
    cuentaContableGastoId: '',
    centroCostoId: '',
    fechaEmision: new Date().toISOString().slice(0, 10),
    fechaVencimiento: new Date(Date.now() + 30 * 86400000).toISOString().slice(0, 10),
    valorTotal: '',
};

export default function CuentasPorPagarPage() {
    const { canAccess, isRoleReady } = useRoleAccess(ROLES_LECTURA);
    const { canAccess: puedeEscribir } = useRoleAccess(ROLES_ESCRITURA);

    const cuentasQuery = useCuentasPorPagar();
    const cuentasContablesQuery = useCuentasContables();
    const bancosQuery = useGetCuentasBancarias();
    const registrar = useRegistrarCuentaPorPagar();
    const pagar = usePagarCuentaPorPagar();
    const anular = useAnularCuentaPorPagar();

    const centrosCostoQuery = useQuery({
        queryKey: ['transacciones', 'catalogo', 'centros-costo'],
        queryFn: async () => {
            const response = await apiClient.get<RespuestaApi[]>('/api/transacciones/centros-costo');
            return (response.data ?? []).map((c) => ({
                id: String(c?.id ?? ''),
                nombre: String(c?.nombre ?? ''),
            }));
        },
    });

    const [form, setForm] = useState(VACIO);
    const [pagando, setPagando] = useState<CuentaPorPagar | null>(null);
    const [montoPago, setMontoPago] = useState('');
    const [bancoPago, setBancoPago] = useState('');
    const [medioPago, setMedioPago] = useState<number>(MEDIO_PAGO_POR_DEFECTO);
    const [mensaje, setMensaje] = useState<{ tipo: 'ok' | 'error'; texto: string } | null>(null);

    if (!isRoleReady) {
        return <p className="p-8 text-sm text-slate-500">Verificando permisos...</p>;
    }

    if (!canAccess) {
        return (
            <div className="p-8">
                <h1 className="text-xl font-bold text-slate-900">Sin permiso</h1>
                <p className="mt-2 text-sm text-slate-600">Las cuentas por pagar requieren un rol con acceso a tesorería.</p>
            </div>
        );
    }

    // Solo gasto y costo: una factura de proveedor no puede imputarse a un
    // ingreso ni a un activo. El backend impone la misma regla.
    const cuentasGasto = (cuentasContablesQuery.data ?? []).filter(
        (c) => c.permiteMovimiento && (c.codigo.startsWith('5') || c.codigo.startsWith('6')),
    );

    const onRegistrar = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        setMensaje(null);

        const valor = Number(form.valorTotal);
        if (!Number.isFinite(valor) || valor <= 0) {
            setMensaje({ tipo: 'error', texto: 'El valor total debe ser mayor a cero.' });
            return;
        }

        try {
            await registrar.mutateAsync({ ...form, valorTotal: valor });
            setForm(VACIO);
            setMensaje({ tipo: 'ok', texto: 'Factura registrada.' });
        } catch (error) {
            setMensaje({ tipo: 'error', texto: error instanceof Error ? error.message : 'Error al registrar.' });
        }
    };

    const onPagar = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        if (!pagando) return;
        setMensaje(null);

        const monto = Number(montoPago);
        if (!Number.isFinite(monto) || monto <= 0) {
            setMensaje({ tipo: 'error', texto: 'El monto debe ser mayor a cero.' });
            return;
        }

        try {
            await pagar.mutateAsync({ id: pagando.id, monto, bancoId: bancoPago, medioPago });
            setPagando(null);
            setMontoPago('');
            setMensaje({ tipo: 'ok', texto: 'Pago aplicado y egreso registrado.' });
        } catch (error) {
            setMensaje({ tipo: 'error', texto: error instanceof Error ? error.message : 'Error al pagar.' });
        }
    };

    const onAnular = async (cuenta: CuentaPorPagar) => {
        setMensaje(null);
        try {
            await anular.mutateAsync(cuenta.id);
            setMensaje({ tipo: 'ok', texto: `Factura ${cuenta.numeroFactura} anulada.` });
        } catch (error) {
            setMensaje({ tipo: 'error', texto: error instanceof Error ? error.message : 'Error al anular.' });
        }
    };

    const cuentas = cuentasQuery.data ?? [];
    const totalPendiente = cuentas.reduce((s, c) => s + c.saldoPendiente, 0);
    const totalVencido = cuentas.filter((c) => c.estaVencida).reduce((s, c) => s + c.saldoPendiente, 0);

    const campo = (k: keyof typeof VACIO, v: string) => setForm((p) => ({ ...p, [k]: v }));
    const claseInput = 'w-full rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-900';

    return (
        <div className="p-8">
            <h1 className="text-2xl font-bold text-slate-900">Cuentas por pagar</h1>
            <p className="mt-1 text-sm text-slate-600">
                Obligaciones con proveedores. La factura se reconoce cuando llega, no cuando se paga.
            </p>

            <div className="mt-4 flex flex-wrap gap-6 text-sm">
                <span className="text-slate-600">
                    Pendiente <strong className="tabular-nums text-slate-900">{formatoCOP.format(totalPendiente)}</strong>
                </span>
                <span className="text-slate-600">
                    Vencido <strong className="tabular-nums text-rose-700">{formatoCOP.format(totalVencido)}</strong>
                </span>
            </div>

            {mensaje ? (
                <div
                    className={`mt-4 rounded-lg border px-3 py-2 text-sm ${
                        mensaje.tipo === 'ok'
                            ? 'border-emerald-200 bg-emerald-50 text-emerald-800'
                            : 'border-rose-200 bg-rose-50 text-rose-700'
                    }`}
                >
                    {mensaje.texto}
                </div>
            ) : null}

            {puedeEscribir ? (
                <form onSubmit={onRegistrar} className="mt-6 rounded-xl border border-slate-200 bg-white p-5">
                    <h2 className="text-lg font-semibold text-slate-900">Registrar factura</h2>

                    <div className="mt-4 grid grid-cols-1 gap-4 md:grid-cols-3">
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Proveedor</label>
                            <input value={form.nombreProveedor} onChange={(e) => campo('nombreProveedor', e.target.value)} className={claseInput} required />
                        </div>
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">NIT</label>
                            <input value={form.nitProveedor} onChange={(e) => campo('nitProveedor', e.target.value)} className={claseInput} required />
                        </div>
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Número de factura</label>
                            <input value={form.numeroFactura} onChange={(e) => campo('numeroFactura', e.target.value)} className={claseInput} required />
                        </div>
                        <div className="md:col-span-3">
                            <label className="mb-1 block text-sm font-medium text-slate-700">Concepto</label>
                            <input value={form.concepto} onChange={(e) => campo('concepto', e.target.value)} className={claseInput} required />
                        </div>
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Cuenta de gasto</label>
                            <select value={form.cuentaContableGastoId} onChange={(e) => campo('cuentaContableGastoId', e.target.value)} className={claseInput} required>
                                <option value="">Seleccione...</option>
                                {cuentasGasto.map((c) => (
                                    <option key={c.id} value={c.id}>{c.codigo} - {c.descripcion}</option>
                                ))}
                            </select>
                        </div>
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Centro de costo</label>
                            <select value={form.centroCostoId} onChange={(e) => campo('centroCostoId', e.target.value)} className={claseInput} required>
                                <option value="">Seleccione...</option>
                                {(centrosCostoQuery.data ?? []).map((c) => (
                                    <option key={c.id} value={c.id}>{c.nombre}</option>
                                ))}
                            </select>
                        </div>
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Valor total</label>
                            <input type="number" min="0" step="1000" value={form.valorTotal} onChange={(e) => campo('valorTotal', e.target.value)} className={claseInput} required />
                        </div>
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Fecha de emisión</label>
                            <input type="date" value={form.fechaEmision} onChange={(e) => campo('fechaEmision', e.target.value)} className={claseInput} required />
                        </div>
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Vencimiento</label>
                            <input type="date" value={form.fechaVencimiento} onChange={(e) => campo('fechaVencimiento', e.target.value)} className={claseInput} required />
                        </div>
                    </div>

                    <button type="submit" disabled={registrar.isPending} className="mt-4 rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:opacity-60">
                        {registrar.isPending ? 'Registrando...' : 'Registrar factura'}
                    </button>
                </form>
            ) : null}

            <div className="mt-6 overflow-x-auto rounded-xl border border-slate-200 bg-white">
                <table className="w-full text-sm">
                    <thead className="border-b border-slate-200 bg-slate-50 text-left text-slate-600">
                        <tr>
                            <th className="px-4 py-3 font-medium">Proveedor</th>
                            <th className="px-4 py-3 font-medium">Factura</th>
                            <th className="px-4 py-3 font-medium">Vencimiento</th>
                            <th className="px-4 py-3 text-right font-medium">Total</th>
                            <th className="px-4 py-3 text-right font-medium">Saldo</th>
                            <th className="px-4 py-3 font-medium">Estado</th>
                            <th className="px-4 py-3 text-right font-medium">Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        {cuentasQuery.isLoading ? (
                            <tr><td colSpan={7} className="px-4 py-6 text-center text-slate-500">Cargando...</td></tr>
                        ) : null}

                        {!cuentasQuery.isLoading && cuentas.length === 0 ? (
                            <tr><td colSpan={7} className="px-4 py-6 text-center text-slate-500">No hay facturas registradas.</td></tr>
                        ) : null}

                        {cuentas.map((c) => (
                            <tr key={c.id} className="border-b border-slate-100 last:border-0">
                                <td className="px-4 py-3">
                                    <span className="font-medium text-slate-900">{c.nombreProveedor}</span>
                                    <span className="block text-xs text-slate-400">{c.nitProveedor}</span>
                                </td>
                                <td className="px-4 py-3 text-slate-600">
                                    {c.numeroFactura}
                                    <span className="block text-xs text-slate-400">{c.concepto}</span>
                                </td>
                                <td className={`px-4 py-3 ${c.estaVencida ? 'font-medium text-rose-700' : 'text-slate-600'}`}>
                                    {c.fechaVencimiento.slice(0, 10)}
                                    {c.estaVencida ? <span className="block text-xs">Vencida</span> : null}
                                </td>
                                <td className="px-4 py-3 text-right tabular-nums">{formatoCOP.format(c.valorTotal)}</td>
                                <td className="px-4 py-3 text-right tabular-nums font-medium">{formatoCOP.format(c.saldoPendiente)}</td>
                                <td className="px-4 py-3">
                                    <span className="rounded-full bg-slate-100 px-2.5 py-0.5 text-xs font-medium text-slate-700">
                                        {NOMBRE_ESTADO_CXP[c.estado] ?? c.estado}
                                    </span>
                                </td>
                                <td className="px-4 py-3">
                                    {puedeEscribir ? (
                                        <div className="flex items-center justify-end gap-2">
                                            {c.saldoPendiente > 0 && c.estado !== 4 ? (
                                                <button
                                                    type="button"
                                                    onClick={() => {
                                                        setPagando(c);
                                                        setMontoPago(String(c.saldoPendiente));
                                                        setBancoPago(bancosQuery.data?.[0]?.id ?? '');
                                                    }}
                                                    className="rounded-lg bg-slate-900 px-3 py-1.5 text-xs font-medium text-white transition hover:bg-slate-800"
                                                >
                                                    Pagar
                                                </button>
                                            ) : null}
                                            {c.estado === 1 ? (
                                                <button
                                                    type="button"
                                                    onClick={() => void onAnular(c)}
                                                    className="rounded-lg border border-slate-300 px-3 py-1.5 text-xs font-medium text-slate-700 transition hover:bg-slate-50"
                                                >
                                                    Anular
                                                </button>
                                            ) : null}
                                        </div>
                                    ) : null}
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            {pagando ? (
                <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/55 p-4">
                    <form onSubmit={onPagar} className="w-full max-w-md rounded-2xl border border-slate-200 bg-white p-6 shadow-2xl">
                        <h2 className="text-xl font-bold text-slate-900">Pagar factura</h2>
                        <p className="mt-1 text-sm text-slate-600">
                            {pagando.nombreProveedor} · {pagando.numeroFactura} · saldo {formatoCOP.format(pagando.saldoPendiente)}
                        </p>

                        <div className="mt-4 space-y-4">
                            <div>
                                <label className="mb-1 block text-sm font-medium text-slate-700">Monto</label>
                                <input type="number" min="0" step="1000" value={montoPago} onChange={(e) => setMontoPago(e.target.value)} className={claseInput} required />
                            </div>
                            <div>
                                <label className="mb-1 block text-sm font-medium text-slate-700">Cuenta bancaria</label>
                                <select value={bancoPago} onChange={(e) => setBancoPago(e.target.value)} className={claseInput} required>
                                    <option value="">Seleccione...</option>
                                    {(bancosQuery.data ?? []).map((b) => (
                                        <option key={b.id} value={b.id}>{b.nombre}</option>
                                    ))}
                                </select>
                            </div>
                            <div>
                                <label className="mb-1 block text-sm font-medium text-slate-700">Medio de pago</label>
                                <select value={medioPago} onChange={(e) => setMedioPago(Number(e.target.value))} className={claseInput}>
                                    {MEDIOS_PAGO.map((m) => (
                                        <option key={m.value} value={m.value}>{m.label}</option>
                                    ))}
                                </select>
                            </div>
                        </div>

                        <div className="mt-5 flex justify-end gap-2">
                            <button type="button" onClick={() => setPagando(null)} className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700">
                                Cancelar
                            </button>
                            <button type="submit" disabled={pagar.isPending} className="rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white disabled:opacity-60">
                                {pagar.isPending ? 'Aplicando...' : 'Aplicar pago'}
                            </button>
                        </div>
                    </form>
                </div>
            ) : null}
        </div>
    );
}
