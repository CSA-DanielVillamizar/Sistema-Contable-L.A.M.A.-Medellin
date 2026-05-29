'use client';

import { usePathname } from 'next/navigation';

// ---------------------------------------------------------------------------
// Mapeo de rutas a títulos de sección
// ---------------------------------------------------------------------------
const ROUTE_TITLES: Record<string, string> = {
    '/': 'Dashboard Ejecutivo',
    '/cartera': 'Cartera',
    '/tesoreria': 'Tesorería',
    '/merchandising': 'Merchandising',
    '/miembros': 'Directorio de Miembros',
    '/eventos': 'Eventos y Rodadas',
    '/seguridad': 'Gestión de Accesos',
    '/reportes': 'Reportes',
    '/tributario': 'Reportes Tributarios',
    '/contabilidad': 'Contabilidad',
    '/transacciones': 'Transacciones',
    '/donaciones': 'Donaciones',
    '/proyectos': 'Gestión Misional',
};

function getSectionTitle(pathname: string): string {
    // Buscar primero la ruta exacta, luego el prefijo más largo que coincida
    if (ROUTE_TITLES[pathname]) return ROUTE_TITLES[pathname];

    const match = Object.keys(ROUTE_TITLES)
        .filter((route) => route !== '/' && pathname.startsWith(route))
        .sort((a, b) => b.length - a.length)[0];

    return match ? ROUTE_TITLES[match] : 'Sistema Contable';
}

// ---------------------------------------------------------------------------
// Navbar Component
// ---------------------------------------------------------------------------
export default function Navbar() {
    const pathname = usePathname();
    const title = getSectionTitle(pathname);

    return (
        <header className="flex h-16 shrink-0 items-center justify-between border-b border-slate-200 bg-white px-6">
            {/* Título de la sección actual */}
            <h1 className="text-lg font-semibold text-slate-900">{title}</h1>

            {/* Área de perfil / usuario */}
            <div className="flex items-center gap-3">
                {/* Badge de entorno */}
                <span className="hidden rounded-full bg-emerald-100 px-2.5 py-0.5 text-xs font-medium text-emerald-700 sm:inline-flex">
                    Producción
                </span>

                {/* Avatar placeholder */}
                <div
                    className="flex size-8 items-center justify-center rounded-full bg-slate-900 text-xs font-bold text-white"
                    title="Usuario autenticado vía Entra ID"
                >
                    LA
                </div>
            </div>
        </header>
    );
}
