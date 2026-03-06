import type { ReporteExogenaItem } from '@/features/tributario/hooks/useReporteExogena';

function escapeCsvValue(value: string | number): string {
    const normalized = String(value ?? '');
    const escaped = normalized.replace(/"/g, '""');
    return `"${escaped}"`;
}

export function downloadCsv(headers: string[], rows: Array<Array<string | number>>, fileName: string) {
    const lines = rows.map((row) => row.map((value) => escapeCsvValue(value)).join(','));
    const csv = [headers.join(','), ...lines].join('\n');
    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);

    const anchor = document.createElement('a');
    anchor.href = url;
    anchor.download = fileName.endsWith('.csv') ? fileName : `${fileName}.csv`;
    document.body.appendChild(anchor);
    anchor.click();
    document.body.removeChild(anchor);

    URL.revokeObjectURL(url);
}

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
