import apiClient from '@/lib/apiClient';

export type ProductoMerchandising = {
    id: string;
    nombre: string;
    sku: string;
    precioVentaCOP: number;
    cantidadStock: number;
    cuentaContableIngresoId: string;
    cuentaContableIngresoCodigo: string;
    cuentaContableIngresoDescripcion: string;
};

export type CrearProductoPayload = {
    nombre: string;
    sku: string;
    precioVentaCOP: number;
    cuentaContableIngresoId: string;
};

export type RegistrarEntradaPayload = {
    cantidad: number;
    fecha: string;
    observaciones?: string | null;
};

export type RegistrarVentaPayload = {
    cantidad: number;
    cajaId: string;
    centroCostoId: string;
    fecha: string;
    observaciones?: string | null;
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
    sku?: string;
    SKU?: string;
    precioVentaCOP?: number;
    PrecioVentaCOP?: number;
    cantidadStock?: number;
    CantidadStock?: number;
    cuentaContableIngresoId?: string;
    CuentaContableIngresoId?: string;
    cuentaContableIngresoCodigo?: string;
    CuentaContableIngresoCodigo?: string;
    cuentaContableIngresoDescripcion?: string;
    CuentaContableIngresoDescripcion?: string;
};

function toId(response: IdResponseDto | undefined): string {
    return String(response?.id ?? response?.Id ?? '');
}

export async function getProductos(): Promise<ProductoMerchandising[]> {
    const response = await apiClient.get<ProductoDto[]>('/api/merchandising/productos');

    return (response.data ?? []).map((item) => ({
        id: String(item?.id ?? item?.Id ?? ''),
        nombre: String(item?.nombre ?? item?.Nombre ?? ''),
        sku: String(item?.sku ?? item?.SKU ?? ''),
        precioVentaCOP: Number(item?.precioVentaCOP ?? item?.PrecioVentaCOP ?? 0),
        cantidadStock: Number(item?.cantidadStock ?? item?.CantidadStock ?? 0),
        cuentaContableIngresoId: String(item?.cuentaContableIngresoId ?? item?.CuentaContableIngresoId ?? ''),
        cuentaContableIngresoCodigo: String(item?.cuentaContableIngresoCodigo ?? item?.CuentaContableIngresoCodigo ?? ''),
        cuentaContableIngresoDescripcion: String(item?.cuentaContableIngresoDescripcion ?? item?.CuentaContableIngresoDescripcion ?? ''),
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
