'use client';

import {
    useActualizarCuentaBancaria,
    useCrearCuentaBancaria,
} from '@/features/administracion/hooks/useCuentasBancariasAdmin';
import type { CuentaBancaria } from '@/features/administracion/services/cuentasBancariasService';
import { useCuentasContables } from '@/features/contabilidad/hooks/useCuentasContables';
import { useState } from 'react';

type ModalCuentaBancariaProps = {
    /** null = alta; con valor = edicion de esa cuenta. */
    cuenta: CuentaBancaria | null;
    onCerrar: () => void;
};

const VACIO = { nombre: '', numeroCuenta: '', cuentaContableId: '' };

/**
 * El estado inicial se resuelve en el montaje, no en un efecto: la pagina monta
 * y desmonta este modal al abrirlo y cerrarlo, y le pasa una `key` distinta por
 * cuenta. Sincronizarlo con useEffect obligaria a un render intermedio con los
 * datos de la cuenta anterior.
 */
export default function ModalCuentaBancaria({ cuenta, onCerrar }: ModalCuentaBancariaProps) {
    const crear = useCrearCuentaBancaria();
    const actualizar = useActualizarCuentaBancaria();
    const cuentasContablesQuery = useCuentasContables();

    const [values, setValues] = useState(() =>
        cuenta
            ? {
                  nombre: cuenta.nombre,
                  numeroCuenta: cuenta.numeroCuenta,
                  cuentaContableId: cuenta.cuentaContableId,
              }
            : VACIO,
    );
    const [error, setError] = useState<string | null>(null);

    // Solo el disponible puede respaldar una cuenta bancaria; el backend impone
    // la misma regla, aqui solo se evita ofrecer opciones que va a rechazar.
    const cuentasDisponibles = (cuentasContablesQuery.data ?? []).filter(
        (item) => item.codigo.startsWith('11') && item.permiteMovimiento,
    );

    const onChange = (campo: keyof typeof VACIO, valor: string) => {
        setValues((previo) => ({ ...previo, [campo]: valor }));
    };

    const onSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        if (!values.nombre.trim() || !values.numeroCuenta.trim() || !values.cuentaContableId) {
            setError('Nombre, numero de cuenta y cuenta contable son obligatorios.');
            return;
        }

        setError(null);

        const payload = {
            nombre: values.nombre.trim(),
            numeroCuenta: values.numeroCuenta.trim(),
            cuentaContableId: values.cuentaContableId,
        };

        try {
            if (cuenta) {
                await actualizar.mutateAsync({ id: cuenta.id, ...payload });
            } else {
                await crear.mutateAsync(payload);
            }

            onCerrar();
        } catch {
            setError('No fue posible guardar la cuenta bancaria.');
        }
    };

    const guardando = crear.isPending || actualizar.isPending;

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/55 p-4">
            <div className="w-full max-w-xl rounded-2xl border border-slate-200 bg-white p-6 shadow-2xl">
                <div className="flex items-start justify-between gap-4">
                    <div>
                        <h2 className="text-xl font-bold text-slate-900">
                            {cuenta ? 'Editar cuenta bancaria' : 'Nueva cuenta bancaria'}
                        </h2>
                        <p className="mt-1 text-sm text-slate-600">
                            El saldo no se edita aquí: resulta de los movimientos registrados.
                        </p>
                    </div>

                    <button
                        type="button"
                        onClick={onCerrar}
                        className="rounded-lg border border-slate-300 px-3 py-1.5 text-sm font-medium text-slate-700"
                    >
                        Cerrar
                    </button>
                </div>

                <form className="mt-5 grid grid-cols-1 gap-4" onSubmit={onSubmit}>
                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Nombre</label>
                        <input
                            type="text"
                            value={values.nombre}
                            onChange={(event) => onChange('nombre', event.target.value)}
                            placeholder="Bancolombia - Cuenta corriente"
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Número de cuenta</label>
                        <input
                            type="text"
                            value={values.numeroCuenta}
                            onChange={(event) => onChange('numeroCuenta', event.target.value)}
                            placeholder="123-456789-01"
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Cuenta contable</label>
                        <select
                            value={values.cuentaContableId}
                            onChange={(event) => onChange('cuentaContableId', event.target.value)}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        >
                            <option value="">Seleccione...</option>
                            {cuentasDisponibles.map((item) => (
                                <option key={item.id} value={item.id}>
                                    {item.codigo} - {item.descripcion}
                                </option>
                            ))}
                        </select>
                        {cuentasContablesQuery.isLoading ? (
                            <p className="mt-1 text-xs text-slate-500">Cargando plan de cuentas...</p>
                        ) : null}
                        {!cuentasContablesQuery.isLoading && cuentasDisponibles.length === 0 ? (
                            <p className="mt-1 text-xs text-amber-700">
                                No hay cuentas del disponible (11xx) que admitan movimiento.
                            </p>
                        ) : null}
                    </div>

                    {error ? (
                        <div className="rounded-lg border border-rose-200 bg-rose-50 px-3 py-2 text-sm text-rose-700">
                            {error}
                        </div>
                    ) : null}

                    <div className="flex items-center justify-end gap-2 pt-1">
                        <button
                            type="button"
                            onClick={onCerrar}
                            className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700"
                        >
                            Cancelar
                        </button>

                        <button
                            type="submit"
                            disabled={guardando}
                            className="rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-60"
                        >
                            {guardando ? 'Guardando...' : 'Guardar'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
