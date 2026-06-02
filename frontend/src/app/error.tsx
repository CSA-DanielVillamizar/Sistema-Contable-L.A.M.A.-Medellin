'use client';

import { useEffect } from 'react';

type GlobalErrorProps = {
    error: Error & { digest?: string };
    reset: () => void;
};

export default function GlobalError({ error, reset }: GlobalErrorProps) {
    useEffect(() => {
        console.error('Error global de la app:', error);
    }, [error]);

    return (
        <main className="flex min-h-screen items-center justify-center bg-slate-50 px-6">
            <div className="w-full max-w-lg rounded-2xl border border-red-200 bg-white p-8 shadow-sm">
                <p className="mb-2 text-xs font-semibold uppercase tracking-wide text-red-700">Error inesperado</p>
                <h1 className="text-2xl font-semibold text-slate-900">No pudimos cargar esta pantalla</h1>
                <p className="mt-3 text-sm text-slate-600">
                    Ocurrió un fallo temporal. Puedes reintentar sin perder tu sesión.
                </p>

                <div className="mt-6 flex items-center gap-3">
                    <button
                        type="button"
                        onClick={reset}
                        className="rounded-lg bg-red-700 px-4 py-2 text-sm font-medium text-white hover:bg-red-800"
                    >
                        Reintentar
                    </button>
                    <span className="text-xs text-slate-500">Si persiste, reporta el incidente al equipo técnico.</span>
                </div>
            </div>
        </main>
    );
}
