export default function GlobalLoading() {
    return (
        <main className="flex min-h-screen items-center justify-center bg-slate-50 px-6">
            <div className="flex flex-col items-center gap-4 rounded-2xl border border-slate-200 bg-white px-8 py-7 shadow-sm">
                <div className="h-12 w-12 animate-spin rounded-full border-4 border-slate-200 border-t-red-700" />
                <p className="text-sm font-medium text-slate-600">Cargando el sistema contable...</p>
            </div>
        </main>
    );
}
