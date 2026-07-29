/**
 * Medios de pago. Espeja el enum MedioPago del dominio; los valores numericos
 * son los que persiste el backend y no deben reordenarse.
 *
 * La historia 0-6 del backlog exige capturar el medio en todo ingreso, egreso
 * y pago: sin el no se puede conciliar contra el extracto bancario.
 */
export const MEDIOS_PAGO = [
    { value: 1, label: 'Transferencia' },
    { value: 2, label: 'Consignación en efectivo' },
    { value: 3, label: 'Corresponsal bancario' },
    { value: 4, label: 'QR' },
] as const;

export const MEDIO_PAGO_POR_DEFECTO = 1;

export function nombreMedioPago(valor: number): string {
    return MEDIOS_PAGO.find((m) => m.value === valor)?.label ?? 'No especificado';
}
