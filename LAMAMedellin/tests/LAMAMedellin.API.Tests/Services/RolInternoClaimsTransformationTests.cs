using System.Security.Claims;
using FluentAssertions;
using LAMAMedellin.API.Services;
using LAMAMedellin.Application.Common.Interfaces.Repositories;
using LAMAMedellin.Domain.Entities;
using LAMAMedellin.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace LAMAMedellin.API.Tests.Services;

public sealed class RolInternoClaimsTransformationTests
{
    private const string ObjectId = "8f14e45f-ceea-467a-9f4b-1a2b3c4d5e6f";

    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock = new();

    [Fact]
    public async Task DebeProyectarElRolInternoComoClaimDeRol()
    {
        // El punto del cambio: lo que se asigna en la pantalla de seguridad
        // tiene que llegar a [Authorize(Roles = ...)].
        ConfigurarUsuario(RolSistema.Contador, esActivo: true);

        var resultado = await TransformarAsync(CrearPrincipal());

        resultado.IsInRole("Contador").Should().BeTrue();
    }

    [Fact]
    public async Task UsuarioDeshabilitado_DebePerderTodosLosRoles()
    {
        // Historia 0-2: deshabilitar el perfil interno bloquea el acceso aunque
        // Entra siga autenticando y el token traiga roles.
        ConfigurarUsuario(RolSistema.Admin, esActivo: false);

        var resultado = await TransformarAsync(CrearPrincipal(rolesDelToken: ["Admin", "Tesorero"]));

        resultado.IsInRole("Admin").Should().BeFalse();
        resultado.IsInRole("Tesorero").Should().BeFalse();
        resultado.HasClaim(RolInternoClaimsTransformation.ClaimUsuarioActivo, "false").Should().BeTrue();
    }

    [Fact]
    public async Task PorDefecto_DebeConservarLosRolesDelTokenDeEntra()
    {
        // Comportamiento aditivo: encender la exclusividad antes de poblar los
        // roles internos dejaria a todos sin permisos, asi que no es el default.
        ConfigurarUsuario(RolSistema.Logistica, esActivo: true);

        var resultado = await TransformarAsync(CrearPrincipal(rolesDelToken: ["Admin"]));

        resultado.IsInRole("Admin").Should().BeTrue();
        resultado.IsInRole("Logistica").Should().BeTrue();
    }

    [Fact]
    public async Task ConRolesExclusivos_DebeDescartarLosRolesDelToken()
    {
        ConfigurarUsuario(RolSistema.Logistica, esActivo: true);

        var resultado = await TransformarAsync(
            CrearPrincipal(rolesDelToken: ["Admin"]),
            rolesInternosExclusivos: true);

        resultado.IsInRole("Admin").Should().BeFalse();
        resultado.IsInRole("Logistica").Should().BeTrue();
    }

    [Fact]
    public async Task SinPerfilInterno_NoDebeConcederRolNiBloquear()
    {
        // Usuario recien autenticado: todavia no tiene perfil. No se le concede
        // rol, pero tampoco se le marca inactivo, porque el endpoint que le crea
        // el perfil tambien exige autenticacion.
        _usuarioRepositoryMock
            .Setup(r => r.GetByEntraObjectIdAsync(ObjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Usuario?)null);

        var resultado = await TransformarAsync(CrearPrincipal());

        resultado.Claims.Should().NotContain(c => c.Type == RolInternoClaimsTransformation.ClaimUsuarioActivo);
    }

    [Fact]
    public async Task SinObjectId_DebeDevolverElPrincipalIntacto()
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity([], "Test"));

        var resultado = await TransformarAsync(principal);

        resultado.Should().BeSameAs(principal);
        _usuarioRepositoryMock.Verify(
            r => r.GetByEntraObjectIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task NoDebeDuplicarClaimsSiSeTransformaDosVeces()
    {
        ConfigurarUsuario(RolSistema.Tesorero, esActivo: true);

        var sut = CrearSut();
        var unaVez = await sut.TransformAsync(CrearPrincipal());
        var dosVeces = await sut.TransformAsync(unaVez);

        dosVeces.FindAll(ClaimTypes.Role).Should().ContainSingle();
    }

    private void ConfigurarUsuario(RolSistema rol, bool esActivo)
    {
        var usuario = new Usuario("socio@lamamedellin.org", ObjectId, rol, esActivo, null);

        _usuarioRepositoryMock
            .Setup(r => r.GetByEntraObjectIdAsync(ObjectId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(usuario);
    }

    private static ClaimsPrincipal CrearPrincipal(string[]? rolesDelToken = null)
    {
        var claims = new List<Claim> { new("oid", ObjectId) };
        claims.AddRange((rolesDelToken ?? []).Select(rol => new Claim(ClaimTypes.Role, rol)));

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"));
    }

    private RolInternoClaimsTransformation CrearSut(bool rolesInternosExclusivos = false)
    {
        var configuracion = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Seguridad:RolesInternosExclusivos"] = rolesInternosExclusivos ? "true" : "false",
            })
            .Build();

        return new RolInternoClaimsTransformation(
            _usuarioRepositoryMock.Object,
            configuracion,
            NullLogger<RolInternoClaimsTransformation>.Instance);
    }

    private Task<ClaimsPrincipal> TransformarAsync(
        ClaimsPrincipal principal,
        bool rolesInternosExclusivos = false)
    {
        return CrearSut(rolesInternosExclusivos).TransformAsync(principal);
    }
}
