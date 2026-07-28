'use client';

import ModalEntradaInventario from '@/features/merchandising/components/ModalEntradaInventario';
import ModalNuevoProducto from '@/features/merchandising/components/ModalNuevoProducto';
import ModalVenta from '@/features/merchandising/components/ModalVenta';
import { useGetMovimientosProducto } from '@/features/merchandising/hooks/useGetMovimientosProducto';
import { useGetProductos } from '@/features/merchandising/hooks/useGetProductos';
import { useSubirImagenProducto } from '@/features/merchandising/hooks/useSubirImagenProducto';
import { useEffect, useMemo, useState } from 'react';

type DetailTab = 'info' | 'kardex';

function formatCOP(value: number): string {
    return new Intl.NumberFormat('es-CO', {
        style: 'currency',
        currency: 'COP',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
    }).format(value);
}

function formatFecha(value: string): string {
    if (!value) {
        return '-';
    }

    const parsed = new Date(value);
    if (Number.isNaN(parsed.getTime())) {
        return value;
    }

    return new Intl.DateTimeFormat('es-CO', {
        dateStyle: 'medium',
        timeStyle: 'short',
    }).format(parsed);
}

export default function MerchandisingPage() {
    const productosQuery = useGetProductos();
    const subirImagenMutation = useSubirImagenProducto();

    const [selectedProductId, setSelectedProductId] = useState<string | null>(null);
    const [tabActiva, setTabActiva] = useState<DetailTab>('info');
    const [nuevoProductoAbierto, setNuevoProductoAbierto] = useState(false);
    const [productoEntradaId, setProductoEntradaId] = useState<string | null>(null);
    const [productoVentaId, setProductoVentaId] = useState<string | null>(null);
    const [ajusteMenuAbierto, setAjusteMenuAbierto] = useState(false);
    const [archivoImagen, setArchivoImagen] = useState<File | null>(null);

    const productos = useMemo(() => productosQuery.data ?? [], [productosQuery.data]);

    const productoSeleccionado = useMemo(
        () => productos.find((producto) => producto.id === selectedProductId) ?? null,
        [productos, selectedProductId],
    );

    const movimientosQuery = useGetMovimientosProducto(productoSeleccionado?.id ?? null);

    useEffect(() => {
        if (!selectedProductId && productos.length > 0) {
            // eslint-disable-next-line react-hooks/set-state-in-effect -- Deuda conocida: reinicio de estado al cambiar props. La correccion idiomatica (remontar por key o derivar en render) cambia el comportamiento del componente y requiere verificarse en la interfaz.
            setSelectedProductId(productos[0].id);
            return;
        }

        if (selectedProductId && !productos.some((producto) => producto.id === selectedProductId)) {
            setSelectedProductId(productos[0]?.id ?? null);
        }
    }, [selectedProductId, productos]);

    const onSubirImagen = async () => {
        if (!productoSeleccionado || !archivoImagen) {
            return;
        }

        await subirImagenMutation.mutateAsync({
            productoId: productoSeleccionado.id,
            file: archivoImagen,
        });

        setArchivoImagen(null);
    };

    return (
        <main className="min-h-screen bg-slate-50 px-6 py-8">
            <div className="mx-auto w-full max-w-7xl space-y-5">
                <header className="rounded-2xl border border-slate-200 bg-white px-5 py-4 shadow-sm">
                    <h1 className="text-2xl font-semibold text-slate-900">Merchandising - Catalogo Operativo</h1>
                    <p className="mt-1 text-sm text-slate-600">
                        Gestion de productos, imagenes y trazabilidad de movimientos de inventario.
                    </p>
                </header>

                <section className="grid gap-4 xl:grid-cols-[1.2fr_1fr]">
                    <div className="rounded-xl border border-slate-200 bg-white shadow-sm">
                        <div className="flex items-center justify-between border-b border-slate-200 px-4 py-3">
                            <h2 className="text-sm font-semibold uppercase tracking-wide text-slate-700">Catalogo tabular</h2>
                            <button
                                type="button"
                                onClick={() => setNuevoProductoAbierto(true)}
                                className="rounded-lg bg-indigo-700 px-3 py-2 text-xs font-semibold text-white hover:bg-indigo-800"
                            >
                                Nuevo Producto
                            </button>
                        </div>

                        <div className="overflow-x-auto">
                            <table className="min-w-full divide-y divide-slate-200 text-sm">
                                <thead className="bg-slate-50">
                                    <tr>
                                        <th className="px-3 py-2 text-left text-xs font-semibold uppercase tracking-wide text-slate-600">Producto</th>
                                        <th className="px-3 py-2 text-left text-xs font-semibold uppercase tracking-wide text-slate-600">SKU</th>
                                        <th className="px-3 py-2 text-right text-xs font-semibold uppercase tracking-wide text-slate-600">Stock</th>
                                        <th className="px-3 py-2 text-right text-xs font-semibold uppercase tracking-wide text-slate-600">Precio</th>
                                    </tr>
                                </thead>
                                <tbody className="divide-y divide-slate-100 bg-white">
                                    {productos.map((producto) => {
                                        const isSelected = producto.id === productoSeleccionado?.id;
                                        const stockBajo = producto.cantidadEnStock <= producto.cantidadMinima;

                                        return (
                                            <tr
                                                key={producto.id}
                                                className={`cursor-pointer ${isSelected ? 'bg-indigo-50' : 'hover:bg-slate-50'}`}
                                                onClick={() => {
                                                    setSelectedProductId(producto.id);
                                                    setTabActiva('info');
                                                }}
                                            >
                                                <td className="px-3 py-3 text-slate-900">{producto.nombre}</td>
                                                <td className="px-3 py-3 text-slate-700">{producto.codigoSKU}</td>
                                                <td className="px-3 py-3 text-right">
                                                    <span className={`font-semibold ${stockBajo ? 'text-rose-700' : 'text-slate-900'}`}>
                                                        {producto.cantidadEnStock}
                                                    </span>
                                                </td>
                                                <td className="px-3 py-3 text-right font-semibold text-slate-900">{formatCOP(producto.precioVenta)}</td>
                                            </tr>
                                        );
                                    })}
                                </tbody>
                            </table>

                            {productosQuery.isLoading ? (
                                <p className="px-4 py-6 text-sm text-slate-600">Cargando catalogo...</p>
                            ) : null}

                            {productosQuery.isError ? (
                                <p className="px-4 py-6 text-sm text-rose-700">No fue posible cargar el catalogo de productos.</p>
                            ) : null}
                        </div>
                    </div>

                    <aside className="rounded-xl border border-slate-200 bg-white p-4 shadow-sm">
                        <div className="flex flex-wrap items-center justify-between gap-3 border-b border-slate-200 pb-3">
                            <h2 className="text-sm font-semibold uppercase tracking-wide text-slate-700">Detalle de producto</h2>
                            <div className="relative">
                                <button
                                    type="button"
                                    onClick={() => setAjusteMenuAbierto((prev) => !prev)}
                                    disabled={!productoSeleccionado}
                                    className="rounded-lg bg-emerald-700 px-3 py-2 text-xs font-semibold text-white hover:bg-emerald-800 disabled:cursor-not-allowed disabled:opacity-50"
                                >
                                    Ajustar Inventario
                                </button>

                                {ajusteMenuAbierto && productoSeleccionado ? (
                                    <div className="absolute right-0 z-20 mt-2 w-44 rounded-lg border border-slate-200 bg-white p-1 shadow-lg">
                                        <button
                                            type="button"
                                            className="w-full rounded-md px-3 py-2 text-left text-sm text-slate-700 hover:bg-slate-100"
                                            onClick={() => {
                                                setProductoEntradaId(productoSeleccionado.id);
                                                setAjusteMenuAbierto(false);
                                            }}
                                        >
                                            Registrar Entrada
                                        </button>
                                        <button
                                            type="button"
                                            className="w-full rounded-md px-3 py-2 text-left text-sm text-slate-700 hover:bg-slate-100"
                                            onClick={() => {
                                                setProductoVentaId(productoSeleccionado.id);
                                                setAjusteMenuAbierto(false);
                                            }}
                                        >
                                            Registrar Salida
                                        </button>
                                    </div>
                                ) : null}
                            </div>
                        </div>

                        {productoSeleccionado ? (
                            <>
                                <div className="mt-3 flex items-center gap-2 rounded-lg bg-slate-100 p-1">
                                    <button
                                        type="button"
                                        onClick={() => setTabActiva('info')}
                                        className={`rounded-md px-3 py-1.5 text-sm font-medium ${tabActiva === 'info' ? 'bg-white text-slate-900 shadow-sm' : 'text-slate-600 hover:text-slate-900'}`}
                                    >
                                        Informacion
                                    </button>
                                    <button
                                        type="button"
                                        onClick={() => setTabActiva('kardex')}
                                        className={`rounded-md px-3 py-1.5 text-sm font-medium ${tabActiva === 'kardex' ? 'bg-white text-slate-900 shadow-sm' : 'text-slate-600 hover:text-slate-900'}`}
                                    >
                                        Movimientos (Kardex)
                                    </button>
                                </div>

                                {tabActiva === 'info' ? (
                                    <div className="mt-4 space-y-4">
                                        <div className="overflow-hidden rounded-lg border border-slate-200 bg-slate-50">
                                            {productoSeleccionado.imageUrl ? (
                                                <img
                                                    src={productoSeleccionado.imageUrl}
                                                    alt={`Imagen de ${productoSeleccionado.nombre}`}
                                                    className="h-52 w-full object-cover"
                                                />
                                            ) : (
                                                <div className="flex h-52 items-center justify-center text-sm text-slate-500">
                                                    Producto sin imagen.
                                                </div>
                                            )}
                                        </div>

                                        <div className="rounded-lg border border-slate-200 p-3">
                                            <label className="mb-2 block text-xs font-semibold uppercase tracking-wide text-slate-600">
                                                Subir / Cambiar Foto
                                            </label>
                                            <input
                                                type="file"
                                                accept="image/png,image/jpeg,image/jpg,image/webp"
                                                onChange={(event) => setArchivoImagen(event.target.files?.[0] ?? null)}
                                                className="w-full text-sm text-slate-700"
                                            />
                                            <button
                                                type="button"
                                                onClick={() => void onSubirImagen()}
                                                disabled={!archivoImagen || subirImagenMutation.isPending}
                                                className="mt-3 rounded-lg bg-indigo-700 px-3 py-2 text-xs font-semibold text-white hover:bg-indigo-800 disabled:cursor-not-allowed disabled:opacity-60"
                                            >
                                                {subirImagenMutation.isPending ? 'Subiendo...' : 'Guardar imagen'}
                                            </button>
                                        </div>

                                        <div className="grid gap-3 rounded-lg bg-slate-50 p-3 text-sm text-slate-700">
                                            <p><span className="font-semibold text-slate-900">SKU:</span> {productoSeleccionado.codigoSKU}</p>
                                            <p><span className="font-semibold text-slate-900">Precio:</span> {formatCOP(productoSeleccionado.precioVenta)}</p>
                                            <p>
                                                <span className="font-semibold text-slate-900">Descripcion:</span>{' '}
                                                {productoSeleccionado.cuentaContableIngresoDescripcion || 'Descripcion comercial no parametrizada.'}
                                            </p>
                                            <p><span className="font-semibold text-slate-900">Cuenta ingreso:</span> {productoSeleccionado.cuentaContableIngresoCodigo || '-'}</p>
                                        </div>
                                    </div>
                                ) : (
                                    <div className="mt-4 overflow-hidden rounded-lg border border-slate-200">
                                        <div className="max-h-[360px] overflow-y-auto">
                                            <table className="min-w-full divide-y divide-slate-200 text-xs sm:text-sm">
                                                <thead className="bg-slate-50">
                                                    <tr>
                                                        <th className="px-3 py-2 text-left font-semibold uppercase tracking-wide text-slate-600">Fecha</th>
                                                        <th className="px-3 py-2 text-left font-semibold uppercase tracking-wide text-slate-600">Tipo</th>
                                                        <th className="px-3 py-2 text-right font-semibold uppercase tracking-wide text-slate-600">Cantidad</th>
                                                        <th className="px-3 py-2 text-left font-semibold uppercase tracking-wide text-slate-600">Concepto</th>
                                                    </tr>
                                                </thead>
                                                <tbody className="divide-y divide-slate-100 bg-white">
                                                    {(movimientosQuery.data ?? []).map((mov) => (
                                                        <tr key={mov.id}>
                                                            <td className="px-3 py-2 text-slate-700">{formatFecha(mov.fecha)}</td>
                                                            <td className="px-3 py-2">
                                                                <span className={`rounded-full px-2 py-0.5 text-xs font-semibold ${mov.tipoMovimiento === 1 ? 'bg-emerald-100 text-emerald-700' : 'bg-rose-100 text-rose-700'}`}>
                                                                    {mov.tipoMovimientoNombre}
                                                                </span>
                                                            </td>
                                                            <td className="px-3 py-2 text-right font-semibold text-slate-900">{mov.cantidad}</td>
                                                            <td className="px-3 py-2 text-slate-700">{mov.concepto || '-'}</td>
                                                        </tr>
                                                    ))}
                                                </tbody>
                                            </table>
                                        </div>

                                        {movimientosQuery.isLoading ? (
                                            <p className="px-3 py-4 text-sm text-slate-600">Cargando kardex...</p>
                                        ) : null}

                                        {movimientosQuery.isError ? (
                                            <p className="px-3 py-4 text-sm text-rose-700">No fue posible cargar el kardex del producto.</p>
                                        ) : null}

                                        {!movimientosQuery.isLoading && !movimientosQuery.isError && (movimientosQuery.data?.length ?? 0) === 0 ? (
                                            <p className="px-3 py-4 text-sm text-slate-600">Sin movimientos registrados para este producto.</p>
                                        ) : null}
                                    </div>
                                )}
                            </>
                        ) : (
                            <div className="mt-4 rounded-lg border border-dashed border-slate-300 bg-slate-50 p-6 text-center text-sm text-slate-500">
                                Selecciona un producto en el catalogo para ver su detalle.
                            </div>
                        )}
                    </aside>
                </section>
            </div>

            <ModalNuevoProducto abierto={nuevoProductoAbierto} onCerrar={() => setNuevoProductoAbierto(false)} />

            <ModalEntradaInventario
                productoId={productoEntradaId}
                onCerrar={() => setProductoEntradaId(null)}
            />

            <ModalVenta
                productoId={productoVentaId}
                onCerrar={() => setProductoVentaId(null)}
            />
        </main>
    );
}
