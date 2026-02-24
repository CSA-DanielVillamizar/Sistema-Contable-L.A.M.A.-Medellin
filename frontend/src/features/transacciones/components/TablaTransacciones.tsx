'use client';

import { useTransacciones } from '@/features/transacciones/hooks/useTransacciones';

function formatCOP(value: number): string {
  return new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0,
  }).format(value);
}

function formatFecha(value: string): string {
  return new Intl.DateTimeFormat('es-CO', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
  }).format(new Date(value));
}

export default function TablaTransacciones() {
  const { data, isLoading, error } = useTransacciones();

  if (isLoading) {
    return (
      <div className="rounded-lg border border-slate-200 bg-white p-6 text-sm text-slate-600">
        Cargando transacciones...
      </div>
    );
  }

  if (error) {
    return (
      <div className="rounded-lg border border-red-200 bg-red-50 p-4 text-sm text-red-700">
        No fue posible cargar el listado de transacciones.
      </div>
    );
  }

  if (!data || data.length === 0) {
    return (
      <div className="rounded-lg border border-slate-200 bg-slate-50 p-6 text-sm text-slate-600">
        No hay movimientos registrados.
      </div>
    );
  }

  return (
    <div className="overflow-hidden rounded-xl border border-slate-200 bg-white shadow-sm">
      <div className="overflow-x-auto">
        <table className="min-w-full divide-y divide-slate-200">
          <thead className="bg-slate-50">
            <tr>
              <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Fecha</th>
              <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Tipo</th>
              <th className="px-4 py-3 text-right text-xs font-semibold uppercase tracking-wider text-slate-600">Monto</th>
              <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Centro de costo</th>
              <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Banco</th>
              <th className="px-4 py-3 text-left text-xs font-semibold uppercase tracking-wider text-slate-600">Descripción</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-200 bg-white">
            {data.map((transaccion) => {
              // Normalizamos el tipo a mayúsculas para evitar inconsistencias
              const tipoNormalizado = String(transaccion.tipo || '').trim();
              const esIngreso = tipoNormalizado === 'Ingreso';
              const signo = esIngreso ? '+' : '-';
              const montoConSigno = `${signo}${formatCOP(transaccion.montoCOP)}`;
              const montoClass = esIngreso ? 'text-emerald-700' : 'text-red-700';

              return (
                <tr key={transaccion.id} className="hover:bg-slate-50">
                  <td className="whitespace-nowrap px-4 py-3 text-sm text-slate-700">{formatFecha(transaccion.fecha)}</td>
                  <td className="whitespace-nowrap px-4 py-3 text-sm font-medium text-slate-900">{transaccion.tipo}</td>
                  <td className={`whitespace-nowrap px-4 py-3 text-right text-sm font-semibold ${montoClass}`}>
                    {montoConSigno}
                  </td>
                  <td className="whitespace-nowrap px-4 py-3 text-sm text-slate-700">{transaccion.centroCosto}</td>
                  <td className="whitespace-nowrap px-4 py-3 text-sm text-slate-700">{transaccion.banco}</td>
                  <td className="px-4 py-3 text-sm text-slate-700">{transaccion.descripcion}</td>
                </tr>
              );
            })}
          </tbody>
        </table>
      </div>
    </div>
  );
}
