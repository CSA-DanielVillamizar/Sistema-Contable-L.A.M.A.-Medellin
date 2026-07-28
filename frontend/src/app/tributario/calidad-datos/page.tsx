'use client';

import TablaCalidadDatos from '@/features/tributario/components/TablaCalidadDatos';
import { useReporteCalidadDatos } from '@/features/tributario/hooks/useReporteCalidadDatos';
import { TRIBUTARIO_ALLOWED_ROLES } from '@/lib/authRoles';
import { useRoleAccess } from '@/lib/useRoleAccess';

export default function CalidadDatosPage() {
    const { canAccess, isRoleReady } = useRoleAccess(TRIBUTARIO_ALLOWED_ROLES);

    const query = useReporteCalidadDatos({ enabled: canAccess && isRoleReady });

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
                    No tienes permisos para acceder al módulo de auditoría de datos.
                </div>
            </main>
        );
    }

    return (
        <main className="min-h-screen bg-slate-50 px-6 py-10">
            <div className="mx-auto flex w-full max-w-7xl flex-col gap-6">
                <header>
                    <h1 className="text-3xl font-bold text-slate-900">Auditoría de Datos Tributarios</h1>
                    <p className="mt-1 text-slate-600">Inconsistencias de documento y tipo de documento en Miembros y Donantes</p>
                </header>

                <TablaCalidadDatos
                    rows={query.data ?? []}
                    isLoading={query.isLoading}
                    errorMessage={query.isError ? ((query.error as Error | null)?.message ?? 'No fue posible cargar la auditoría de datos.') : undefined}
                />
            </div>
        </main>
    );
}
