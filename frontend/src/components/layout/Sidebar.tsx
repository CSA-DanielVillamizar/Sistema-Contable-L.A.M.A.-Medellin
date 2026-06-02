'use client';

import {
    CalendarDays,
    ChevronLeft,
    LayoutDashboard,
    Menu,
    Shield,
    Store,
    UsersRound,
    Wallet,
    WalletCards,
} from 'lucide-react';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useState } from 'react';

// ---------------------------------------------------------------------------
// Definición de la navegación principal del ERP
// ---------------------------------------------------------------------------
const NAV_ITEMS = [
    {
        label: 'Dashboard',
        href: '/',
        icon: <LayoutDashboard size={20} strokeWidth={2} />,
    },
    {
        label: 'Cartera',
        href: '/cartera',
        icon: <WalletCards size={20} strokeWidth={2} />,
    },
    {
        label: 'Tesorería',
        href: '/tesoreria',
        icon: <Wallet size={20} strokeWidth={2} />,
    },
    {
        label: 'Merchandising',
        href: '/merchandising',
        icon: <Store size={20} strokeWidth={2} />,
    },
    {
        label: 'Miembros',
        href: '/miembros',
        icon: <UsersRound size={20} strokeWidth={2} />,
    },
    {
        label: 'Eventos',
        href: '/eventos',
        icon: <CalendarDays size={20} strokeWidth={2} />,
    },
    {
        label: 'Seguridad',
        href: '/seguridad',
        icon: <Shield size={20} strokeWidth={2} />,
    },
] as const;

// ---------------------------------------------------------------------------
// Sidebar Component
// ---------------------------------------------------------------------------
export default function Sidebar() {
    const pathname = usePathname();
    const [collapsed, setCollapsed] = useState(false);

    return (
        <aside
            className={`
                flex h-screen flex-col border-r border-slate-200 bg-slate-900
                transition-all duration-300 ease-in-out
                ${collapsed ? 'w-16' : 'w-60'}
            `}
        >
            {/* Logo / Marca */}
            <div className="flex h-16 items-center justify-between px-4">
                {!collapsed && (
                    <span className="truncate text-sm font-bold tracking-widest text-white uppercase">
                        L.A.M.A.
                    </span>
                )}
                <button
                    type="button"
                    onClick={() => setCollapsed((prev) => !prev)}
                    aria-label={collapsed ? 'Expandir menú' : 'Contraer menú'}
                    className="rounded-md p-1.5 text-slate-400 hover:bg-slate-800 hover:text-white"
                >
                    {collapsed ? (
                        <Menu size={20} strokeWidth={2} />
                    ) : (
                        <ChevronLeft size={20} strokeWidth={2} />
                    )}
                </button>
            </div>

            {/* Separador */}
            <div className="mx-3 border-t border-slate-700" />

            {/* Navegación */}
            <nav className="mt-3 flex flex-1 flex-col gap-1 overflow-y-auto px-2 pb-4">
                {NAV_ITEMS.map((item) => {
                    // La ruta "/" solo está activa cuando el path es exactamente "/"
                    const isActive =
                        item.href === '/'
                            ? pathname === '/'
                            : pathname === item.href || pathname.startsWith(`${item.href}/`);

                    return (
                        <Link
                            key={item.href}
                            href={item.href}
                            title={collapsed ? item.label : undefined}
                            className={`
                                flex items-center gap-3 rounded-lg px-2.5 py-2.5 text-sm font-medium
                                transition-colors duration-150
                                ${isActive
                                    ? 'bg-slate-700 text-white'
                                    : 'text-slate-400 hover:bg-slate-800 hover:text-white'
                                }
                                ${collapsed ? 'justify-center' : ''}
                            `}
                        >
                            <span className="shrink-0">{item.icon}</span>
                            {!collapsed && <span className="truncate">{item.label}</span>}
                        </Link>
                    );
                })}
            </nav>

            {/* Pie del sidebar */}
            {!collapsed && (
                <div className="border-t border-slate-700 px-4 py-3">
                    <p className="text-xs text-slate-500">Capítulo Región Norte</p>
                    <p className="text-xs font-semibold text-slate-400">Medellín</p>
                </div>
            )}
        </aside>
    );
}
