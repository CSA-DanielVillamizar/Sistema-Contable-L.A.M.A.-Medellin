'use client';

import type { CertificadoDonacionItem } from '@/features/donaciones/hooks/useDonaciones';

type VistaCertificadoProps = {
    data: CertificadoDonacionItem;
    onClose?: () => void;
};

function formatCOP(value: number): string {
    return new Intl.NumberFormat('es-CO', {
        style: 'currency',
        currency: 'COP',
        maximumFractionDigits: 0,
    }).format(value);
}

export default function VistaCertificado({ data, onClose }: VistaCertificadoProps) {
    const fechaActual = new Date().toLocaleDateString('es-CO');
    const formaDonacion = data.formaDonacion.toLowerCase().includes('especie') || data.formaDonacion === '2' ? 'Especie' : 'Dinero';

    return (
        <section className="mx-auto w-full max-w-3xl rounded-xl border border-slate-300 bg-white p-8 text-slate-900 shadow-lg print:shadow-none">
            <header className="border-b border-slate-300 pb-4 text-center">
                <h2 className="text-lg font-bold uppercase">FUNDACIÓN L.A.M.A. MEDELLÍN - NIT: 900.000.000-0</h2>
                <p className="mt-2 text-sm text-left">Medellín, {fechaActual}</p>
                <h3 className="mt-4 text-lg font-semibold uppercase">CERTIFICADO DE DONACIÓN - AÑO GRAVABLE 2026</h3>
            </header>

            <div className="mt-6 space-y-3 text-sm leading-6">
                <p>La suscrita Fundación L.A.M.A. Medellín CERTIFICA que ha recibido de:</p>
                <p>
                    <strong>{data.donante.nombreDonante}</strong>, con NIT/Cédula <strong>{data.donante.numeroDocumento}</strong>, la siguiente donación:
                </p>

                <p>
                    <strong>FORMA DE DONACIÓN:</strong> {formaDonacion}
                </p>
                <p>
                    <strong>MONTO / VALORACIÓN:</strong> {formatCOP(data.monto.valorCOP)} ({data.monto.enLetras})
                </p>
                <p>
                    <strong>MEDIO DE PAGO / DETALLE:</strong> {data.medioPagoODescripcion}
                </p>

                <div className="mt-6 space-y-2 rounded-lg border border-slate-200 bg-slate-50 p-4 text-xs leading-5 text-slate-700">
                    <p className="font-semibold">DECLARACIONES LEGALES:</p>
                    <p>- Los fondos o bienes recibidos se destinarán a actividades meritorias del objeto social de la Fundación (Art. 19 E.T.).</p>
                    <p>- Esta donación no constituye contraprestación ni retribución por servicios (Art. 125-1 E.T.).</p>
                    <p>- La Fundación certifica el cumplimiento de los requisitos de los Artículos 125-1 y 125-2 del Estatuto Tributario.</p>
                </div>
            </div>

            <footer className="mt-10 border-t border-slate-300 pt-6">
                <div className="mt-12 flex justify-center">
                    <div className="text-center text-sm">
                        <p className="font-semibold">DANIEL VILLAMIZAR ARAQUE</p>
                        <p>Representante Legal / Tesorero</p>
                    </div>
                </div>

                <div className="flex justify-end gap-2 print:hidden">
                    {onClose ? (
                        <button type="button" onClick={onClose} className="rounded-lg border border-slate-300 px-4 py-2 text-sm">
                            Cerrar
                        </button>
                    ) : null}
                    <button type="button" onClick={() => window.print()} className="rounded-lg bg-slate-800 px-4 py-2 text-sm text-white">
                        Imprimir
                    </button>
                </div>
            </footer>
        </section>
    );
}
