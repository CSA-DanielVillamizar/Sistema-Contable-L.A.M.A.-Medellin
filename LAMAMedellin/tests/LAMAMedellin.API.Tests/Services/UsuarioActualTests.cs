using System.Security.Claims;
using FluentAssertions;
using LAMAMedellin.API.Services;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace LAMAMedellin.API.Tests.Services;

public sealed class UsuarioActualTests
{
    [Fact]
    public void Identificador_SinContextoHttp_DebeSerNulo()
    {
        // Es el caso de la siembra inicial y de las tareas de arranque: no hay
        // usuario, y la auditoria debe reflejarlo en vez de inventar uno.
        var sut = new UsuarioActual(new HttpContextAccessor { HttpContext = null });

        sut.Identificador.Should().BeNull();
    }

    [Fact]
    public void Identificador_ConUsuarioNoAutenticado_DebeSerNulo()
    {
        var sut = CrearSut(new ClaimsIdentity());

        sut.Identificador.Should().BeNull();
    }

    [Fact]
    public void Identificador_DebePreferirElCorreo()
    {
        var sut = CrearSut(new ClaimsIdentity(
            [
                new Claim("oid", "8f14e45f-ceea-467a-9f4b-1a2b3c4d5e6f"),
                new Claim("preferred_username", "tesorero@lamamedellin.org"),
            ],
            authenticationType: "Test"));

        sut.Identificador.Should().Be("tesorero@lamamedellin.org");
    }

    [Fact]
    public void Identificador_SinCorreo_DebeCaerAlObjectIdDeEntra()
    {
        var sut = CrearSut(new ClaimsIdentity(
            [new Claim("oid", "8f14e45f-ceea-467a-9f4b-1a2b3c4d5e6f")],
            authenticationType: "Test"));

        sut.Identificador.Should().Be("8f14e45f-ceea-467a-9f4b-1a2b3c4d5e6f");
    }

    [Fact]
    public void Identificador_DebeRecortarseA256Caracteres()
    {
        // La columna de auditoria admite 256; un claim mas largo no debe
        // reventar el guardado.
        var correoLargo = new string('a', 300);
        var sut = CrearSut(new ClaimsIdentity(
            [new Claim("preferred_username", correoLargo)],
            authenticationType: "Test"));

        sut.Identificador.Should().HaveLength(256);
    }

    private static UsuarioActual CrearSut(ClaimsIdentity identidad)
    {
        var contexto = new DefaultHttpContext { User = new ClaimsPrincipal(identidad) };

        return new UsuarioActual(new HttpContextAccessor { HttpContext = contexto });
    }
}
