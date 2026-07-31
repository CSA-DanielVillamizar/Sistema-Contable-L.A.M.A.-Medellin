using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Configuracion.MapeoContable.Queries.GetMapeosContables;

public sealed class GetMapeosContablesQueryHandler(
    IMapeoContableRepository mapeoRepository,
    ICuentaContableRepository cuentaContableRepository)
    : IRequestHandler<GetMapeosContablesQuery, IReadOnlyList<MapeoContableDto>>
{
    /// <summary>
    /// Nombres legibles de cada operacion. Van aqui y no en la pantalla para
    /// que backend y frontend no puedan discrepar sobre que significa cada una.
    /// </summary>
    private static readonly Dictionary<TipoOperacionContable, string> Nombres = new()
    {
        [TipoOperacionContable.IngresoCuotas] = "Ingresos por cuotas",
        [TipoOperacionContable.IngresoDonaciones] = "Ingresos por donaciones",
        [TipoOperacionContable.IngresoMerchandising] = "Ingresos por merchandising",
        [TipoOperacionContable.IngresoDiferenciaCambio] = "Ingresos por diferencia en cambio",
        [TipoOperacionContable.GastoDiferenciaCambio] = "Gastos por diferencia en cambio",
        [TipoOperacionContable.GastoAdministrativo] = "Gastos administrativos",
        [TipoOperacionContable.GastoOperativo] = "Gastos operativos",
        [TipoOperacionContable.GastoEventos] = "Gastos de eventos",
        [TipoOperacionContable.GastoProyectos] = "Gastos de proyectos",
        [TipoOperacionContable.GastoBancario] = "Gastos bancarios",
        [TipoOperacionContable.CompraInventario] = "Compra de inventario",
    };

    public async Task<IReadOnlyList<MapeoContableDto>> Handle(
        GetMapeosContablesQuery request,
        CancellationToken cancellationToken)
    {
        var mapeos = await mapeoRepository.GetAllAsync(cancellationToken);
        var cuentas = await cuentaContableRepository.GetAllAsync(cancellationToken);

        // Se recorre el enum y no la tabla: asi una operacion sin configurar
        // aparece igual, marcada como pendiente. Listar solo lo guardado
        // escondia justamente lo que falta por decidir.
        return Enum.GetValues<TipoOperacionContable>()
            .Select(operacion =>
            {
                var mapeo = mapeos.FirstOrDefault(m => m.TipoOperacion == operacion);
                var cuenta = mapeo is null
                    ? null
                    : cuentas.FirstOrDefault(c => c.Id == mapeo.CuentaContableId);

                return new MapeoContableDto(
                    (int)operacion,
                    Nombres.TryGetValue(operacion, out var nombre) ? nombre : operacion.ToString(),
                    mapeo?.CuentaContableId,
                    cuenta?.Codigo,
                    cuenta?.Descripcion);
            })
            .ToList();
    }
}
