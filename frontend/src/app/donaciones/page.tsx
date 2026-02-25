'use client';

import { useState } from 'react';
import TablaDonaciones from '@/features/donaciones/components/TablaDonaciones';
import ModalNuevoDonante from '@/features/donaciones/components/ModalNuevoDonante';
import ModalNuevaDonacion from '@/features/donaciones/components/ModalNuevaDonacion';

export default function DonacionesPage() {
    const [openDonante, setOpenDonante] = useState(false);
    const [openDonacion, setOpenDonacion] = useState(false);

    return (
        <main className="mx-auto max-w-7xl px-6 py-10">
            <header className="mb-6 flex flex-wrap items-center justify-between gap-3">
                <div>
                    <h1 className="text-2xl font-semibold text-slate-900">Gestión de Donaciones</h1>
                    <p className="mt-1 text-sm text-slate-600">Registro de donantes y trazabilidad de aportes.</p>
                </div>

                <div className="flex gap-2">
                    <button
                        type="button"
                        onClick={() => setOpenDonante(true)}
                        className="rounded-lg bg-slate-800 px-4 py-2 text-sm font-medium text-white hover:bg-slate-900"
                    >
                        Registrar Donante
                    </button>
                    <button
                        type="button"
                        onClick={() => setOpenDonacion(true)}
                        className="rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white hover:bg-blue-800"
                    >
                        Registrar Donación
                    </button>
                </div>
            </header>

            <TablaDonaciones />

            <ModalNuevoDonante open={openDonante} onClose={() => setOpenDonante(false)} />
            <ModalNuevaDonacion open={openDonacion} onClose={() => setOpenDonacion(false)} />
        </main>
    );
}
