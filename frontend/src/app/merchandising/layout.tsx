'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';

const NAV_ITEMS = [
    { href: '/merchandising', label: 'Catálogo & Ventas', exact: true },
    { href: '/merchandising/inventario', label: 'Inventario', exact: false },
    { href: '/merchandising/ventas', label: 'Historial Ventas', exact: false },
    { href: '/merchandising/reportes', label: 'Reportes', exact: false },
] as const;

export default function MerchandisingLayout({ children }: { children: React.ReactNode }) {
    const pathname = usePathname();

    return (
        <div className="min-h-screen bg-slate-50">
            {/* Barra de módulo */}
            <nav className="border-b border-slate-200 bg-white shadow-sm">
                <div className="mx-auto flex w-full max-w-6xl flex-col gap-3 px-6 py-4 md:flex-row md:items-center md:justify-between">
                    <div className="flex items-center gap-2">
                        <Link
                            href="/"
                            className="text-sm text-slate-500 hover:text-slate-700"
                        >
                            ← Dashboard
                        </Link>
                        <span className="text-slate-300">/</span>
                        <span className="text-sm font-semibold text-indigo-700">Merchandising</span>
                    </div>

                    <div className="flex flex-wrap gap-1">
                        {NAV_ITEMS.map((item) => {
                            const isActive = item.exact
                                ? pathname === item.href
                                : pathname.startsWith(item.href);

                            return (
                                <Link
                                    key={item.href}
                                    href={item.href}
                                    className={[
                                        'rounded-lg px-4 py-1.5 text-sm font-medium transition-colors',
                                        isActive
                                            ? 'bg-indigo-600 text-white'
                                            : 'text-slate-600 hover:bg-slate-100',
                                    ].join(' ')}
                                >
                                    {item.label}
                                </Link>
                            );
                        })}
                    </div>
                </div>
            </nav>

            {/* Contenido de la sub-ruta */}
            {children}
        </div>
    );
}
