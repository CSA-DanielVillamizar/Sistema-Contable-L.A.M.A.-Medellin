'use client';

import { useEffect, useMemo, useState } from 'react';
import TablaExogena from '@/features/tributario/components/TablaExogena';
import { useReporteExogena } from '@/features/tributario/hooks/useReporteExogena';
import { exportExogenaCsv } from '@/features/tributario/utils/exportExogenaCsv';
import { getUserRolesFromToken, hasAnyAllowedRole, TRIBUTARIO_ALLOWED_ROLES } from '@/lib/authRoles';

const MONTH_OPTIONS = [
    { value: '', label: 'Todos los meses' },
    { value: '1', label: 'Enero' },
    { value: '2', label: 'Febrero' },
    { value: '3', label: 'Marzo' },
    { value: '4', label: 'Abril' },
    { value: '5', label: 'Mayo' },
    { value: '6', label: 'Junio' },
    { value: '7', label: 'Julio' },
    { value: '8', label: 'Agosto' },
    { value: '9', label: 'Septiembre' },
    { value: '10', label: 'Octubre' },
    { value: '11', label: 'Noviembre' },
    { value: '12', label: 'Diciembre' },
] as const;

export default function ExogenaPage() {
    const [anio, setAnio] = useState(new Date().getFullYear());
    const [mesValue, setMesValue] = useState('');
    const [canAccess, setCanAccess] = useState(false);
    const [isRoleReady, setIsRoleReady] = useState(false);

    useEffect(() => {
        const token = localStorage.getItem('token');
        const roles = getUserRolesFromToken(token);
        setCanAccess(hasAnyAllowedRole(roles, TRIBUTARIO_ALLOWED_ROLES));
        setIsRoleReady(true);
    }, []);

    const mes = mesValue ? Number(mesValue) : undefined;
    const query = useReporteExogena({ anio, mes, enabled: canAccess && isRoleReady });

    const isExportDisabled = query.isLoading || (query.data?.length ?? 0) === 0;
    const nombreArchivo = useMemo(() => {
        const sufijoMes = mes ? `-mes-${String(mes).padStart(2, '0')}` : '-anual';
        return `reporte-exogena-${anio}${sufijoMes}.csv`;
    }, [anio, mes]);

    if (!isRoleReady) {
        return (
            <main className="min-h-screen bg-slate-50 px-6 py-10">
                <div className="mx-auto w-full max-w-7xl rounded-xl border border-slate-200 bg-white p-6 text-slate-600">
                    Validando permisos...
                </div>
            </main>
        );
    }

    if (!canAccess) {
        return (
            <main className="min-h-screen bg-slate-50 px-6 py-10">
                <div className="mx-auto w-full max-w-7xl rounded-xl border border-red-200 bg-white p-6 text-red-700">
                    No tienes permisos para acceder al módulo de exógena.
                </div>
            </main>
        );
    }

    return (
        <main className="min-h-screen bg-slate-50 px-6 py-10">
            <div className="mx-auto flex w-full max-w-7xl flex-col gap-6">
                <header className="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
                    <div>
                        <h1 className="text-3xl font-bold text-slate-900">Reporte Exógena</h1>
                        <p className="mt-1 text-slate-600">Agrupado por tercero y cuenta contable (Sprint 1)</p>
                    </div>

                    <button
                        type="button"
                        onClick={() => exportExogenaCsv(query.data ?? [], nombreArchivo)}
                        disabled={isExportDisabled}
                        className="rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-50"
                    >
                        Exportar a CSV
                    </button>
                </header>

                <section className="grid grid-cols-1 gap-4 rounded-xl border border-slate-200 bg-white p-4 md:grid-cols-2">
                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Año</label>
                        <input
                            type="number"
                            min={2000}
                            max={2100}
                            value={anio}
                            onChange={(event) => setAnio(Number(event.target.value || new Date().getFullYear()))}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Mes (opcional)</label>
                        <select
                            value={mesValue}
                            onChange={(event) => setMesValue(event.target.value)}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        >
                            {MONTH_OPTIONS.map((option) => (
                                <option key={option.value} value={option.value}>{option.label}</option>
                            ))}
                        </select>
                    </div>
                </section>

                <TablaExogena
                    rows={query.data ?? []}
                    isLoading={query.isLoading}
                    errorMessage={query.isError ? ((query.error as Error | null)?.message ?? 'No fue posible cargar el reporte exógena.') : undefined}
                />
            </div>
        </main>
    );
}
