'use client';

export default function TablaComprobantes() {
    return (
        <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
            <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-slate-200">
                    <thead className="bg-slate-50">
                        <tr>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Número</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Fecha</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Tipo</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Descripción</th>
                            <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Estado</th>
                        </tr>
                    </thead>
                    <tbody className="bg-white">
                        <tr>
                            <td colSpan={5} className="px-4 py-8 text-center text-sm text-slate-500">
                                Tabla preparada para integrar endpoint GET de comprobantes en el siguiente sprint.
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    );
}
