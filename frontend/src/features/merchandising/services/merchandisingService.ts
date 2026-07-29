import apiClient from '@/lib/apiClient';

export type ProductoMerchandising = {
    id: string;
    nombre: string;
    codigoSKU: string;
    precioVenta: number;
    cantidadEnStock: number;
    cantidadMinima: number;
    cuentaContableIngresoCodigo: string;
    cuentaContableIngresoDescripcion: string;
    imageUrl: string | null;
};

export type CrearProductoPayload = {
    nombre: string;
    codigoSKU: string;
    precioVenta: number;
    cantidadEnStock: number;
    cantidadMinima: number;
    cuentaContableIngresoId: string;
};

/**
 * Espeja RegistrarEntradaInventarioCommand. ProductoId viaja por la ruta.
 * 'concepto' no existia en el backend: lo que el formulario captura son las
 * observaciones del movimiento. Y 'fecha' era obligatoria pero nunca se
 * enviaba, asi que toda entrada quedaba fechada en 0001-01-01.
 */
export type RegistrarEntradaPayload = {
    cantidad: number;
    fecha: string;
    observaciones?: string | null;
};

export type RegistrarVentaPayload = {
    cantidad: number;
    bancoId: string;
    concepto: string;
    observaciones?: string | null;
    medioPago: number;
};

export type MovimientoProducto = {
    id: string;
    productoId: string;
    tipoMovimiento: number;
    tipoMovimientoNombre: string;
    cantidad: number;
    fecha: string;
    concepto: string;
    observaciones: string | null;
};

type IdResponseDto = {
    id?: string;
};

type ProductoDto = {
    id?: string;
    nombre?: string;
    codigoSKU?: string;
    sku?: string;
    SKU?: string;
    precioVenta?: number;
    precioVentaCOP?: number;
    cantidadEnStock?: number;
    cantidadMinima?: number;
    cantidadStock?: number;
    cuentaContableIngresoCodigo?: string;
    cuentaContableIngresoDescripcion?: string;
    imageUrl?: string | null;
};

type MovimientoProductoDto = {
    id?: string;
    productoId?: string;
    tipoMovimiento?: number;
    tipoMovimientoNombre?: string;
    cantidad?: number;
    fecha?: string;
    concepto?: string;
    observaciones?: string | null;
};

function toId(response: IdResponseDto | undefined): string {
    return String(response?.id ?? '');
}

export async function getProductos(): Promise<ProductoMerchandising[]> {
    const response = await apiClient.get<ProductoDto[]>('/api/merchandising/productos');

    return (response.data ?? []).map((item) => ({
        id: String(item?.id ?? ''),
        nombre: String(item?.nombre ?? ''),
        codigoSKU: String(item?.codigoSKU ?? item?.sku ?? ''),
        precioVenta: Number(item?.precioVenta ?? item?.precioVentaCOP ?? 0),
        cantidadEnStock: Number(item?.cantidadEnStock ?? item?.cantidadStock ?? 0),
        cantidadMinima: Number(item?.cantidadMinima ?? 0),
        cuentaContableIngresoCodigo: String(item?.cuentaContableIngresoCodigo ?? ''),
        cuentaContableIngresoDescripcion: String(item?.cuentaContableIngresoDescripcion ?? ''),
        imageUrl: item?.imageUrl ?? null,
    }));
}

export async function crearProducto(payload: CrearProductoPayload): Promise<{ id: string }> {
    const response = await apiClient.post<IdResponseDto>('/api/merchandising/productos', payload);
    return { id: toId(response.data) };
}

export async function registrarEntrada(productoId: string, payload: RegistrarEntradaPayload): Promise<{ id: string }> {
    const response = await apiClient.post<IdResponseDto>(`/api/merchandising/productos/${productoId}/entradas`, payload);
    return { id: toId(response.data) };
}

export async function registrarVenta(productoId: string, payload: RegistrarVentaPayload): Promise<{ id: string }> {
    const response = await apiClient.post<IdResponseDto>(`/api/merchandising/productos/${productoId}/ventas`, payload);
    return { id: toId(response.data) };
}

export async function getMovimientosProducto(productoId: string): Promise<MovimientoProducto[]> {
    const response = await apiClient.get<MovimientoProductoDto[]>(`/api/merchandising/productos/${productoId}/movimientos`);

    return (response.data ?? []).map((item) => ({
        id: String(item?.id ?? ''),
        productoId: String(item?.productoId ?? ''),
        tipoMovimiento: Number(item?.tipoMovimiento ?? 0),
        tipoMovimientoNombre: String(item?.tipoMovimientoNombre ?? ''),
        cantidad: Number(item?.cantidad ?? 0),
        fecha: String(item?.fecha ?? ''),
        concepto: String(item?.concepto ?? ''),
        observaciones: item?.observaciones ?? null,
    }));
}

export async function subirImagenProducto(productoId: string, file: File): Promise<{ imageUrl: string }> {
    const formData = new FormData();
    formData.append('imagen', file);

    const response = await apiClient.post<{ imageUrl?: string; ImageUrl?: string }>(
        `/api/merchandising/productos/${productoId}/imagen`,
        formData,
        { headers: { 'Content-Type': 'multipart/form-data' } },
    );

    return { imageUrl: String(response.data?.imageUrl ?? '') };
}
