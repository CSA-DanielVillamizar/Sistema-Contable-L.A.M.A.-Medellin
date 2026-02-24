'use client';

import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { useQuery } from '@tanstack/react-query';
import { zodResolver } from '@hookform/resolvers/zod';
import { egresoSchema, type EgresoFormInput, type EgresoFormValues } from '@/features/transacciones/schemas/egresoSchema';
import { useCrearEgreso } from '@/features/transacciones/hooks/useCrearEgreso';
import { useTrmOficial } from '@/features/transacciones/hooks/useTrmOficial';
import apiClient from '@/lib/apiClient';

const defaultValues: EgresoFormInput = {
  MontoCOP: 0,
  CentroCostoId: '',
  BancoId: '',
  MedioPago: '',
  Descripcion: '',
  EsMonedaOrigenUSD: false,
};

type BancoCatalogo = {
  id: string;
  numeroCuenta: string;
};

type CentroCostoCatalogo = {
  id: string;
  nombre: string;
};

export default function RegistroEgresoForm() {
  const [mensajeExito, setMensajeExito] = useState<string>('');
  const { mutateAsync, isPending, error } = useCrearEgreso();

  const bancosQuery = useQuery({
    queryKey: ['transacciones', 'catalogo', 'bancos'],
    queryFn: async () => {
      const response = await apiClient.get<any[]>('/api/transacciones/bancos');

      return (response.data ?? [])
        .map((item) => ({
          id: String(item?.id ?? item?.Id ?? ''),
          numeroCuenta: String(item?.numeroCuenta ?? item?.NumeroCuenta ?? ''),
        }))
        .filter((item) => item.id.length > 0);
    },
  });

  const centrosCostoQuery = useQuery({
    queryKey: ['transacciones', 'catalogo', 'centros-costo'],
    queryFn: async () => {
      const response = await apiClient.get<any[]>('/api/transacciones/centros-costo');

      return (response.data ?? [])
        .map((item) => ({
          id: String(item?.id ?? item?.Id ?? ''),
          nombre: String(item?.nombre ?? item?.Nombre ?? ''),
        }))
        .filter((item) => item.id.length > 0);
    },
  });

  const {
    register,
    watch,
    handleSubmit,
    reset,
    setValue,
    getValues,
    formState: { errors },
  } = useForm<EgresoFormInput, unknown, EgresoFormValues>({
    resolver: zodResolver(egresoSchema),
    defaultValues,
  });

  useEffect(() => {
    if ((bancosQuery.data?.length ?? 0) > 0 && !getValues('BancoId')) {
      setValue('BancoId', bancosQuery.data![0].id, { shouldValidate: true });
    }
  }, [bancosQuery.data, getValues, setValue]);

  useEffect(() => {
    if ((centrosCostoQuery.data?.length ?? 0) > 0 && !getValues('CentroCostoId')) {
      setValue('CentroCostoId', centrosCostoQuery.data![0].id, { shouldValidate: true });
    }
  }, [centrosCostoQuery.data, getValues, setValue]);

  const esMonedaOrigenUSD = watch('EsMonedaOrigenUSD') ?? false;
  const trmQuery = useTrmOficial(esMonedaOrigenUSD);

  useEffect(() => {
    if (!esMonedaOrigenUSD || !trmQuery.data) {
      return;
    }

    if (!getValues('TasaCambioUsada')) {
      setValue('TasaCambioUsada', trmQuery.data.tasaCambioUsada, { shouldValidate: true });
    }

    if (!getValues('FechaTasaCambio')) {
      setValue('FechaTasaCambio', trmQuery.data.fechaTasaCambio, { shouldValidate: true });
    }

    if (!getValues('FuenteTasaCambio')) {
      setValue('FuenteTasaCambio', trmQuery.data.fuenteTasaCambio, { shouldValidate: true });
    }
  }, [esMonedaOrigenUSD, getValues, setValue, trmQuery.data]);

  const cargarTrmOficial = async () => {
    const response = await trmQuery.refetch();
    if (!response.data) {
      return;
    }

    setValue('TasaCambioUsada', response.data.tasaCambioUsada, { shouldValidate: true });
    setValue('FechaTasaCambio', response.data.fechaTasaCambio, { shouldValidate: true });
    setValue('FuenteTasaCambio', response.data.fuenteTasaCambio, { shouldValidate: true });
  };

  const onSubmit = async (values: EgresoFormValues) => {
    setMensajeExito('');

    await mutateAsync({
      MontoCOP: values.MontoCOP,
      CentroCostoId: values.CentroCostoId,
      BancoId: values.BancoId,
      MedioPago: Number(values.MedioPago),
      Descripcion: values.Descripcion,
      MonedaOrigen: values.EsMonedaOrigenUSD ? 'USD' : undefined,
      MontoMonedaOrigen: values.EsMonedaOrigenUSD ? values.MontoMonedaOrigen : undefined,
      TasaCambioUsada: values.EsMonedaOrigenUSD ? values.TasaCambioUsada : undefined,
      FechaTasaCambio: values.EsMonedaOrigenUSD ? values.FechaTasaCambio : undefined,
      FuenteTasaCambio: values.EsMonedaOrigenUSD ? values.FuenteTasaCambio : undefined,
    });

    setMensajeExito('Egreso registrado correctamente.');
    reset({
      ...defaultValues,
      BancoId: bancosQuery.data?.[0]?.id ?? '',
      CentroCostoId: centrosCostoQuery.data?.[0]?.id ?? '',
    });
  };

  return (
    <section className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700">Monto en COP</label>
          <input
            type="number"
            step="0.01"
            {...register('MontoCOP')}
            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
          />
          {errors.MontoCOP && <p className="mt-1 text-sm text-red-600">{errors.MontoCOP.message}</p>}
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700">Centro de costo</label>
          <select
            {...register('CentroCostoId')}
            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
            disabled={centrosCostoQuery.isLoading || (centrosCostoQuery.data?.length ?? 0) === 0}
          >
            {[{ id: '', nombre: 'Seleccione...' }, ...(centrosCostoQuery.data ?? [])].map((centroCosto, index) => (
              <option key={`centro-${centroCosto.id}-${index}`} value={centroCosto.id}>
                {centroCosto.nombre}
              </option>
            ))}
          </select>
          {centrosCostoQuery.isLoading && <p className="mt-1 text-sm text-slate-500">Cargando centros de costo...</p>}
          {centrosCostoQuery.isError && <p className="mt-1 text-sm text-red-600">No fue posible cargar centros de costo.</p>}
          {errors.CentroCostoId && <p className="mt-1 text-sm text-red-600">{errors.CentroCostoId.message}</p>}
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700">Banco</label>
          <select
            {...register('BancoId')}
            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
            disabled={bancosQuery.isLoading || (bancosQuery.data?.length ?? 0) === 0}
          >
            {[{ id: '', numeroCuenta: 'Seleccione...' }, ...(bancosQuery.data ?? [])].map((banco, index) => (
              <option key={`banco-${banco.id}-${index}`} value={banco.id}>
                {banco.numeroCuenta}
              </option>
            ))}
          </select>
          {bancosQuery.isLoading && <p className="mt-1 text-sm text-slate-500">Cargando bancos...</p>}
          {bancosQuery.isError && <p className="mt-1 text-sm text-red-600">No fue posible cargar bancos.</p>}
          {errors.BancoId && <p className="mt-1 text-sm text-red-600">{errors.BancoId.message}</p>}
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700">Medio de pago</label>
          <select
            {...register('MedioPago')}
            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
          >
            {[
              { value: '', label: 'Seleccione...' },
              { value: '1', label: 'Transferencia' },
              { value: '2', label: 'Consignación' },
              { value: '3', label: 'Corresponsal' },
              { value: '4', label: 'QR' },
            ].map((medio, index) => (
              <option key={`medio-${medio.value}-${index}`} value={medio.value}>
                {medio.label}
              </option>
            ))}
          </select>
          {errors.MedioPago && <p className="mt-1 text-sm text-red-600">{errors.MedioPago.message}</p>}
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700">Descripción</label>
          <textarea
            rows={3}
            {...register('Descripcion')}
            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
          />
          {errors.Descripcion && <p className="mt-1 text-sm text-red-600">{errors.Descripcion.message}</p>}
        </div>

        <label className="flex items-center gap-2 rounded-lg border border-slate-200 bg-slate-50 px-3 py-2 text-sm text-slate-700">
          <input type="checkbox" {...register('EsMonedaOrigenUSD')} />
          La transacción fue en USD
        </label>

        {esMonedaOrigenUSD && (
          <div className="grid grid-cols-1 gap-4 rounded-lg border border-blue-100 bg-blue-50/40 p-4 md:grid-cols-4">
            <div className="md:col-span-4">
              <button
                type="button"
                onClick={cargarTrmOficial}
                disabled={trmQuery.isFetching}
                className="inline-flex items-center rounded-lg border border-blue-200 bg-white px-3 py-2 text-sm font-medium text-blue-700 transition hover:bg-blue-50 disabled:cursor-not-allowed disabled:opacity-60"
              >
                {trmQuery.isFetching ? 'Consultando TRM oficial...' : 'Cargar TRM oficial'}
              </button>
              {trmQuery.isError && (
                <p className="mt-1 text-sm text-red-600">{(trmQuery.error as Error).message}</p>
              )}
              {trmQuery.data && (
                <p className="mt-1 text-sm text-slate-600">
                  Fuente: {trmQuery.data.fuenteNombre} · Fecha: {trmQuery.data.fechaTasaCambio}
                </p>
              )}
            </div>

            <div>
              <label className="mb-1 block text-sm font-medium text-slate-700">Monto en USD</label>
              <input
                type="number"
                step="0.01"
                {...register('MontoMonedaOrigen')}
                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
              />
              {errors.MontoMonedaOrigen && <p className="mt-1 text-sm text-red-600">{errors.MontoMonedaOrigen.message}</p>}
            </div>

            <div>
              <label className="mb-1 block text-sm font-medium text-slate-700">Tasa de cambio</label>
              <input
                type="number"
                step="0.0001"
                {...register('TasaCambioUsada')}
                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
              />
              {errors.TasaCambioUsada && <p className="mt-1 text-sm text-red-600">{errors.TasaCambioUsada.message}</p>}
            </div>

            <div>
              <label className="mb-1 block text-sm font-medium text-slate-700">Fecha de tasa de cambio</label>
              <input
                type="date"
                {...register('FechaTasaCambio')}
                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
              />
              {errors.FechaTasaCambio && <p className="mt-1 text-sm text-red-600">{errors.FechaTasaCambio.message}</p>}
            </div>

            <div>
              <label className="mb-1 block text-sm font-medium text-slate-700">Fuente de tasa de cambio</label>
              <select
                {...register('FuenteTasaCambio')}
                className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
              >
                {[
                  { value: '', label: 'Seleccione...' },
                  { value: '1', label: 'TRM SFC' },
                  { value: '2', label: 'Tasa Banco' },
                  { value: '3', label: 'Manual con Soporte' },
                ].map((fuente, index) => (
                  <option key={`fuente-${fuente.value}-${index}`} value={fuente.value}>
                    {fuente.label}
                  </option>
                ))}
              </select>
              {errors.FuenteTasaCambio && <p className="mt-1 text-sm text-red-600">{errors.FuenteTasaCambio.message}</p>}
            </div>
          </div>
        )}

        {error && (
          <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">{error.message}</div>
        )}

        {mensajeExito && (
          <div className="rounded-lg border border-emerald-200 bg-emerald-50 px-3 py-2 text-sm text-emerald-700">{mensajeExito}</div>
        )}

        <button
          type="submit"
          disabled={isPending}
          className="inline-flex items-center rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white transition hover:bg-blue-800 disabled:cursor-not-allowed disabled:opacity-60"
        >
          {isPending ? 'Guardando...' : 'Registrar egreso'}
        </button>
      </form>
    </section>
  );
}
