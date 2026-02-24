# Sistema Contable L.A.M.A. Medellin

Plataforma contable integral para la Fundacion / Capitulo L.A.M.A. Medellin, con tesoreria, cartera y control de movimientos financieros.

## Tecnologias

- Backend: .NET 8, CQRS (MediatR), Clean Architecture
- Frontend: Next.js (App Router), React Query, Tailwind CSS
- Base de datos: Azure SQL (EF Core)

## Ejecucion local

### Backend (.NET 8)

```bash
cd LAMAMedellin
dotnet build
dotnet run --project src/LAMAMedellin.API
```

La API queda disponible en `http://localhost:5006` (segun tu configuracion local).

### Frontend (Next.js)

```bash
cd frontend
npm install
npm run dev
```

El frontend queda disponible en `http://localhost:3000`.

## Documentacion

- Manual de usuario: docs/MANUAL_USUARIO.md
- Arquitectura: docs/docs_ARCHITECTURE-AZURE.md
- Backlog: docs/docs_BACKLOG.md
