import { z } from 'zod';

/**
 * Cargo directivo del miembro. Espejo de RangoClub.cs. Es opcional: la
 * mayoria de los miembros no ocupa ningun cargo.
 */
export const rangosClubOptions = [
    { value: 1, label: 'President' },
    { value: 2, label: 'Vice President' },
    { value: 3, label: 'Treasurer' },
    { value: 4, label: 'Business Manager' },
    { value: 5, label: 'Secretary' },
    { value: 6, label: 'Moto Touring Officer (MTO)' },
    { value: 7, label: 'Sergeant At Arms' },
    { value: 8, label: 'Road Captain' },
] as const;

/**
 * Tipo de afiliacion del miembro. Espejo de TipoAfiliacion.cs. Se omite
 * LadyLama(6) a proposito: quedo obsoleto en el backend en favor de las tres
 * etapas de Dama L.A.M.A. (Prospect/Rockets/Full Color) y no debe elegirse
 * para miembros nuevos.
 */
export const tipoAfiliacionOptions = [
    { value: 1, label: 'Full Color Member' },
    { value: 3, label: 'Prospect' },
    { value: 2, label: 'Rockets' },
    { value: 8, label: 'Dama L.A.M.A. Prospect' },
    { value: 9, label: 'Dama L.A.M.A. Rockets' },
    { value: 10, label: 'Dama L.A.M.A. Full Color Member' },
    { value: 4, label: 'Spousal' },
    { value: 5, label: 'Associate' },
    { value: 7, label: 'Youth' },
    { value: 11, label: 'Honorary Member' },
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
    modeloMoto: z.string().trim().min(1, 'El modelo de la moto es obligatoria.').max(100, 'El modelo no puede superar 100 caracteres.'),
    cilindraje: z.coerce.number().int().positive('El cilindraje debe ser mayor a cero.'),
    placa: z.string().trim().min(1, 'La placa es obligatoria.').max(20, 'La placa no puede superar 20 caracteres.'),
    tipoAfiliacion: z.coerce.number().int(),
    rango: z.coerce.number().int().min(1).max(8).nullable(),
    esActivo: z.boolean(),
});

export const miembroUpdateSchema = z.object({
    tipoSangre: z.coerce.number().int().min(1).max(8),
    nombreContactoEmergencia: z.string().trim().min(1, 'El contacto de emergencia es obligatorio.').max(150, 'El contacto no puede superar 150 caracteres.'),
    telefonoContactoEmergencia: z.string().trim().min(7, 'El teléfono de emergencia debe tener al menos 7 caracteres.').max(30, 'El teléfono no puede superar 30 caracteres.'),
    marcaMoto: z.string().trim().min(1, 'La marca de la moto es obligatoria.').max(100, 'La marca no puede superar 100 caracteres.'),
    modeloMoto: z.string().trim().min(1, 'El modelo de la moto es obligatoria.').max(100, 'El modelo no puede superar 100 caracteres.'),
    cilindraje: z.coerce.number().int().positive('El cilindraje debe ser mayor a cero.'),
    placa: z.string().trim().min(1, 'La placa es obligatoria.').max(20, 'La placa no puede superar 20 caracteres.'),
    tipoAfiliacion: z.coerce.number().int(),
    rango: z.coerce.number().int().min(1).max(8).nullable(),
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

function normalizarParaComparar(value: string): string {
    return value.replace(/[\s().]/g, '').toUpperCase();
}

/**
 * El backend devuelve el nombre del enum en ingles sin espacios (ej.
 * "VicePresident"), y las opciones de UI usan una etiqueta legible (ej.
 * "Vice President"): se comparan sin espacios/mayusculas para que ambas
 * formas calcen.
 */
export function mapRangoClubToValue(value: string): number | null {
    if (!value) {
        return null;
    }

    return rangosClubOptions.find((item) => normalizarParaComparar(item.label) === normalizarParaComparar(value))?.value ?? null;
}

export function mapTipoAfiliacionToValue(value: string): number {
    if (!value) {
        return 3;
    }

    const porNombreEnum: Record<string, number> = {
        FULLCOLOR: 1,
        ROCKETS: 2,
        PROSPECT: 3,
        ESPOSA: 4,
        ASOCIADO: 5,
        LADYLAMA: 6,
        YOUTH: 7,
        DAMAPROSPECT: 8,
        DAMAROCKETS: 9,
        DAMAFULLCOLOR: 10,
        HONORARY: 11,
    };

    const normalizado = normalizarParaComparar(value);
    return porNombreEnum[normalizado]
        ?? tipoAfiliacionOptions.find((item) => normalizarParaComparar(item.label) === normalizado)?.value
        ?? 3;
}

/** Etiqueta legible para mostrar en el directorio a partir del nombre de enum que devuelve el backend. */
export function labelDeTipoAfiliacion(nombreEnum: string): string {
    if (!nombreEnum) {
        return 'Sin definir';
    }

    if (normalizarParaComparar(nombreEnum) === 'LADYLAMA') {
        return 'Lady L.A.M.A. (legado)';
    }

    const valor = mapTipoAfiliacionToValue(nombreEnum);
    return tipoAfiliacionOptions.find((item) => item.value === valor)?.label ?? nombreEnum;
}

/** Etiqueta legible del cargo directivo, o "Sin cargo" si el miembro no tiene ninguno. */
export function labelDeRango(nombreEnum: string): string {
    if (!nombreEnum) {
        return 'Sin cargo';
    }

    const valor = mapRangoClubToValue(nombreEnum);
    return rangosClubOptions.find((item) => item.value === valor)?.label ?? nombreEnum;
}
