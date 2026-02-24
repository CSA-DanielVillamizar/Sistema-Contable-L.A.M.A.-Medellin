'use client';

import { useState } from 'react';
import ModalPagoCartera from '@/features/cartera/components/ModalPagoCartera';
import { useCarteraPendiente } from '@/features/cartera/hooks/useCartera';

export default function TablaCartera() {
    const { data: cartera, isLoading, error } = useCarteraPendiente();
    const [carteraSeleccionadaId, setCarteraSeleccionadaId] = useState<string | null>(null);
    const [montoSugerido, setMontoSugerido] = useState(0);

    if (isLoading) {
        return (
            <div className="flex items-center justify-center py-12">
                <div className="flex flex-col items-center gap-2">
                    <div className="h-8 w-8 animate-spin rounded-full border-4 border-gray-300 border-t-blue-600"></div>
                    <p className="text-sm text-gray-600">Cargando cartera...</p>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="rounded-lg border border-red-200 bg-red-50 p-4">
                <p className="text-sm text-red-800">
                    Error al cargar la cartera: {error instanceof Error ? error.message : 'Error desconocido'}
                </p>
            </div>
        );
    }

    if (!cartera || cartera.length === 0) {
        return (
            <div className="rounded-lg border border-gray-200 bg-gray-50 p-8 text-center">
                <p className="text-gray-600">No hay cuentas por cobrar pendientes.</p>
            </div>
        );
    }

    const formatCurrency = (value: number): string => {
        return new Intl.NumberFormat('es-CO', {
            style: 'currency',
            currency: 'COP',
            minimumFractionDigits: 0,
            maximumFractionDigits: 0,
        }).format(value);
    };

    const totalSaldo = cartera.reduce((sum, item) => sum + item.saldoPendienteCOP, 0);

    const abrirModalPago = (id: string, saldo: number) => {
        setCarteraSeleccionadaId(id);
        setMontoSugerido(saldo);
    };

    const cerrarModalPago = () => {
        setCarteraSeleccionadaId(null);
        setMontoSugerido(0);
    };

    return (
        <div className="space-y-4">
            <div className="overflow-hidden rounded-lg border border-gray-200 bg-white shadow-sm">
                <div className="overflow-x-auto">
                    <table className="min-w-full divide-y divide-gray-200">
                        <thead className="bg-gray-50">
                            <tr>
                                <th scope="col" className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
                                    Miembro
                                </th>
                                <th scope="col" className="px-6 py-3 text-left text-xs font-medium uppercase tracking-wider text-gray-500">
                                    Periodo
                                </th>
                                <th scope="col" className="px-6 py-3 text-right text-xs font-medium uppercase tracking-wider text-gray-500">
                                    Valor
                                </th>
                                <th scope="col" className="px-6 py-3 text-right text-xs font-medium uppercase tracking-wider text-gray-500">
                                    Saldo
                                </th>
                                <th scope="col" className="px-6 py-3 text-right text-xs font-medium uppercase tracking-wider text-gray-500">
                                    Acciones
                                </th>
                            </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200 bg-white">
                            {cartera.map((item, index) => (
                                <tr key={`${item.id || item.miembroId || 'fila'}-${item.periodo}-${index}`} className="hover:bg-gray-50">
                                    <td className="whitespace-nowrap px-6 py-4 text-sm font-medium text-gray-900">
                                        {item.nombreMiembro}
                                    </td>
                                    <td className="whitespace-nowrap px-6 py-4 text-sm text-gray-600">
                                        {item.periodo}
                                    </td>
                                    <td className="whitespace-nowrap px-6 py-4 text-right text-sm text-gray-900">
                                        {formatCurrency(item.valorEsperadoCOP)}
                                    </td>
                                    <td className="whitespace-nowrap px-6 py-4 text-right text-sm font-semibold text-gray-900">
                                        {formatCurrency(item.saldoPendienteCOP)}
                                    </td>
                                    <td className="whitespace-nowrap px-6 py-4 text-right text-sm">
                                        <button
                                            type="button"
                                            onClick={() => abrirModalPago(item.id, item.saldoPendienteCOP)}
                                            className="inline-flex items-center rounded-md bg-blue-600 px-3 py-2 text-sm font-medium text-white shadow-sm transition-colors hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
                                        >
                                            Registrar Pago
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                        <tfoot className="bg-gray-50">
                            <tr>
                                <td colSpan={3} className="px-6 py-4 text-right text-sm font-semibold text-gray-700">
                                    Total Saldo Pendiente:
                                </td>
                                <td className="whitespace-nowrap px-6 py-4 text-right text-sm font-bold text-gray-900">
                                    {formatCurrency(totalSaldo)}
                                </td>
                                <td></td>
                            </tr>
                        </tfoot>
                    </table>
                </div>
            </div>

            <ModalPagoCartera
                open={Boolean(carteraSeleccionadaId)}
                carteraId={carteraSeleccionadaId}
                montoSugerido={montoSugerido}
                onClose={cerrarModalPago}
            />

            <div className="flex items-center justify-between rounded-lg border border-blue-200 bg-blue-50 p-4">
                <div className="flex items-center gap-2">
                    <svg className="h-5 w-5 text-blue-600" fill="none" strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" viewBox="0 0 24 24" stroke="currentColor">
                        <path d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path>
                    </svg>
                    <p className="text-sm text-blue-800">
                        Mostrando {cartera.length} cuenta{cartera.length !== 1 ? 's' : ''} pendiente{cartera.length !== 1 ? 's' : ''} de cobro
                    </p>
                </div>
            </div>
        </div>
    );
}
