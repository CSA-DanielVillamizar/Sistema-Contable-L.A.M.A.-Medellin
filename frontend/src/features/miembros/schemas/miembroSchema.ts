import { z } from 'zod';

export const tiposAfiliacionOptions = [
    { value: 1, label: 'FullColor' },
    { value: 2, label: 'Rockets' },
    { value: 3, label: 'Prospect' },
    { value: 4, label: 'Esposa' },
    { value: 5, label: 'Asociado' },
] as const;

export const estadosMiembroOptions = [
    { value: 1, label: 'Activo' },
    { value: 2, label: 'Inactivo' },
    { value: 3, label: 'Suspendido' },
    { value: 4, label: 'Trasladado' },
] as const;

export const miembroSchema = z.object({
    Nombre: z.string().trim().min(1, 'El nombre es obligatorio.').max(100, 'El nombre no puede superar 100 caracteres.'),
    Apellidos: z.string().trim().min(1, 'Los apellidos son obligatorios.').max(120, 'Los apellidos no pueden superar 120 caracteres.'),
    Documento: z.string().trim().min(1, 'El documento es obligatorio.').max(30, 'El documento no puede superar 30 caracteres.'),
    Email: z.string().trim().min(1, 'El correo es obligatorio.').email('El correo no tiene un formato válido.').max(200, 'El correo no puede superar 200 caracteres.'),
    Telefono: z.string().trim().min(1, 'El teléfono es obligatorio.').max(30, 'El teléfono no puede superar 30 caracteres.'),
    TipoAfiliacion: z.coerce.number().int().min(1).max(5),
    Estado: z.coerce.number().int().min(1).max(4),
});

export type MiembroFormInput = z.input<typeof miembroSchema>;
export type MiembroFormValues = z.output<typeof miembroSchema>;
