import RegistroIngresoForm from '@/features/transacciones/components/RegistroIngresoForm';

export default function RegistroIngresoPage() {
    return (
        <main className="mx-auto max-w-3xl px-6 py-10">
            <header className="mb-6">
                <h1 className="text-2xl font-semibold text-slate-900">Registro de Ingreso</h1>
                <p className="mt-1 text-sm text-slate-600">
                    Registra ingresos bancarios en COP con soporte opcional para transacciones en USD.
                </p>
            </header>

            <RegistroIngresoForm />
        </main>
    );
}
