'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useState } from 'react';

// ─── Tipos ───────────────────────────────────────────────────────────────────
type NavItem = {
    label: string;
    href: string;
    icon: React.ReactNode;
};

type NavGroup = {
    group: string;
    items: NavItem[];
};

// ─── Íconos SVG inline (sin dependencia externa) ─────────────────────────────
function IconHome() {
    return (
        <svg className="h-5 w-5 shrink-0" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="m2.25 12 8.954-8.955c.44-.439 1.152-.439 1.591 0L21.75 12M4.5 9.75v10.125c0 .621.504 1.125 1.125 1.125H9.75v-4.875c0-.621.504-1.125 1.125-1.125h2.25c.621 0 1.125.504 1.125 1.125V21h4.125c.621 0 1.125-.504 1.125-1.125V9.75M8.25 21h8.25" />
        </svg>
    );
}
function IconCartera() {
    return (
        <svg className="h-5 w-5 shrink-0" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="M2.25 8.25h19.5M2.25 9h19.5m-16.5 5.25h6m-6 2.25h3m-3.75 3h15a2.25 2.25 0 0 0 2.25-2.25V6.75A2.25 2.25 0 0 0 19.5 4.5h-15a2.25 2.25 0 0 0-2.25 2.25v10.5A2.25 2.25 0 0 0 4.5 19.5Z" />
        </svg>
    );
}
function IconTesoreria() {
    return (
        <svg className="h-5 w-5 shrink-0" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 6v12m-3-2.818.879.659c1.171.879 3.07.879 4.242 0 1.172-.879 1.172-2.303 0-3.182C13.536 12.219 12.768 12 12 12c-.725 0-1.45-.22-2.003-.659-1.106-.879-1.106-2.303 0-3.182s2.9-.879 4.006 0l.415.33M21 12a9 9 0 1 1-18 0 9 9 0 0 1 18 0Z" />
        </svg>
    );
}
function IconContabilidad() {
    return (
        <svg className="h-5 w-5 shrink-0" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 7.5h1.5m-1.5 3h1.5m-7.5 3h7.5m-7.5 3h7.5m3-9h3.375c.621 0 1.125.504 1.125 1.125V18a2.25 2.25 0 0 1-2.25 2.25M16.5 7.5V18a2.25 2.25 0 0 0 2.25 2.25M16.5 7.5V4.875c0-.621-.504-1.125-1.125-1.125H4.125C3.504 3.75 3 4.254 3 4.875V18a2.25 2.25 0 0 0 2.25 2.25h13.5M6 7.5h3v3H6v-3Z" />
        </svg>
    );
}
function IconMiembros() {
    return (
        <svg className="h-5 w-5 shrink-0" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="M15 19.128a9.38 9.38 0 0 0 2.625.372 9.337 9.337 0 0 0 4.121-.952 4.125 4.125 0 0 0-7.533-2.493M15 19.128v-.003c0-1.113-.285-2.16-.786-3.07M15 19.128v.106A12.318 12.318 0 0 1 8.624 21c-2.331 0-4.512-.645-6.374-1.766l-.001-.109a6.375 6.375 0 0 1 11.964-3.07M12 6.375a3.375 3.375 0 1 1-6.75 0 3.375 3.375 0 0 1 6.75 0Zm8.25 2.25a2.625 2.625 0 1 1-5.25 0 2.625 2.625 0 0 1 5.25 0Z" />
        </svg>
    );
}
function IconEventos() {
    return (
        <svg className="h-5 w-5 shrink-0" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="M6.75 3v2.25M17.25 3v2.25M3 18.75V7.5a2.25 2.25 0 0 1 2.25-2.25h13.5A2.25 2.25 0 0 1 21 7.5v11.25m-18 0A2.25 2.25 0 0 0 5.25 21h13.5A2.25 2.25 0 0 0 21 18.75m-18 0v-7.5A2.25 2.25 0 0 1 5.25 9h13.5A2.25 2.25 0 0 1 21 11.25v7.5" />
        </svg>
    );
}
function IconMerchandising() {
    return (
        <svg className="h-5 w-5 shrink-0" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="M15.75 10.5V6a3.75 3.75 0 1 0-7.5 0v4.5m11.356-1.993 1.263 12c.07.665-.45 1.243-1.119 1.243H4.25a1.125 1.125 0 0 1-1.12-1.243l1.264-12A1.125 1.125 0 0 1 5.513 7.5h12.974c.576 0 1.059.435 1.119 1.007ZM8.625 10.5a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Zm7.5 0a.375.375 0 1 1-.75 0 .375.375 0 0 1 .75 0Z" />
        </svg>
    );
}
function IconSeguridad() {
    return (
        <svg className="h-5 w-5 shrink-0" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="M9 12.75 11.25 15 15 9.75m-3-7.036A11.959 11.959 0 0 1 3.598 6 11.99 11.99 0 0 0 3 9.749c0 5.592 3.824 10.29 9 11.623 5.176-1.332 9-6.03 9-11.622 0-1.31-.21-2.571-.598-3.751h-.152c-3.196 0-6.1-1.248-8.25-3.285Z" />
        </svg>
    );
}
function IconChevronLeft() {
    return (
        <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="m15.75 19.5-7.5-7.5 7.5-7.5" />
        </svg>
    );
}
function IconChevronRight() {
    return (
        <svg className="h-5 w-5" fill="none" viewBox="0 0 24 24" strokeWidth={1.5} stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="m8.25 4.5 7.5 7.5-7.5 7.5" />
        </svg>
    );
}

// ─── Navegación ───────────────────────────────────────────────────────────────
const NAV: NavGroup[] = [
    {
        group: 'Principal',
        items: [
            { label: 'Dashboard', href: '/', icon: <IconHome /> },
        ],
    },
    {
        group: 'Finanzas',
        items: [
            { label: 'Cartera', href: '/cartera/listado', icon: <IconCartera /> },
            { label: 'Tesorería', href: '/transacciones/listado', icon: <IconTesoreria /> },
        ],
    },
    {
        group: 'Contabilidad',
        items: [
            { label: 'Catálogo de Cuentas', href: '/contabilidad/cuentas', icon: <IconContabilidad /> },
        ],
    },
    {
        group: 'Organización',
        items: [
            { label: 'Miembros', href: '/miembros', icon: <IconMiembros /> },
            { label: 'Eventos', href: '/eventos', icon: <IconEventos /> },
            { label: 'Merchandising', href: '/merchandising', icon: <IconMerchandising /> },
        ],
    },
    {
        group: 'Administración',
        items: [
            { label: 'Seguridad', href: '/seguridad', icon: <IconSeguridad /> },
        ],
    },
];

// ─── Componente ───────────────────────────────────────────────────────────────
export default function Sidebar() {
    const pathname = usePathname();
    const [collapsed, setCollapsed] = useState(false);

    return (
        <aside
            className={`relative flex h-full flex-col border-r border-slate-200 bg-slate-900 transition-all duration-300 ${collapsed ? 'w-16' : 'w-60'}`}
        >
            {/* ── Logo / Marca ── */}
            <div className={`flex items-center gap-3 px-4 py-5 ${collapsed ? 'justify-center' : ''}`}>
                <span className="flex h-8 w-8 shrink-0 items-center justify-center rounded-lg bg-amber-500 text-sm font-black text-slate-900">
                    L
                </span>
                {!collapsed && (
                    <span className="truncate text-sm font-bold leading-tight text-white">
                        L.A.M.A. <br />
                        <span className="text-xs font-normal text-slate-400">Medellín</span>
                    </span>
                )}
            </div>

            {/* ── Navegación ── */}
            <nav className="flex-1 overflow-y-auto px-2 py-2">
                {NAV.map((group) => (
                    <div key={group.group} className="mb-4">
                        {!collapsed && (
                            <p className="mb-1 px-2 text-[10px] font-semibold uppercase tracking-widest text-slate-500">
                                {group.group}
                            </p>
                        )}
                        <ul className="space-y-0.5">
                            {group.items.map((item) => {
                                const isActive = pathname === item.href || (item.href !== '/' && pathname.startsWith(item.href));
                                return (
                                    <li key={item.href}>
                                        <Link
                                            href={item.href}
                                            title={collapsed ? item.label : undefined}
                                            className={`flex items-center gap-3 rounded-lg px-2 py-2 text-sm font-medium transition-colors ${
                                                isActive
                                                    ? 'bg-amber-500 text-slate-900'
                                                    : 'text-slate-300 hover:bg-slate-800 hover:text-white'
                                            } ${collapsed ? 'justify-center' : ''}`}
                                        >
                                            {item.icon}
                                            {!collapsed && <span className="truncate">{item.label}</span>}
                                        </Link>
                                    </li>
                                );
                            })}
                        </ul>
                    </div>
                ))}
            </nav>

            {/* ── Botón colapsar ── */}
            <button
                type="button"
                onClick={() => setCollapsed((prev) => !prev)}
                title={collapsed ? 'Expandir menú' : 'Colapsar menú'}
                className="flex w-full items-center justify-center border-t border-slate-700 py-3 text-slate-400 transition-colors hover:bg-slate-800 hover:text-white"
            >
                {collapsed ? <IconChevronRight /> : <IconChevronLeft />}
            </button>
        </aside>
    );
}
