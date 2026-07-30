import type { DonacionItem } from '@/features/donaciones/hooks/useDonaciones';
import { downloadCsv } from '@/lib/csv';

/**
 * Exporta las donaciones que se estan viendo, con los filtros ya aplicados
 * (historia 2-4).
 *
 * Se exporta lo que devolvio el servidor y no una seleccion aparte, para que
 * el archivo coincida siempre con lo que el usuario tiene en pantalla.
 */
export function exportDonacionesCsv(donaciones: DonacionItem[], nombreArchivo: string): void {
    const encabezados = [
        'Fecha',
        'Donante',
        'MontoCOP',
        'Banco',
        'CentroCosto',
        'FormaDonacion',
        'MedioPago',
        'CertificadoEmitido',
        'CodigoVerificacion',
    ];

    const filas = donaciones.map((donacion) => [
        donacion.fecha,
        donacion.nombreDonante,
        donacion.montoCOP,
        donacion.banco,
        donacion.centroCosto,
        donacion.formaDonacion,
        donacion.medioPagoODescripcion,
        donacion.certificadoEmitido ? 'Si' : 'No',
        donacion.codigoVerificacion,
    ]);

    downloadCsv(encabezados, filas, nombreArchivo);
}
