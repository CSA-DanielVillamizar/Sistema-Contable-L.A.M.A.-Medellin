'use client';

import { useState } from 'react';
import ModalNuevoArticulo from '@/features/merchandising/components/ModalNuevoArticulo';
import ModalNuevaVenta from '@/features/merchandising/components/ModalNuevaVenta';
import TablaArticulos from '@/features/merchandising/components/TablaArticulos';
import TablaVentas from '@/features/merchandising/components/TablaVentas';
import { useArticulos } from '@/features/merchandising/hooks/useArticulos';
import { useVentas } from '@/features/merchandising/hooks/useVentas';

type SeccionInventario = 'inventario' | 'historial';

export default function InventarioPage() {
    const [modalAbierto, setModalAbierto] = useState(false);
    const [modalVentaAbierto, setModalVentaAbierto] = useState(false);
    const [seccionActiva, setSeccionActiva] = useState<SeccionInventario>('inventario');
    const articulosQuery = useArticulos();
    const ventasQuery = useVentas();

    return (
        <main className="min-h-screen bg-slate-50 px-6 py-10">
            <div className="mx-auto flex w-full max-w-6xl flex-col gap-6">
                <header className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
                    <div>
                        <h1 className="text-3xl font-bold text-slate-900">Inventario y Merchandising</h1>
                        <p className="mt-1 text-slate-600">Gestión del catálogo de artículos y stock actual</p>
                    </div>

                    <div className="flex gap-2">
                        <button
                            type="button"
                            onClick={() => setModalAbierto(true)}
                            className="rounded-lg bg-slate-900 px-4 py-2 text-sm font-medium text-white"
                        >
                            Nuevo Artículo
                        </button>
                        <button
                            type="button"
                            onClick={() => setModalVentaAbierto(true)}
                            className="rounded-lg bg-emerald-700 px-4 py-2 text-sm font-semibold text-white"
                        >
                            Registrar Venta
                        </button>
                    </div>
                </header>

                <section className="flex gap-2">
                    <button
                        type="button"
                        onClick={() => setSeccionActiva('inventario')}
                        className={`rounded-lg px-4 py-2 text-sm font-medium ${seccionActiva === 'inventario' ? 'bg-slate-900 text-white' : 'border border-slate-300 text-slate-700'}`}
                    >
                        Inventario
                    </button>
                    <button
                        type="button"
                        onClick={() => setSeccionActiva('historial')}
                        className={`rounded-lg px-4 py-2 text-sm font-medium ${seccionActiva === 'historial' ? 'bg-slate-900 text-white' : 'border border-slate-300 text-slate-700'}`}
                    >
                        Historial de Ventas
                    </button>
                </section>

                {seccionActiva === 'inventario' ? (
                    <TablaArticulos
                        articulos={articulosQuery.data ?? []}
                        isLoading={articulosQuery.isLoading}
                        isError={articulosQuery.isError}
                        error={articulosQuery.error as Error | null}
                    />
                ) : (
                    <TablaVentas
                        ventas={ventasQuery.data ?? []}
                        isLoading={ventasQuery.isLoading}
                        isError={ventasQuery.isError}
                        error={ventasQuery.error as Error | null}
                    />
                )}
            </div>

            <ModalNuevoArticulo open={modalAbierto} onClose={() => setModalAbierto(false)} />
            <ModalNuevaVenta open={modalVentaAbierto} onClose={() => setModalVentaAbierto(false)} />
        </main>
    );
}
