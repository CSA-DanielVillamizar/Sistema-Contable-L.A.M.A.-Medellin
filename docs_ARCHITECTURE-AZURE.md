# Arquitectura mínima en Azure (costos sostenibles)

## Objetivo
Desplegar una solución segura, auditable y de bajo costo, escalable sin rediseño.

## Componentes
- Next.js (web)
- ASP.NET Core 8 Web API (api)
- Azure SQL Database (datos)
- Azure Blob Storage (soportes)
- Azure Key Vault (secretos)
- Entra External ID (auth + MFA)
- Application Insights + Log Analytics (observabilidad)

## Despliegue recomendado
- API .NET: Azure App Service (Linux), plan bajo con autoscale.
- Web Next.js:
  - Opción 1: Azure Static Web Apps (si SSR compatible/limitado).
  - Opción 2: App Service Node para SSR completo.
- SQL: serverless o S0 (según patrón de uso).
- Blob: lifecycle (hot→cool→archive).

## Seguridad
- Managed Identity para Key Vault y Blob.
- Zero secrets en appsettings del repo.
- RBAC interno + MFA Entra.

## Backups
- SQL: backups automáticos + retención.
- Blob: soft delete opcional + lifecycle.
- Runbook: restore probado periódicamente.