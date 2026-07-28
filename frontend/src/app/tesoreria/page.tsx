'use client';

import TesoreriaMovimientoModal, {
    type TesoreriaCatalogItem,
    type TesoreriaMovimientoFormValues,
} from '@/features/tesoreria/components/TesoreriaMovimientoModal';
import { useGetCajas } from '@/features/tesoreria/hooks/useGetCajas';
import { useGetEgresos } from '@/features/tesoreria/hooks/useGetEgresos';
import { useRegistrarEgreso } from '@/features/tesoreria/hooks/useRegistrarEgreso';
import { useRegistrarIngreso } from '@/features/tesoreria/hooks/useRegistrarIngreso';
import { type RegistrarMovimientoTesoreriaPayload } from '@/features/tesoreria/services/tesoreriaService';
import { useTransacciones } from '@/features/transacciones/hooks/useTransacciones';
import apiClient from '@/lib/apiClient';
import { useQuery } from '@tanstack/react-query';
import { useMemo, useState } from 'react';

type TabLibro = 'consolidado' | 'ingresos' | 'egresos';

type CentroCostoApiDto = {
    id?: string;
    nombre?: string;
};

type CuentaContableApiDto = {
    id?: string;
    codigo?: string;
    descripcion?: string;
    naturaleza?: number;
    permiteMovimiento?: boolean;
};

type CuentaContableMov = {
    id: string;
    codigo: string;
    descripcion: string;
    naturaleza: number;
};

type MovimientoLibro = {
    id: string;
    fecha: string;
    tipo: 'Ingreso' | 'Egreso';
    concepto: string;
    origen: string;
    detalle: string;
    monto: number;
};

function formatCOP(value: number): string {
    return new Intl.NumberFormat('es-CO', {
        style: 'currency',
        currency: 'COP',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
    }).format(value);
}

function formatFecha(value: string): string {
    if (!value) {
        return '-';
    }

    const parsed = new Date(value);
    if (Number.isNaN(parsed.getTime())) {
        return value;
    }

    return new Intl.DateTimeFormat('es-CO', {
        dateStyle: 'medium',
        timeStyle: 'short',
    }).format(parsed);
}

function toMovimientoPayload(values: TesoreriaMovimientoFormValues): RegistrarMovimientoTesoreriaPayload {
    return {
        fecha: new Date(values.fecha).toISOString(),
        monto: Number(values.monto),
        concepto: values.concepto.trim(),
        terceroId: null,
        cuentaContableId: values.cuentaContableId,
        cajaId: values.cajaId,
        centroCostoId: values.centroCostoId,
    };
}

export default function TesoreriaPage() {
    const [modalActivo, setModalActivo] = useState<'ingreso' | 'egreso' | null>(null);
    const [tabActiva, setTabActiva] = useState<TabLibro>('consolidado');
    const [mensajeExito, setMensajeExito] = useState<string | null>(null);

    const cajasQuery = useGetCajas();
    const egresosQuery = useGetEgresos();
    const transaccionesQuery = useTransacciones();
    const registrarIngresoMutation = useRegistrarIngreso();
    const registrarEgresoMutation = useRegistrarEgreso();

    const centrosCostoQuery = useQuery({
        queryKey: ['tesoreria', 'catalogos', 'centros-costo'],
        queryFn: async () => {
            const response = await apiClient.get<CentroCostoApiDto[]>('/api/transacciones/centros-costo');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? ''),
                nombre: String(item?.nombre ?? ''),
            }));
        },
    });

    const cuentasContablesQuery = useQuery({
        queryKey: ['tesoreria', 'catalogos', 'cuentas-contables'],
        queryFn: async () => {
            const response = await apiClient.get<CuentaContableApiDto[]>('/api/cuentas-contables');

            return (response.data ?? [])
                .map((item) => ({
                    id: String(item?.id ?? ''),
                    codigo: String(item?.codigo ?? ''),
                    descripcion: String(item?.descripcion ?? ''),
                    naturaleza: Number(item?.naturaleza ?? 0),
                    permiteMovimiento: Boolean(item?.permiteMovimiento ?? false),
                }))
                .filter((item) => item.permiteMovimiento) as CuentaContableMov[];
        },
    });

    const cajasCatalogo = useMemo<TesoreriaCatalogItem[]>(() => {
        return (cajasQuery.data ?? []).map((caja) => ({ id: caja.id, nombre: caja.nombre }));
    }, [cajasQuery.data]);

    const centrosCostoCatalogo = useMemo<TesoreriaCatalogItem[]>(() => {
        return (centrosCostoQuery.data ?? []).map((centro) => ({ id: centro.id, nombre: centro.nombre }));
    }, [centrosCostoQuery.data]);

    const cuentasContablesCatalogo = useMemo<TesoreriaCatalogItem[]>(() => {
        const cuentas = cuentasContablesQuery.data ?? [];

        if (modalActivo === 'ingreso') {
            return cuentas
                .filter((cuenta) => cuenta.naturaleza === 2)
                .map((cuenta) => ({ id: cuenta.id, nombre: `${cuenta.codigo} - ${cuenta.descripcion}` }));
        }

        if (modalActivo === 'egreso') {
            return cuentas
                .filter((cuenta) => cuenta.naturaleza === 1)
                .map((cuenta) => ({ id: cuenta.id, nombre: `${cuenta.codigo} - ${cuenta.descripcion}` }));
        }

        return cuentas.map((cuenta) => ({ id: cuenta.id, nombre: `${cuenta.codigo} - ${cuenta.descripcion}` }));
    }, [cuentasContablesQuery.data, modalActivo]);

    const ingresosLibro = useMemo<MovimientoLibro[]>(() => {
        const transacciones = transaccionesQuery.data ?? [];

        return transacciones
            .filter((item) => String(item.tipo).trim().toLowerCase() === 'ingreso')
            .map((item) => ({
                id: item.id,
                fecha: item.fecha,
                tipo: 'Ingreso' as const,
                concepto: item.descripcion,
                origen: item.banco || 'Banco no especificado',
                detalle: item.centroCosto || 'Sin centro de costo',
                monto: Number(item.montoCOP ?? 0),
            }));
    }, [transaccionesQuery.data]);

    const egresosLibro = useMemo<MovimientoLibro[]>(() => {
        const egresos = egresosQuery.data ?? [];

        return egresos.map((item) => ({
            id: item.id,
            fecha: item.fecha,
            tipo: 'Egreso' as const,
            concepto: item.concepto,
            origen: item.cajaNombre || 'Caja no especificada',
            detalle: item.cuentaContableNombre || 'Cuenta contable no especificada',
            monto: Number(item.monto ?? 0),
        }));
    }, [egresosQuery.data]);

    const movimientosConsolidados = useMemo<MovimientoLibro[]>(() => {
        return [...ingresosLibro, ...egresosLibro].sort((a, b) => {
            return new Date(b.fecha).getTime() - new Date(a.fecha).getTime();
        });
    }, [ingresosLibro, egresosLibro]);

    const movimientosTabla = useMemo<MovimientoLibro[]>(() => {
        if (tabActiva === 'ingresos') {
            return ingresosLibro;
        }

        if (tabActiva === 'egresos') {
            return egresosLibro;
        }

        return movimientosConsolidados;
    }, [tabActiva, ingresosLibro, egresosLibro, movimientosConsolidados]);

    const totalSaldo = (cajasQuery.data ?? []).reduce((sum, caja) => sum + caja.saldoActual, 0);

    const onEnviarMovimiento = async (values: TesoreriaMovimientoFormValues) => {
        const payload = toMovimientoPayload(values);

        if (modalActivo === 'egreso') {
            await registrarEgresoMutation.mutateAsync(payload);
            setMensajeExito('Egreso registrado y libro actualizado.');
            setTabActiva('egresos');
        } else {
            await registrarIngresoMutation.mutateAsync(payload);
            setMensajeExito('Ingreso registrado y libro actualizado.');
            setTabActiva('ingresos');
        }

        setModalActivo(null);
    };

    const errorMovimiento = modalActivo === 'egreso'
        ? registrarEgresoMutation.error?.message ?? null
        : registrarIngresoMutation.error?.message ?? null;

    const enviandoMovimiento = modalActivo === 'egreso'
        ? registrarEgresoMutation.isPending
        : registrarIngresoMutation.isPending;

    return (
        <main className="min-h-screen bg-slate-50 px-6 py-8">
            <div className="mx-auto w-full max-w-7xl space-y-6">
                <header className="rounded-2xl border border-slate-200 bg-white px-5 py-4 shadow-sm">
                    <h1 className="text-2xl font-semibold text-slate-900">Tesoreria Operativa</h1>
                    <p className="mt-1 text-sm text-slate-600">
                        Gestion de saldos por caja y libro mayor operativo de ingresos y egresos.
                    </p>
                </header>

                {mensajeExito ? (
                    <div className="rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-700">
                        {mensajeExito}
                    </div>
                ) : null}

                <section className="grid gap-4 xl:grid-cols-[1.15fr_1fr]">
                    <div className="space-y-4">
                        <article className="rounded-xl border border-emerald-200 bg-emerald-50 p-5">
                            <p className="text-xs font-semibold uppercase tracking-wide text-emerald-700">Saldo total consolidado</p>
                            <p className="mt-2 text-3xl font-bold text-emerald-900">{formatCOP(totalSaldo)}</p>
                        </article>

                        {cajasQuery.isLoading ? (
                            <div className="rounded-xl border border-slate-200 bg-white px-4 py-6 text-sm text-slate-600">
                                Cargando cajas de tesoreria...
                            </div>
                        ) : null}

                        {cajasQuery.isError ? (
                            <div className="rounded-xl border border-rose-200 bg-rose-50 px-4 py-6 text-sm text-rose-700">
                                No fue posible cargar el resumen de cajas.
                            </div>
                        ) : null}

                        {!cajasQuery.isLoading && !cajasQuery.isError ? (
                            <div className="grid gap-3 sm:grid-cols-2">
                                {(cajasQuery.data ?? []).map((caja) => (
                                    <article key={caja.id} className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
                                        <p className="text-sm font-semibold text-slate-900">{caja.nombre}</p>
                                        <p className="mt-1 text-xs text-slate-500">{caja.cuentaContable || 'Sin cuenta contable asociada'}</p>
                                        <p className="mt-3 text-2xl font-bold text-slate-900">{formatCOP(caja.saldoActual)}</p>
                                    </article>
                                ))}
                            </div>
                        ) : null}
                    </div>

                    <aside className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
                        <h2 className="text-sm font-semibold uppercase tracking-wide text-slate-700">Acciones Contextuales</h2>
                        <p className="mt-1 text-xs text-slate-500">
                            Registra movimientos sin salir del contexto de saldos y libro.
                        </p>

                        <div className="mt-4 grid gap-3">
                            <button
                                type="button"
                                onClick={() => {
                                    setMensajeExito(null);
                                    setModalActivo('ingreso');
                                }}
                                className="rounded-xl bg-emerald-700 px-4 py-3 text-sm font-semibold text-white transition hover:bg-emerald-800"
                            >
                                Registrar Ingreso
                            </button>

                            <button
                                type="button"
                                onClick={() => {
                                    setMensajeExito(null);
                                    setModalActivo('egreso');
                                }}
                                className="rounded-xl bg-rose-700 px-4 py-3 text-sm font-semibold text-white transition hover:bg-rose-800"
                            >
                                Registrar Egreso
                            </button>
                        </div>
                    </aside>
                </section>

                <section className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
                    <div className="flex flex-wrap items-center justify-between gap-3 border-b border-slate-200 pb-3">
                        <div>
                            <h2 className="text-base font-semibold text-slate-900">Libro de Movimientos</h2>
                            <p className="text-sm text-slate-600">Seguimiento operacional de ingresos y egresos.</p>
                        </div>

                        <div className="flex items-center gap-2 rounded-lg bg-slate-100 p-1">
                            {([
                                { key: 'consolidado', label: 'Consolidado' },
                                { key: 'ingresos', label: 'Ingresos' },
                                { key: 'egresos', label: 'Egresos' },
                            ] as const).map((tab) => (
                                <button
                                    key={tab.key}
                                    type="button"
                                    onClick={() => setTabActiva(tab.key)}
                                    className={`rounded-md px-3 py-1.5 text-sm font-medium transition ${tabActiva === tab.key
                                            ? 'bg-white text-slate-900 shadow-sm'
                                            : 'text-slate-600 hover:text-slate-900'
                                        }`}
                                >
                                    {tab.label}
                                </button>
                            ))}
                        </div>
                    </div>

                    <div className="mt-4 overflow-x-auto">
                        <table className="min-w-full divide-y divide-slate-200">
                            <thead className="bg-slate-50">
                                <tr>
                                    <th className="px-3 py-2 text-left text-xs font-semibold uppercase tracking-wide text-slate-600">Fecha</th>
                                    <th className="px-3 py-2 text-left text-xs font-semibold uppercase tracking-wide text-slate-600">Tipo</th>
                                    <th className="px-3 py-2 text-left text-xs font-semibold uppercase tracking-wide text-slate-600">Concepto</th>
                                    <th className="px-3 py-2 text-left text-xs font-semibold uppercase tracking-wide text-slate-600">Origen</th>
                                    <th className="px-3 py-2 text-left text-xs font-semibold uppercase tracking-wide text-slate-600">Detalle</th>
                                    <th className="px-3 py-2 text-right text-xs font-semibold uppercase tracking-wide text-slate-600">Monto</th>
                                </tr>
                            </thead>
                            <tbody className="divide-y divide-slate-100 bg-white">
                                {movimientosTabla.map((mov) => (
                                    <tr key={`${mov.tipo}-${mov.id}`} className="hover:bg-slate-50">
                                        <td className="whitespace-nowrap px-3 py-2 text-sm text-slate-700">{formatFecha(mov.fecha)}</td>
                                        <td className="whitespace-nowrap px-3 py-2 text-sm">
                                            <span className={`rounded-full px-2 py-1 text-xs font-semibold ${mov.tipo === 'Ingreso' ? 'bg-emerald-100 text-emerald-700' : 'bg-rose-100 text-rose-700'}`}>
                                                {mov.tipo}
                                            </span>
                                        </td>
                                        <td className="px-3 py-2 text-sm text-slate-900">{mov.concepto || '-'}</td>
                                        <td className="px-3 py-2 text-sm text-slate-700">{mov.origen || '-'}</td>
                                        <td className="px-3 py-2 text-sm text-slate-700">{mov.detalle || '-'}</td>
                                        <td className={`whitespace-nowrap px-3 py-2 text-right text-sm font-semibold ${mov.tipo === 'Ingreso' ? 'text-emerald-700' : 'text-rose-700'}`}>
                                            {mov.tipo === 'Ingreso' ? '+' : '-'} {formatCOP(mov.monto)}
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>

                        {!egresosQuery.isLoading && !transaccionesQuery.isLoading && movimientosTabla.length === 0 ? (
                            <div className="px-3 py-6 text-center text-sm text-slate-600">
                                No hay movimientos para el filtro seleccionado.
                            </div>
                        ) : null}
                    </div>
                </section>
            </div>

            <TesoreriaMovimientoModal
                modo={modalActivo === 'egreso' ? 'egreso' : 'ingreso'}
                abierto={modalActivo !== null}
                cajas={cajasCatalogo}
                cuentasContables={cuentasContablesCatalogo}
                centrosCosto={centrosCostoCatalogo}
                enviando={enviandoMovimiento}
                error={errorMovimiento}
                onCerrar={() => setModalActivo(null)}
                onEnviar={onEnviarMovimiento}
            />
        </main>
    );
}
