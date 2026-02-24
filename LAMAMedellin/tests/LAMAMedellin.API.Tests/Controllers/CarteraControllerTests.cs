using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using LAMAMedellin.Application.Features.Cartera.Commands.RegistrarPago;
using LAMAMedellin.Application.Features.Cartera.Queries.GetCarteraPendiente;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;

namespace LAMAMedellin.API.Tests.Controllers;

public sealed class CarteraControllerTests
{
    [Fact]
    public async Task GetPendiente_SinAutenticacion_DebeRetornarUnauthorized()
    {
        await using var factory = new CarteraApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/cartera/pendiente");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetPendiente_ConAutenticacion_DebeRetornarListaTipadaEnOrdenEsperado()
    {
        await using var factory = new CarteraApiFactory();
        factory.Sender.CarteraPendienteResponse =
        [
            new CarteraPendienteDto(Guid.NewGuid(), Guid.NewGuid(), "Ana Zapata", "2026-02", 100000m, 25000m),
            new CarteraPendienteDto(Guid.NewGuid(), Guid.NewGuid(), "Bruno Perez", "2026-01", 100000m, 100000m)
        ];

        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.SchemeName, "ok");

        var response = await client.GetAsync("/api/cartera/pendiente");
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<List<CarteraPendienteDto>>(payload, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(data);
        data.Should().HaveCount(2);
        data[0].Periodo.Should().Be("2026-02");
        data[0].NombreMiembro.Should().Be("Ana Zapata");
        data[1].Periodo.Should().Be("2026-01");

        factory.Sender.CapturedRequests.Should().ContainSingle(r => r is GetCarteraPendienteQuery);
    }

    [Fact]
    public async Task PostPago_SinAutenticacion_DebeRetornarUnauthorized()
    {
        await using var factory = new CarteraApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync($"/api/cartera/{Guid.NewGuid()}/pago", JsonContent(new { MontoCOP = 100000m }));

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task PostPago_ConAutenticacion_DebeRetornarOk()
    {
        await using var factory = new CarteraApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.SchemeName, "ok");

        var cuentaId = Guid.NewGuid();
        var monto = 150000m;

        var response = await client.PostAsync($"/api/cartera/{cuentaId}/pago", JsonContent(new { MontoCOP = monto }));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PostPago_ConAutenticacion_DebeEnviarCommandConDatosCorrectos()
    {
        await using var factory = new CarteraApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.SchemeName, "ok");

        var cuentaId = Guid.NewGuid();
        var monto = 175000m;

        var response = await client.PostAsync($"/api/cartera/{cuentaId}/pago", JsonContent(new { MontoCOP = monto }));
        response.EnsureSuccessStatusCode();

        factory.Sender.CapturedRequests.Should().ContainSingle();
        factory.Sender.CapturedRequests[0].Should().BeOfType<RegistrarPagoCuotaCommand>();

        var command = (RegistrarPagoCuotaCommand)factory.Sender.CapturedRequests[0];
        command.CuentaPorCobrarId.Should().Be(cuentaId);
        command.MontoCOP.Should().Be(monto);
    }

    private static StringContent JsonContent(object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}

internal sealed class CarteraApiFactory : WebApplicationFactory<Program>
{
    public TestSender Sender { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ISender>();
            services.AddSingleton<ISender>(Sender);

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                    options.DefaultScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });

            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                options.DefaultScheme = TestAuthHandler.SchemeName;
            });

            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, _ => { });
        });
    }
}

internal sealed class TestSender : ISender
{
    public List<object> CapturedRequests { get; } = [];
    public List<CarteraPendienteDto> CarteraPendienteResponse { get; set; } = [];

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        CapturedRequests.Add(request);

        return Task.FromResult<object?>(request is IRequest<Unit> ? Unit.Value : null);
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        CapturedRequests.Add(request);

        if (request is GetCarteraPendienteQuery && typeof(TResponse) == typeof(List<CarteraPendienteDto>))
        {
            return Task.FromResult((TResponse)(object)CarteraPendienteResponse);
        }

        if (typeof(TResponse) == typeof(Unit))
        {
            return Task.FromResult((TResponse)(object)Unit.Value);
        }

        throw new InvalidOperationException($"No response configured for {typeof(TResponse).Name}.");
    }

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
        where TRequest : IRequest
    {
        CapturedRequests.Add(request!);
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }
}

internal sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "TestAuth";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        System.Text.Encodings.Web.UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Headers.Authorization.Count == 0)
        {
            return Task.FromResult(AuthenticateResult.Fail("Authorization header missing."));
        }

        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, "integration-test-user") };
        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
