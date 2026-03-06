'use client';

import { useEffect, useMemo, useState } from 'react';
import TablaBeneficiariosFinales from '@/features/tributario/components/TablaBeneficiariosFinales';
import { useReporteBeneficiariosFinales } from '@/features/tributario/hooks/useReporteBeneficiariosFinales';
import { exportBeneficiariosFinalesCsv } from '@/features/tributario/utils/exportBeneficiariosFinalesCsv';
import { getUserRolesFromToken, hasAnyAllowedRole, TRIBUTARIO_ALLOWED_ROLES } from '@/lib/authRoles';

export default function BeneficiariosFinalesPage() {
    const [canAccess, setCanAccess] = useState(false);
    const [isRoleReady, setIsRoleReady] = useState(false);

    useEffect(() => {
        const token = localStorage.getItem('token');
        const roles = getUserRolesFromToken(token);
        setCanAccess(hasAnyAllowedRole(roles, TRIBUTARIO_ALLOWED_ROLES));
        setIsRoleReady(true);
    }, []);

    const query = useReporteBeneficiariosFinales({ enabled: canAccess && isRoleReady });
    const isExportDisabled = query.isLoading || (query.data?.length ?? 0) === 0;

    const nombreArchivo = useMemo(() => {
        return `reporte-beneficiarios-finales-${new Date().getFullYear()}.csv`;
    }, []);

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
                    No tienes permisos para acceder al módulo de beneficiarios finales.
                </div>
            </main>
        );
    }

    return (
        <main className="min-h-screen bg-slate-50 px-6 py-10">
            <div className="mx-auto flex w-full max-w-7xl flex-col gap-6">
                <header className="flex flex-col gap-4 md:flex-row md:items-end md:justify-between">
                    <div>
                        <h1 className="text-3xl font-bold text-slate-900">Reporte Beneficiarios Finales (RUB)</h1>
                        <p className="mt-1 text-slate-600">Listado de miembros activos para control y revisión tributaria</p>
                    </div>

                    <button
                        type="button"
                        onClick={() => exportBeneficiariosFinalesCsv(query.data ?? [], nombreArchivo)}
                        disabled={isExportDisabled}
                        className="rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-50"
                    >
                        Exportar a CSV
                    </button>
                </header>

                <TablaBeneficiariosFinales
                    rows={query.data ?? []}
                    isLoading={query.isLoading}
                    errorMessage={query.isError ? ((query.error as Error | null)?.message ?? 'No fue posible cargar el reporte RUB.') : undefined}
                />
            </div>
        </main>
    );
}
