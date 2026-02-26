import { z } from 'zod';

export const estadosProyectoOptions = [
    { value: 1, label: 'Planificación' },
    { value: 2, label: 'Ejecución' },
    { value: 3, label: 'Finalizado' },
] as const;

const fechaRegex = /^\d{4}-\d{2}-\d{2}$/;

export const proyectoSchema = z
    .object({
        Nombre: z.string().trim().min(1, 'El nombre es obligatorio.').max(200, 'Máximo 200 caracteres.'),
        Descripcion: z.string().trim().min(1, 'La descripción es obligatoria.').max(1000, 'Máximo 1000 caracteres.'),
        FechaInicio: z.string().regex(fechaRegex, 'Fecha de inicio inválida.'),
        FechaFin: z.preprocess(
            (value) => (typeof value === 'string' && value.trim() === '' ? undefined : value),
            z.string().regex(fechaRegex, 'Fecha de fin inválida.').optional(),
        ),
        PresupuestoEstimado: z.coerce.number().min(0, 'El presupuesto no puede ser negativo.'),
        Estado: z.coerce.number().int().min(1).max(3),
    })
    .refine((value) => !value.FechaFin || value.FechaFin >= value.FechaInicio, {
        message: 'La fecha de fin debe ser mayor o igual a la fecha de inicio.',
        path: ['FechaFin'],
    });

export type ProyectoFormInput = z.input<typeof proyectoSchema>;
export type ProyectoFormValues = z.output<typeof proyectoSchema>;
