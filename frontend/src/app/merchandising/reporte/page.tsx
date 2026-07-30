'use client';

import apiClient, { type RespuestaApi } from '@/lib/apiClient';
import { useRoleAccess } from '@/lib/useRoleAccess';
import { useQuery } from '@tanstack/react-query';
import { useState } from 'react';

/**
 * Inventario, ventas y utilidad (historia 4-3).
 *
 * El rango filtra las ventas. El costo promedio se pondera sobre todas las
 * entradas historicas, porque la mercancia vendida hoy pudo entrar el mes
 * pasado; el backend lo calcula asi y aqui solo se muestra.
 */
const ROLES_PERMITIDOS = ['Admin', 'Operador', 'Tesorero', 'Contador', 'Junta', 'Inventario'] as const;

const formatoCOP = new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0,
});

type Linea = {
    productoId: string;
    nombre: string;
    codigoSKU: string;
    cantidadEnStock: number;
    cantidadMinima: number;
    bajoMinimo: boolean;
    costoPromedio: number;
    valorInventario: number;
    unidadesVendidas: number;
    ingresoVentas: number;
    costoVentas: number;
    utilidad: number;
    margenPorcentaje: number;
};

type Reporte = {
    valorTotalInventario: number;
    totalUnidadesVendidas: number;
    totalIngresoVentas: number;
    totalCostoVentas: number;
    utilidadTotal: number;
    productosBajoMinimo: number;
    lineas: Linea[];
};

export default function ReporteInventarioPage() {
    const { canAccess, isRoleReady } = useRoleAccess(ROLES_PERMITIDOS);

    const [desde, setDesde] = useState('');
    const [hasta, setHasta] = useState('');

    const reporteQuery = useQuery<Reporte>({
        queryKey: ['merchandising', 'reporte', desde, hasta],
        queryFn: async () => {
            const response = await apiClient.get<RespuestaApi>('/api/merchandising/reporte', {
                params: { desde: desde || undefined, hasta: hasta || undefined },
            });

            const d = response.data ?? {};
            const lineas = Array.isArray(d.lineas) ? (d.lineas as RespuestaApi[]) : [];

            return {
                valorTotalInventario: Number(d.valorTotalInventario ?? 0),
                totalUnidadesVendidas: Number(d.totalUnidadesVendidas ?? 0),
                totalIngresoVentas: Number(d.totalIngresoVentas ?? 0),
                totalCostoVentas: Number(d.totalCostoVentas ?? 0),
                utilidadTotal: Number(d.utilidadTotal ?? 0),
                productosBajoMinimo: Number(d.productosBajoMinimo ?? 0),
                lineas: lineas.map((l) => ({
                    productoId: String(l?.productoId ?? ''),
                    nombre: String(l?.nombre ?? ''),
                    codigoSKU: String(l?.codigoSKU ?? ''),
                    cantidadEnStock: Number(l?.cantidadEnStock ?? 0),
                    cantidadMinima: Number(l?.cantidadMinima ?? 0),
                    bajoMinimo: Boolean(l?.bajoMinimo),
                    costoPromedio: Number(l?.costoPromedio ?? 0),
                    valorInventario: Number(l?.valorInventario ?? 0),
                    unidadesVendidas: Number(l?.unidadesVendidas ?? 0),
                    ingresoVentas: Number(l?.ingresoVentas ?? 0),
                    costoVentas: Number(l?.costoVentas ?? 0),
                    utilidad: Number(l?.utilidad ?? 0),
                    margenPorcentaje: Number(l?.margenPorcentaje ?? 0),
                })),
            };
        },
    });

    if (!isRoleReady) {
        return <p className="p-8 text-sm text-slate-500">Verificando permisos...</p>;
    }

    if (!canAccess) {
        return (
            <div className="p-8">
                <h1 className="text-xl font-bold text-slate-900">Sin permiso</h1>
                <p className="mt-2 text-sm text-slate-600">El reporte de inventario requiere un rol autorizado.</p>
            </div>
        );
    }

    const r = reporteQuery.data;

    return (
        <div className="p-8">
            <h1 className="text-2xl font-bold text-slate-900">Reporte de inventario</h1>
            <p className="mt-1 text-sm text-slate-600">
                Existencias, ventas y utilidad. Un producto sin costo registrado aparece en cero: el reporte no
                supone márgenes que nadie declaró.
            </p>

            <div className="mt-5 flex flex-wrap items-end gap-4 rounded-xl border border-slate-200 bg-white p-4">
                <div>
                    <label className="mb-1 block text-sm font-medium text-slate-700">Ventas desde</label>
                    <input type="date" value={desde} onChange={(e) => setDesde(e.target.value)} className="rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-900" />
                </div>
                <div>
                    <label className="mb-1 block text-sm font-medium text-slate-700">Hasta</label>
                    <input type="date" value={hasta} onChange={(e) => setHasta(e.target.value)} className="rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-900" />
                </div>
                {desde || hasta ? (
                    <button type="button" onClick={() => { setDesde(''); setHasta(''); }} className="rounded-lg border border-slate-300 px-3 py-2 text-sm font-medium text-slate-700">
                        Todo el histórico
                    </button>
                ) : null}
            </div>

            {r ? (
                <div className="mt-5 grid grid-cols-2 gap-4 lg:grid-cols-5">
                    {[
                        { etiqueta: 'Valor del inventario', valor: formatoCOP.format(r.valorTotalInventario) },
                        { etiqueta: 'Unidades vendidas', valor: String(r.totalUnidadesVendidas) },
                        { etiqueta: 'Ingreso por ventas', valor: formatoCOP.format(r.totalIngresoVentas) },
                        { etiqueta: 'Costo de ventas', valor: formatoCOP.format(r.totalCostoVentas) },
                        { etiqueta: 'Utilidad', valor: formatoCOP.format(r.utilidadTotal), destacar: true },
                    ].map((t) => (
                        <div key={t.etiqueta} className="rounded-xl border border-slate-200 bg-white p-4">
                            <p className="text-xs font-medium uppercase tracking-wider text-slate-400">{t.etiqueta}</p>
                            <p className={`mt-1 tabular-nums ${t.destacar ? 'text-lg font-bold text-slate-900' : 'text-base font-semibold text-slate-800'}`}>
                                {t.valor}
                            </p>
                        </div>
                    ))}
                </div>
            ) : null}

            {r && r.productosBajoMinimo > 0 ? (
                <div className="mt-4 rounded-lg border border-amber-300 bg-amber-50 px-3 py-2 text-sm text-amber-800">
                    {r.productosBajoMinimo === 1
                        ? 'Hay 1 producto en o por debajo de su mínimo.'
                        : `Hay ${r.productosBajoMinimo} productos en o por debajo de su mínimo.`}
                </div>
            ) : null}

            <div className="mt-6 overflow-x-auto rounded-xl border border-slate-200 bg-white">
                <table className="w-full text-sm">
                    <thead className="border-b border-slate-200 bg-slate-50 text-left text-slate-600">
                        <tr>
                            <th className="px-4 py-3 font-medium">Producto</th>
                            <th className="px-4 py-3 text-right font-medium">Stock</th>
                            <th className="px-4 py-3 text-right font-medium">Costo prom.</th>
                            <th className="px-4 py-3 text-right font-medium">Valor</th>
                            <th className="px-4 py-3 text-right font-medium">Vendidas</th>
                            <th className="px-4 py-3 text-right font-medium">Ingreso</th>
                            <th className="px-4 py-3 text-right font-medium">Utilidad</th>
                            <th className="px-4 py-3 text-right font-medium">Margen</th>
                        </tr>
                    </thead>
                    <tbody>
                        {reporteQuery.isLoading ? (
                            <tr><td colSpan={8} className="px-4 py-6 text-center text-slate-500">Generando...</td></tr>
                        ) : null}

                        {reporteQuery.isError ? (
                            <tr><td colSpan={8} className="px-4 py-6 text-center text-rose-700">No fue posible generar el reporte.</td></tr>
                        ) : null}

                        {r && r.lineas.length === 0 ? (
                            <tr><td colSpan={8} className="px-4 py-6 text-center text-slate-500">No hay productos registrados.</td></tr>
                        ) : null}

                        {(r?.lineas ?? []).map((l) => (
                            <tr key={l.productoId} className="border-b border-slate-100 last:border-0">
                                <td className="px-4 py-3">
                                    <span className="font-medium text-slate-900">{l.nombre}</span>
                                    <span className="block text-xs text-slate-400">{l.codigoSKU}</span>
                                </td>
                                <td className={`px-4 py-3 text-right tabular-nums ${l.bajoMinimo ? 'font-medium text-amber-700' : 'text-slate-700'}`}>
                                    {l.cantidadEnStock}
                                    {l.bajoMinimo ? <span className="block text-xs">mín. {l.cantidadMinima}</span> : null}
                                </td>
                                <td className="px-4 py-3 text-right tabular-nums text-slate-600">{formatoCOP.format(l.costoPromedio)}</td>
                                <td className="px-4 py-3 text-right tabular-nums text-slate-600">{formatoCOP.format(l.valorInventario)}</td>
                                <td className="px-4 py-3 text-right tabular-nums text-slate-700">{l.unidadesVendidas}</td>
                                <td className="px-4 py-3 text-right tabular-nums text-slate-700">{formatoCOP.format(l.ingresoVentas)}</td>
                                <td className={`px-4 py-3 text-right tabular-nums font-medium ${l.utilidad < 0 ? 'text-rose-700' : 'text-slate-900'}`}>
                                    {formatoCOP.format(l.utilidad)}
                                </td>
                                <td className="px-4 py-3 text-right tabular-nums text-slate-600">{l.margenPorcentaje}%</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
