import { z } from 'zod';

export const categoriasArticuloOptions = [
    { value: 1, label: 'Parche' },
    { value: 2, label: 'Indumentaria' },
    { value: 3, label: 'Accesorio' },
    { value: 4, label: 'Otro' },
] as const;

export const articuloSchema = z.object({
    Nombre: z.string().trim().min(1, 'El nombre es obligatorio.').max(200, 'Máximo 200 caracteres.'),
    SKU: z.string().trim().min(1, 'El SKU es obligatorio.').max(100, 'Máximo 100 caracteres.'),
    Descripcion: z.string().trim().min(1, 'La descripción es obligatoria.').max(500, 'Máximo 500 caracteres.'),
    Categoria: z.coerce.number().int().min(1).max(4),
    PrecioVenta: z.coerce.number().gt(0, 'El precio debe ser mayor a 0.'),
    CostoPromedio: z.coerce.number().gt(0, 'El costo debe ser mayor a 0.'),
    StockActual: z.coerce.number().int().min(0, 'El stock no puede ser negativo.'),
    CuentaContableIngresoId: z.string().uuid('Selecciona una cuenta contable válida.'),
});

export type ArticuloFormInput = z.input<typeof articuloSchema>;
export type ArticuloFormValues = z.output<typeof articuloSchema>;
