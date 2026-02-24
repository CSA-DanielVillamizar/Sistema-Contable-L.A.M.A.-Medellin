'use client';

import { zodResolver } from '@hookform/resolvers/zod';
import { useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { useForm } from 'react-hook-form';
import { useActualizarMiembro, useCrearMiembro } from '@/features/miembros/hooks/useMutateMiembro';
import { useMiembroById } from '@/features/miembros/hooks/useMiembros';
import {
    estadosMiembroOptions,
    miembroSchema,
    tiposAfiliacionOptions,
    type MiembroFormInput,
    type MiembroFormValues,
} from '@/features/miembros/schemas/miembroSchema';

type MiembroFormProps = {
    mode: 'create' | 'edit';
    miembroId?: string;
};

const defaultValues: MiembroFormInput = {
    Nombre: '',
    Apellidos: '',
    Documento: '',
    Email: '',
    Telefono: '',
    TipoAfiliacion: 1,
    Estado: 1,
};

function getTipoAfiliacionValue(value: string): number {
    return tiposAfiliacionOptions.find((option) => option.label === value)?.value ?? 1;
}

function getEstadoValue(value: string): number {
    return estadosMiembroOptions.find((option) => option.label === value)?.value ?? 1;
}

export default function MiembroForm({ mode, miembroId }: MiembroFormProps) {
    const router = useRouter();
    const crearMiembro = useCrearMiembro();
    const actualizarMiembro = useActualizarMiembro();
    const miembroQuery = useMiembroById(mode === 'edit' ? miembroId : undefined);

    const {
        register,
        handleSubmit,
        reset,
        formState: { errors },
    } = useForm<MiembroFormInput, unknown, MiembroFormValues>({
        resolver: zodResolver(miembroSchema),
        defaultValues,
    });

    useEffect(() => {
        if (!miembroQuery.data || mode !== 'edit') {
            return;
        }

        reset({
            Nombre: miembroQuery.data.nombreCompleto.split(' ').slice(0, -1).join(' ') || miembroQuery.data.nombreCompleto,
            Apellidos: miembroQuery.data.nombreCompleto.split(' ').slice(-1).join(' '),
            Documento: miembroQuery.data.documento,
            Email: miembroQuery.data.email,
            Telefono: miembroQuery.data.telefono,
            TipoAfiliacion: getTipoAfiliacionValue(miembroQuery.data.tipoAfiliacion),
            Estado: getEstadoValue(miembroQuery.data.estado),
        });
    }, [miembroQuery.data, mode, reset]);

    const onSubmit = async (values: MiembroFormValues) => {
        try {
            if (mode === 'edit' && miembroId) {
                await actualizarMiembro.mutateAsync({ id: miembroId, request: values });
            } else {
                await crearMiembro.mutateAsync(values);
            }

            router.push('/miembros');
        } catch {
            return;
        }
    };

    const isPending = crearMiembro.isPending || actualizarMiembro.isPending;
    const mutationError = (crearMiembro.error ?? actualizarMiembro.error) as Error | null;

    if (mode === 'edit' && miembroQuery.isLoading) {
        return <p className="text-sm text-slate-600">Cargando información del miembro...</p>;
    }

    if (mode === 'edit' && miembroQuery.isError) {
        return <p className="text-sm text-red-600">{(miembroQuery.error as Error).message}</p>;
    }

    return (
        <section className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
            <header className="mb-5">
                <h2 className="text-xl font-semibold text-slate-900">
                    {mode === 'edit' ? 'Editar miembro' : 'Agregar miembro'}
                </h2>
            </header>

            <form onSubmit={handleSubmit(onSubmit)} className="grid grid-cols-1 gap-4 md:grid-cols-2">
                <div>
                    <label className="mb-1 block text-sm font-medium text-slate-700">Nombres</label>
                    <input
                        {...register('Nombre')}
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
                    />
                    {errors.Nombre && <p className="mt-1 text-xs text-red-600">{errors.Nombre.message}</p>}
                </div>

                <div>
                    <label className="mb-1 block text-sm font-medium text-slate-700">Apellidos</label>
                    <input
                        {...register('Apellidos')}
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
                    />
                    {errors.Apellidos && <p className="mt-1 text-xs text-red-600">{errors.Apellidos.message}</p>}
                </div>

                <div>
                    <label className="mb-1 block text-sm font-medium text-slate-700">Documento</label>
                    <input
                        {...register('Documento')}
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
                    />
                    {errors.Documento && <p className="mt-1 text-xs text-red-600">{errors.Documento.message}</p>}
                </div>

                <div>
                    <label className="mb-1 block text-sm font-medium text-slate-700">Correo</label>
                    <input
                        type="email"
                        {...register('Email')}
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
                    />
                    {errors.Email && <p className="mt-1 text-xs text-red-600">{errors.Email.message}</p>}
                </div>

                <div>
                    <label className="mb-1 block text-sm font-medium text-slate-700">Teléfono</label>
                    <input
                        {...register('Telefono')}
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
                    />
                    {errors.Telefono && <p className="mt-1 text-xs text-red-600">{errors.Telefono.message}</p>}
                </div>

                <div>
                    <label className="mb-1 block text-sm font-medium text-slate-700">Tipo de afiliación</label>
                    <select
                        {...register('TipoAfiliacion')}
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
                    >
                        {tiposAfiliacionOptions.map((option, index) => (
                            <option key={`tipo-${option.value}-${index}`} value={option.value}>{option.label}</option>
                        ))}
                    </select>
                </div>

                <div>
                    <label className="mb-1 block text-sm font-medium text-slate-700">Estado</label>
                    <select
                        {...register('Estado')}
                        className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
                    >
                        {estadosMiembroOptions.map((option, index) => (
                            <option key={`estado-${option.value}-${index}`} value={option.value}>{option.label}</option>
                        ))}
                    </select>
                </div>

                {mutationError && (
                    <div className="md:col-span-2 rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">
                        {mutationError.message}
                    </div>
                )}

                <div className="md:col-span-2 flex items-center gap-3">
                    <button
                        type="submit"
                        disabled={isPending}
                        className="rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white hover:bg-blue-800 disabled:cursor-not-allowed disabled:opacity-60"
                    >
                        {isPending ? 'Guardando...' : mode === 'edit' ? 'Actualizar miembro' : 'Crear miembro'}
                    </button>
                    <button
                        type="button"
                        onClick={() => router.push('/miembros')}
                        className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700 hover:bg-slate-50"
                    >
                        Cancelar
                    </button>
                </div>
            </form>
        </section>
    );
}
