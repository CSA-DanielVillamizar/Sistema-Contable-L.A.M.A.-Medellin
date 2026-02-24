"use client";

import TablaCartera from '@/features/cartera/components/TablaCartera';
import { useGenerarCarteraMensual } from '@/features/cartera/hooks/useCartera';

function getPeriodoActual() {
    const fecha = new Date();
    const mes = String(fecha.getMonth() + 1).padStart(2, '0');
    return `${fecha.getFullYear()}-${mes}`;
}

export default function ListadoCarteraPage() {
    const generarCarteraMutation = useGenerarCarteraMensual();

    const handleGenerarMesActual = async () => {
        await generarCarteraMutation.mutateAsync({ Periodo: getPeriodoActual() });
    };

    return (
        <main className="mx-auto max-w-7xl px-6 py-10">
            <header className="mb-6 flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-semibold text-slate-900">Cartera Pendiente</h1>
                    <p className="mt-1 text-sm text-slate-600">
                        Consulta de todas las cuentas por cobrar con saldo pendiente de pago.
                    </p>
                </div>

                <button
                    type="button"
                    onClick={() => void handleGenerarMesActual()}
                    disabled={generarCarteraMutation.isPending}
                    className="inline-flex items-center rounded-lg bg-slate-800 px-6 py-4 text-base font-semibold text-white shadow-sm transition-colors hover:bg-slate-900 disabled:opacity-60"
                >
                    {generarCarteraMutation.isPending ? 'Generando cartera...' : 'Generar Cartera del Mes Actual'}
                </button>
            </header>

            {generarCarteraMutation.error ? (
                <div className="mb-4 rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
                    {generarCarteraMutation.error.message}
                </div>
            ) : null}

            <TablaCartera />
        </main>
    );
}
