import { z } from 'zod';

const guidVacio = '00000000-0000-0000-0000-000000000000';

const asientoSchema = z
    .object({
        CuentaContableId: z
            .string()
            .trim()
            .uuid('Cuenta contable inválida.')
            .refine((value) => value !== guidVacio, 'Debes seleccionar una cuenta contable válida.'),
        CentroCostoId: z
            .string()
            .trim()
            .uuid('Centro de costo inválido.')
            .refine((value) => value !== guidVacio, 'Debes seleccionar un centro de costo válido.'),
        TerceroId: z
            .string()
            .trim()
            .uuid('Tercero inválido.')
            .optional()
            .or(z.literal('')),
        Referencia: z.string().trim().min(1, 'La referencia es obligatoria.').max(500, 'Máximo 500 caracteres.'),
        Debe: z.coerce.number().min(0, 'Debe no puede ser negativo.'),
        Haber: z.coerce.number().min(0, 'Haber no puede ser negativo.'),
    })
    .superRefine((value, ctx) =>
    {
        if (value.Debe > 0 && value.Haber > 0)
        {
            ctx.addIssue({
                code: z.ZodIssueCode.custom,
                path: ['Debe'],
                message: 'Una línea no puede tener Debe y Haber mayores a 0 al mismo tiempo.',
            });
        }

        if (value.Debe === 0 && value.Haber === 0)
        {
            ctx.addIssue({
                code: z.ZodIssueCode.custom,
                path: ['Debe'],
                message: 'Debes registrar un valor en Debe o Haber.',
            });
        }
    });

function toCentavos(value: number): number {
    return Math.round(value * 100);
}

export const comprobanteSchema = z
    .object({
        Fecha: z.string().trim().min(1, 'La fecha es obligatoria.'),
        Tipo: z.coerce.number().int().min(1).max(5),
        Descripcion: z.string().trim().min(1, 'La descripción es obligatoria.').max(500, 'Máximo 500 caracteres.'),
        Asientos: z.array(asientoSchema).min(2, 'El comprobante debe tener al menos 2 líneas de asiento.'),
    })
    .refine(
        (values) =>
        {
            const totalDebe = values.Asientos.reduce((sum, item) => sum + toCentavos(item.Debe), 0);
            const totalHaber = values.Asientos.reduce((sum, item) => sum + toCentavos(item.Haber), 0);
            return totalDebe === totalHaber;
        },
        {
            path: ['Asientos'],
            message: 'El comprobante está descuadrado (Debe != Haber)',
        });

export type ComprobanteFormInput = z.input<typeof comprobanteSchema>;
export type ComprobanteFormValues = z.output<typeof comprobanteSchema>;
