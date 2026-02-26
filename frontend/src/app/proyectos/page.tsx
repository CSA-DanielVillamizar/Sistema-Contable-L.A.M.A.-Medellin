'use client';

import { useState } from 'react';
import TablaProyectos from '@/features/proyectos/components/TablaProyectos';
import TablaBeneficiarios from '@/features/proyectos/components/TablaBeneficiarios';
import ModalNuevoProyecto from '@/features/proyectos/components/ModalNuevoProyecto';
import ModalNuevoBeneficiario from '@/features/proyectos/components/ModalNuevoBeneficiario';

type TabActiva = 'proyectos' | 'beneficiarios';

export default function ProyectosPage() {
    const [tabActiva, setTabActiva] = useState<TabActiva>('proyectos');
    const [openProyecto, setOpenProyecto] = useState(false);
    const [openBeneficiario, setOpenBeneficiario] = useState(false);

    return (
        <main className="mx-auto max-w-7xl px-6 py-10">
            <header className="mb-6">
                <h1 className="text-2xl font-semibold text-slate-900">Proyectos y Beneficiarios</h1>
                <p className="mt-1 text-sm text-slate-600">Gestión misional con control de consentimiento Habeas Data.</p>
            </header>

            <div className="mb-4 flex flex-wrap items-center justify-between gap-3">
                <div className="flex gap-2">
                    <button
                        type="button"
                        onClick={() => setTabActiva('proyectos')}
                        className={`rounded-lg px-4 py-2 text-sm font-medium ${
                            tabActiva === 'proyectos' ? 'bg-slate-900 text-white' : 'border border-slate-300 text-slate-700'
                        }`}
                    >
                        Proyectos Sociales
                    </button>
                    <button
                        type="button"
                        onClick={() => setTabActiva('beneficiarios')}
                        className={`rounded-lg px-4 py-2 text-sm font-medium ${
                            tabActiva === 'beneficiarios' ? 'bg-slate-900 text-white' : 'border border-slate-300 text-slate-700'
                        }`}
                    >
                        Beneficiarios
                    </button>
                </div>

                {tabActiva === 'proyectos' ? (
                    <button
                        type="button"
                        onClick={() => setOpenProyecto(true)}
                        className="rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white hover:bg-blue-800"
                    >
                        Nuevo Proyecto
                    </button>
                ) : (
                    <button
                        type="button"
                        onClick={() => setOpenBeneficiario(true)}
                        className="rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white hover:bg-blue-800"
                    >
                        Nuevo Beneficiario
                    </button>
                )}
            </div>

            {tabActiva === 'proyectos' ? <TablaProyectos /> : <TablaBeneficiarios />}

            <ModalNuevoProyecto open={openProyecto} onClose={() => setOpenProyecto(false)} />
            <ModalNuevoBeneficiario open={openBeneficiario} onClose={() => setOpenBeneficiario(false)} />
        </main>
    );
}
