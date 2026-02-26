using FluentValidation;
using LAMAMedellin.Application.Common.Exceptions;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using MediatR;

namespace LAMAMedellin.Application.Features.Merchandising.Commands.CreateArticulo;

public sealed record CreateArticuloCommand(
    string Nombre,
    string SKU,
    string Descripcion,
    CategoriaArticulo Categoria,
    decimal PrecioVenta,
    decimal CostoPromedio,
    int StockActual,
    Guid CuentaContableIngresoId) : IRequest<Guid>;

public sealed class CreateArticuloCommandValidator : AbstractValidator<CreateArticuloCommand>
{
    public CreateArticuloCommandValidator()
    {
        RuleFor(x => x.Nombre).NotEmpty().MaximumLength(200);
        RuleFor(x => x.SKU).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Descripcion).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Categoria).IsInEnum();
        RuleFor(x => x.PrecioVenta).GreaterThan(0);
        RuleFor(x => x.CostoPromedio).GreaterThan(0);
        RuleFor(x => x.StockActual).GreaterThanOrEqualTo(0);
        RuleFor(x => x.CuentaContableIngresoId).NotEmpty();
    }
}

public sealed class CreateArticuloCommandHandler(IArticuloRepository articuloRepository)
    : IRequestHandler<CreateArticuloCommand, Guid>
{
    public async Task<Guid> Handle(CreateArticuloCommand request, CancellationToken cancellationToken)
    {
        var skuNormalizado = request.SKU.Trim();

        var skuExiste = await articuloRepository.ExistsBySkuAsync(skuNormalizado, cancellationToken);
        if (skuExiste)
        {
            throw new ExcepcionNegocio("Ya existe un artículo con el mismo SKU.");
        }

        var articulo = new Articulo(
            request.Nombre,
            skuNormalizado,
            request.Descripcion,
            request.Categoria,
            request.PrecioVenta,
            request.CostoPromedio,
            request.StockActual,
            request.CuentaContableIngresoId);

        await articuloRepository.AddAsync(articulo, cancellationToken);
        await articuloRepository.SaveChangesAsync(cancellationToken);

        return articulo.Id;
    }
}
