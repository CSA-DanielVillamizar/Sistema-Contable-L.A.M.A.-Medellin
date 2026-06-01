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

export type RegistrarEntradaPayload = {
    cantidad: number;
    concepto: string;
    observaciones?: string | null;
};

export type RegistrarVentaPayload = {
    cantidad: number;
    cajaId: string;
    concepto: string;
    observaciones?: string | null;
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
    Id?: string;
};

type ProductoDto = {
    id?: string;
    Id?: string;
    nombre?: string;
    Nombre?: string;
    codigoSKU?: string;
    CodigoSKU?: string;
    sku?: string;
    SKU?: string;
    precioVenta?: number;
    PrecioVenta?: number;
    precioVentaCOP?: number;
    PrecioVentaCOP?: number;
    cantidadEnStock?: number;
    CantidadEnStock?: number;
    cantidadMinima?: number;
    CantidadMinima?: number;
    cantidadStock?: number;
    CantidadStock?: number;
    cuentaContableIngresoCodigo?: string;
    CuentaContableIngresoCodigo?: string;
    cuentaContableIngresoDescripcion?: string;
    CuentaContableIngresoDescripcion?: string;
    imageUrl?: string | null;
    ImageUrl?: string | null;
};

type MovimientoProductoDto = {
    id?: string;
    Id?: string;
    productoId?: string;
    ProductoId?: string;
    tipoMovimiento?: number;
    TipoMovimiento?: number;
    tipoMovimientoNombre?: string;
    TipoMovimientoNombre?: string;
    cantidad?: number;
    Cantidad?: number;
    fecha?: string;
    Fecha?: string;
    concepto?: string;
    Concepto?: string;
    observaciones?: string | null;
    Observaciones?: string | null;
};

function toId(response: IdResponseDto | undefined): string {
    return String(response?.id ?? response?.Id ?? '');
}

export async function getProductos(): Promise<ProductoMerchandising[]> {
    const response = await apiClient.get<ProductoDto[]>('/api/merchandising/productos');

    return (response.data ?? []).map((item) => ({
        id: String(item?.id ?? item?.Id ?? ''),
        nombre: String(item?.nombre ?? item?.Nombre ?? ''),
        codigoSKU: String(item?.codigoSKU ?? item?.CodigoSKU ?? item?.sku ?? item?.SKU ?? ''),
        precioVenta: Number(item?.precioVenta ?? item?.PrecioVenta ?? item?.precioVentaCOP ?? item?.PrecioVentaCOP ?? 0),
        cantidadEnStock: Number(item?.cantidadEnStock ?? item?.CantidadEnStock ?? item?.cantidadStock ?? item?.CantidadStock ?? 0),
        cantidadMinima: Number(item?.cantidadMinima ?? item?.CantidadMinima ?? 0),
        cuentaContableIngresoCodigo: String(item?.cuentaContableIngresoCodigo ?? item?.CuentaContableIngresoCodigo ?? ''),
        cuentaContableIngresoDescripcion: String(item?.cuentaContableIngresoDescripcion ?? item?.CuentaContableIngresoDescripcion ?? ''),
        imageUrl: item?.imageUrl ?? item?.ImageUrl ?? null,
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
        id: String(item?.id ?? item?.Id ?? ''),
        productoId: String(item?.productoId ?? item?.ProductoId ?? ''),
        tipoMovimiento: Number(item?.tipoMovimiento ?? item?.TipoMovimiento ?? 0),
        tipoMovimientoNombre: String(item?.tipoMovimientoNombre ?? item?.TipoMovimientoNombre ?? ''),
        cantidad: Number(item?.cantidad ?? item?.Cantidad ?? 0),
        fecha: String(item?.fecha ?? item?.Fecha ?? ''),
        concepto: String(item?.concepto ?? item?.Concepto ?? ''),
        observaciones: item?.observaciones ?? item?.Observaciones ?? null,
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

    return { imageUrl: String(response.data?.imageUrl ?? response.data?.ImageUrl ?? '') };
}
