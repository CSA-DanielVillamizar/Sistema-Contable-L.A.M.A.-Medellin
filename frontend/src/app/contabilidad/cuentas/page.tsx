import TablaCuentasContables from '@/features/contabilidad/components/TablaCuentasContables';

export default function CatalogoCuentasPage() {
    return (
        <main className="mx-auto max-w-7xl px-6 py-10">
            <header className="mb-6">
                <h1 className="text-2xl font-semibold text-slate-900">Catálogo de Cuentas</h1>
                <p className="mt-1 text-sm text-slate-600">
                    Plan Único de Cuentas (PUC) adaptado para la Fundación L.A.M.A. Medellín —
                    NIIF Grupo III · Microempresas · Gobernación de Antioquia
                </p>
            </header>

            <TablaCuentasContables />
        </main>
    );
}
