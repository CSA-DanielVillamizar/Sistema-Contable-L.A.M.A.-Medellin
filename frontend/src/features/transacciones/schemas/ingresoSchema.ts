import { z } from 'zod';

export const ingresoSchema = z
    .object({
        MontoCOP: z.coerce.number().gt(0, 'MontoCOP debe ser mayor a 0.'),
        CentroCostoId: z.string().trim().min(1, 'CentroCostoId es requerido.'),
        MedioPago: z.string().trim().min(1, 'MedioPago es requerido.'),
        EsMonedaOrigenUSD: z.boolean().default(false),
        MontoMonedaOrigen: z.coerce.number().optional(),
        TasaCambioUsada: z.coerce.number().optional(),
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
                message: 'MontoMonedaOrigen es obligatorio y debe ser mayor a 0 cuando la moneda origen es USD.',
            });
        }

        if (value.TasaCambioUsada === undefined || value.TasaCambioUsada <= 0) {
            ctx.addIssue({
                code: z.ZodIssueCode.custom,
                path: ['TasaCambioUsada'],
                message: 'TasaCambioUsada es obligatoria y debe ser mayor a 0 cuando la moneda origen es USD.',
            });
        }

        if (value.FuenteTasaCambio === undefined || value.FuenteTasaCambio <= 0) {
            ctx.addIssue({
                code: z.ZodIssueCode.custom,
                path: ['FuenteTasaCambio'],
                message: 'FuenteTasaCambio es obligatoria y debe ser mayor a 0 cuando la moneda origen es USD.',
            });
        }
    });

export type IngresoFormValues = z.infer<typeof ingresoSchema>;
