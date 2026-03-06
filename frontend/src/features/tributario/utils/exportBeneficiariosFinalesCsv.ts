import type { BeneficiarioFinalItem } from '@/features/tributario/hooks/useReporteBeneficiariosFinales';
import { downloadCsv } from '@/features/tributario/utils/exportExogenaCsv';

export function exportBeneficiariosFinalesCsv(rows: BeneficiarioFinalItem[], fileName: string) {
    const headers = [
        'TipoDocumento',
        'NumeroDocumento',
        'Nombres',
        'Apellidos',
        'PaisResidencia',
        'CargoORol',
    ];

    const dataRows = rows.map((row) => [
        row.tipoDocumento,
        row.numeroDocumento,
        row.nombres,
        row.apellidos,
        row.paisResidencia,
        row.cargoORol,
    ]);

    downloadCsv(headers, dataRows, fileName);
}
