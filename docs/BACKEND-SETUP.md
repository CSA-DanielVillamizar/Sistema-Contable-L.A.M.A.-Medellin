# Backend Setup Guide - LAMA Medellín

## Overview

El backend de LAMA Medellín es una **ASP.NET Core Web API** basada en **Clean Architecture** con 4 capas independientes y desacopladas.

```
┌─────────────────────────────────────────────────────────┐
│                   API (Controllers)                      │
│              (LAMAMedellin.API - WebAPI)                 │
└────────────────┬────────────────────────────────────────┘
                 │ ref: Application + Infrastructure
┌────────────────┴─────────────┬──────────────────────────┐
│   Application (Use Cases)     │  Infrastructure (EF+DB)  │
│ (LAMAMedellin.Application)    │(LAMAMedellin.Infra...)   │
└────────────────┬─────────────┴────────────┬──────────────┘
                 │ ref: Domain               │ ref: Domain
┌────────────────┴───────────────────────────┴──────────────┐
│          Domain (Entities + Business Logic)               │
│           (LAMAMedellin.Domain - ClassLib)                │
└───────────────────────────────────────────────────────────┘
```

## Architecture Layers

### **1. Domain (LAMAMedellin.Domain)**

- **Type**: Class Library (.NET 8)
- **Dependencies**: None (Pure Business Logic)
- **Responsibility**:
  - Entities (ej: `Banco`, `CentroCosto`, `Transaccion`)
  - Value Objects (ej: `Monto`, `CuentaBancaria`)
  - Domain Interfaces (Contracts, no implementations)
  - Business Rules & Constants

**Folder Structure** (TBD):

```
Domain/
├── Entities/           # Aggregate Roots (Banco, etc.)
├── ValueObjects/       # Immutable value types (Monto, etc.)
├── Interfaces/         # Repository & Service contracts
└── Constants/          # Business constants (Monedas, Estados, etc.)
```

### **2. Application (LAMAMedellin.Application)**

- **Type**: Class Library (.NET 8)
- **Dependencies**: Domain
- **Responsibility**:
  - Commands & Queries (CQRS via MediatR)
  - Use Cases (Application Services)
  - DTOs (Data Transfer Objects)
  - Validation (FluentValidation)

  - Orchestration logic

**Folder Structure** (TBD):

```
Application/
├── Commands/           # Write operations (RegistrarCuotaCommand, etc.)
├── Queries/            # Read operations (ObtenerBancosQuery, etc.)
├── DTOs/               # Contracts for API payloads
├── Interfaces/         # Application service contracts
├── Validators/         # FluentValidation rules

└── Mappings/           # AutoMapper profiles (Domain → DTO)
```

### **3. Infrastructure (LAMAMedellin.Infrastructure)**

- **Type**: Class Library (.NET 8)
- **Dependencies**: Domain
- **Responsibility**:
  - Database persistence (Entity Framework Core)

  - Repository implementations
  - External service integrations (Azure Storage, etc.)
  - Configuration & Dependency Injection

**Folder Structure** (TBD):

```
Infrastructure/
├── Persistence/        # DbContext, EF migrations, repositories

├── Services/           # External service clients (Azure, etc.)
├── Configuration/      # DI setup, EF configuration
└── Seeders/            # Initial data (test usuarios, etc.)
```

### **4. API (LAMAMedellin.API)**

- **Type**: ASP.NET Core Web API 8.0
- **Dependencies**: Application + Infrastructure

- **Responsibility**:
  - HTTP Controllers (Minimal APIs or Traditional)
  - Middleware (Error handling, CORS, Auth)
  - Swagger/OpenAPI docs
  - Request/Response mapping

**Folder Structure** (TBD):

```
API/
├── Controllers/        # Endpoint handlers
├── Middleware/         # Exception handler, auth, logging

├── Extensions/         # DI extension methods
├── Program.cs          # Startup configuration
└── appsettings.json    # Config (connection strings, etc.)
```

## Getting Started

### Prerequisites

- **.NET SDK 8.0+** ([Download](https://dotnet.microsoft.com/en-us/download/dotnet/8.0))
- **SQL Server** (Local or Azure SQL)
- **Visual Studio Code** (recommended extensions: C#, REST Client)

### Build & Run

```powershell
# Navigate to backend root
cd .\LAMAMedellin

# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run API server (will start on http://localhost:5001)

dotnet run --project src/LAMAMedellin.API

# Run unit tests (when added)
dotnet test

```

## Key Design Decisions

### 1. **No Local User Tables**

- All authentication via **Microsoft Entra External ID**
- NEVER create a `Users` or `Identity` table in Domain/Persistence

### 2. **Mandatory Fields**

- **CentroCostoId**: Required on all monetary transactions

- **TasaCambio** (if USD used): Must include rate, date, and source

### 3. **Soft Deletes**

- Financial entities NEVER deleted physically
- Use `IsDeleted` or `Anulado` flag instead

### 4. **Language Conventions**

- **Business Logic**: Spanish (ej: `RegistrarCuota`, `AbrirCuenta`)
- **Infrastructure**: English (ej: `AzureBlobStorageService`, `EntityTypeConfiguration`)

### 5. **CQRS Pattern**

- Commands: Write operations (ej: `RegistrarTransaccionCommand`)
- Queries: Read operations (ej: `ObtenerSaldoBancoQuery`)
- Handlers: Business logic execution (via MediatR)

## Project References

```
LAMAMedellin.Application
  └─→ LAMAMedellin.Domain


LAMAMedellin.Infrastructure
  └─→ LAMAMedellin.Domain

LAMAMedellin.API
  ├─→ LAMAMedellin.Application
  └─→ LAMAMedellin.Infrastructure
```

## Adding a New Feature

### Example: Register a "Cuota" (Payment)

1. **Create Domain Entity** (`Domain/Entities/Cuota.cs`)

   ```csharp
   public class Cuota : AggregateRoot
   {

       public string Id { get; private set; }
       public decimal Monto { get; private set; }
       public DateTime FechaVencimiento { get; private set; }
       public string CentroCostoId { get; private set; } // Mandatory

       // Factory method for domain logic
       public static Cuota Crear(decimal monto, DateTime vencimiento, string centroCostoId)
       {
           if (monto <= 0) throw new DomainException("Monto debe ser positivo");
           return new Cuota { Monto = monto, FechaVencimiento = vencimiento, CentroCostoId = centroCostoId };

       }
   }
   ```

2. **Create Repository Interface** (`Domain/Interfaces/ICuotaRepository.cs`)

   ```csharp
   public interface ICuotaRepository
   {
       Task<Cuota> ObtenerPorIdAsync(string id);

       Task AgregarAsync(Cuota cuota);
       Task ActualizarAsync(Cuota cuota);
   }
   ```

3. **Create Command** (`Application/Commands/RegistrarCuotaCommand.cs`)

   ```csharp
   public class RegistrarCuotaCommand : IRequest<string>
   {
       public decimal Monto { get; set; }
       public DateTime Vencimiento { get; set; }
       public string CentroCostoId { get; set; }
   }
   ```

4. **Create Command Handler** (`Application/Commands/RegistrarCuotaCommandHandler.cs`)

   ```csharp
   public class RegistrarCuotaCommandHandler : IRequestHandler<RegistrarCuotaCommand, string>
   {
       private readonly ICuotaRepository _repo;

       public async Task<string> Handle(RegistrarCuotaCommand request, CancellationToken ct)
       {
           var cuota = Cuota.Crear(request.Monto, request.Vencimiento, request.CentroCostoId);
           await _repo.AgregarAsync(cuota);
           return cuota.Id;
       }
   }
   ```

5. **Create API Endpoint** (`API/Controllers/CuotasController.cs`)

   ```csharp
   [Route("api/[controller]")]
   public class CuotasController : ControllerBase
   {
       private readonly IMediator _mediator;

       [HttpPost]
       public async Task<ActionResult<string>> Registrar([FromBody] RegistrarCuotaCommand cmd)
       {
           var id = await _mediator.Send(cmd);
           return CreatedAtAction(nameof(Registrar), new { id });
       }
   }
   ```

6. **Create Unit Test**

   ```csharp
   [Fact]
   public async Task RegistrarCuota_ConMontoPositivo_DebeCrearEntidad()
   {
       // Arrange
       var cmd = new RegistrarCuotaCommand { Monto = 100000, CentroCostoId = "CC001" };
       var handler = new RegistrarCuotaCommandHandler(mockRepo);

       // Act
       var id = await handler.Handle(cmd, CancellationToken.None);

       // Assert
       Assert.NotEmpty(id);
       mockRepo.Verify(x => x.AgregarAsync(It.IsAny<Cuota>()), Times.Once);
   }
   ```

## Dependencies (To Install)

These will be added as features develop:

```bash
# CQRS & Mediator
dotnet add package MediatR

# Validation
dotnet add package FluentValidation

# ORM & Migrations
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer

# Dependency Injection
dotnet add package Microsoft.Extensions.DependencyInjection

# Logging
dotnet add package Serilog
dotnet add package Serilog.AspNetCore

# Mapping
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection

# Azure Integration (when needed)
dotnet add package Azure.Storage.Blobs
dotnet add package Microsoft.Extensions.Azure
```

## Database Setup (Pending)

When ready to integrate EF Core:

```powershell
# Add migrations
dotnet ef migrations add InitialCreate --project src/LAMAMedellin.Infrastructure

# Apply to database
dotnet ef database update --project src/LAMAMedellin.Infrastructure
```

## Further Reading

- [Clean Architecture Handbook](https://www.o-reilly.com/library/view/clean-architecture/9780134494272/)
- [CQRS Pattern](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs)
- [EF Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [ASP.NET Core Best Practices](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/)

---

**Last Updated**: 2025 | **Status**: Backend scaffold ready for feature development
