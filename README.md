# L.A.M.A. ERP - Sistema de Gestion de Moto Clubs

L.A.M.A. ERP es una plataforma integral para la gestion operativa, financiera y administrativa del Capitulo L.A.M.A. Medellin. Centraliza procesos criticos del club en una sola solucion, con enfoque en trazabilidad, control y escalabilidad empresarial.

## Modulos Principales

1. Cartera

- Gestion de conceptos de cobro, cuentas por cobrar, cuotas y seguimiento de recaudos pendientes.

1. Tesoreria

- Control de cajas, ingresos, egresos y saldos disponibles para operacion diaria.

1. Merchandising

- Administracion de inventario, ventas y movimiento de productos del club.

1. Miembros

- Directorio de miembros, perfil ampliado, datos de emergencia y estado activo del capitulo.

1. Eventos

- Agenda de eventos y rodadas, detalle por evento y control de asistencia.

## Arquitectura

### Backend

- .NET 8 Web API
- Clean Architecture
- CQRS con MediatR
- Entity Framework Core
- Azure SQL

### Frontend

- Next.js (App Router)
- TypeScript
- React Query (TanStack Query)
- TailwindCSS

## Actualizacion Frontend (2026-05-28)

- HU-T02: Cliente API robustecido con normalizacion centralizada de errores ProblemDetails para manejo consistente en hooks y vistas.
- HU-T03: App Shell global implementado con Sidebar y Navbar persistentes, manteniendo area de contenido con scroll independiente.
- HU-T01: Home simplificada para enfoque ejecutivo sobre KPIs (ResumenKpis), removiendo la grilla masiva de accesos.
- Validacion: build de produccion frontend ejecutado exitosamente.

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
