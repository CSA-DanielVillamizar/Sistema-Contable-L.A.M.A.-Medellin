import { z } from 'zod';

export const tiposDocumentoBeneficiarioOptions = [
    { value: 'CC', label: 'CC' },
    { value: 'TI', label: 'TI' },
    { value: 'CE', label: 'CE' },
    { value: 'NIT', label: 'NIT' },
    { value: 'Pasaporte', label: 'Pasaporte' },
    { value: 'Otro', label: 'Otro' },
] as const;

export const beneficiarioSchema = z.object({
    NombreCompleto: z.string().trim().min(1, 'El nombre completo es obligatorio.').max(200, 'Máximo 200 caracteres.'),
    TipoDocumento: z.string().trim().min(1, 'El tipo de documento es obligatorio.').max(30, 'Máximo 30 caracteres.'),
    NumeroDocumento: z.string().trim().min(1, 'El número de documento es obligatorio.').max(30, 'Máximo 30 caracteres.'),
    Email: z.string().trim().min(1, 'El correo es obligatorio.').email('Correo inválido.').max(200, 'Máximo 200 caracteres.'),
    Telefono: z.string().trim().min(1, 'El teléfono es obligatorio.').max(30, 'Máximo 30 caracteres.'),
    TieneConsentimientoHabeasData: z.boolean().refine((value) => value === true, {
        message: 'Debes aceptar el consentimiento de Habeas Data.',
    }),
});

export type BeneficiarioFormInput = z.input<typeof beneficiarioSchema>;
export type BeneficiarioFormValues = z.output<typeof beneficiarioSchema>;
