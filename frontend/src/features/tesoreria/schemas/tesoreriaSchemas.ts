import { z } from 'zod';

const uuidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i;

const baseRegistrarMovimientoSchema = z.object({
    monto: z
        .coerce
        .number({ error: 'Monto debe ser un numero.' })
        .gt(0, 'Monto debe ser mayor a cero.'),
    concepto: z
        .string()
        .trim()
        .min(1, 'Concepto es obligatorio.')
        .max(500, 'Concepto no puede exceder 500 caracteres.'),
    cuentaContableId: z
        .string()
        .trim()
        .min(1, 'CuentaContableId es obligatorio.')
        .regex(uuidRegex, 'CuentaContableId debe tener formato UUID (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).'),
    bancoId: z
        .string()
        .trim()
        .min(1, 'BancoId es obligatorio.')
        .regex(uuidRegex, 'BancoId debe tener formato UUID (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).'),
    centroCostoId: z
        .string()
        .trim()
        .min(1, 'CentroCostoId es obligatorio.')
        .regex(uuidRegex, 'CentroCostoId debe tener formato UUID (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx).'),
});

export const registrarIngresoSchema = baseRegistrarMovimientoSchema;
export const registrarEgresoSchema = baseRegistrarMovimientoSchema;

export type RegistrarIngresoFormInput = z.input<typeof registrarIngresoSchema>;
export type RegistrarIngresoFormValues = z.output<typeof registrarIngresoSchema>;

export type RegistrarEgresoFormInput = z.input<typeof registrarEgresoSchema>;
export type RegistrarEgresoFormValues = z.output<typeof registrarEgresoSchema>;
