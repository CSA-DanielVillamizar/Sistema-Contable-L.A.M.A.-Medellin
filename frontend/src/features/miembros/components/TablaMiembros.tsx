'use client';

import { useRouter } from 'next/navigation';
import { useEliminarMiembro } from '@/features/miembros/hooks/useMutateMiembro';
import { useMiembros } from '@/features/miembros/hooks/useMiembros';

function estadoClass(estado: string): string {
    const normalized = estado.toLowerCase();

    if (normalized === 'activo') {
        return 'bg-emerald-100 text-emerald-700';
    }

    if (normalized === 'inactivo') {
        return 'bg-slate-100 text-slate-700';
    }

    if (normalized === 'suspendido') {
        return 'bg-amber-100 text-amber-700';
    }

    return 'bg-red-100 text-red-700';
}

export default function TablaMiembros() {
    const router = useRouter();
    const { data, isLoading, isError, error } = useMiembros();
    const eliminarMiembro = useEliminarMiembro();

    const handleEditar = (id: string) => {
        router.push(`/miembros/nuevo?id=${id}`);
    };

    const handleEliminar = async (id: string, nombreCompleto: string) => {
        const confirmar = window.confirm(`¿Deseas desactivar a ${nombreCompleto}?`);
        if (!confirmar) {
            return;
        }

        await eliminarMiembro.mutateAsync(id);
    };

    if (isLoading) {
        return <p className="text-sm text-slate-600">Cargando miembros...</p>;
    }

    if (isError) {
        return <p className="text-sm text-red-600">{(error as Error).message}</p>;
    }

    if (!data || data.length === 0) {
        return <p className="text-sm text-slate-600">No hay miembros registrados.</p>;
    }

    return (
        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
            <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-slate-200">
                    <thead className="bg-slate-50">
                        <tr>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Nombre</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Documento</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Correo</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Teléfono</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Tipo</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Estado</th>
                            <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-600">Acciones</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-200 bg-white">
                        {data.map((miembro, index) => (
                            <tr key={`${miembro.id}-${index}`} className="hover:bg-slate-50">
                                <td className="px-4 py-3 text-sm text-slate-900">{miembro.nombreCompleto}</td>
                                <td className="px-4 py-3 text-sm text-slate-700">{miembro.documento}</td>
                                <td className="px-4 py-3 text-sm text-slate-700">{miembro.email}</td>
                                <td className="px-4 py-3 text-sm text-slate-700">{miembro.telefono}</td>
                                <td className="px-4 py-3 text-sm text-slate-700">{miembro.tipoAfiliacion}</td>
                                <td className="px-4 py-3 text-sm">
                                    <span className={`inline-flex rounded-full px-2 py-1 text-xs font-medium ${estadoClass(miembro.estado)}`}>
                                        {miembro.estado}
                                    </span>
                                </td>
                                <td className="px-4 py-3 text-right text-sm">
                                    <div className="inline-flex gap-2">
                                        <button
                                            type="button"
                                            onClick={() => handleEditar(miembro.id)}
                                            className="rounded-md border border-blue-200 bg-blue-50 px-3 py-1.5 text-blue-700 hover:bg-blue-100"
                                        >
                                            Editar
                                        </button>
                                        <button
                                            type="button"
                                            onClick={() => void handleEliminar(miembro.id, miembro.nombreCompleto)}
                                            disabled={eliminarMiembro.isPending}
                                            className="rounded-md border border-red-200 bg-red-50 px-3 py-1.5 text-red-700 hover:bg-red-100 disabled:cursor-not-allowed disabled:opacity-60"
                                        >
                                            Eliminar
                                        </button>
                                    </div>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
        </div>
    );
}
