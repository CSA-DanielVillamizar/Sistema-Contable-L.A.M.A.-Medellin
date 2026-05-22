'use client';

import { usePathname } from 'next/navigation';

// ─── Mapa de rutas → título de sección ───────────────────────────────────────
const SECTION_TITLES: { prefix: string; label: string }[] = [
    { prefix: '/cartera', label: 'Cartera' },
    { prefix: '/transacciones', label: 'Tesorería' },
    { prefix: '/contabilidad', label: 'Contabilidad' },
    { prefix: '/miembros', label: 'Miembros' },
    { prefix: '/eventos', label: 'Eventos' },
    { prefix: '/merchandising', label: 'Merchandising' },
    { prefix: '/seguridad', label: 'Seguridad' },
    { prefix: '/', label: 'Dashboard' },
];

function resolveTitle(pathname: string): string {
    for (const { prefix, label } of SECTION_TITLES) {
        if (pathname === prefix || (prefix !== '/' && pathname.startsWith(prefix))) {
            return label;
        }
    }
    return 'Dashboard';
}

function IconUser() {
    return (
        <svg className="h-5 w-5 text-slate-500" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="M17.982 18.725A7.488 7.488 0 0 0 12 15.75a7.488 7.488 0 0 0-5.982 2.975m11.963 0a9 9 0 1 0-11.963 0m11.963 0A8.966 8.966 0 0 1 12 21a8.966 8.966 0 0 1-5.982-2.275M15 9.75a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z" />
        </svg>
    );
}

// ─── Componente ───────────────────────────────────────────────────────────────
export default function Navbar() {
    const pathname = usePathname();
    const title = resolveTitle(pathname);

    return (
        <header className="flex h-14 shrink-0 items-center justify-between border-b border-slate-200 bg-white px-6">
            <h1 className="text-base font-semibold text-slate-800">{title}</h1>

            <div className="flex items-center gap-2 rounded-full border border-slate-200 px-3 py-1.5">
                <IconUser />
                <span className="text-sm text-slate-600">Fundación L.A.M.A.</span>
            </div>
        </header>
    );
}
