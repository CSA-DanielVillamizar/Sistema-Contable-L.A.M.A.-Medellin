import { z } from 'zod';

const guidVacio = '00000000-0000-0000-0000-000000000000';

export const tiposPersonaOptions = [
    { value: 1, label: 'Natural' },
    { value: 2, label: 'Jurídica' },
] as const;

export const tiposDocumentoOptions = [
    { value: 1, label: 'CC' },
    { value: 2, label: 'NIT' },
    { value: 3, label: 'CE' },
    { value: 4, label: 'Pasaporte' },
    { value: 5, label: 'Otro' },
] as const;

export const mediosPagoOptions = [
    { value: 1, label: 'Transferencia' },
    { value: 2, label: 'Consignación' },
    { value: 3, label: 'Corresponsal' },
    { value: 4, label: 'QR' },
] as const;

export const donanteSchema = z.object({
    NombreORazonSocial: z.string().trim().min(1, 'Nombre o razón social es obligatorio.').max(200, 'Máximo 200 caracteres.'),
    TipoDocumento: z.coerce.number().int().min(1).max(5),
    NumeroDocumento: z.string().trim().min(1, 'Número de documento es obligatorio.').max(30, 'Máximo 30 caracteres.'),
    Email: z.string().trim().min(1, 'Correo es obligatorio.').email('Correo inválido.').max(200, 'Máximo 200 caracteres.'),
    TipoPersona: z.coerce.number().int().min(1).max(2),
});

export const donacionSchema = z.object({
    DonanteId: z.string().trim().uuid('Donante inválido.').refine((v) => v !== guidVacio, 'Debes seleccionar un donante.'),
    MontoCOP: z.coerce.number().gt(0, 'El monto debe ser mayor a 0.'),
    BancoId: z.string().trim().uuid('Banco inválido.').refine((v) => v !== guidVacio, 'Debes seleccionar un banco válido.'),
    CentroCostoId: z
        .string()
        .trim()
        .uuid('Centro de costo inválido.')
        .refine((v) => v !== guidVacio, 'Debes seleccionar un centro de costo válido.'),
    MedioPago: z.coerce.number().int().min(1).max(4),
    Descripcion: z.string().trim().max(500, 'Máximo 500 caracteres.').optional(),
});

export type DonanteFormInput = z.input<typeof donanteSchema>;
export type DonanteFormValues = z.output<typeof donanteSchema>;
export type DonacionFormInput = z.input<typeof donacionSchema>;
export type DonacionFormValues = z.output<typeof donacionSchema>;
