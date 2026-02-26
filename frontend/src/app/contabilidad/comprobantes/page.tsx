'use client';

import { useState } from 'react';
import ModalNuevoComprobante from '@/features/contabilidad/components/ModalNuevoComprobante';
import TablaComprobantes from '@/features/contabilidad/components/TablaComprobantes';

export default function ComprobantesPage() {
    const [openModal, setOpenModal] = useState(false);

    return (
        <main className="mx-auto max-w-7xl px-6 py-10">
            <header className="mb-6 flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-semibold text-slate-900">Comprobantes Contables</h1>
                    <p className="mt-1 text-sm text-slate-600">Registro de comprobantes de partida doble balanceados.</p>
                </div>

                <button
                    type="button"
                    onClick={() => setOpenModal(true)}
                    className="rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white hover:bg-blue-800"
                >
                    Nuevo Comprobante
                </button>
            </header>

            <TablaComprobantes />

            <ModalNuevoComprobante
                open={openModal}
                onClose={() => setOpenModal(false)}
            />
        </main>
    );
}
