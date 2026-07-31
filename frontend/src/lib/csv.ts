/**
 * Descarga de tablas en CSV.
 *
 * Vivia dentro del reporte de exogena y desde alli lo importaba el de
 * beneficiarios finales, que no tiene nada que ver con la DIAN. Al necesitarlo
 * tambien donaciones se traslada aqui: no es una regla tributaria, es una forma
 * de sacar datos.
 */
function escaparValor(valor: string | number): string {
    const texto = String(valor ?? '');
    return `"${texto.replace(/"/g, '""')}"`;
}

export function downloadCsv(
    headers: string[],
    rows: Array<Array<string | number>>,
    fileName: string,
): void {
    const lineas = rows.map((fila) => fila.map(escaparValor).join(','));
    const csv = [headers.join(','), ...lineas].join('\n');
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
