'use client';

import { useState } from 'react';
import { useCuentasAsentables, useCuentasContables } from '@/features/contabilidad/hooks/useCuentasContables';

const INDENT: Record<number, string> = {
    1: '',
    2: 'pl-4',
    3: 'pl-8',
    4: 'pl-12',
    5: 'pl-16',
};

function NaturalezaBadge({ naturaleza }: { naturaleza: string }) {
    const esCredito = naturaleza.toLowerCase() === 'credito';
    return (
        <span
            className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${
                esCredito ? 'bg-emerald-100 text-emerald-700' : 'bg-orange-100 text-orange-700'
            }`}
        >
            {esCredito ? 'Crédito' : 'Débito'}
        </span>
    );
}

function NivelBadge({ nivelNombre }: { nivelNombre: string }) {
    const colores: Record<string, string> = {
        Clase: 'bg-slate-900 text-white',
        Grupo: 'bg-slate-700 text-white',
        Cuenta: 'bg-slate-500 text-white',
        Subcuenta: 'bg-slate-200 text-slate-700',
        Auxiliar: 'bg-slate-100 text-slate-600',
    };
    return (
        <span className={`inline-flex items-center rounded-full px-2 py-0.5 text-xs font-medium ${colores[nivelNombre] ?? 'bg-slate-100 text-slate-600'}`}>
            {nivelNombre}
        </span>
    );
}

export default function TablaCuentasContables() {
    const [soloAsentables, setSoloAsentables] = useState(false);

    const catalogoCompleto = useCuentasContables();
    const catalogoAsentables = useCuentasAsentables();

    const query = soloAsentables ? catalogoAsentables : catalogoCompleto;
    const { data: cuentas, isLoading, isError } = query;

    if (isLoading) {
        return (
            <div className="flex items-center justify-center py-12">
                <div className="flex flex-col items-center gap-2">
                    <div className="h-8 w-8 animate-spin rounded-full border-4 border-slate-200 border-t-blue-600" />
                    <p className="text-sm text-slate-600">Cargando catálogo de cuentas...</p>
                </div>
            </div>
        );
    }

    if (isError) {
        return (
            <div className="rounded-lg border border-red-200 bg-red-50 p-4">
                <p className="text-sm text-red-800">
                    No fue posible cargar el catálogo de cuentas. Intente recargar la página.
                </p>
            </div>
        );
    }

    if (!cuentas || cuentas.length === 0) {
        return (
            <div className="rounded-lg border border-slate-200 bg-slate-50 p-8 text-center">
                <p className="text-sm text-slate-600">El catálogo de cuentas está vacío.</p>
            </div>
        );
    }

    return (
        <div className="space-y-4">
            {/* ── Controles ── */}
            <div className="flex items-center gap-3">
                <label className="flex cursor-pointer items-center gap-2 rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700 shadow-sm">
                    <input
                        type="checkbox"
                        checked={soloAsentables}
                        onChange={(e) => setSoloAsentables(e.target.checked)}
                        className="h-4 w-4 rounded border-slate-300 text-blue-600"
                    />
                    Solo cuentas asentables
                </label>
                <span className="text-sm text-slate-500">
                    {cuentas.length} cuenta{cuentas.length !== 1 ? 's' : ''}
                </span>
            </div>

            {/* ── Leyenda ── */}
            <div className="flex flex-wrap items-center gap-3 rounded-lg border border-slate-200 bg-slate-50 px-4 py-2 text-xs text-slate-600">
                <span className="font-medium">Leyenda:</span>
                <span className="flex items-center gap-1">
                    <span className="text-blue-600">✔</span> Permite movimiento (nodo hoja)
                </span>
                <span className="flex items-center gap-1">
                    <span className="text-amber-600">⚠</span> Exige identificación de tercero (DIAN exógena)
                </span>
            </div>

            {/* ── Tabla ── */}
            <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
                <div className="overflow-x-auto">
                    <table className="min-w-full divide-y divide-slate-200">
                        <thead className="bg-slate-50">
                            <tr>
                                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">
                                    Código
                                </th>
                                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">
                                    Descripción
                                </th>
                                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">
                                    Nivel
                                </th>
                                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">
                                    Naturaleza
                                </th>
                                <th className="px-4 py-3 text-center text-xs font-semibold uppercase tracking-wider text-slate-600">
                                    Movimiento
                                </th>
                                <th className="px-4 py-3 text-center text-xs font-semibold uppercase tracking-wider text-slate-600">
                                    Tercero
                                </th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-slate-100 bg-white">
                            {cuentas.map((cuenta) => {
                                const esAgrupador = !cuenta.permiteMovimiento;
                                return (
                                    <tr
                                        key={cuenta.id}
                                        className={esAgrupador ? 'bg-slate-50' : 'hover:bg-blue-50/30'}
                                    >
                                        <td className="whitespace-nowrap px-4 py-2.5 font-mono text-sm font-medium text-slate-800">
                                            {cuenta.codigo}
                                        </td>
                                        <td className={`px-4 py-2.5 text-sm ${INDENT[cuenta.nivel] ?? ''} ${esAgrupador ? 'font-semibold text-slate-700' : 'text-slate-800'}`}>
                                            {cuenta.descripcion}
                                        </td>
                                        <td className="whitespace-nowrap px-4 py-2.5">
                                            <NivelBadge nivelNombre={cuenta.nivelNombre} />
                                        </td>
                                        <td className="whitespace-nowrap px-4 py-2.5">
                                            <NaturalezaBadge naturaleza={cuenta.naturalezaNombre} />
                                        </td>
                                        <td className="whitespace-nowrap px-4 py-2.5 text-center">
                                            {cuenta.permiteMovimiento ? (
                                                <span className="text-blue-600" title="Permite asientos contables">✔</span>
                                            ) : (
                                                <span className="text-slate-300" title="Solo agrupador">—</span>
                                            )}
                                        </td>
                                        <td className="whitespace-nowrap px-4 py-2.5 text-center">
                                            {cuenta.exigeTercero ? (
                                                <span
                                                    className="text-amber-600"
                                                    title="Exige identificación de tercero (información exógena DIAN)"
                                                >
                                                    ⚠
                                                </span>
                                            ) : (
                                                <span className="text-slate-300">—</span>
                                            )}
                                        </td>
                                    </tr>
                                );
                            })}
                        </tbody>
                    </table>
                </div>
            </div>

            <p className="text-xs text-slate-400">
                Marco normativo: NIIF para Microempresas (Grupo III) · ESAL vigilada por Gobernación de Antioquia
            </p>
        </div>
    );
}
