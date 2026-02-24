'use client';

import { useEffect } from 'react';
import { useQuery } from '@tanstack/react-query';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import apiClient from '@/lib/apiClient';
import { useRegistrarPagoCartera } from '@/features/cartera/hooks/useCartera';

type BancoCatalogo = {
    id: string;
    numeroCuenta: string;
};

type CentroCostoCatalogo = {
    id: string;
    nombre: string;
};

const schema = z.object({
    MontoPagadoCOP: z.number().gt(0, 'El monto debe ser mayor a 0.'),
    BancoId: z.string().uuid('Selecciona un banco válido.'),
    CentroCostoId: z.string().uuid('Selecciona un centro de costo válido.'),
});

type FormValues = z.infer<typeof schema>;

type ModalPagoCarteraProps = {
    open: boolean;
    carteraId: string | null;
    montoSugerido: number;
    onClose: () => void;
};

export default function ModalPagoCartera({ open, carteraId, montoSugerido, onClose }: ModalPagoCarteraProps) {
    const registrarPagoMutation = useRegistrarPagoCartera();

    const bancosQuery = useQuery({
        queryKey: ['transacciones', 'catalogo', 'bancos'],
        queryFn: async () => {
            const response = await apiClient.get<any[]>('/api/transacciones/bancos');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? item?.Id ?? ''),
                numeroCuenta: String(item?.numeroCuenta ?? item?.NumeroCuenta ?? ''),
            })) as BancoCatalogo[];
        },
        enabled: open,
    });

    const centrosQuery = useQuery({
        queryKey: ['transacciones', 'catalogo', 'centros-costo'],
        queryFn: async () => {
            const response = await apiClient.get<any[]>('/api/transacciones/centros-costo');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? item?.Id ?? ''),
                nombre: String(item?.nombre ?? item?.Nombre ?? ''),
            })) as CentroCostoCatalogo[];
        },
        enabled: open,
    });

    const {
        register,
        handleSubmit,
        reset,
        setValue,
        formState: { errors },
    } = useForm<FormValues>({
        resolver: zodResolver(schema),
        defaultValues: {
            MontoPagadoCOP: montoSugerido,
            BancoId: '',
            CentroCostoId: '',
        },
    });

    useEffect(() => {
        if (!open) {
            return;
        }

        reset({
            MontoPagadoCOP: montoSugerido,
            BancoId: bancosQuery.data?.[0]?.id ?? '',
            CentroCostoId: centrosQuery.data?.[0]?.id ?? '',
        });
    }, [open, montoSugerido, bancosQuery.data, centrosQuery.data, reset]);

    useEffect(() => {
        if (open && bancosQuery.data?.[0]?.id) {
            setValue('BancoId', bancosQuery.data[0].id, { shouldValidate: true });
        }
    }, [open, bancosQuery.data, setValue]);

    useEffect(() => {
        if (open && centrosQuery.data?.[0]?.id) {
            setValue('CentroCostoId', centrosQuery.data[0].id, { shouldValidate: true });
        }
    }, [open, centrosQuery.data, setValue]);

    const onSubmit = async (values: FormValues) => {
        if (!carteraId) {
            return;
        }

        await registrarPagoMutation.mutateAsync({
            id: carteraId,
            MontoPagadoCOP: values.MontoPagadoCOP,
            BancoId: values.BancoId,
            CentroCostoId: values.CentroCostoId,
        });

        onClose();
    };

    if (!open) {
        return null;
    }

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
            <div className="w-full max-w-md rounded-xl bg-white p-6 shadow-xl">
                <h2 className="text-lg font-semibold text-slate-900">Registrar pago de cartera</h2>

                <form className="mt-4 space-y-4" onSubmit={handleSubmit(onSubmit)}>
                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Monto pagado</label>
                        <input
                            type="number"
                            step="0.01"
                            {...register('MontoPagadoCOP', { valueAsNumber: true })}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        />
                        {errors.MontoPagadoCOP ? <p className="mt-1 text-xs text-red-600">{errors.MontoPagadoCOP.message}</p> : null}
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Banco</label>
                        <select
                            {...register('BancoId')}
                            disabled={bancosQuery.isLoading || (bancosQuery.data?.length ?? 0) === 0}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        >
                            <option value="">Seleccione...</option>
                            {(bancosQuery.data ?? []).map((banco) => (
                                <option key={banco.id} value={banco.id}>
                                    {banco.numeroCuenta}
                                </option>
                            ))}
                        </select>
                        {errors.BancoId ? <p className="mt-1 text-xs text-red-600">{errors.BancoId.message}</p> : null}
                    </div>

                    <div>
                        <label className="mb-1 block text-sm font-medium text-slate-700">Centro de costo</label>
                        <select
                            {...register('CentroCostoId')}
                            disabled={centrosQuery.isLoading || (centrosQuery.data?.length ?? 0) === 0}
                            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900"
                        >
                            <option value="">Seleccione...</option>
                            {(centrosQuery.data ?? []).map((centro) => (
                                <option key={centro.id} value={centro.id}>
                                    {centro.nombre}
                                </option>
                            ))}
                        </select>
                        {errors.CentroCostoId ? <p className="mt-1 text-xs text-red-600">{errors.CentroCostoId.message}</p> : null}
                    </div>

                    {registrarPagoMutation.error ? (
                        <p className="rounded-lg border border-red-200 bg-red-50 p-2 text-sm text-red-700">{registrarPagoMutation.error.message}</p>
                    ) : null}

                    <div className="flex justify-end gap-2">
                        <button
                            type="button"
                            onClick={onClose}
                            className="rounded-lg border border-slate-300 px-4 py-2 text-sm font-medium text-slate-700"
                        >
                            Cancelar
                        </button>
                        <button
                            type="submit"
                            disabled={registrarPagoMutation.isPending}
                            className="rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white disabled:opacity-60"
                        >
                            {registrarPagoMutation.isPending ? 'Registrando...' : 'Registrar pago'}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}
