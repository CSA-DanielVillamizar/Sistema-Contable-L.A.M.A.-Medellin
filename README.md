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

Requisitos: .NET 8 SDK, Node.js 24 y Docker Desktop. No hacen falta credenciales de
Azure ni conexion a internet.

### 1. Base de datos

Levanta un SQL Server local (mismo motor que produccion) en el puerto 14330:

```bash
docker compose up -d
```

El puerto 14330 evita chocar con una instancia de SQL Server ya instalada en la
maquina. Para empezar de cero en cualquier momento: `docker compose down -v`.

### 2. Backend (.NET 8)

Copia la plantilla de configuracion local (esta en .gitignore, no se commitea):

```bash
cp LAMAMedellin/src/LAMAMedellin.API/appsettings.Development.example.json LAMAMedellin/src/LAMAMedellin.API/appsettings.Development.json
```

Compila y ejecuta:

```bash
cd LAMAMedellin
dotnet build LAMAMedellin.slnx
dotnet run --project src/LAMAMedellin.API --no-launch-profile
```

En el primer arranque crea el esquema y siembra los datos base (52 cuentas del PUC,
38 miembros, cajas, banco y tarifas). Queda escuchando en `http://localhost:5006`;
`GET /` responde el estado del servicio. Todos los demas endpoints exigen token, asi
que sin login responden `401`.

Pruebas:

```bash
cd LAMAMedellin
dotnet test LAMAMedellin.slnx
```

### 3. Frontend (Next.js)

```bash
cd frontend
npm install
npm run dev
```

Queda en `http://localhost:3000` y apunta por defecto a `http://localhost:5006`.
El login usa Entra ID: requiere que el app registration tenga `http://localhost:3000`
como redirect URI de tipo SPA.

### Notas importantes

- **Nunca apuntes el entorno Development a la base de produccion.** Al arrancar en
  Development el API inicializa el esquema y siembra datos automaticamente.
- El esquema local se crea aplicando la migracion base `20260727233255_Baseline`.
  El historial anterior estaba roto y se colapso en esa unica migracion; el detalle
  y el procedimiento para registrarla en produccion estan en
  [docs/BACKEND-SETUP.md](docs/BACKEND-SETUP.md#historial-de-migraciones).
- Para correr el API sin base de datos (solo verificar que compila y arranca):
  `ASPNETCORE_ENVIRONMENT=Staging dotnet run --project src/LAMAMedellin.API --no-launch-profile`

## Construido con orgullo

Construido con orgullo para fortalecer la gestion institucional de L.A.M.A. Medellin.
