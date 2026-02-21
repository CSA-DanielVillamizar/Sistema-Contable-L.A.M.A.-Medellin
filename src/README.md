Source Code & Development Projects

This directory contains all production code and project-specific scripts.

## Structure

```
src/
├── LAMAMedellin/               .NET 8 Backend Solution
│   ├── LAMAMedellin.slnx      Solution file
│   ├── src/                    Project sources
│   │   ├── LAMAMedellin.Domain/           Business entities
│   │   ├── LAMAMedellin.Application/      Use cases (CQRS)
│   │   ├── LAMAMedellin.Infrastructure/   Data access & services
│   │   └── LAMAMedellin.API/              HTTP endpoints
│   └── tests/                  Unit & integration tests
│
└── scripts/                    Project-specific automation
    └── create_github_backlog.ps1  Backlog synchronization
```

## Backend Development

The backend is built with:

- Framework: ASP.NET Core 8.0 Web API
- Architecture: Clean Architecture (4-layer)
- Data Access: Entity Framework Core with Azure SQL
- Patterns: CQRS + MediatR for command/query handling

For detailed setup and development workflow, see:

- [Backend Setup Guide](../docs/BACKEND-SETUP.md)
- [Local Development Setup](../docs/guides/LOCAL-SETUP.md)

## Building & Running

From project root:

```powershell
cd src/LAMAMedellin

# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run API server
dotnet run --project src/LAMAMedellin.API

# Run tests (when added)
dotnet test
```

## Project Organization

Each layer in LAMAMedellin follows Clean Architecture principles:

**Domain**

- Entities and value objects
- Domain interfaces (contracts only)
- Business rules and constants
- No external dependencies

**Application**

- Use cases and business logic
- Commands and queries (CQRS pattern)
- Data transfer objects (DTOs)
- Validators and mappers
- Depends on: Domain

**Infrastructure**

- Database context (EF Core)
- Repository implementations
- External service clients
- Depends on: Domain

**API**

- HTTP controllers
- Middleware and filters
- Dependency injection setup
- Depends on: Application + Infrastructure

## Adding New Features

When implementing a new feature, follow this sequence:

1. **Domain** - Define entities and value objects
2. **Application** - Write commands/queries and handlers
3. **Infrastructure** - Implement repositories and data access
4. **API** - Create controllers and endpoints
5. **Tests** - Add unit tests for domain and application logic

See [Contributing Guide](../docs/guides/CONTRIBUTING.md) for detailed workflow.

## Scripts

**create_github_backlog.ps1**
Synchronizes the canonical issue catalog with GitHub, ensuring backlog consistency.

Usage:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\create_github_backlog.ps1 `
  -Repo "CSA-DanielVillamizar/Sistema-Contable-L.A.M.A.-Medellin" `
  -BacklogPath "C:\Path\To\backlog" `
  -ResetCatalog  # Optional: reset to canonical state
```

## Code Standards

All code must follow:

- Language: .NET C# for backend
- Naming: Spanish for business logic, English for infrastructure
- No emojis in comments or strings
- Clean code principles
- See [Code Standards](../governance/process-docs/CODE-STANDARDS.md) for details

## For More Information

- Documentation: [docs/README.md](../docs/README.md)
- Governance: [governance/README.md](../governance/README.md)
- Project repository: [GitHub](https://github.com/CSA-DanielVillamizar/Sistema-Contable-L.A.M.A.-Medellin)
