'use client';

import { useCrearMiembro } from '@/features/cartera/hooks/useCrearMiembro';
import {
  crearMiembroSchema,
  type CrearMiembroFormInput,
  type CrearMiembroFormValues,
} from '@/features/cartera/schemas/carteraSchemas';
import { zodResolver } from '@hookform/resolvers/zod';
import { useState } from 'react';
import { useForm } from 'react-hook-form';

const defaultValues: CrearMiembroFormInput = {
  documentoIdentidad: '',
  nombres: '',
  apellidos: '',
  apodo: '',
  fechaIngreso: '',
  tipoSangre: 1,
  nombreContactoEmergencia: '',
  telefonoContactoEmergencia: '',
  marcaMoto: '',
  modeloMoto: '',
  cilindraje: 0,
  placa: '',
  rango: '',
};

// Valores tomados de RangoClub.cs. La mayoria de los miembros no ocupa
// ningun cargo directivo, de ahi la opcion "Sin cargo" en el formulario.
const rangosClub = [
  { value: 1, label: 'President' },
  { value: 2, label: 'Vice President' },
  { value: 3, label: 'Treasurer' },
  { value: 4, label: 'Business Manager' },
  { value: 5, label: 'Secretary' },
  { value: 6, label: 'Moto Touring Officer (MTO)' },
  { value: 7, label: 'Sergeant At Arms' },
  { value: 8, label: 'Road Captain' },
];

// Valores tomados de GrupoSanguineo.cs
const gruposSanguineos = [
  { value: 1, label: 'O+' },
  { value: 2, label: 'O-' },
  { value: 3, label: 'A+' },
  { value: 4, label: 'A-' },
  { value: 5, label: 'B+' },
  { value: 6, label: 'B-' },
  { value: 7, label: 'AB+' },
  { value: 8, label: 'AB-' },
];

export default function CrearMiembroForm() {
  const [mensajeExito, setMensajeExito] = useState('');
  const [mensajeError, setMensajeError] = useState('');

  const { mutateAsync, isPending } = useCrearMiembro({
    onSuccessNotification: (message) => {
      setMensajeError('');
      setMensajeExito(message);
    },
    onErrorNotification: (message) => {
      setMensajeExito('');
      setMensajeError(message);
    },
  });

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CrearMiembroFormInput, unknown, CrearMiembroFormValues>({
    resolver: zodResolver(crearMiembroSchema),
    defaultValues,
  });

  const onSubmit = async (values: CrearMiembroFormValues) => {
    await mutateAsync({
      documentoIdentidad: values.documentoIdentidad,
      nombres: values.nombres,
      apellidos: values.apellidos,
      apodo: values.apodo,
      fechaIngreso: values.fechaIngreso,
      tipoSangre: values.tipoSangre,
      nombreContactoEmergencia: values.nombreContactoEmergencia,
      telefonoContactoEmergencia: values.telefonoContactoEmergencia,
      marcaMoto: values.marcaMoto,
      modeloMoto: values.modeloMoto,
      cilindraje: values.cilindraje,
      placa: values.placa,
      rango: values.rango,
    });

    reset({
      ...defaultValues,
      rango: values.rango === null ? '' : values.rango,
    });
  };

  return (
    <section className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700">Documento de identidad</label>
          <input
            type="text"
            {...register('documentoIdentidad')}
            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
            placeholder="Ej: CC-12345678"
          />
          {errors.documentoIdentidad && <p className="mt-1 text-sm text-red-600">{errors.documentoIdentidad.message}</p>}
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700">Nombres</label>
          <input
            type="text"
            {...register('nombres')}
            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
            placeholder="Ej: Daniel"
          />
          {errors.nombres && <p className="mt-1 text-sm text-red-600">{errors.nombres.message}</p>}
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700">Apellidos</label>
          <input
            type="text"
            {...register('apellidos')}
            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
            placeholder="Ej: Villamizar"
          />
          {errors.apellidos && <p className="mt-1 text-sm text-red-600">{errors.apellidos.message}</p>}
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700">Apodo</label>
          <input
            type="text"
            {...register('apodo')}
            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
            placeholder="Opcional"
          />
          {errors.apodo && <p className="mt-1 text-sm text-red-600">{errors.apodo.message}</p>}
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700">Fecha de ingreso</label>
          <input
            type="date"
            {...register('fechaIngreso')}
            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
          />
          {errors.fechaIngreso && <p className="mt-1 text-sm text-red-600">{errors.fechaIngreso.message}</p>}
        </div>

        <div>
          <label className="mb-1 block text-sm font-medium text-slate-700">Cargo en el club (opcional)</label>
          <select
            {...register('rango')}
            className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
          >
            <option value="">Sin cargo</option>
            {rangosClub.map((rango) => (
              <option key={rango.value} value={rango.value}>
                {rango.label}
              </option>
            ))}
          </select>
          {errors.rango && <p className="mt-1 text-sm text-red-600">{errors.rango.message}</p>}
        </div>

        <fieldset className="space-y-5 border-t border-slate-200 pt-5">
          <legend className="text-sm font-semibold text-slate-900">Datos de emergencia</legend>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">Tipo de sangre</label>
            <select
              {...register('tipoSangre')}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
            >
              {gruposSanguineos.map((grupo) => (
                <option key={grupo.value} value={grupo.value}>
                  {grupo.label}
                </option>
              ))}
            </select>
            {errors.tipoSangre && <p className="mt-1 text-sm text-red-600">{errors.tipoSangre.message}</p>}
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">Nombre del contacto de emergencia</label>
            <input
              type="text"
              {...register('nombreContactoEmergencia')}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
              placeholder="Ej: Maria Gonzalez"
            />
            {errors.nombreContactoEmergencia && (
              <p className="mt-1 text-sm text-red-600">{errors.nombreContactoEmergencia.message}</p>
            )}
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">Teléfono del contacto de emergencia</label>
            <input
              type="tel"
              {...register('telefonoContactoEmergencia')}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
              placeholder="Ej: 3104363831"
            />
            {errors.telefonoContactoEmergencia && (
              <p className="mt-1 text-sm text-red-600">{errors.telefonoContactoEmergencia.message}</p>
            )}
          </div>
        </fieldset>

        <fieldset className="space-y-5 border-t border-slate-200 pt-5">
          <legend className="text-sm font-semibold text-slate-900">Motocicleta</legend>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">Marca</label>
            <input
              type="text"
              {...register('marcaMoto')}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
              placeholder="Ej: Harley-Davidson"
            />
            {errors.marcaMoto && <p className="mt-1 text-sm text-red-600">{errors.marcaMoto.message}</p>}
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">Modelo</label>
            <input
              type="text"
              {...register('modeloMoto')}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
              placeholder="Ej: Softail"
            />
            {errors.modeloMoto && <p className="mt-1 text-sm text-red-600">{errors.modeloMoto.message}</p>}
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">Cilindraje (cc)</label>
            <input
              type="number"
              min={1}
              {...register('cilindraje')}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
              placeholder="Ej: 883"
            />
            {errors.cilindraje && <p className="mt-1 text-sm text-red-600">{errors.cilindraje.message}</p>}
          </div>

          <div>
            <label className="mb-1 block text-sm font-medium text-slate-700">Placa</label>
            <input
              type="text"
              {...register('placa')}
              className="w-full rounded-lg border border-slate-300 px-3 py-2 text-slate-900 outline-none ring-blue-100 focus:border-blue-500 focus:ring-2"
              placeholder="Ej: ABC12D"
            />
            {errors.placa && <p className="mt-1 text-sm text-red-600">{errors.placa.message}</p>}
          </div>
        </fieldset>

        {mensajeError && (
          <div className="rounded-lg border border-red-200 bg-red-50 px-3 py-2 text-sm text-red-700">{mensajeError}</div>
        )}

        {mensajeExito && (
          <div className="rounded-lg border border-emerald-200 bg-emerald-50 px-3 py-2 text-sm text-emerald-700">{mensajeExito}</div>
        )}

        <button
          type="submit"
          disabled={isPending}
          className="inline-flex items-center rounded-lg bg-blue-700 px-4 py-2 text-sm font-medium text-white transition hover:bg-blue-800 disabled:cursor-not-allowed disabled:opacity-60"
        >
          {isPending ? 'Guardando...' : 'Crear miembro'}
        </button>
      </form>
    </section>
  );
}
