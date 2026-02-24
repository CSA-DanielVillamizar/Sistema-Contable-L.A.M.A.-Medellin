import { z } from 'zod';

const guidVacio = '00000000-0000-0000-0000-000000000000';

export const egresoSchema = z
    .object({
        MontoCOP: z.coerce.number().gt(0, 'El monto en COP debe ser mayor a 0.'),
        CentroCostoId: z
            .string()
            .trim()
            .uuid('Centro de costo inválido.')
            .refine((value) => value !== guidVacio, 'Debes seleccionar un centro de costo válido.'),
        BancoId: z
            .string()
            .trim()
            .uuid('Banco inválido.')
            .refine((value) => value !== guidVacio, 'Debes seleccionar un banco válido.'),
        MedioPago: z.string().trim().min(1, 'Medio de pago es requerido.'),
        Descripcion: z.string().trim().min(1, 'La descripción es requerida.').max(500, 'La descripción no puede superar 500 caracteres.'),
        EsMonedaOrigenUSD: z.boolean().default(false),
        MontoMonedaOrigen: z.coerce.number().optional(),
        TasaCambioUsada: z.coerce.number().optional(),
        FechaTasaCambio: z.string().optional(),
        FuenteTasaCambio: z.coerce.number().optional(),
    })
    .strict()
    .superRefine((value, ctx) => {
        if (!value.EsMonedaOrigenUSD) {
            return;
        }

        if (value.MontoMonedaOrigen === undefined || value.MontoMonedaOrigen <= 0) {
            ctx.addIssue({
                code: z.ZodIssueCode.custom,
                path: ['MontoMonedaOrigen'],
                message: 'El monto en USD es obligatorio y debe ser mayor a 0 cuando la moneda origen es USD.',
            });
        }

        if (value.TasaCambioUsada === undefined || value.TasaCambioUsada <= 0) {
            ctx.addIssue({
                code: z.ZodIssueCode.custom,
                path: ['TasaCambioUsada'],
                message: 'La tasa de cambio es obligatoria y debe ser mayor a 0 cuando la moneda origen es USD.',
            });
        }

        if (!value.FechaTasaCambio || value.FechaTasaCambio.trim().length === 0) {
            ctx.addIssue({
                code: z.ZodIssueCode.custom,
                path: ['FechaTasaCambio'],
                message: 'La fecha de tasa de cambio es obligatoria cuando la moneda origen es USD.',
            });
        }

        if (value.FuenteTasaCambio === undefined || value.FuenteTasaCambio <= 0) {
            ctx.addIssue({
                code: z.ZodIssueCode.custom,
                path: ['FuenteTasaCambio'],
                message: 'La fuente de tasa de cambio es obligatoria cuando la moneda origen es USD.',
            });
        }
    });

export type EgresoFormInput = z.input<typeof egresoSchema>;
export type EgresoFormValues = z.output<typeof egresoSchema>;
