'use client';

import { useSearchParams } from 'next/navigation';
import MiembroForm from '@/features/miembros/components/MiembroForm';

export default function NuevoMiembroPage() {
    const searchParams = useSearchParams();
    const miembroId = searchParams.get('id') ?? undefined;

    return (
        <main className="mx-auto max-w-4xl px-6 py-10">
            <MiembroForm mode={miembroId ? 'edit' : 'create'} miembroId={miembroId} />
        </main>
    );
}
