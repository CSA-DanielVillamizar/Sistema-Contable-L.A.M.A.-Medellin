using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using LAMAMedellin.Application.Features.Tesoreria.Commands.RegistrarEgreso;
using LAMAMedellin.Domain.Common;
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

public sealed class TesoreriaControllerExceptionTests
{
    [Fact]
    public async Task PostEgreso_ReglaNegocioException_DebeRetornarBadRequest()
    {
        await using var factory = new TesoreriaApiFactory();
        factory.Sender.ExceptionToThrow = new ReglaNegocioException("Saldo insuficiente en caja para registrar el egreso.");

        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TesoreriaTestAuthHandler.SchemeName, "ok");

        var payload = new
        {
            fecha = DateTime.UtcNow,
            monto = 150000m,
            concepto = "Egreso test",
            terceroId = (Guid?)null,
            cuentaContableId = Guid.NewGuid(),
            cajaId = Guid.NewGuid(),
            centroCostoId = Guid.NewGuid()
        };

        var response = await client.PostAsync("/api/tesoreria/egresos", JsonContent(payload));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("Saldo insuficiente en caja para registrar el egreso.");
    }

    private static StringContent JsonContent(object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }
}

internal sealed class TesoreriaApiFactory : WebApplicationFactory<Program>
{
    public TesoreriaTestSender Sender { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ISender>();
            services.AddSingleton<ISender>(Sender);

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TesoreriaTestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TesoreriaTestAuthHandler.SchemeName;
                    options.DefaultScheme = TesoreriaTestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TesoreriaTestAuthHandler>(TesoreriaTestAuthHandler.SchemeName, _ => { });

            services.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = TesoreriaTestAuthHandler.SchemeName;
                options.DefaultChallengeScheme = TesoreriaTestAuthHandler.SchemeName;
                options.DefaultScheme = TesoreriaTestAuthHandler.SchemeName;
            });

            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, _ => { });
        });
    }
}

internal sealed class TesoreriaTestSender : ISender
{
    public Exception? ExceptionToThrow { get; set; }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        ThrowIfConfigured();
        return Task.FromResult<object?>(request is IRequest<Unit> ? Unit.Value : null);
    }

    public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ThrowIfConfigured();

        if (request is RegistrarEgresoCommand && typeof(TResponse) == typeof(Guid))
        {
            return Task.FromResult((TResponse)(object)Guid.NewGuid());
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
        ThrowIfConfigured();
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

    private void ThrowIfConfigured()
    {
        if (ExceptionToThrow is not null)
        {
            throw ExceptionToThrow;
        }
    }
}

internal sealed class TesoreriaTestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "TesoreriaTestAuth";

    public TesoreriaTestAuthHandler(
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

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "integration-test-user"),
            new Claim(ClaimTypes.Role, "Tesorero")
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
