import TablaCuentasContables from '@/features/contabilidad/components/TablaCuentasContables';

export default function CuentasContablesPage() {
    return (
        <main className="mx-auto max-w-7xl px-6 py-10">
            <header className="mb-6">
                <h1 className="text-2xl font-semibold text-slate-900">Catálogo de Cuentas</h1>
                <p className="mt-1 text-sm text-slate-600">Consulta del PUC ESAL con filtro de cuentas asentables.</p>
            </header>

            <TablaCuentasContables />
        </main>
    );
}
