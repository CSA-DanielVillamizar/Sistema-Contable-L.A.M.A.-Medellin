'use client';

import {
    nombreTipoCentroCosto,
    useActualizarCentroCosto,
    useCentrosCostoAdmin,
    useCrearCentroCosto,
    TIPOS_CENTRO_COSTO,
    type CentroCosto,
} from '@/features/administracion/hooks/useCentrosCostoAdmin';
import { mensajeDeError } from '@/lib/apiClient';
import { useRoleAccess } from '@/lib/useRoleAccess';
import { useState } from 'react';

/**
 * Administracion de centros de costo (historia 0-5 del backlog).
 *
 * Solo los creaba el seeder, asi que no se podia abrir un centro para una
 * actividad nueva y todo terminaba imputado al general, que es justo lo que
 * hace inutil un informe por centro.
 *
 * No hay baja: los asientos ya imputados deben conservar su imputacion.
 */
const ROLES_PERMITIDOS = ['Admin'] as const;

export default function CentrosCostoPage() {
    const { canAccess, isRoleReady } = useRoleAccess(ROLES_PERMITIDOS);
    const centrosQuery = useCentrosCostoAdmin();
    const crear = useCrearCentroCosto();
    const actualizar = useActualizarCentroCosto();

    const [enEdicion, setEnEdicion] = useState<CentroCosto | null>(null);
    const [nombre, setNombre] = useState('');
    const [tipo, setTipo] = useState<number>(TIPOS_CENTRO_COSTO[0].value);
    const [error, setError] = useState<string | null>(null);

    if (!isRoleReady) {
        return <p className="p-8 text-sm text-slate-500">Verificando permisos...</p>;
    }

    if (!canAccess) {
        return (
            <div className="p-8">
                <h1 className="text-xl font-bold text-slate-900">Sin permiso</h1>
                <p className="mt-2 text-sm text-slate-600">
                    La administración de centros de costo requiere el rol Admin.
                </p>
            </div>
        );
    }

    const limpiar = () => {
        setEnEdicion(null);
        setNombre('');
        setTipo(TIPOS_CENTRO_COSTO[0].value);
    };

    const editar = (centro: CentroCosto) => {
        setError(null);
        setEnEdicion(centro);
        setNombre(centro.nombre);
        setTipo(centro.tipo);
    };

    const guardar = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();

        if (!nombre.trim()) {
            setError('El nombre es obligatorio.');
            return;
        }

        setError(null);

        try {
            if (enEdicion) {
                await actualizar.mutateAsync({ id: enEdicion.id, nombre: nombre.trim(), tipo });
            } else {
                await crear.mutateAsync({ nombre: nombre.trim(), tipo });
            }

            limpiar();
        } catch (err) {
            setError(mensajeDeError(err, 'No fue posible guardar el centro de costo.'));
        }
    };

    const centros = centrosQuery.data ?? [];
    const guardando = crear.isPending || actualizar.isPending;

    return (
        <div className="p-8">
            <h1 className="text-2xl font-bold text-slate-900">Centros de costo</h1>
            <p className="mt-1 text-sm text-slate-600">
                Cada asiento se imputa a un centro. Sin centros propios todo cae en el general y el informe
                por centro deja de decir nada.
            </p>

            <form
                onSubmit={guardar}
                className="mt-6 flex flex-wrap items-end gap-3 rounded-xl border border-slate-200 bg-white p-4"
            >
                <div className="min-w-56 flex-1">
                    <label className="mb-1 block text-sm font-medium text-slate-700">Nombre</label>
                    <input
                        type="text"
                        value={nombre}
                        onChange={(event) => setNombre(event.target.value)}
                        placeholder="Rodada Aniversario"
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                    />
                </div>

                <div className="min-w-44">
                    <label className="mb-1 block text-sm font-medium text-slate-700">Tipo</label>
                    <select
                        value={tipo}
                        onChange={(event) => setTipo(Number(event.target.value))}
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                    >
                        {TIPOS_CENTRO_COSTO.map((item) => (
                            <option key={item.value} value={item.value}>
                                {item.label}
                            </option>
                        ))}
                    </select>
                </div>

                <button
                    type="submit"
                    disabled={guardando}
                    className="rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white transition hover:bg-slate-800 disabled:cursor-not-allowed disabled:opacity-60"
                >
                    {guardando ? 'Guardando...' : enEdicion ? 'Guardar cambios' : 'Crear centro'}
                </button>

                {enEdicion ? (
                    <button
                        type="button"
                        onClick={limpiar}
                        className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700"
                    >
                        Cancelar
                    </button>
                ) : null}
            </form>

            {error ? (
                <div className="mt-4 rounded-lg border border-rose-200 bg-rose-50 px-3 py-2 text-sm text-rose-700">
                    {error}
                </div>
            ) : null}

            <div className="mt-6 overflow-x-auto rounded-xl border border-slate-200 bg-white">
                <table className="w-full text-sm">
                    <thead className="border-b border-slate-200 bg-slate-50 text-left text-slate-600">
                        <tr>
                            <th className="px-4 py-3 font-medium">Nombre</th>
                            <th className="px-4 py-3 font-medium">Tipo</th>
                            <th className="px-4 py-3 text-right font-medium">Acciones</th>
                        </tr>
                    </thead>
                    <tbody>
                        {centrosQuery.isLoading ? (
                            <tr>
                                <td colSpan={3} className="px-4 py-6 text-center text-slate-500">
                                    Cargando centros de costo...
                                </td>
                            </tr>
                        ) : null}

                        {!centrosQuery.isLoading && centros.length === 0 ? (
                            <tr>
                                <td colSpan={3} className="px-4 py-6 text-center text-slate-500">
                                    Todavía no hay centros de costo.
                                </td>
                            </tr>
                        ) : null}

                        {centros.map((centro) => (
                            <tr key={centro.id} className="border-b border-slate-100 last:border-0">
                                <td className="px-4 py-3 font-medium text-slate-900">{centro.nombre}</td>
                                <td className="px-4 py-3 text-slate-600">{nombreTipoCentroCosto(centro.tipo)}</td>
                                <td className="px-4 py-3 text-right">
                                    <button
                                        type="button"
                                        onClick={() => editar(centro)}
                                        className="rounded-lg border border-slate-300 px-3 py-1.5 text-xs font-medium text-slate-700 transition hover:bg-slate-50"
                                    >
                                        Editar
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
