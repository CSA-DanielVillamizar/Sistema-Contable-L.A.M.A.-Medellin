Code Standards & Conventions

This document defines the code standards and conventions for the LAMA Medellín project.

## Language Conventions

**Business Logic & Domain**: Spanish

- Use Spanish for entity names, commands, queries, and business logic
- Examples: `RegistrarCuota`, `AbrirCuenta`, `CalcularMora`, `TransaccionBancaria`
- Reasoning: Domain experts and stakeholders communicate in Spanish

**Infrastructure & Technical**: English

- Use English for technical infrastructure, patterns, and utilities
- Examples: `AzureBlobStorageService`, `EntityTypeConfiguration`, `RepositoryBase`, `ILogger`
- Reasoning: Technical frameworks and libraries use English

**Code Comments**: English or Spanish as appropriate

- Use Spanish for business logic explanation
- Use English for technical implementation details

## Code Style

### C# (.NET Backend)

**Naming Conventions**

- Classes: PascalCase (`TransaccionBancaria`, `RegistrarCuotaCommand`)
- Interfaces: PascalCase with `I` prefix (`ITransaccionRepository`, `ICuotaService`)
- Properties: PascalCase (`MontoTotal`, `FechaCreacion`)
- Private fields: camelCase with underscore (`_logger`, `_repository`)
- Constants: UPPER_SNAKE_CASE (`MAX_TRANSACTIONAL_AMOUNT`, `DEFAULT_CURRENCY`)
- Methods: PascalCase (`CalcularMora`, `ValidarCuenta`)
- Local variables: camelCase (`bancoPrincipal`, `transaccionesAbiertas`)

**Formatting**

```csharp
// Use 4 spaces for indentation
public class TransaccionBancaria
{
    private readonly ILogger _logger;

    public string Id { get; private set; }
    public decimal Monto { get; private set; }

    // Constructor
    public TransaccionBancaria()
    {
    }
}
```

**No Emojis**

- Never use emojis in code comments or strings
- Exception: Documentation screenshots that include UI elements with emojis

### Architecture Patterns

**Clean Architecture**

- Keep layers strictly separated
- No bypassing layers (Domain should never reference Infrastructure)
- Use dependency injection for cross-layer communication
- All business logic belongs in Domain or Application layers

**CQRS Pattern**

- Separate read (Queries) from write (Commands) operations
- Create explicit command and query classes
- Use MediatR for dispatching commands/queries
- Handlers contain business logic

Example:

```csharp
// Command
public class RegistrarCuotaCommand : IRequest<string>
{
    public decimal Monto { get; set; }
    public string CentroCostoId { get; set; }
}

// Handler
public class RegistrarCuotaHandler : IRequestHandler<RegistrarCuotaCommand, string>
{
    public async Task<string> Handle(RegistrarCuotaCommand request, CancellationToken ct)
    {
        // Business logic here
        return cuotaId;
    }
}
```

**Error Handling**

- Use domain exceptions for business rule violations
- Use standard exceptions for infrastructure issues
- Always include meaningful error messages
- Log errors with context (user, transaction, etc.)

## Database Standards

**Entity Framework Core**

- Use Fluent API for configuration (not data annotations where possible)
- Create `EntityTypeConfiguration<T>` classes for each entity
- Name migrations clearly: `AddTransaccionBancariaTable`, `AddSoftDeleteToFinancialEntities`

**Soft Deletes**

- All financial entities must have `IsDeleted` or `Anulado` flag
- Never physically delete transactions, accounts, or monetary records
- Always query with `.Where(x => !x.IsDeleted)`

**Multimoneda**

- Default currency: COP (Colombian Peso)
- If USD is used, always track:
  - `MontoMonedaOrigen` (original amount in USD)
  - `TasaCambioUsada` (exchange rate applied)
  - `FechaTasaCambio` (date of exchange rate)
  - `FuenteTasaCambio` (source of rate: manual, API, etc.)

## Security Standards

**Authentication**

- 100% delegation to Microsoft Entra External ID
- Never create local user tables
- Use claims-based authorization

**Configuration**

- No hardcoded secrets
- Use Azure Key Vault via IOptions<T>
- Environment-specific configuration via appsettings.{Environment}.json

**Data Access**

- Use parameterized queries (EF Core protection)
- Validate all user input on server side
- Implement row-level security where needed

## Testing Standards

**Unit Tests**

- Use xUnit framework
- Use Moq for mocking
- Use FluentAssertions for readable assertions
- Test names follow pattern: `Method_Scenario_ExpectedResult`

Example:

```csharp
[Fact]
public async Task RegistrarCuota_ConMontoPositivo_DebeCrearEntidad()
{
    // Arrange
    var comando = new RegistrarCuotaCommand { Monto = 100000 };

    // Act
    var resultado = await handler.Handle(comando, CancellationToken.None);

    // Assert
    resultado.Should().NotBeEmpty();
    repositoryMock.Verify(x => x.AgregarAsync(It.IsAny<Cuota>()), Times.Once);
}
```

## Documentation Standards

**Code Comments**

- Comment the "why", not the "what"
- Use XML comments for public APIs
- Keep comments up-to-date with code

**Commit Messages**

- Use imperative mood: "Add feature" not "Added feature"
- Reference issue numbers: `feat: Add cuota registration (#123)`
- Scope prefixes: `feat:`, `fix:`, `docs:`, `refactor:`, `test:`

**Pull Request Descriptions**

- Explain the change clearly
- Link related issues
- Describe testing performed
- Include screenshots if UI changes

## Code Review Checklist

See [Review Checklist](REVIEW-CHECKLIST.md) for pull request review criteria.

## For More Information

- [Architecture Overview](../../docs/ARCHITECTURE.md)
- [Contributing Guide](CONTRIBUTING.md)
- [GitHub Codebooks](https://github.com/CSA-DanielVillamizar/Tesoreriaygerenciade-negocios)
