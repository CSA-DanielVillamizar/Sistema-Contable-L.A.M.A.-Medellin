'use client';

import { useCuentasContables } from '@/features/contabilidad/hooks/useCuentasContables';
import {
    getBalancePrueba,
    getLibroDiario,
    getLibroMayor,
} from '@/features/contabilidad/services/librosService';
import { useRoleAccess } from '@/lib/useRoleAccess';
import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';

/**
 * Libros contables (historia 1-4).
 *
 * El calculo llevaba tiempo hecho y verificado contra SQL Server, pero no habia
 * pantalla: los libros solo se obtenian llamando al API a mano. La historia
 * pide que el contador pueda generarlos, y eso exige interfaz.
 */
const ROLES_PERMITIDOS = ['Contador', 'Admin', 'Tesorero', 'Operador', 'Junta'] as const;

type Libro = 'diario' | 'mayor' | 'balance';

const formatoCOP = new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0,
});

function primerDiaDelMes(): string {
    const hoy = new Date();
    return new Date(hoy.getFullYear(), hoy.getMonth(), 1).toISOString().slice(0, 10);
}

function hoyISO(): string {
    return new Date().toISOString().slice(0, 10);
}

export default function LibrosContablesPage() {
    const { canAccess, isRoleReady } = useRoleAccess(ROLES_PERMITIDOS);
    const cuentasQuery = useCuentasContables();

    const [libro, setLibro] = useState<Libro>('diario');
    const [desde, setDesde] = useState(primerDiaDelMes);
    const [hasta, setHasta] = useState(hoyISO);
    const [cuentaId, setCuentaId] = useState('');
    const [anio, setAnio] = useState(new Date().getFullYear());
    const [mes, setMes] = useState(new Date().getMonth() + 1);
    const [consulta, setConsulta] = useState(0);

    const diarioQuery = useQuery({
        queryKey: ['libros', 'diario', desde, hasta, consulta],
        queryFn: () => getLibroDiario(desde, hasta),
        enabled: libro === 'diario' && consulta > 0,
    });

    const mayorQuery = useQuery({
        queryKey: ['libros', 'mayor', cuentaId, desde, hasta, consulta],
        queryFn: () => getLibroMayor(cuentaId, desde, hasta),
        enabled: libro === 'mayor' && consulta > 0 && cuentaId.length > 0,
    });

    const balanceQuery = useQuery({
        queryKey: ['libros', 'balance', anio, mes, consulta],
        queryFn: () => getBalancePrueba(anio, mes),
        enabled: libro === 'balance' && consulta > 0,
    });

    if (!isRoleReady) {
        return <p className="p-8 text-sm text-slate-500">Verificando permisos...</p>;
    }

    if (!canAccess) {
        return (
            <div className="p-8">
                <h1 className="text-xl font-bold text-slate-900">Sin permiso</h1>
                <p className="mt-2 text-sm text-slate-600">Los libros contables requieren un rol con acceso a contabilidad.</p>
            </div>
        );
    }

    const cuentasAsentables = (cuentasQuery.data ?? []).filter((c) => c.permiteMovimiento);
    const activa = libro === 'diario' ? diarioQuery : libro === 'mayor' ? mayorQuery : balanceQuery;

    const claseTab = (valor: Libro) =>
        `rounded-lg px-4 py-2 text-sm font-medium transition ${
            libro === valor ? 'bg-slate-900 text-white' : 'border border-slate-300 text-slate-700 hover:bg-slate-50'
        }`;

    return (
        <div className="p-8">
            <h1 className="text-2xl font-bold text-slate-900">Libros contables</h1>
            <p className="mt-1 text-sm text-slate-600">
                Diario, mayor y balance de prueba. El balance avisa si el libro no cuadra.
            </p>

            <div className="mt-5 flex flex-wrap gap-2">
                <button type="button" onClick={() => setLibro('diario')} className={claseTab('diario')}>
                    Libro diario
                </button>
                <button type="button" onClick={() => setLibro('mayor')} className={claseTab('mayor')}>
                    Libro mayor
                </button>
                <button type="button" onClick={() => setLibro('balance')} className={claseTab('balance')}>
                    Balance de prueba
                </button>
            </div>

            <div className="mt-5 flex flex-wrap items-end gap-4 rounded-xl border border-slate-200 bg-white p-4">
                {libro === 'balance' ? (
                    <>
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Año</label>
                            <input
                                type="number"
                                value={anio}
                                onChange={(e) => setAnio(Number(e.target.value))}
                                className="rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                            />
                        </div>
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Mes</label>
                            <input
                                type="number"
                                min="1"
                                max="12"
                                value={mes}
                                onChange={(e) => setMes(Number(e.target.value))}
                                className="w-24 rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                            />
                        </div>
                    </>
                ) : (
                    <>
                        {libro === 'mayor' && (
                            <div className="min-w-72">
                                <label className="mb-1 block text-sm font-medium text-slate-700">Cuenta</label>
                                <select
                                    value={cuentaId}
                                    onChange={(e) => setCuentaId(e.target.value)}
                                    className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                                >
                                    <option value="">Seleccione una cuenta...</option>
                                    {cuentasAsentables.map((c) => (
                                        <option key={c.id} value={c.id}>
                                            {c.codigo} - {c.descripcion}
                                        </option>
                                    ))}
                                </select>
                            </div>
                        )}
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Desde</label>
                            <input
                                type="date"
                                value={desde}
                                onChange={(e) => setDesde(e.target.value)}
                                className="rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                            />
                        </div>
                        <div>
                            <label className="mb-1 block text-sm font-medium text-slate-700">Hasta</label>
                            <input
                                type="date"
                                value={hasta}
                                onChange={(e) => setHasta(e.target.value)}
                                className="rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                            />
                        </div>
                    </>
                )}

                <button
                    type="button"
                    onClick={() => setConsulta((n) => n + 1)}
                    disabled={libro === 'mayor' && !cuentaId}
                    className="rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-60"
                >
                    Generar
                </button>
            </div>

            {activa.isLoading ? <p className="mt-4 text-sm text-slate-500">Generando...</p> : null}
            {activa.isError ? (
                <div className="mt-4 rounded-lg border border-rose-200 bg-rose-50 px-3 py-2 text-sm text-rose-700">
                    No fue posible generar el libro.
                </div>
            ) : null}

            {libro === 'diario' && diarioQuery.data ? (
                <Resultado
                    cuadrado={diarioQuery.data.estaCuadrado}
                    debe={diarioQuery.data.totalDebe}
                    haber={diarioQuery.data.totalHaber}
                    columnas={['Fecha', 'Comprobante', 'Cuenta', 'Centro de costo', 'Referencia', 'Debe', 'Haber']}
                    filas={diarioQuery.data.movimientos.map((m, i) => (
                        <tr key={`${m.numeroConsecutivo}-${i}`} className="border-b border-slate-100 last:border-0">
                            <td className="px-3 py-2 text-slate-600">{m.fecha.slice(0, 10)}</td>
                            <td className="px-3 py-2 text-slate-900">{m.numeroConsecutivo}</td>
                            <td className="px-3 py-2 text-slate-600">{m.codigoCuenta} {m.descripcionCuenta}</td>
                            <td className="px-3 py-2 text-slate-600">{m.centroCosto}</td>
                            <td className="px-3 py-2 text-slate-600">{m.referencia}</td>
                            <td className="px-3 py-2 text-right tabular-nums">{m.debe ? formatoCOP.format(m.debe) : ''}</td>
                            <td className="px-3 py-2 text-right tabular-nums">{m.haber ? formatoCOP.format(m.haber) : ''}</td>
                        </tr>
                    ))}
                />
            ) : null}

            {libro === 'mayor' && mayorQuery.data ? (
                <>
                    <p className="mt-5 text-sm font-medium text-slate-700">
                        {mayorQuery.data.codigoCuenta} {mayorQuery.data.descripcionCuenta} · saldo anterior{' '}
                        {formatoCOP.format(mayorQuery.data.saldoAnterior)} · saldo final{' '}
                        {formatoCOP.format(mayorQuery.data.saldoFinal)}
                    </p>
                    <Resultado
                        debe={mayorQuery.data.totalDebe}
                        haber={mayorQuery.data.totalHaber}
                        columnas={['Fecha', 'Comprobante', 'Centro de costo', 'Referencia', 'Debe', 'Haber', 'Saldo']}
                        filas={mayorQuery.data.movimientos.map((m, i) => (
                            <tr key={`${m.numeroConsecutivo}-${i}`} className="border-b border-slate-100 last:border-0">
                                <td className="px-3 py-2 text-slate-600">{m.fecha.slice(0, 10)}</td>
                                <td className="px-3 py-2 text-slate-900">{m.numeroConsecutivo}</td>
                                <td className="px-3 py-2 text-slate-600">{m.centroCosto}</td>
                                <td className="px-3 py-2 text-slate-600">{m.referencia}</td>
                                <td className="px-3 py-2 text-right tabular-nums">{m.debe ? formatoCOP.format(m.debe) : ''}</td>
                                <td className="px-3 py-2 text-right tabular-nums">{m.haber ? formatoCOP.format(m.haber) : ''}</td>
                                <td className="px-3 py-2 text-right tabular-nums font-medium">{formatoCOP.format(m.saldoAcumulado)}</td>
                            </tr>
                        ))}
                    />
                </>
            ) : null}

            {libro === 'balance' && balanceQuery.data ? (
                <Resultado
                    cuadrado={balanceQuery.data.estaCuadrado}
                    debe={balanceQuery.data.totalDebe}
                    haber={balanceQuery.data.totalHaber}
                    columnas={['Código', 'Cuenta', 'Saldo anterior', 'Debe', 'Haber', 'Saldo final']}
                    filas={balanceQuery.data.cuentas.map((c) => (
                        <tr key={c.cuentaContableId} className="border-b border-slate-100 last:border-0">
                            <td className="px-3 py-2 font-medium text-slate-900">{c.codigoCuenta}</td>
                            <td className="px-3 py-2 text-slate-600">{c.descripcionCuenta}</td>
                            <td className="px-3 py-2 text-right tabular-nums">{formatoCOP.format(c.saldoAnterior)}</td>
                            <td className="px-3 py-2 text-right tabular-nums">{formatoCOP.format(c.debe)}</td>
                            <td className="px-3 py-2 text-right tabular-nums">{formatoCOP.format(c.haber)}</td>
                            <td className="px-3 py-2 text-right tabular-nums font-medium">{formatoCOP.format(c.saldoFinal)}</td>
                        </tr>
                    ))}
                />
            ) : null}
        </div>
    );
}

function Resultado({
    cuadrado,
    debe,
    haber,
    columnas,
    filas,
}: {
    cuadrado?: boolean;
    debe: number;
    haber: number;
    columnas: string[];
    filas: React.ReactNode[];
}) {
    return (
        <div className="mt-4">
            <div className="flex flex-wrap items-center gap-4 text-sm">
                <span className="text-slate-600">
                    Debe <strong className="tabular-nums text-slate-900">{formatoCOP.format(debe)}</strong>
                </span>
                <span className="text-slate-600">
                    Haber <strong className="tabular-nums text-slate-900">{formatoCOP.format(haber)}</strong>
                </span>
                {cuadrado !== undefined ? (
                    <span
                        className={
                            cuadrado
                                ? 'rounded-full bg-emerald-100 px-2.5 py-0.5 text-xs font-medium text-emerald-800'
                                : 'rounded-full bg-rose-100 px-2.5 py-0.5 text-xs font-medium text-rose-800'
                        }
                    >
                        {cuadrado ? 'Cuadrado' : 'Descuadrado: revisar antes de cerrar'}
                    </span>
                ) : null}
            </div>

            <div className="mt-3 overflow-x-auto rounded-xl border border-slate-200 bg-white">
                <table className="w-full text-sm">
                    <thead className="border-b border-slate-200 bg-slate-50 text-left text-slate-600">
                        <tr>
                            {columnas.map((c) => (
                                <th
                                    key={c}
                                    className={`px-3 py-2 font-medium ${
                                        ['Debe', 'Haber', 'Saldo', 'Saldo anterior', 'Saldo final'].includes(c)
                                            ? 'text-right'
                                            : ''
                                    }`}
                                >
                                    {c}
                                </th>
                            ))}
                        </tr>
                    </thead>
                    <tbody>
                        {filas.length > 0 ? (
                            filas
                        ) : (
                            <tr>
                                <td colSpan={columnas.length} className="px-3 py-6 text-center text-slate-500">
                                    Sin movimientos en el rango consultado.
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
