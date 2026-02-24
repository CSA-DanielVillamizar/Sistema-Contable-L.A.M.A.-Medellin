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

- **Managed Identity** (System Assigned) para Azure SQL, Key Vault y Blob.
- Zero secrets en appsettings del repo.
- RBAC interno + MFA Entra.

### Autenticación Azure SQL

**En Development (local):**
- El código usa `AzureCliCredential` explícitamente
- Requisito: `az login` activo
- Connection string: **SIN** `Authentication=Active Directory Default`
- El token se inyecta programáticamente desde la sesión de Azure CLI

**En Production (Azure App Service):**
- El código usa `DefaultAzureCredential` que detecta automáticamente Managed Identity
- Connection string: **CON** `Authentication=Active Directory Default`
- Azure App Service debe tener System Assigned Managed Identity habilitada
- El SQL Server debe tener configurado al App Service como Azure AD User con permisos

### Configuración requerida en Azure

#### Azure App Service
1. Habilitar **System Assigned Managed Identity**:
   ```bash
   az webapp identity assign --name <app-name> --resource-group <rg-name>
   ```

2. Obtener el **Object ID** del Managed Identity para configurar SQL

#### Azure SQL Server
1. Configurar Entra ID Admin (si no está):
   ```bash
   az sql server ad-admin create \
     --resource-group <rg-name> \
     --server-name <sql-server-name> \
     --display-name "<admin-display-name>" \
     --object-id <admin-object-id>
   ```

2. Crear usuario de base de datos para el App Service Managed Identity:
   ```sql
   -- Conectarse como Entra ID Admin a la base de datos específica
   CREATE USER [<app-service-name>] FROM EXTERNAL PROVIDER;
   ALTER ROLE db_datareader ADD MEMBER [<app-service-name>];
   ALTER ROLE db_datawriter ADD MEMBER [<app-service-name>];
   GO
   ```

## Backups

- SQL: backups automáticos + retención.
- Blob: soft delete opcional + lifecycle.
- Runbook: restore probado periódicamente.

## Variables de entorno production

En Azure App Service, configurar:

```bash
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection="Server=tcp:<server>.database.windows.net,1433;Initial Catalog=<database>;Authentication=Active Directory Default;Encrypt=True;"
```

**Nota:** El connection string en Production **SÍ** incluye `Authentication=Active Directory Default` para usar Managed Identity.


## Resource Group

SUBSCRIPTIONID="<subscription-id>"
RESOURCE_GROUP="<resource-group-name>"
LOCATION="<azure-region>"
SERVER_NAME="<sql-server-name>.database.windows.net"
DB_NAME="<database-name>"
ADMIN_USER="<sql-admin-user>"
ADMIN_PASSWORD="<do-not-store-secrets-in-repo>"
APPSERVICE="<app-service-name>.azurewebsites.net"
RUNTIME="<runtime-stack>"
BLOB="<storage-account-name>"


## Autenticación Azure

- ✅ Entra ID Admin (Daniel Villamizar)
- ✅ Database User con roles `db_datareader` / `db_datawriter`
- ✅ Autenticación vía Azure CLI (`az account get-access-token`)
