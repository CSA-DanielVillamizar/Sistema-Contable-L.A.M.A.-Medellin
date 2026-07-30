'use client';

import {
    ArrowDownCircle,
    ArrowUpCircle,
    Bike,
    BookOpenText,
    Boxes,
    CalendarDays,
    ChevronDown,
    ChevronLeft,
    ClipboardCheck,
    FileBarChart,
    FileSpreadsheet,
    FolderKanban,
    HandCoins,
    Landmark,
    LayoutDashboard,
    ListTree,
    Menu,
    Receipt,
    Settings,
    Shield,
    Store,
    UserSearch,
    UsersRound,
    Wallet,
    WalletCards,
} from 'lucide-react';
import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useState } from 'react';
import { TRIBUTARIO_ALLOWED_ROLES } from '@/lib/authRoles';
import { useRoleAccess } from '@/lib/useRoleAccess';

// ---------------------------------------------------------------------------
// Navegación principal
//
// El alcance vigente es Phase 0 + Phase 1 del backlog: configuración base,
// contabilidad general, tesorería y cuotas con cartera. Eso ocupa el menú
// principal.
//
// Los módulos de club (eventos, proyectos, merchandising) son Phase 3+ y están
// construidos a medias. No se ocultan: quedan agrupados en una sección
// desplegable y separada, para dejar claro que no forman parte del núcleo
// contable y que sus cifras se apoyan en una contabilidad que apenas está
// completando sus controles.
//
// La administración de catálogos va en su propio grupo y solo se muestra a
// Admin y Tesorero: cambiar una cuenta bancaria o su cuenta contable altera la
// contrapartida de todos los movimientos que se registren después.
// ---------------------------------------------------------------------------
type Grupo = 'contable' | 'transacciones' | 'tributario' | 'club' | 'admin';

type NavItem = {
    label: string;
    href: string;
    icon: React.ReactNode;
    grupo: Grupo;
};

const NAV_ITEMS: NavItem[] = [
    {
        label: 'Dashboard',
        href: '/',
        icon: <LayoutDashboard size={20} strokeWidth={2} />,
        grupo: 'contable',
    },
    {
        label: 'Contabilidad',
        href: '/contabilidad/comprobantes',
        icon: <BookOpenText size={20} strokeWidth={2} />,
        grupo: 'contable',
    },
    {
        label: 'Plan de cuentas',
        href: '/contabilidad/cuentas',
        icon: <ListTree size={20} strokeWidth={2} />,
        grupo: 'contable',
    },
    {
        label: 'Tesorería',
        href: '/tesoreria',
        icon: <Wallet size={20} strokeWidth={2} />,
        grupo: 'contable',
    },
    {
        label: 'Cartera',
        href: '/cartera',
        icon: <WalletCards size={20} strokeWidth={2} />,
        grupo: 'contable',
    },
    {
        label: 'Donaciones',
        href: '/donaciones',
        icon: <HandCoins size={20} strokeWidth={2} />,
        grupo: 'contable',
    },
    {
        label: 'Miembros',
        href: '/miembros',
        icon: <UsersRound size={20} strokeWidth={2} />,
        grupo: 'contable',
    },
    {
        label: 'Reportes',
        href: '/reportes',
        icon: <FileBarChart size={20} strokeWidth={2} />,
        grupo: 'contable',
    },
    {
        label: 'Seguridad',
        href: '/seguridad',
        icon: <Shield size={20} strokeWidth={2} />,
        grupo: 'contable',
    },

    // --- Transacciones: registro directo de movimientos ---
    {
        label: 'Registrar ingreso',
        href: '/transacciones/ingreso',
        icon: <ArrowDownCircle size={20} strokeWidth={2} />,
        grupo: 'transacciones',
    },
    {
        label: 'Registrar egreso',
        href: '/transacciones/egreso',
        icon: <ArrowUpCircle size={20} strokeWidth={2} />,
        grupo: 'transacciones',
    },
    {
        label: 'Listado',
        href: '/transacciones/listado',
        icon: <Receipt size={20} strokeWidth={2} />,
        grupo: 'transacciones',
    },

    // --- Tributario: obligaciones ante la DIAN ---
    {
        label: 'Exógena',
        href: '/tributario/exogena',
        icon: <FileSpreadsheet size={20} strokeWidth={2} />,
        grupo: 'tributario',
    },
    {
        label: 'Calidad de datos',
        href: '/tributario/calidad-datos',
        icon: <ClipboardCheck size={20} strokeWidth={2} />,
        grupo: 'tributario',
    },
    {
        label: 'Beneficiarios finales',
        href: '/tributario/beneficiarios-finales',
        icon: <UserSearch size={20} strokeWidth={2} />,
        grupo: 'tributario',
    },

    // --- Gestion del club: fuera del alcance contable (Phase 3+) ---
    {
        label: 'Merchandising',
        href: '/merchandising',
        icon: <Store size={20} strokeWidth={2} />,
        grupo: 'club',
    },
    {
        label: 'Eventos',
        href: '/eventos',
        icon: <CalendarDays size={20} strokeWidth={2} />,
        grupo: 'club',
    },
    {
        label: 'Proyectos',
        href: '/proyectos',
        icon: <FolderKanban size={20} strokeWidth={2} />,
        grupo: 'club',
    },

    // --- Administracion de catalogos: solo Admin y Tesorero ---
    {
        label: 'Cuentas bancarias',
        href: '/administracion/cuentas-bancarias',
        icon: <Landmark size={20} strokeWidth={2} />,
        grupo: 'admin',
    },
    {
        label: 'Centros de costo',
        href: '/administracion/centros-costo',
        icon: <Boxes size={20} strokeWidth={2} />,
        grupo: 'admin',
    },
];

const NAV_CONTABLE = NAV_ITEMS.filter((item) => item.grupo === 'contable');
const NAV_TRANSACCIONES = NAV_ITEMS.filter((item) => item.grupo === 'transacciones');
const NAV_TRIBUTARIO = NAV_ITEMS.filter((item) => item.grupo === 'tributario');
const NAV_CLUB = NAV_ITEMS.filter((item) => item.grupo === 'club');
const NAV_ADMIN = NAV_ITEMS.filter((item) => item.grupo === 'admin');

/**
 * Solo estos roles ven la administración de catálogos. Es control de UI: la
 * autorización real la impone el backend con [Authorize(Roles = ...)].
 */
const ROLES_ADMINISTRACION = ['Admin', 'Tesorero'] as const;

// ---------------------------------------------------------------------------
// Sección desplegable del menú
// ---------------------------------------------------------------------------
type SeccionDesplegableProps = {
    titulo: string;
    icono: React.ReactNode;
    items: NavItem[];
    abierta: boolean;
    onToggle: () => void;
    collapsed: boolean;
    claseItem: (activo: boolean) => string;
    esActivo: (href: string) => boolean;
    nota: string;
};

function SeccionDesplegable({
    titulo,
    icono,
    items,
    abierta,
    onToggle,
    collapsed,
    claseItem,
    esActivo,
    nota,
}: SeccionDesplegableProps) {
    return (
        <div className="mt-4 border-t border-slate-700 pt-3">
            <button
                type="button"
                onClick={onToggle}
                title={collapsed ? titulo : undefined}
                aria-expanded={abierta}
                className={claseItem(false) + ' w-full'}
            >
                <span className="shrink-0">{icono}</span>
                {!collapsed && (
                    <>
                        <span className="flex-1 truncate text-left">{titulo}</span>
                        <ChevronDown
                            size={16}
                            strokeWidth={2}
                            className={`shrink-0 transition-transform duration-150 ${abierta ? 'rotate-180' : ''}`}
                        />
                    </>
                )}
            </button>

            {abierta && (
                <div className={collapsed ? 'mt-1 flex flex-col gap-1' : 'mt-1 flex flex-col gap-1 pl-3'}>
                    {items.map((item) => (
                        <Link
                            key={item.href}
                            href={item.href}
                            title={collapsed ? item.label : undefined}
                            className={claseItem(esActivo(item.href))}
                        >
                            <span className="shrink-0">{item.icon}</span>
                            {!collapsed && <span className="truncate">{item.label}</span>}
                        </Link>
                    ))}
                    {!collapsed && (
                        <p className="px-2.5 pt-1 text-[11px] leading-tight text-slate-500">{nota}</p>
                    )}
                </div>
            )}
        </div>
    );
}

// ---------------------------------------------------------------------------
// Sidebar Component
// ---------------------------------------------------------------------------
export default function Sidebar() {
    const pathname = usePathname();
    const [collapsed, setCollapsed] = useState(false);
    const [clubAbierto, setClubAbierto] = useState(false);
    const [adminAbierto, setAdminAbierto] = useState(false);
    const [transaccionesAbierto, setTransaccionesAbierto] = useState(false);
    const [tributarioAbierto, setTributarioAbierto] = useState(false);
    const { canAccess: puedeAdministrar } = useRoleAccess(ROLES_ADMINISTRACION);
    const { canAccess: puedeVerTributario } = useRoleAccess(TRIBUTARIO_ALLOWED_ROLES);

    const esActivo = (href: string) =>
        href === '/' ? pathname === '/' : pathname === href || pathname.startsWith(`${href}/`);

    const claseItem = (activo: boolean) => `
        flex items-center gap-3 rounded-lg px-2.5 py-2.5 text-sm font-medium
        transition-colors duration-150
        ${activo ? 'bg-slate-700 text-white' : 'text-slate-400 hover:bg-slate-800 hover:text-white'}
        ${collapsed ? 'justify-center' : ''}
    `;

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
                {NAV_CONTABLE.map((item) => {
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

                <SeccionDesplegable
                    titulo="Transacciones"
                    icono={<Receipt size={20} strokeWidth={2} />}
                    items={NAV_TRANSACCIONES}
                    abierta={transaccionesAbierto}
                    onToggle={() => setTransaccionesAbierto((prev) => !prev)}
                    collapsed={collapsed}
                    claseItem={claseItem}
                    esActivo={esActivo}
                    nota="Registro directo de movimientos"
                />

                {/* Tributario: solo lo consulta quien responde ante la DIAN */}
                {puedeVerTributario && (
                    <SeccionDesplegable
                        titulo="Tributario"
                        icono={<FileSpreadsheet size={20} strokeWidth={2} />}
                        items={NAV_TRIBUTARIO}
                        abierta={tributarioAbierto}
                        onToggle={() => setTributarioAbierto((prev) => !prev)}
                        collapsed={collapsed}
                        claseItem={claseItem}
                        esActivo={esActivo}
                        nota="Requiere rol Contador o Admin"
                    />
                )}

                {/* Administracion de catalogos: solo para Admin y Tesorero */}
                {puedeAdministrar && (
                    <SeccionDesplegable
                        titulo="Administración"
                        icono={<Settings size={20} strokeWidth={2} />}
                        items={NAV_ADMIN}
                        abierta={adminAbierto}
                        onToggle={() => setAdminAbierto((prev) => !prev)}
                        collapsed={collapsed}
                        claseItem={claseItem}
                        esActivo={esActivo}
                        nota="Requiere rol Admin o Tesorero"
                    />
                )}

                {/* Gestion del club: agrupada y fuera del nucleo contable */}
                <SeccionDesplegable
                    titulo="Gestión del club"
                    icono={<Bike size={20} strokeWidth={2} />}
                    items={NAV_CLUB}
                    abierta={clubAbierto}
                    onToggle={() => setClubAbierto((prev) => !prev)}
                    collapsed={collapsed}
                    claseItem={claseItem}
                    esActivo={esActivo}
                    nota="Fuera del alcance contable vigente"
                />
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
