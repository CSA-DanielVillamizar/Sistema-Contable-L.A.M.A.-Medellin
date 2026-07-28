import apiClient from '@/lib/apiClient';

export type EstadoResultadosDetalle = {
    tipoMovimiento: string;
    concepto: string;
    total: number;
};

export type EstadoResultados = {
    totalIngresos: number;
    totalEgresos: number;
    balanceNeto: number;
    totalesPorConcepto: EstadoResultadosDetalle[];
};

export type CarteraMoraDetalle = {
    nombreMiembro: string;
    concepto: string;
    fechaVencimiento: string;
    saldoPendiente: number;
};

export type CarteraMora = {
    totalEnMora: number;
    detalleMora: CarteraMoraDetalle[];
};

type EstadoResultadosApiDto = {
    totalIngresos?: number | string;
    totalEgresos?: number | string;
    balanceNeto?: number | string;
    totalesPorConcepto?: EstadoResultadosDetalleApiDto[];
};

type EstadoResultadosDetalleApiDto = {
    tipoMovimiento?: string;
    concepto?: string;
    total?: number | string;
};

type CarteraMoraApiDto = {
    totalEnMora?: number | string;
    detalleMora?: CarteraMoraDetalleApiDto[];
};

type CarteraMoraDetalleApiDto = {
    nombreMiembro?: string;
    concepto?: string;
    fechaVencimiento?: string;
    saldoPendiente?: number | string;
};

function toNumber(value: unknown): number {
    const parsed = typeof value === 'number' ? value : Number(value);
    return Number.isFinite(parsed) ? parsed : 0;
}

export async function getEstadoResultados(fechaInicio: string, fechaFin: string): Promise<EstadoResultados> {
    const response = await apiClient.get<EstadoResultadosApiDto>('/api/reportes/estado-resultados', {
        params: {
            fechaInicio,
            fechaFin,
        },
    });

    const item = response.data ?? {};
    const detalle = (item.totalesPorConcepto ?? []).map((d) => ({
        tipoMovimiento: String(d?.tipoMovimiento ?? ''),
        concepto: String(d?.concepto ?? ''),
        total: toNumber(d?.total ?? 0),
    }));

    return {
        totalIngresos: toNumber(item.totalIngresos ?? 0),
        totalEgresos: toNumber(item.totalEgresos ?? 0),
        balanceNeto: toNumber(item.balanceNeto ?? 0),
        totalesPorConcepto: detalle,
    };
}

export async function getCarteraMora(): Promise<CarteraMora> {
    const response = await apiClient.get<CarteraMoraApiDto>('/api/reportes/cartera-mora');

    const item = response.data ?? {};
    const detalle = (item.detalleMora ?? []).map((d) => ({
        nombreMiembro: String(d?.nombreMiembro ?? ''),
        concepto: String(d?.concepto ?? ''),
        fechaVencimiento: String(d?.fechaVencimiento ?? ''),
        saldoPendiente: toNumber(d?.saldoPendiente ?? 0),
    }));

    return {
        totalEnMora: toNumber(item.totalEnMora ?? 0),
        detalleMora: detalle,
    };
}
