'use client';

import { useComprobantes } from '@/features/contabilidad/hooks/useComprobantes';
import apiClient from '@/lib/apiClient';
import { useRoleAccess } from '@/lib/useRoleAccess';
import { useState } from 'react';

/**
 * Recibos (historia 1-7).
 *
 * El PDF lleva un QR que apunta a la verificacion publica del sistema, no al
 * propio documento: quien lo recibe comprueba contra la fuente en vez de creer
 * lo que el papel afirma.
 */
const ROLES_PERMITIDOS = ['Admin', 'Operador', 'Tesorero', 'Contador', 'Junta'] as const;

const formatoCOP = new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0,
});

export default function RecibosPage() {
    const { canAccess, isRoleReady } = useRoleAccess(ROLES_PERMITIDOS);
    const comprobantesQuery = useComprobantes();

    const [filtro, setFiltro] = useState('');
    const [descargando, setDescargando] = useState<string | null>(null);
    const [error, setError] = useState<string | null>(null);

    if (!isRoleReady) {
        return <p className="p-8 text-sm text-slate-500">Verificando permisos...</p>;
    }

    if (!canAccess) {
        return (
            <div className="p-8">
                <h1 className="text-xl font-bold text-slate-900">Sin permiso</h1>
                <p className="mt-2 text-sm text-slate-600">Los recibos requieren un rol con acceso contable.</p>
            </div>
        );
    }

    const descargar = async (numeroConsecutivo: string) => {
        setError(null);
        setDescargando(numeroConsecutivo);

        try {
            // responseType blob: el PDF llega en binario y el interceptor no
            // debe intentar interpretarlo como JSON.
            const respuesta = await apiClient.get(`/api/recibos/${numeroConsecutivo}/pdf`, {
                responseType: 'blob',
            });

            const url = URL.createObjectURL(respuesta.data as Blob);
            const enlace = document.createElement('a');
            enlace.href = url;
            enlace.download = `recibo-${numeroConsecutivo}.pdf`;
            enlace.click();
            URL.revokeObjectURL(url);
        } catch {
            setError(`No fue posible generar el recibo ${numeroConsecutivo}.`);
        } finally {
            setDescargando(null);
        }
    };

    const comprobantes = (comprobantesQuery.data ?? []).filter((c) => {
        const texto = filtro.trim().toLowerCase();
        if (!texto) return true;
        return (
            c.numeroConsecutivo.toLowerCase().includes(texto) ||
            c.descripcion.toLowerCase().includes(texto)
        );
    });

    return (
        <div className="p-8">
            <h1 className="text-2xl font-bold text-slate-900">Recibos</h1>
            <p className="mt-1 text-sm text-slate-600">
                Cada recibo lleva un código QR que apunta a la verificación pública. Quien lo escanea confirma
                contra el sistema que el movimiento existe, su fecha y su valor.
            </p>

            <div className="mt-5 max-w-md">
                <label className="mb-1 block text-sm font-medium text-slate-700">Buscar</label>
                <input
                    value={filtro}
                    onChange={(e) => setFiltro(e.target.value)}
                    placeholder="Consecutivo o descripción"
                    className="w-full rounded-lg border border-slate-300 px-3 py-2 text-sm text-slate-900"
                />
            </div>

            {error ? (
                <div className="mt-4 rounded-lg border border-rose-200 bg-rose-50 px-3 py-2 text-sm text-rose-700">
                    {error}
                </div>
            ) : null}

            <div className="mt-6 overflow-x-auto rounded-xl border border-slate-200 bg-white">
                <table className="w-full text-sm">
                    <thead className="border-b border-slate-200 bg-slate-50 text-left text-slate-600">
                        <tr>
                            <th className="px-4 py-3 font-medium">Consecutivo</th>
                            <th className="px-4 py-3 font-medium">Fecha</th>
                            <th className="px-4 py-3 font-medium">Descripción</th>
                            <th className="px-4 py-3 text-right font-medium">Valor</th>
                            <th className="px-4 py-3 font-medium">Estado</th>
                            <th className="px-4 py-3 text-right font-medium">Recibo</th>
                        </tr>
                    </thead>
                    <tbody>
                        {comprobantesQuery.isLoading ? (
                            <tr><td colSpan={6} className="px-4 py-6 text-center text-slate-500">Cargando...</td></tr>
                        ) : null}

                        {comprobantesQuery.isError ? (
                            <tr><td colSpan={6} className="px-4 py-6 text-center text-rose-700">
                                No fue posible cargar los comprobantes.
                            </td></tr>
                        ) : null}

                        {!comprobantesQuery.isLoading && comprobantes.length === 0 ? (
                            <tr><td colSpan={6} className="px-4 py-6 text-center text-slate-500">
                                {filtro ? 'Ningún comprobante coincide con la búsqueda.' : 'Todavía no hay comprobantes.'}
                            </td></tr>
                        ) : null}

                        {comprobantes.map((c) => {
                            const anulado = c.estado === 'Anulado';

                            return (
                                <tr key={c.id} className="border-b border-slate-100 last:border-0">
                                    <td className="px-4 py-3 font-medium text-slate-900">{c.numeroConsecutivo}</td>
                                    <td className="px-4 py-3 text-slate-600">{c.fecha.slice(0, 10)}</td>
                                    <td className="px-4 py-3 text-slate-600">
                                        {c.descripcion}
                                        <span className="block text-xs text-slate-400">{c.tipoComprobante}</span>
                                    </td>
                                    <td className="px-4 py-3 text-right tabular-nums">{formatoCOP.format(c.total)}</td>
                                    <td className="px-4 py-3">
                                        <span
                                            className={`rounded-full px-2.5 py-0.5 text-xs font-medium ${
                                                anulado ? 'bg-rose-100 text-rose-800' : 'bg-emerald-100 text-emerald-800'
                                            }`}
                                        >
                                            {c.estado}
                                        </span>
                                    </td>
                                    <td className="px-4 py-3 text-right">
                                        <button
                                            type="button"
                                            onClick={() => void descargar(c.numeroConsecutivo)}
                                            disabled={descargando === c.numeroConsecutivo}
                                            className="rounded-lg border border-slate-300 px-3 py-1.5 text-xs font-medium text-slate-700 transition hover:bg-slate-50 disabled:opacity-60"
                                        >
                                            {descargando === c.numeroConsecutivo ? 'Generando...' : 'Descargar PDF'}
                                        </button>
                                    </td>
                                </tr>
                            );
                        })}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
