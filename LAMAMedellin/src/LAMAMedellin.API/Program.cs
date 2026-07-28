using LAMAMedellin.API.Middleware;
using LAMAMedellin.API.Services;
using LAMAMedellin.Application;
using LAMAMedellin.Application.Common.Interfaces.Services;
using LAMAMedellin.Infrastructure.Configuration;
using LAMAMedellin.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Se usa la convencion por defecto de ASP.NET Core: camelCase al serializar y
// binding insensible a mayusculas al deserializar. Antes se anulaba la politica
// de nombres para emitir PascalCase, lo que obligaba al frontend a leer cada
// campo dos veces (`item.saldo ?? item.Saldo`) en todos sus DTOs.
builder.Services.AddControllers();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment);

// Necesario para que la pista de auditoria sepa quien ejecuta cada operacion.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUsuarioActual, UsuarioActual>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("NextJsCors", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

// El rol interno de la base de datos se proyecta como claim en cada peticion,
// de modo que [Authorize(Roles = ...)] evalue lo que administra la aplicacion y
// no solo los app roles de Entra.
builder.Services.AddScoped<IClaimsTransformation, RolInternoClaimsTransformation>();

builder.Services.AddAuthorization();

// Un usuario dado de baja debe quedar bloqueado tambien en los endpoints que se
// conforman con [Authorize] sin exigir rol. Se aplica como filtro global para
// que cubra todos los controladores; [AllowAnonymous] sigue teniendo prioridad.
var politicaUsuarioActivo = new AuthorizationPolicyBuilder()
    .RequireAuthenticatedUser()
    .RequireAssertion(contexto => !contexto.User.HasClaim(
        RolInternoClaimsTransformation.ClaimUsuarioActivo,
        "false"))
    .Build();

builder.Services.Configure<MvcOptions>(options =>
    options.Filters.Add(new AuthorizeFilter(politicaUsuarioActivo)));

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

var app = builder.Build();

// Seed database on startup (Development mode only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<LamaDbContext>();

    // Run migrations
    await context.Database.MigrateAsync();

    // Seed initial data
    await context.SeedAsync();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors("NextJsCors");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program;
