'use client';

import TesoreriaMovimientoModal, {
    type TesoreriaCatalogItem,
    type TesoreriaMovimientoFormValues,
} from '@/features/tesoreria/components/TesoreriaMovimientoModal';
import {
    getCajas,
    registrarEgreso,
    registrarIngreso,
    type RegistrarMovimientoTesoreriaPayload,
} from '@/features/tesoreria/services/tesoreriaService';
import apiClient from '@/lib/apiClient';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import axios from 'axios';
import { useMemo, useState } from 'react';

type CentroCostoApi = {
    id: string;
    nombre: string;
};

type CentroCostoApiDto = {
    id?: string;
    Id?: string;
    nombre?: string;
    Nombre?: string;
};

type CuentaContableApi = {
    id: string;
    codigo: string;
    descripcion: string;
    naturaleza: number;
    permiteMovimiento: boolean;
};

type CuentaContableApiDto = {
    id?: string;
    Id?: string;
    codigo?: string;
    Codigo?: string;
    descripcion?: string;
    Descripcion?: string;
    naturaleza?: number;
    Naturaleza?: number;
    permiteMovimiento?: boolean;
    PermiteMovimiento?: boolean;
};

type ProblemDetails = {
    title?: string;
    detail?: string;
    errors?: Record<string, string[]>;
};

function formatCOP(value: number): string {
    return new Intl.NumberFormat('es-CO', {
        style: 'currency',
        currency: 'COP',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
    }).format(value);
}

function getErrorMessage(error: unknown): string {
    if (axios.isAxiosError<ProblemDetails>(error)) {
        const validationErrors = error.response?.data?.errors;
        const firstValidationError = validationErrors
            ? Object.values(validationErrors).flat().find((message) => message)
            : undefined;

        return firstValidationError ?? error.response?.data?.detail ?? error.response?.data?.title ?? 'No fue posible completar la operación.';
    }

    if (error instanceof Error && error.message) {
        return error.message;
    }

    return 'No fue posible completar la operación.';
}

function toMovimientoPayload(values: TesoreriaMovimientoFormValues): RegistrarMovimientoTesoreriaPayload {
    return {
        fecha: new Date(values.fecha).toISOString(),
        monto: Number(values.monto),
        concepto: values.concepto.trim(),
        terceroId: null,
        cuentaContableId: values.cuentaContableId,
        cajaId: values.cajaId,
        centroCostoId: values.centroCostoId,
    };
}

export default function TesoreriaDashboard() {
    const queryClient = useQueryClient();
    const [modalActivo, setModalActivo] = useState<'ingreso' | 'egreso' | null>(null);
    const [mensajeExito, setMensajeExito] = useState<string | null>(null);

    const cajasQuery = useQuery({
        queryKey: ['tesoreria', 'cajas'],
        queryFn: getCajas,
    });

    const centrosCostoQuery = useQuery({
        queryKey: ['tesoreria', 'catalogos', 'centros-costo'],
        queryFn: async () => {
            const response = await apiClient.get<CentroCostoApiDto[]>('/api/transacciones/centros-costo');

            return (response.data ?? []).map((item) => ({
                id: String(item?.id ?? item?.Id ?? ''),
                nombre: String(item?.nombre ?? item?.Nombre ?? ''),
            })) as CentroCostoApi[];
        },
    });

    const cuentasContablesQuery = useQuery({
        queryKey: ['tesoreria', 'catalogos', 'cuentas-contables'],
        queryFn: async () => {
            const response = await apiClient.get<CuentaContableApiDto[]>('/api/cuentas-contables');

            return (response.data ?? [])
                .map((item) => ({
                    id: String(item?.id ?? item?.Id ?? ''),
                    codigo: String(item?.codigo ?? item?.Codigo ?? ''),
                    descripcion: String(item?.descripcion ?? item?.Descripcion ?? ''),
                    naturaleza: Number(item?.naturaleza ?? item?.Naturaleza ?? 0),
                    permiteMovimiento: Boolean(item?.permiteMovimiento ?? item?.PermiteMovimiento ?? false),
                }))
                .filter((item) => item.permiteMovimiento) as CuentaContableApi[];
        },
    });

    const registrarIngresoMutation = useMutation({
        mutationFn: registrarIngreso,
        onSuccess: async () => {
            setMensajeExito('Ingreso registrado correctamente.');
            setModalActivo(null);
            await queryClient.invalidateQueries({ queryKey: ['tesoreria', 'cajas'] });
        },
    });

    const registrarEgresoMutation = useMutation({
        mutationFn: registrarEgreso,
        onSuccess: async () => {
            setMensajeExito('Egreso registrado correctamente.');
            setModalActivo(null);
            await queryClient.invalidateQueries({ queryKey: ['tesoreria', 'cajas'] });
        },
    });

    const cajasCatalogo = useMemo<TesoreriaCatalogItem[]>(() => {
        return (cajasQuery.data ?? []).map((caja) => ({ id: caja.id, nombre: caja.nombre }));
    }, [cajasQuery.data]);

    const centrosCostoCatalogo = useMemo<TesoreriaCatalogItem[]>(() => {
        return (centrosCostoQuery.data ?? []).map((centro) => ({ id: centro.id, nombre: centro.nombre }));
    }, [centrosCostoQuery.data]);

    const cuentasContablesCatalogo = useMemo<TesoreriaCatalogItem[]>(() => {
        const cuentas = cuentasContablesQuery.data ?? [];

        if (modalActivo === 'ingreso') {
            return cuentas
                .filter((cuenta) => String(cuenta.naturaleza) === '2')
                .map((cuenta) => ({ id: cuenta.id, nombre: `${cuenta.codigo} - ${cuenta.descripcion}` }));
        }

        if (modalActivo === 'egreso') {
            return cuentas
                .filter((cuenta) => String(cuenta.naturaleza) === '1')
                .map((cuenta) => ({ id: cuenta.id, nombre: `${cuenta.codigo} - ${cuenta.descripcion}` }));
        }

        return cuentas.map((cuenta) => ({ id: cuenta.id, nombre: `${cuenta.codigo} - ${cuenta.descripcion}` }));
    }, [cuentasContablesQuery.data, modalActivo]);

    const onOpenModal = (tipo: 'ingreso' | 'egreso') => {
        setMensajeExito(null);
        setModalActivo(tipo);
    };

    const onSubmitIngreso = async (values: TesoreriaMovimientoFormValues) => {
        await registrarIngresoMutation.mutateAsync(toMovimientoPayload(values));
    };

    const onSubmitEgreso = async (values: TesoreriaMovimientoFormValues) => {
        await registrarEgresoMutation.mutateAsync(toMovimientoPayload(values));
    };

    return (
        <main className="min-h-screen bg-slate-50 px-6 py-10">
            <div className="mx-auto w-full max-w-6xl">
                <header className="mb-6">
                    <h1 className="text-3xl font-bold text-slate-900">Tesoreria</h1>
                    <p className="mt-1 text-slate-600">Control de cajas con registro de ingresos y egresos.</p>
                </header>

                {mensajeExito ? (
                    <div className="mb-6 rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3 text-sm text-emerald-700">
                        {mensajeExito}
                    </div>
                ) : null}

                <section className="grid grid-cols-1 gap-4 md:grid-cols-2 xl:grid-cols-3">
                    {(cajasQuery.data ?? []).map((caja) => (
                        <article key={caja.id} className="rounded-xl border border-slate-200 bg-white p-5 shadow-sm">
                            <p className="text-sm text-slate-500">{caja.nombre}</p>
                            <p className="mt-2 text-3xl font-bold text-slate-900">{formatCOP(caja.saldoActual)}</p>
                        </article>
                    ))}

                    {cajasQuery.isLoading ? (
                        <article className="rounded-xl border border-slate-200 bg-white p-5 text-sm text-slate-500">
                            Cargando cajas...
                        </article>
                    ) : null}

                    {cajasQuery.isError ? (
                        <article className="rounded-xl border border-rose-200 bg-rose-50 p-5 text-sm text-rose-700">
                            {getErrorMessage(cajasQuery.error)}
                        </article>
                    ) : null}
                </section>

                <section className="mt-8 grid grid-cols-1 gap-4 md:grid-cols-2">
                    <button
                        type="button"
                        onClick={() => onOpenModal('ingreso')}
                        className="rounded-2xl bg-emerald-700 px-6 py-5 text-lg font-semibold text-white transition hover:bg-emerald-800"
                    >
                        Registrar Ingreso
                    </button>

                    <button
                        type="button"
                        onClick={() => onOpenModal('egreso')}
                        className="rounded-2xl bg-rose-700 px-6 py-5 text-lg font-semibold text-white transition hover:bg-rose-800"
                    >
                        Registrar Egreso
                    </button>
                </section>
            </div>

            <TesoreriaMovimientoModal
                modo="ingreso"
                abierto={modalActivo === 'ingreso'}
                cajas={cajasCatalogo}
                cuentasContables={cuentasContablesCatalogo}
                centrosCosto={centrosCostoCatalogo}
                enviando={registrarIngresoMutation.isPending}
                error={registrarIngresoMutation.isError ? getErrorMessage(registrarIngresoMutation.error) : null}
                onCerrar={() => setModalActivo(null)}
                onEnviar={onSubmitIngreso}
            />

            <TesoreriaMovimientoModal
                modo="egreso"
                abierto={modalActivo === 'egreso'}
                cajas={cajasCatalogo}
                cuentasContables={cuentasContablesCatalogo}
                centrosCosto={centrosCostoCatalogo}
                enviando={registrarEgresoMutation.isPending}
                error={registrarEgresoMutation.isError ? getErrorMessage(registrarEgresoMutation.error) : null}
                onCerrar={() => setModalActivo(null)}
                onEnviar={onSubmitEgreso}
            />
        </main>
    );
}
