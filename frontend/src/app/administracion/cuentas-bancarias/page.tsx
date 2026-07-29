'use client';

import ModalCuentaBancaria from '@/features/administracion/components/ModalCuentaBancaria';
import {
    useCambiarEstadoCuentaBancaria,
    useCuentasBancariasAdmin,
} from '@/features/administracion/hooks/useCuentasBancariasAdmin';
import type { CuentaBancaria } from '@/features/administracion/services/cuentasBancariasService';
import { useRoleAccess } from '@/lib/useRoleAccess';
import { useState } from 'react';

/**
 * Administracion de cuentas bancarias.
 *
 * Antes no existia: las cuentas solo se creaban desde el seeder, asi que el
 * capitulo no podia dar de alta una cuenta nueva ni corregir la que quedo con
 * un numero de cuenta provisional tras la consolidacion de tesoreria.
 */
const ROLES_PERMITIDOS = ['Admin', 'Tesorero'] as const;

const formatoCOP = new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    maximumFractionDigits: 0,
});

export default function CuentasBancariasPage() {
    const { canAccess, isRoleReady } = useRoleAccess(ROLES_PERMITIDOS);
    const cuentasQuery = useCuentasBancariasAdmin();
    const cambiarEstado = useCambiarEstadoCuentaBancaria();

    const [modalAbierto, setModalAbierto] = useState(false);
    const [cuentaEnEdicion, setCuentaEnEdicion] = useState<CuentaBancaria | null>(null);
    const [errorEstado, setErrorEstado] = useState<string | null>(null);

    if (!isRoleReady) {
        return <p className="p-8 text-sm text-slate-500">Verificando permisos...</p>;
    }

    if (!canAccess) {
        return (
            <div className="p-8">
                <h1 className="text-xl font-bold text-slate-900">Sin permiso</h1>
                <p className="mt-2 text-sm text-slate-600">
                    La administración de cuentas bancarias requiere el rol Admin o Tesorero.
                </p>
            </div>
        );
    }

    const abrirAlta = () => {
        setCuentaEnEdicion(null);
        setModalAbierto(true);
    };

    const abrirEdicion = (cuenta: CuentaBancaria) => {
        setCuentaEnEdicion(cuenta);
        setModalAbierto(true);
    };

    const alternarEstado = async (cuenta: CuentaBancaria) => {
        setErrorEstado(null);

        try {
            await cambiarEstado.mutateAsync({ id: cuenta.id, esActivo: !cuenta.esActivo });
        } catch {
            setErrorEstado(
                cuenta.esActivo
                    ? `No fue posible desactivar "${cuenta.nombre}". Una cuenta con saldo debe trasladarse primero.`
                    : `No fue posible reactivar "${cuenta.nombre}".`,
            );
        }
    };

    const cuentas = cuentasQuery.data ?? [];

    return (
        <div className="p-8">
            <div className="flex items-start justify-between gap-4">
                <div>
                    <h1 className="text-2xl font-bold text-slate-900">Cuentas bancarias</h1>
                    <p className="mt-1 text-sm text-slate-600">
                        Toda la tesorería pasa por estas cuentas. Una cuenta inactiva conserva su historia
                        pero desaparece de los desplegables de ingresos, egresos y donaciones.
                    </p>
                </div>

                <button
                    type="button"
                    onClick={abrirAlta}
                    className="shrink-0 rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800"
                >
                    Nueva cuenta
                </button>
            </div>

            {errorEstado ? (
                <div className="mt-4 rounded-lg border border-rose-200 bg-rose-50 px-3 py-2 text-sm text-rose-700">
                    {errorEstado}
                </div>
            ) : null}

            <div className="mt-6 overflow-x-auto rounded-xl border border-slate-200 bg-white">
                <table className="w-full text-sm">
                    <thead className="border-b border-slate-200 bg-slate-50 text-left text-slate-600">
                        <tr>
                            <th className="px-4 py-3 font-medium">Nombre</th>
                            <th className="px-4 py-3 font-medium">Número de cuenta</th>
                            <th className="px-4 py-3 text-right font-medium">Saldo</th>
                            <th className="px-4 py-3 font-medium">Estado</th>
                            <th className="px-4 py-3 text-right font-medium">Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        {cuentasQuery.isLoading ? (
                            <tr>
                                <td colSpan={5} className="px-4 py-6 text-center text-slate-500">
                                    Cargando cuentas...
                                </td>
                            </tr>
                        ) : null}

                        {cuentasQuery.isError ? (
                            <tr>
                                <td colSpan={5} className="px-4 py-6 text-center text-rose-700">
                                    No fue posible cargar las cuentas bancarias.
                                </td>
                            </tr>
                        ) : null}

                        {!cuentasQuery.isLoading && !cuentasQuery.isError && cuentas.length === 0 ? (
                            <tr>
                                <td colSpan={5} className="px-4 py-6 text-center text-slate-500">
                                    Todavía no hay cuentas bancarias registradas.
                                </td>
                            </tr>
                        ) : null}

                        {cuentas.map((cuenta) => (
                            <tr key={cuenta.id} className="border-b border-slate-100 last:border-0">
                                <td className="px-4 py-3 font-medium text-slate-900">{cuenta.nombre}</td>
                                <td className="px-4 py-3 text-slate-600">{cuenta.numeroCuenta}</td>
                                <td className="px-4 py-3 text-right tabular-nums text-slate-900">
                                    {formatoCOP.format(cuenta.saldoActual)}
                                </td>
                                <td className="px-4 py-3">
                                    <span
                                        className={
                                            cuenta.esActivo
                                                ? 'rounded-full bg-emerald-100 px-2.5 py-0.5 text-xs font-medium text-emerald-800'
                                                : 'rounded-full bg-slate-200 px-2.5 py-0.5 text-xs font-medium text-slate-700'
                                        }
                                    >
                                        {cuenta.esActivo ? 'Activa' : 'Inactiva'}
                                    </span>
                                </td>
                                <td className="px-4 py-3">
                                    <div className="flex items-center justify-end gap-2">
                                        <button
                                            type="button"
                                            onClick={() => abrirEdicion(cuenta)}
                                            className="rounded-lg border border-slate-300 px-3 py-1.5 text-xs font-medium text-slate-700 transition hover:bg-slate-50"
                                        >
                                            Editar
                                        </button>
                                        <button
                                            type="button"
                                            onClick={() => void alternarEstado(cuenta)}
                                            disabled={cambiarEstado.isPending}
                                            className="rounded-lg border border-slate-300 px-3 py-1.5 text-xs font-medium text-slate-700 transition hover:bg-slate-50 disabled:cursor-not-allowed disabled:opacity-60"
                                        >
                                            {cuenta.esActivo ? 'Desactivar' : 'Reactivar'}
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>

            {/* Montar y desmontar en vez de ocultar: asi el formulario arranca
                con los datos de la cuenta elegida sin sincronizarlo por efecto. */}
            {modalAbierto ? (
                <ModalCuentaBancaria
                    key={cuentaEnEdicion?.id ?? 'nueva'}
                    cuenta={cuentaEnEdicion}
                    onCerrar={() => setModalAbierto(false)}
                />
            ) : null}
        </div>
    );
}
