import type { ReporteExogenaItem } from '@/features/tributario/hooks/useReporteExogena';
import { downloadCsv } from '@/lib/csv';

export function exportExogenaCsv(rows: ReporteExogenaItem[], fileName: string) {
    const headers = [
        'TerceroId',
        'NombreTercero',
        'CuentaContableCodigo',
        'CuentaContableNombre',
        'TotalDebito',
        'TotalCredito',
        'SaldoMovimiento',
    ];

    const dataRows = rows.map((row) => [
        row.terceroId,
        row.nombreTercero,
        row.cuentaContableCodigo,
        row.cuentaContableNombre,
        row.totalDebito,
        row.totalCredito,
        row.saldoMovimiento,
    ]);

    downloadCsv(headers, dataRows, fileName);
}
