using FluentAssertions;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Xunit;

namespace LAMAMedellin.Application.Tests.Domain.Entities;

public sealed class CuentaContableTests
{
    // ─── Constructor: validaciones de código ────────────────────────────────

    [Theory]
    [InlineData("3")]        // Clase
    [InlineData("41")]       // Grupo
    [InlineData("4105")]     // Cuenta
    [InlineData("410505")]   // Subcuenta
    [InlineData("31050501")] // Auxiliar (8 dígitos)
    public void Constructor_CuandoCodigoEsValido_DebeCrearEntidad(string codigo)
    {
        var sut = new CuentaContable(codigo, "Descripción de prueba", NaturalezaCuenta.Credito, false, false);

        sut.Codigo.Should().Be(codigo);
        sut.Descripcion.Should().Be("Descripción de prueba");
        sut.Naturaleza.Should().Be(NaturalezaCuenta.Credito);
        sut.PermiteMovimiento.Should().BeFalse();
        sut.ExigeTercero.Should().BeFalse();
        sut.CuentaPadreId.Should().BeNull();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Constructor_CuandoCodigoEsNuloOVacio_DebeLanzarArgumentException(string? codigo)
    {
        var act = () => new CuentaContable(codigo!, "Descripción", NaturalezaCuenta.Credito, false, false);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*código*");
    }

    [Theory]
    [InlineData("3A")]    // Contiene letra
    [InlineData("1 2")]   // Contiene espacio
    [InlineData("ABC")]   // Solo letras
    public void Constructor_CuandoCodigoContieneCaracteresNoNumericos_DebeLanzarArgumentException(string codigo)
    {
        var act = () => new CuentaContable(codigo, "Descripción", NaturalezaCuenta.Debito, false, false);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*solo dígitos*");
    }

    [Theory]
    [InlineData("123")]   // 3 dígitos (inválido)
    [InlineData("12345")] // 5 dígitos (inválido)
    [InlineData("1234567")] // 7 dígitos (inválido)
    public void Constructor_CuandoLongitudCodigoEsInvalida_DebeLanzarArgumentException(string codigo)
    {
        var act = () => new CuentaContable(codigo, "Descripción", NaturalezaCuenta.Debito, false, false);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*longitud*");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_CuandoDescripcionEsVacia_DebeLanzarArgumentException(string descripcion)
    {
        var act = () => new CuentaContable("3105", descripcion, NaturalezaCuenta.Credito, false, false);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*descripción*");
    }

    // ─── Nivel jerárquico ───────────────────────────────────────────────────

    [Theory]
    [InlineData("3",        NivelCuenta.Clase)]
    [InlineData("41",       NivelCuenta.Grupo)]
    [InlineData("4105",     NivelCuenta.Cuenta)]
    [InlineData("410505",   NivelCuenta.Subcuenta)]
    [InlineData("31050501", NivelCuenta.Auxiliar)]
    public void Nivel_DebeResolverseCorrectamenteDesdeLongitudDeCodigo(string codigo, NivelCuenta nivelEsperado)
    {
        var cuenta = new CuentaContable(codigo, "Descripción", NaturalezaCuenta.Credito, false, false);

        cuenta.Nivel.Should().Be(nivelEsperado);
    }

    // ─── ResolverCodigoPadre ────────────────────────────────────────────────

    [Theory]
    [InlineData("3",        null)]     // Clase no tiene padre
    [InlineData("41",       "4")]      // Grupo → Clase
    [InlineData("4105",     "41")]     // Cuenta → Grupo
    [InlineData("410505",   "4105")]   // Subcuenta → Cuenta
    [InlineData("31050501", "310505")] // Auxiliar → Subcuenta
    public void ResolverCodigoPadre_DebeRetornarCodigoCorrectoSegunNivel(string codigo, string? codigoPadreEsperado)
    {
        var resultado = CuentaContable.ResolverCodigoPadre(codigo);

        resultado.Should().Be(codigoPadreEsperado);
    }

    // ─── EstablecerPadre ────────────────────────────────────────────────────

    [Fact]
    public void EstablecerPadre_CuandoIdEsValido_DebeAsignarCuentaPadreId()
    {
        var cuenta = new CuentaContable("410505", "Cuotas de Afiliación", NaturalezaCuenta.Credito, true, true);
        var idPadre = Guid.NewGuid();

        cuenta.EstablecerPadre(idPadre);

        cuenta.CuentaPadreId.Should().Be(idPadre);
    }

    [Fact]
    public void EstablecerPadre_CuandoIdEsGuidVacio_DebeLanzarArgumentException()
    {
        var cuenta = new CuentaContable("410505", "Cuotas de Afiliación", NaturalezaCuenta.Credito, true, true);

        var act = () => cuenta.EstablecerPadre(Guid.Empty);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*CuentaPadreId*");
    }

    // ─── Datos del problema: cuentas del PUC ESAL ──────────────────────────

    [Fact]
    public void Constructor_CuentaExcedente_DebeCrearseConNaturalezaCredito()
    {
        var cuenta = new CuentaContable("3205", "Excedente del Ejercicio", NaturalezaCuenta.Credito, true, false);

        cuenta.Nivel.Should().Be(NivelCuenta.Cuenta);
        cuenta.Naturaleza.Should().Be(NaturalezaCuenta.Credito);
        cuenta.PermiteMovimiento.Should().BeTrue();
        cuenta.ExigeTercero.Should().BeFalse();
    }

    [Fact]
    public void Constructor_CuentaDeficit_DebeCrearseConNaturalezaDebito()
    {
        var cuenta = new CuentaContable("3210", "Déficit del Ejercicio", NaturalezaCuenta.Debito, true, false);

        cuenta.Nivel.Should().Be(NivelCuenta.Cuenta);
        cuenta.Naturaleza.Should().Be(NaturalezaCuenta.Debito);
        cuenta.PermiteMovimiento.Should().BeTrue();
    }

    [Fact]
    public void Constructor_SubcuentaDonacionesCondicionadas_DebeExigirTercero()
    {
        var cuenta = new CuentaContable("411510", "Donaciones Condicionadas (Proyectos)", NaturalezaCuenta.Credito, true, true);

        cuenta.Nivel.Should().Be(NivelCuenta.Subcuenta);
        cuenta.ExigeTercero.Should().BeTrue();
        cuenta.PermiteMovimiento.Should().BeTrue();
    }
}
