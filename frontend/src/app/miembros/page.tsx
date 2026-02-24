import Link from 'next/link';
import TablaMiembros from '@/features/miembros/components/TablaMiembros';

export default function MiembrosPage() {
    return (
        <main className="mx-auto max-w-7xl px-6 py-10">
            <header className="mb-6 flex items-center justify-between">
                <div>
                    <h1 className="text-2xl font-semibold text-slate-900">Directorio de Miembros</h1>
                    <p className="mt-1 text-sm text-slate-600">
                        Gestión de integrantes del capítulo L.A.M.A. Medellín.
                    </p>
                </div>
                <Link
                    href="/miembros/nuevo"
                    className="inline-flex items-center rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white hover:bg-blue-800"
                >
                    Agregar Miembro
                </Link>
            </header>

            <TablaMiembros />
        </main>
    );
}
