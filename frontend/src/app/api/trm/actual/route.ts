import { NextResponse } from 'next/server';

type TrmSfcResponse = {
    data?: {
        valor?: string;
        vigenciadesde?: string;
    }[];
};

export async function GET() {
    try {
        const url = 'https://www.datos.gov.co/resource/32sa-8pi3.json?$limit=1&$order=vigenciadesde%20DESC';
        const response = await fetch(url, {
            method: 'GET',
            cache: 'no-store',
            headers: {
                Accept: 'application/json',
            },
        });

        if (!response.ok) {
            return NextResponse.json(
                { message: 'No fue posible consultar la TRM oficial en este momento.' },
                { status: 502 },
            );
        }

        const json = (await response.json()) as TrmSfcResponse['data'];
        const item = Array.isArray(json) ? json[0] : undefined;

        const valor = item?.valor ? Number(item.valor) : NaN;
        const vigenciaDesdeRaw = item?.vigenciadesde;

        if (!Number.isFinite(valor) || !vigenciaDesdeRaw) {
            return NextResponse.json(
                { message: 'La respuesta de TRM oficial no tiene el formato esperado.' },
                { status: 502 },
            );
        }

        const fecha = new Date(vigenciaDesdeRaw);
        const fechaTasaCambio = Number.isNaN(fecha.getTime())
            ? null
            : fecha.toISOString().slice(0, 10);

        if (!fechaTasaCambio) {
            return NextResponse.json(
                { message: 'No fue posible interpretar la fecha de la TRM oficial.' },
                { status: 502 },
            );
        }

        return NextResponse.json({
            tasaCambioUsada: valor,
            fechaTasaCambio,
            fuenteTasaCambio: 1,
            fuenteNombre: 'TRM SFC',
        });
    } catch {
        return NextResponse.json(
            { message: 'No fue posible consultar la TRM oficial en este momento.' },
            { status: 502 },
        );
    }
}
