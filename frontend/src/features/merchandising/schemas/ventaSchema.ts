import { z } from 'zod';

export const metodosPagoVentaOptions = [
    { value: 1, label: 'Efectivo' },
    { value: 2, label: 'Transferencia' },
    { value: 3, label: 'Tarjeta' },
] as const;

const detalleVentaSchema = z.object({
    ArticuloId: z.string().uuid('El artículo es obligatorio.'),
    Cantidad: z.coerce.number().int().gt(0, 'La cantidad debe ser mayor a 0.'),
    PrecioUnitario: z.coerce.number().gt(0, 'El precio unitario debe ser mayor a 0.'),
});

export const ventaSchema = z.object({
    CompradorId: z.preprocess(
        (value) => (typeof value === 'string' && value.trim() === '' ? undefined : value),
        z.string().uuid('Comprador inválido.').optional(),
    ),
    MetodoPago: z.coerce.number().int().min(1).max(3),
    Detalles: z.array(detalleVentaSchema).min(1, 'Debe agregar al menos 1 artículo en la venta.'),
});

export type VentaFormInput = z.input<typeof ventaSchema>;
export type VentaFormValues = z.output<typeof ventaSchema>;
