import { z } from 'zod';

export const rangosClubOptions = [
    { value: 1, label: 'Aspirante' },
    { value: 2, label: 'Prospecto' },
    { value: 3, label: 'MiembroActivo' },
    { value: 4, label: 'Directivo' },
] as const;

export const gruposSanguineosOptions = [
    { value: 1, label: 'O+' },
    { value: 2, label: 'O-' },
    { value: 3, label: 'A+' },
    { value: 4, label: 'A-' },
    { value: 5, label: 'B+' },
    { value: 6, label: 'B-' },
    { value: 7, label: 'AB+' },
    { value: 8, label: 'AB-' },
] as const;

export const miembroCreateSchema = z.object({
    documentoIdentidad: z.string().trim().min(1, 'El documento de identidad es obligatorio.').max(50, 'El documento no puede superar 50 caracteres.'),
    nombres: z.string().trim().min(1, 'Los nombres son obligatorios.').max(150, 'Los nombres no pueden superar 150 caracteres.'),
    apellidos: z.string().trim().min(1, 'Los apellidos son obligatorios.').max(150, 'Los apellidos no pueden superar 150 caracteres.'),
    apodo: z.string().trim().max(100, 'El apodo no puede superar 100 caracteres.'),
    fechaIngreso: z.string().min(1, 'La fecha de ingreso es obligatoria.'),
    tipoSangre: z.coerce.number().int().min(1).max(8),
    nombreContactoEmergencia: z.string().trim().min(1, 'El contacto de emergencia es obligatorio.').max(150, 'El contacto no puede superar 150 caracteres.'),
    telefonoContactoEmergencia: z.string().trim().min(7, 'El teléfono de emergencia debe tener al menos 7 caracteres.').max(30, 'El teléfono no puede superar 30 caracteres.'),
    marcaMoto: z.string().trim().min(1, 'La marca de la moto es obligatoria.').max(100, 'La marca no puede superar 100 caracteres.'),
    modeloMoto: z.string().trim().min(1, 'El modelo de la moto es obligatorio.').max(100, 'El modelo no puede superar 100 caracteres.'),
    cilindraje: z.coerce.number().int().positive('El cilindraje debe ser mayor a cero.'),
    placa: z.string().trim().min(1, 'La placa es obligatoria.').max(20, 'La placa no puede superar 20 caracteres.'),
    rango: z.coerce.number().int().min(1).max(4),
    esActivo: z.boolean(),
});

export const miembroUpdateSchema = z.object({
    tipoSangre: z.coerce.number().int().min(1).max(8),
    nombreContactoEmergencia: z.string().trim().min(1, 'El contacto de emergencia es obligatorio.').max(150, 'El contacto no puede superar 150 caracteres.'),
    telefonoContactoEmergencia: z.string().trim().min(7, 'El teléfono de emergencia debe tener al menos 7 caracteres.').max(30, 'El teléfono no puede superar 30 caracteres.'),
    marcaMoto: z.string().trim().min(1, 'La marca de la moto es obligatoria.').max(100, 'La marca no puede superar 100 caracteres.'),
    modeloMoto: z.string().trim().min(1, 'El modelo de la moto es obligatorio.').max(100, 'El modelo no puede superar 100 caracteres.'),
    cilindraje: z.coerce.number().int().positive('El cilindraje debe ser mayor a cero.'),
    placa: z.string().trim().min(1, 'La placa es obligatoria.').max(20, 'La placa no puede superar 20 caracteres.'),
    rango: z.coerce.number().int().min(1).max(4),
    esActivo: z.boolean(),
});

export type MiembroCreateFormValues = z.infer<typeof miembroCreateSchema>;
export type MiembroUpdateFormValues = z.infer<typeof miembroUpdateSchema>;
export type MiembroCreateFormInput = MiembroCreateFormValues;

export function mapGrupoSanguineoToValue(value: string): number {
    const normalized = value.trim().toUpperCase();

    const map: Record<string, number> = {
        O_POSITIVO: 1,
        O_NEGATIVO: 2,
        A_POSITIVO: 3,
        A_NEGATIVO: 4,
        B_POSITIVO: 5,
        B_NEGATIVO: 6,
        AB_POSITIVO: 7,
        AB_NEGATIVO: 8,
        'O+': 1,
        'O-': 2,
        'A+': 3,
        'A-': 4,
        'B+': 5,
        'B-': 6,
        'AB+': 7,
        'AB-': 8,
    };

    return map[normalized] ?? 1;
}

export function mapRangoClubToValue(value: string): number {
    return rangosClubOptions.find((item) => item.label.toUpperCase() === value.toUpperCase())?.value ?? 1;
}
