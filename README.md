# Sistema Contable L.A.M.A. Medellin

Plataforma integral para la gestion operativa, financiera y administrativa del Capitulo L.A.M.A. Medellin y su Fundacion. Centraliza procesos contables, tesoreria, cartera, donaciones y proyectos sociales bajo arquitectura empresarial escalable.

## Estructura del Repositorio

```
Sistema-Contable-L.A.M.A.-Medellin/
├── LAMAMedellin/          Backend .NET 8 (Clean Architecture)
│   ├── src/
│   │   ├── LAMAMedellin.Domain/           Entidades y contratos de dominio
│   │   ├── LAMAMedellin.Application/      Casos de uso (CQRS + MediatR)
│   │   ├── LAMAMedellin.Infrastructure/   EF Core, repositorios, servicios Azure
│   │   └── LAMAMedellin.API/              Endpoints HTTP (Minimal APIs)
│   └── tests/
│       ├── LAMAMedellin.API.Tests/
│       └── LAMAMedellin.Application.Tests/
├── frontend/              Next.js 14 App Router (TypeScript + TailwindCSS)
│   └── src/
│       ├── app/           Paginas y rutas (Server Components)
│       └── components/    Componentes reutilizables
├── docs/                  Documentacion del proyecto
│   ├── ESPECIFICACION_PUC_ESAL.md
│   ├── BACKEND-SETUP.md
│   ├── DESPLIEGUE_AZURE.md
│   └── MANUAL_USUARIO.md
├── scripts/               Scripts de infraestructura y base de datos
│   └── sql/               Migraciones SQL idempotentes
├── tools/
│   └── qa-scripts/        Scripts de QA y pruebas E2E (uso local)
├── backlog/               Templates de issues (historico)
└── governance/            Estandares de codigo y proceso
```

## Modulos del Sistema

| Modulo | Descripcion | Fase |
|--------|-------------|------|
| IAM | Autenticacion Entra External ID + RBAC | Phase 0 |
| Infra | Azure Key Vault, Blob Storage, Observabilidad | Phase 0 |
| Contabilidad | PUC ESAL, comprobantes, libros, cierres | Phase 1 |
| Tesoreria | Movimientos bancarios, recibos, anulaciones | Phase 1 |
| Cartera | Cuotas, CxC, mora, recaudo miembros | Phase 1 |
| CxP | Facturas proveedor, vencimientos, pagos | Phase 1 |
| Multimoneda | USD informativo, diferencia en cambio, TRM | Phase 1 |
| Donaciones | Campanas, certificados obligatorios (ESAL) | Phase 2 |
| Proyectos Sociales | Beneficiarios, consentimiento, rendicion | Phase 3 |
| Negocios | Inventario, compras/ventas, comprobante interno | Phase 4 |
| Tributario | Exogena, beneficiarios finales | Phase 5 |

## Arquitectura

### Backend (.NET 8)

- **Patron:** Clean Architecture + CQRS con MediatR
- **Validacion:** FluentValidation como Behavior en pipeline MediatR
- **Persistencia:** Entity Framework Core sobre Azure SQL
- **Seguridad:** Microsoft Entra External ID (sin tablas de usuarios locales)
- **Secretos:** Azure Key Vault via Managed Identity (`IOptions<T>`)
- **Errores:** Middleware global + `ProblemDetails` estandarizados

### Frontend (Next.js 14)

- **Router:** App Router con TypeScript estricto
- **Estado servidor:** TanStack Query (React Query)
- **Estilos:** TailwindCSS
- **Componentes:** Server Components por defecto; `'use client'` solo para interactividad

## Ejecucion Local

### Backend

```bash
cd LAMAMedellin
dotnet restore
dotnet build
dotnet run --project src/LAMAMedellin.API
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

## Documentacion

- [Especificacion PUC ESAL](docs/ESPECIFICACION_PUC_ESAL.md)
- [Setup del Backend](docs/BACKEND-SETUP.md)
- [Despliegue en Azure](docs/DESPLIEGUE_AZURE.md)
- [Manual de Usuario](docs/MANUAL_USUARIO.md)
- [Arquitectura Azure](docs/docs_ARCHITECTURE-AZURE.md)

## Reglas de Negocio Centrales

- Toda transaccion monetaria requiere `CentroCostosId` obligatorio.
- Las entidades financieras nunca se borran fisicamente (soft-delete con `Anulado`/`IsDeleted`).
- Moneda funcional: **COP**. USD es informativo; requiere TRM, fecha y fuente registrados.
- Autenticacion 100% delegada a Entra External ID con MFA obligatorio.

---

Construido con orgullo para fortalecer la gestion institucional de L.A.M.A. Medellin.
