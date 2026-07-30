import apiClient, { type RespuestaApi } from '@/lib/apiClient';

/**
 * Libros contables. Espeja ReportesController.
 *
 * El backend llevaba tiempo construido y verificado, pero no habia pantalla:
 * los libros solo se podian obtener llamando al API a mano, lo que deja la
 * historia 1-4 sin cumplir por mas que el calculo estuviera bien.
 */
export type MovimientoLibroDiario = {
    fecha: string;
    numeroConsecutivo: string;
    tipoComprobante: string;
    descripcionComprobante: string;
    codigoCuenta: string;
    descripcionCuenta: string;
    centroCosto: string;
    referencia: string;
    debe: number;
    haber: number;
};

export type LibroDiario = {
    desde: string;
    hasta: string;
    totalDebe: number;
    totalHaber: number;
    estaCuadrado: boolean;
    movimientos: MovimientoLibroDiario[];
};

export type MovimientoLibroMayor = {
    fecha: string;
    numeroConsecutivo: string;
    descripcionComprobante: string;
    centroCosto: string;
    referencia: string;
    debe: number;
    haber: number;
    saldoAcumulado: number;
};

export type LibroMayor = {
    codigoCuenta: string;
    descripcionCuenta: string;
    naturaleza: string;
    saldoAnterior: number;
    totalDebe: number;
    totalHaber: number;
    saldoFinal: number;
    movimientos: MovimientoLibroMayor[];
};

export type SaldoCuentaBalance = {
    cuentaContableId: string;
    codigoCuenta: string;
    descripcionCuenta: string;
    naturaleza: string;
    saldoAnterior: number;
    debe: number;
    haber: number;
    saldoFinal: number;
};

export type BalancePrueba = {
    anio: number;
    mes: number;
    totalDebe: number;
    totalHaber: number;
    estaCuadrado: boolean;
    cuentas: SaldoCuentaBalance[];
};

function num(valor: unknown): number {
    return Number(valor ?? 0);
}

function texto(valor: unknown): string {
    return String(valor ?? '');
}

export async function getLibroDiario(
    desde: string,
    hasta: string,
    centroCostoId?: string,
): Promise<LibroDiario> {
    const response = await apiClient.get<RespuestaApi>('/api/reportes/libro-diario', {
        params: { desde, hasta, centroCostoId: centroCostoId || undefined },
    });

    const d = response.data ?? {};
    const movimientos = Array.isArray(d.movimientos) ? (d.movimientos as RespuestaApi[]) : [];

    return {
        desde: texto(d.desde),
        hasta: texto(d.hasta),
        totalDebe: num(d.totalDebe),
        totalHaber: num(d.totalHaber),
        estaCuadrado: Boolean(d.estaCuadrado),
        movimientos: movimientos.map((m) => ({
            fecha: texto(m?.fecha),
            numeroConsecutivo: texto(m?.numeroConsecutivo),
            tipoComprobante: texto(m?.tipoComprobante),
            descripcionComprobante: texto(m?.descripcionComprobante),
            codigoCuenta: texto(m?.codigoCuenta),
            descripcionCuenta: texto(m?.descripcionCuenta),
            centroCosto: texto(m?.centroCosto),
            referencia: texto(m?.referencia),
            debe: num(m?.debe),
            haber: num(m?.haber),
        })),
    };
}

export async function getLibroMayor(
    cuentaContableId: string,
    desde: string,
    hasta: string,
    centroCostoId?: string,
): Promise<LibroMayor> {
    const response = await apiClient.get<RespuestaApi>('/api/reportes/libro-mayor', {
        params: { cuentaContableId, desde, hasta, centroCostoId: centroCostoId || undefined },
    });

    const d = response.data ?? {};
    const movimientos = Array.isArray(d.movimientos) ? (d.movimientos as RespuestaApi[]) : [];

    return {
        codigoCuenta: texto(d.codigoCuenta),
        descripcionCuenta: texto(d.descripcionCuenta),
        naturaleza: texto(d.naturaleza),
        saldoAnterior: num(d.saldoAnterior),
        totalDebe: num(d.totalDebe),
        totalHaber: num(d.totalHaber),
        saldoFinal: num(d.saldoFinal),
        movimientos: movimientos.map((m) => ({
            fecha: texto(m?.fecha),
            numeroConsecutivo: texto(m?.numeroConsecutivo),
            descripcionComprobante: texto(m?.descripcionComprobante),
            centroCosto: texto(m?.centroCosto),
            referencia: texto(m?.referencia),
            debe: num(m?.debe),
            haber: num(m?.haber),
            saldoAcumulado: num(m?.saldoAcumulado),
        })),
    };
}

export async function getBalancePrueba(
    anio: number,
    mes: number,
    centroCostoId?: string,
): Promise<BalancePrueba> {
    const response = await apiClient.get<RespuestaApi>('/api/reportes/balance-prueba', {
        params: { anio, mes, centroCostoId: centroCostoId || undefined },
    });

    const d = response.data ?? {};
    const cuentas = Array.isArray(d.cuentas) ? (d.cuentas as RespuestaApi[]) : [];

    return {
        anio: num(d.anio),
        mes: num(d.mes),
        totalDebe: num(d.totalDebe),
        totalHaber: num(d.totalHaber),
        estaCuadrado: Boolean(d.estaCuadrado),
        cuentas: cuentas.map((c) => ({
            cuentaContableId: texto(c?.cuentaContableId),
            codigoCuenta: texto(c?.codigoCuenta),
            descripcionCuenta: texto(c?.descripcionCuenta),
            naturaleza: texto(c?.naturaleza),
            saldoAnterior: num(c?.saldoAnterior),
            debe: num(c?.debe),
            haber: num(c?.haber),
            saldoFinal: num(c?.saldoFinal),
        })),
    };
}
