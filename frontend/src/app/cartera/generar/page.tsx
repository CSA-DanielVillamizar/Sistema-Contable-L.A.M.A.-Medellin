import GenerarCarteraForm from '@/features/cartera/components/GenerarCarteraForm';

export default function GenerarCarteraPage() {
    return (
        <main className="mx-auto max-w-3xl px-6 py-10">
            <header className="mb-6">
                <h1 className="text-2xl font-semibold text-slate-900">Generación Masiva de Cuotas</h1>
                <p className="mt-1 text-sm text-slate-600">Ejecuta la facturación mensual de cartera por período.</p>
            </header>

            <GenerarCarteraForm />
        </main>
    );
}
