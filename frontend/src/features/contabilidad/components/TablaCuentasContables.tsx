'use client';

import { useMemo, useState } from 'react';
import { useCuentasContables } from '@/features/contabilidad/hooks/useCuentasContables';

export default function TablaCuentasContables() {
    const [soloAsentables, setSoloAsentables] = useState(false);
    const cuentasQuery = useCuentasContables();

    const cuentas = useMemo(() => {
        const listado = cuentasQuery.data ?? [];
        return soloAsentables ? listado.filter((item) => item.permiteMovimiento) : listado;
    }, [cuentasQuery.data, soloAsentables]);

    if (cuentasQuery.isLoading) {
        return <p className="text-sm text-slate-600">Cargando catálogo de cuentas...</p>;
    }

    if (cuentasQuery.isError) {
        return <p className="text-sm text-red-600">No fue posible cargar el catálogo de cuentas.</p>;
    }

    return (
        <section className="space-y-4">
            <label className="inline-flex items-center gap-2 rounded-lg border border-slate-200 bg-white px-3 py-2 text-sm text-slate-700">
                <input
                    type="checkbox"
                    checked={soloAsentables}
                    onChange={(event) => setSoloAsentables(event.target.checked)}
                />
                Solo cuentas asentables
            </label>

            <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
                <div className="overflow-x-auto">
                    <table className="min-w-full divide-y divide-slate-200">
                        <thead className="bg-slate-50">
                            <tr>
                                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Código</th>
                                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Descripción</th>
                                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Naturaleza</th>
                                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Movimiento</th>
                                <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Exige Tercero</th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-slate-200 bg-white">
                            {cuentas.map((cuenta) => {
                                const esAgrupadora = !cuenta.permiteMovimiento;

                                return (
                                    <tr
                                        key={cuenta.id}
                                        className={esAgrupadora ? 'bg-slate-50 font-semibold text-slate-900' : undefined}
                                    >
                                        <td className="px-4 py-3 text-sm">{cuenta.codigo}</td>
                                        <td className="px-4 py-3 text-sm">{cuenta.descripcion}</td>
                                        <td className="px-4 py-3 text-sm">{cuenta.naturaleza}</td>
                                        <td className="px-4 py-3 text-sm">{cuenta.permiteMovimiento ? 'Sí' : 'No'}</td>
                                        <td className="px-4 py-3 text-sm">
                                            {cuenta.exigeTercero ? (
                                                <span className="inline-flex items-center gap-1">
                                                    Sí <span title="Requiere tercero para exógena DIAN">⚠</span>
                                                </span>
                                            ) : (
                                                'No'
                                            )}
                                        </td>
                                    </tr>
                                );
                            })}
                        </tbody>
                    </table>
                </div>
            </div>
        </section>
    );
}
