'use client';

import type { FiltrosDonaciones as Filtros } from '@/features/donaciones/hooks/useDonaciones';
import { useDonantes } from '@/features/donaciones/hooks/useDonaciones';
import { useCentrosCostoCatalogo } from '@/features/transacciones/hooks/useCentrosCostoCatalogo';

type FiltrosDonacionesProps = {
    filtros: Filtros;
    onCambiar: (filtros: Filtros) => void;
    onExportar: () => void;
    /** Se desactiva la exportacion cuando no hay nada que exportar. */
    puedeExportar: boolean;
};

const CLASE_CAMPO =
    'w-full rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-900 focus:border-blue-500 focus:outline-none';

/** El estado del certificado es de tres valores, y un select solo da texto. */
function aCertificado(valor: string): boolean | undefined {
    return valor === '' ? undefined : valor === 'emitido';
}

function desdeCertificado(valor: boolean | undefined): string {
    if (valor === undefined) {
        return '';
    }

    return valor ? 'emitido' : 'pendiente';
}

export default function FiltrosDonaciones({
    filtros,
    onCambiar,
    onExportar,
    puedeExportar,
}: FiltrosDonacionesProps) {
    const donantesQuery = useDonantes();
    const centrosQuery = useCentrosCostoCatalogo();

    const actualizar = (cambio: Partial<Filtros>) => onCambiar({ ...filtros, ...cambio });

    const hayFiltros = Object.values(filtros).some(
        (valor) => valor !== undefined && valor !== '',
    );

    return (
        <div className="mb-4 rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
            <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-5">
                <label className="block">
                    <span className="mb-1 block text-xs font-medium text-slate-600">Desde</span>
                    <input
                        type="date"
                        value={filtros.desde ?? ''}
                        onChange={(evento) => actualizar({ desde: evento.target.value })}
                        className={CLASE_CAMPO}
                    />
                </label>

                <label className="block">
                    <span className="mb-1 block text-xs font-medium text-slate-600">Hasta</span>
                    <input
                        type="date"
                        value={filtros.hasta ?? ''}
                        onChange={(evento) => actualizar({ hasta: evento.target.value })}
                        className={CLASE_CAMPO}
                    />
                </label>

                <label className="block">
                    <span className="mb-1 block text-xs font-medium text-slate-600">Donante</span>
                    <select
                        value={filtros.donanteId ?? ''}
                        onChange={(evento) => actualizar({ donanteId: evento.target.value })}
                        className={CLASE_CAMPO}
                    >
                        <option value="">Todos</option>
                        {(donantesQuery.data ?? []).map((donante) => (
                            <option key={donante.id} value={donante.id}>
                                {donante.nombreORazonSocial}
                            </option>
                        ))}
                    </select>
                </label>

                <label className="block">
                    <span className="mb-1 block text-xs font-medium text-slate-600">Centro de costo</span>
                    <select
                        value={filtros.centroCostoId ?? ''}
                        onChange={(evento) => actualizar({ centroCostoId: evento.target.value })}
                        className={CLASE_CAMPO}
                    >
                        <option value="">Todos</option>
                        {(centrosQuery.data ?? []).map((centro) => (
                            <option key={centro.id} value={centro.id}>
                                {centro.nombre}
                            </option>
                        ))}
                    </select>
                </label>

                <label className="block">
                    <span className="mb-1 block text-xs font-medium text-slate-600">Certificado</span>
                    <select
                        value={desdeCertificado(filtros.certificadoEmitido)}
                        onChange={(evento) =>
                            actualizar({ certificadoEmitido: aCertificado(evento.target.value) })
                        }
                        className={CLASE_CAMPO}
                    >
                        <option value="">Todos</option>
                        <option value="emitido">Emitido</option>
                        <option value="pendiente">Pendiente</option>
                    </select>
                </label>
            </div>

            <div className="mt-3 flex flex-wrap justify-end gap-2">
                {hayFiltros && (
                    <button
                        type="button"
                        onClick={() => onCambiar({})}
                        className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
                    >
                        Limpiar filtros
                    </button>
                )}
                <button
                    type="button"
                    onClick={onExportar}
                    disabled={!puedeExportar}
                    className="rounded-lg border border-emerald-200 bg-emerald-50 px-4 py-2 text-sm font-medium text-emerald-700 hover:bg-emerald-100 disabled:cursor-not-allowed disabled:opacity-50"
                >
                    Exportar CSV
                </button>
            </div>
        </div>
    );
}
