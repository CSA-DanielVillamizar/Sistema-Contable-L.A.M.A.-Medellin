'use client';

import {
    ArrowDownCircle,
    ArrowUpCircle,
    Ban,
    Banknote,
    Bike,
    BookMarked,
    BookOpenText,
    Boxes,
    CalendarCheck,
    CalendarDays,
    ChevronDown,
    ChevronLeft,
    ClipboardCheck,
    CreditCard,
    FileBarChart,
    FileSpreadsheet,
    FileText,
    FolderKanban,
    HandCoins,
    HeartHandshake,
    Landmark,
    LayoutDashboard,
    Link2,
    ListTree,
    Megaphone,
    Menu,
    PackageSearch,
    PiggyBank,
    Receipt,
    ReceiptText,
    Settings,
    Shield,
    SlidersHorizontal,
    Store,
    Target,
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
// Navegacion principal
//
// El menu esta agrupado por area de trabajo, no por pantalla. Antes eran
// diecinueve enlaces sueltos uno detras de otro: para llegar a "Cierre de
// periodo" habia que recorrer la lista entera, y con la barra contraida los
// nombres largos se salian del ancho. Agrupado se ve una fila por area y solo
// se despliega aquella en la que se esta trabajando.
//
// Cada grupo corresponde a una responsabilidad distinta dentro de la fundacion:
// quien registra movimientos no entra a contabilidad, quien cierra el periodo no
// administra catalogos. Por eso el corte es por area y no, por ejemplo,
// alfabetico.
//
// Tributario y Administracion ademas exigen rol. Es control de interfaz: la
// autorizacion real la impone el backend con [Authorize(Roles = ...)]; aqui solo
// se evita ofrecer pantallas que el usuario no va a poder usar.
// ---------------------------------------------------------------------------
type NavItem = {
    label: string;
    href: string;
    icon: React.ReactNode;
};

type Seccion = {
    id: string;
    titulo: string;
    icono: React.ReactNode;
    items: NavItem[];
    /** Cuando esta presente, la seccion solo se muestra a estos roles. */
    roles?: readonly string[];
    /** Aclaracion al pie del grupo. Solo donde el alcance no es evidente. */
    nota?: string;
};

/** Enlaces que no pertenecen a ningun area: se entra a ellos desde cualquiera. */
const NAV_SUELTOS: NavItem[] = [
    {
        label: 'Dashboard',
        href: '/',
        icon: <LayoutDashboard size={20} strokeWidth={2} />,
    },
    {
        label: 'Reportes',
        href: '/reportes',
        icon: <FileBarChart size={20} strokeWidth={2} />,
    },
];

const SECCIONES: Seccion[] = [
    {
        id: 'contabilidad',
        titulo: 'Contabilidad',
        icono: <BookOpenText size={20} strokeWidth={2} />,
        items: [
            {
                label: 'Comprobantes',
                href: '/contabilidad/comprobantes',
                icon: <FileText size={20} strokeWidth={2} />,
            },
            {
                label: 'Plan de cuentas',
                href: '/contabilidad/cuentas',
                icon: <ListTree size={20} strokeWidth={2} />,
            },
            {
                label: 'Libros',
                href: '/contabilidad/libros',
                icon: <BookMarked size={20} strokeWidth={2} />,
            },
            {
                label: 'Cierre de periodo',
                href: '/contabilidad/cierre',
                icon: <CalendarCheck size={20} strokeWidth={2} />,
            },
            {
                label: 'Recibos',
                href: '/contabilidad/recibos',
                icon: <ReceiptText size={20} strokeWidth={2} />,
            },
            {
                label: 'Anulaciones',
                href: '/contabilidad/anulaciones',
                icon: <Ban size={20} strokeWidth={2} />,
            },
        ],
    },
    {
        id: 'tesoreria',
        titulo: 'Tesorería',
        icono: <Wallet size={20} strokeWidth={2} />,
        items: [
            {
                label: 'Cuentas y saldos',
                href: '/tesoreria',
                icon: <PiggyBank size={20} strokeWidth={2} />,
            },
            {
                label: 'Registrar ingreso',
                href: '/transacciones/ingreso',
                icon: <ArrowDownCircle size={20} strokeWidth={2} />,
            },
            {
                label: 'Registrar egreso',
                href: '/transacciones/egreso',
                icon: <ArrowUpCircle size={20} strokeWidth={2} />,
            },
            {
                label: 'Movimientos',
                href: '/transacciones/listado',
                icon: <Receipt size={20} strokeWidth={2} />,
            },
        ],
    },
    {
        id: 'cobros-y-pagos',
        titulo: 'Cobros y pagos',
        icono: <CreditCard size={20} strokeWidth={2} />,
        items: [
            {
                label: 'Cartera',
                href: '/cartera',
                icon: <WalletCards size={20} strokeWidth={2} />,
            },
            {
                label: 'Cuentas por pagar',
                href: '/cuentas-por-pagar',
                icon: <Banknote size={20} strokeWidth={2} />,
            },
        ],
    },
    {
        id: 'miembros-y-donaciones',
        titulo: 'Miembros y donaciones',
        icono: <HeartHandshake size={20} strokeWidth={2} />,
        items: [
            {
                label: 'Miembros',
                href: '/miembros',
                icon: <UsersRound size={20} strokeWidth={2} />,
            },
            {
                label: 'Donaciones',
                href: '/donaciones',
                icon: <HandCoins size={20} strokeWidth={2} />,
            },
            {
                label: 'Campañas',
                href: '/donaciones/campanas',
                icon: <Megaphone size={20} strokeWidth={2} />,
            },
        ],
    },
    {
        id: 'tributario',
        titulo: 'Tributario',
        icono: <FileSpreadsheet size={20} strokeWidth={2} />,
        roles: TRIBUTARIO_ALLOWED_ROLES,
        nota: 'Obligaciones ante la DIAN',
        items: [
            {
                label: 'Exógena',
                href: '/tributario/exogena',
                icon: <FileSpreadsheet size={20} strokeWidth={2} />,
            },
            {
                label: 'Calidad de datos',
                href: '/tributario/calidad-datos',
                icon: <ClipboardCheck size={20} strokeWidth={2} />,
            },
            {
                label: 'Beneficiarios',
                href: '/tributario/beneficiarios-finales',
                icon: <UserSearch size={20} strokeWidth={2} />,
            },
        ],
    },
    {
        id: 'club',
        titulo: 'Gestión del club',
        icono: <Bike size={20} strokeWidth={2} />,
        nota: 'Fuera del alcance contable vigente',
        items: [
            {
                label: 'Eventos',
                href: '/eventos',
                icon: <CalendarDays size={20} strokeWidth={2} />,
            },
            {
                label: 'Proyectos',
                href: '/proyectos',
                icon: <FolderKanban size={20} strokeWidth={2} />,
            },
            {
                label: 'Rendición',
                href: '/proyectos/rendicion',
                icon: <Target size={20} strokeWidth={2} />,
            },
            {
                label: 'Merchandising',
                href: '/merchandising',
                icon: <Store size={20} strokeWidth={2} />,
            },
            {
                label: 'Inventario',
                href: '/merchandising/reporte',
                icon: <PackageSearch size={20} strokeWidth={2} />,
            },
        ],
    },
    {
        id: 'administracion',
        titulo: 'Administración',
        icono: <Settings size={20} strokeWidth={2} />,
        roles: ['Admin', 'Tesorero'],
        nota: 'Cambiar un catálogo afecta los movimientos que se registren después',
        items: [
            {
                label: 'Cuentas bancarias',
                href: '/administracion/cuentas-bancarias',
                icon: <Landmark size={20} strokeWidth={2} />,
            },
            {
                label: 'Centros de costo',
                href: '/administracion/centros-costo',
                icon: <Boxes size={20} strokeWidth={2} />,
            },
            {
                label: 'Mapeo contable',
                href: '/administracion/mapeo-contable',
                icon: <Link2 size={20} strokeWidth={2} />,
            },
            {
                label: 'Parámetros de cartera',
                href: '/administracion/parametros-cartera',
                icon: <SlidersHorizontal size={20} strokeWidth={2} />,
            },
            {
                label: 'Accesos',
                href: '/seguridad',
                icon: <Shield size={20} strokeWidth={2} />,
            },
        ],
    },
];

/** Un grupo sin exigencia de rol se evalua contra esta lista, que no se usa. */
const SIN_EXIGENCIA_DE_ROL: readonly string[] = [];

const TODOS_LOS_HREF = [
    ...NAV_SUELTOS.map((item) => item.href),
    ...SECCIONES.flatMap((seccion) => seccion.items.map((item) => item.href)),
];

function cubreLaRuta(href: string, pathname: string): boolean {
    return href === '/'
        ? pathname === '/'
        : pathname === href || pathname.startsWith(`${href}/`);
}

/**
 * Enlace que corresponde a la ruta actual, uno solo.
 *
 * Se queda con la coincidencia mas larga porque hay rutas que son prefijo de
 * otras: estando en /proyectos/rendicion, comparar por prefijo marcaba tambien
 * "Proyectos", y el menu senalaba dos sitios a la vez. Pasaba igual con
 * donaciones y merchandising.
 */
function hrefActivo(pathname: string): string | null {
    return TODOS_LOS_HREF
        .filter((href) => cubreLaRuta(href, pathname))
        .sort((a, b) => b.length - a.length)[0] ?? null;
}

/**
 * Grupo al que pertenece la ruta actual, para abrirlo solo.
 *
 * Sin esto, entrar directo a una URL o recargar la pagina dejaba el menu
 * cerrado y sin ninguna pista de donde estaba parado el usuario.
 */
function seccionDeLaRuta(activo: string | null): string | null {
    if (!activo) {
        return null;
    }

    return SECCIONES.find((seccion) => seccion.items.some((item) => item.href === activo))?.id
        ?? null;
}

// ---------------------------------------------------------------------------
// Seccion desplegable del menu
// ---------------------------------------------------------------------------
type SeccionDesplegableProps = {
    seccion: Seccion;
    abierta: boolean;
    onToggle: () => void;
    collapsed: boolean;
    /** Href del enlace que corresponde a la ruta actual, si esta en el menu. */
    activo: string | null;
    claseItem: (activo: boolean) => string;
};

function SeccionDesplegable({
    seccion,
    abierta,
    onToggle,
    collapsed,
    activo,
    claseItem,
}: SeccionDesplegableProps) {
    const { canAccess } = useRoleAccess(seccion.roles ?? SIN_EXIGENCIA_DE_ROL);

    if (seccion.roles && !canAccess) {
        return null;
    }

    // Con la seccion cerrada, el titulo es lo unico que indica donde esta el
    // usuario: se resalta si la ruta actual vive dentro.
    const contieneLaRuta = seccion.items.some((item) => item.href === activo);

    return (
        <div>
            <button
                type="button"
                onClick={onToggle}
                title={collapsed ? seccion.titulo : undefined}
                aria-expanded={abierta}
                className={`${claseItem(contieneLaRuta && !abierta)} w-full`}
            >
                <span className="shrink-0">{seccion.icono}</span>
                {!collapsed && (
                    <>
                        <span className="min-w-0 flex-1 truncate text-left">{seccion.titulo}</span>
                        <ChevronDown
                            size={16}
                            strokeWidth={2}
                            className={`shrink-0 transition-transform duration-150 ${abierta ? 'rotate-180' : ''}`}
                        />
                    </>
                )}
            </button>

            {abierta && (
                <div
                    className={
                        collapsed
                            ? 'mt-0.5 flex flex-col gap-0.5'
                            : 'mt-0.5 ml-4 flex flex-col gap-0.5 border-l border-slate-700 pl-2'
                    }
                >
                    {seccion.items.map((item) => (
                        <Link
                            key={item.href}
                            href={item.href}
                            title={collapsed ? item.label : undefined}
                            className={claseItem(item.href === activo)}
                        >
                            <span className="shrink-0">{item.icon}</span>
                            {!collapsed && <span className="min-w-0 truncate">{item.label}</span>}
                        </Link>
                    ))}
                    {!collapsed && seccion.nota && (
                        <p className="px-2.5 pt-1 pb-2 text-[11px] leading-tight text-slate-500">
                            {seccion.nota}
                        </p>
                    )}
                </div>
            )}
        </div>
    );
}

// ---------------------------------------------------------------------------
// Sidebar
// ---------------------------------------------------------------------------
export default function Sidebar() {
    const pathname = usePathname();
    const [collapsed, setCollapsed] = useState(false);
    const activo = hrefActivo(pathname);
    const seccionActiva = seccionDeLaRuta(activo);
    // Por defecto solo esta abierta el area donde el usuario esta trabajando.
    // Aqui se guarda unicamente lo que el decidio distinto, para poder olvidarlo
    // al cambiar de area sin perder lo que abrio en el camino.
    const [aperturaManual, setAperturaManual] = useState<Record<string, boolean>>({});
    const [areaPrevia, setAreaPrevia] = useState(seccionActiva);

    if (areaPrevia !== seccionActiva) {
        setAreaPrevia(seccionActiva);
        setAperturaManual({});
    }

    const estaAbierta = (id: string) => aperturaManual[id] ?? id === seccionActiva;

    const alternar = (id: string) => {
        setAperturaManual((previas) => ({ ...previas, [id]: !estaAbierta(id) }));
    };

    const claseItem = (activo: boolean) => `
        flex items-center gap-3 rounded-lg px-2.5 py-2 text-sm font-medium
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

            <div className="mx-3 border-t border-slate-700" />

            {/* Navegacion. `overflow-x-hidden` evita que un nombre largo empuje
                el ancho de la barra y aparezca desplazamiento horizontal. */}
            <nav className="mt-3 flex flex-1 flex-col gap-0.5 overflow-x-hidden overflow-y-auto px-2 pb-4">
                {NAV_SUELTOS.map((item) => (
                    <Link
                        key={item.href}
                        href={item.href}
                        title={collapsed ? item.label : undefined}
                        className={claseItem(item.href === activo)}
                    >
                        <span className="shrink-0">{item.icon}</span>
                        {!collapsed && <span className="min-w-0 truncate">{item.label}</span>}
                    </Link>
                ))}

                <div className="my-2 border-t border-slate-700" />

                {SECCIONES.map((seccion) => (
                    <SeccionDesplegable
                        key={seccion.id}
                        seccion={seccion}
                        abierta={estaAbierta(seccion.id)}
                        onToggle={() => alternar(seccion.id)}
                        collapsed={collapsed}
                        activo={activo}
                        claseItem={claseItem}
                    />
                ))}
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
