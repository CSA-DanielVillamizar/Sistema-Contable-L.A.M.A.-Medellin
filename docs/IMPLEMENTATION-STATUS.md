# Estado de Implementación - Módulo Cartera

## Actualización Ejecutiva — 04 de Marzo 2026

### ✅ Cierre de Fase 3 — Proyectos y Donaciones

- Migración productiva aplicada: `20260304200521_AddProyectoSocialIdToBeneficiario`.
- Validación técnica en Azure SQL completada (`MigrationApplied=1`, columna/FK/índice en `Beneficiarios`).
- Prueba E2E en entorno real ejecutada:
  - Proyecto de prueba creado y asociado a beneficiario.
  - Beneficiario guardado con `ProyectoSocialId` y `TieneConsentimientoHabeasData=true`.
  - Verificación en listado: cruce de proyecto y estado de Habeas Data correcto.
  - Limpieza de datos de prueba ejecutada (beneficiario y proyecto eliminados).

**Estado Épica Fase 3:** `COMPLETADA`.

### ▶️ Inicio de Fase 4 — Negocios

- Se inicia la épica de `inventario simple de merchandising y ventas`.
- Alcance inicial priorizado para Fase 4:
  1. Catálogo básico de artículos (camisetas, parches).
  2. Movimientos mínimos de inventario (entrada/salida).
  3. Registro de venta con impacto en inventario.
  4. Comprobante interno de venta (base para PDF + QR).

**Estado Épica Fase 4:** `EN INICIO`.

**Fecha:** 21 de Febrero 2026
**Branch:** `docs/spec-v1`
**Última actualización:** Commit `b5b3ee0`

## ✅ Completado

### 1. Frontend - Interfaz de Generación de Cartera

- **Componente:** `GenerarCarteraForm.tsx`
  - Form con React Hook Form + Zod validation
  - Input de período (formato YYYY-MM)
  - Centro de costo obligatorio
  - Selector de tipo de afiliación (multi-select)
  - Llamada a endpoint `/api/cartera/generar`

- **Página:** `/cartera/generar`
  - Integrada en Next.js App Router
  - Accesible desde la aplicación

- **Estado:** ✅ Build exitoso, Lint OK (1 warning pre-existente no relacionado)

### 2. Backend - Lógica de Negocio con Período Granular

#### 2.1. Repositorio Refactorizado

**Archivo:** `ICuotaAsambleaRepository.cs` / `CuotaAsambleaRepository.cs`

**Cambio principal:** De `GetByAnioAsync(int anio)` a `GetVigentePorPeriodoAsync(int anio, int mes)`

**Lógica implementada:**

```csharp
// Busca la cuota más reciente donde:
// - El año sea menor al solicitado, O
// - El año sea igual Y el mes de inicio de cobro sea menor o igual al solicitado
// Ordenado descendente por (Anio, MesInicioCobro)

WHERE Anio < @targetAnio OR (Anio = @targetAnio AND MesInicioCobro <= @targetMes)
ORDER BY Anio DESC, MesInicioCobro DESC
```

**Casos de prueba validados:**

- ✅ 2026-01 → $20,000 (cuota histórica)
- ✅ 2026-02 → $25,000 (cuota nueva)
- ✅ 2026-03+ → $25,000 (usa cuota feb 2026)
- ✅ 2025-12 → Sin cuota (correcto, no existe)

#### 2.2. Command Handler Actualizado

**Archivo:** `GenerarObligacionesMensualesCommandHandler.cs`

- Parsea `mes` desde el string `Periodo` (YYYY-MM)
- Llama a `GetVigentePorPeriodoAsync(anio, mes)`
- Error message actualizado para incluir período completo

#### 2.3. Entidades Actualizadas

**Archivos:** `CuotaAsamblea.cs` / `CuentaPorCobrar.cs`

- Agregados constructores privados sin parámetros para EF Core
- Soluciona error: "No suitable constructor found for entity type"
- Mantiene constructor público con validaciones de negocio

**Estado:** ✅ Build exitoso, 0 errores, 0 warnings

### 3. Base de Datos - Azure SQL

#### 3.1. Esquema Creado

**Script:** `2026-02-21_configuracion-cartera-y-cuotaasamblea.sql`

- Tabla `Miembros` (NumeroMiembro UNIQUE, TipoAfiliacion, EstadoMiembro, IsDeleted)
- Tabla `CuotasAsamblea` (Anio, ValorMensualCOP, MesInicioCobro, ActaSoporte)
- Tabla `CuentasPorCobrar` (Periodo YYYY-MM, MiembroId, CuotaAsambleaId, FK constraints)

**Estado:** ✅ Ejecutado en `LAMAMedellinContable`

#### 3.2. Datos Sembrados

**Script:** `2026-02-21_seed-cuotas-y-miembros.sql`

**CuotasAsamblea:**

| Año  | Mes Inicio | Valor COP | Acta Soporte                                   |
|------|------------|-----------|------------------------------------------------|
| 2026 | 1          | 20,000    | Acta Asamblea Diciembre 2025 (Cuota Histórica)|
| 2026 | 2          | 25,000    | Acta Asamblea Enero 2026                       |

**Miembros:** 37 registros activos

| Tipo Afiliación | Cantidad |
|-----------------|----------|
| Full Color (1)  | 15       |
| Rockets (2)     | 10       |
| Prospect (3)    | 5        |
| Esposa (4)      | 7        |

**Estado:** ✅ Seed ejecutado, datos verificados

#### 3.3. Autenticación Azure

- ✅ Entra ID Admin configurado (Daniel Villamizar)
- ✅ Database User con roles `db_datareader` / `db_datawriter`
- ✅ Autenticación vía Azure CLI (`az account get-access-token`)

### 4. Seeders Automáticos (Development Mode)

**Archivos:**

- `LamaDbContextSeed.cs` - Orquestador de seeding
- `CuotaAsambleaSeeder.cs` - Seed de cuotas
- `MiembroSeeder.cs` - Seed de miembros
- `Program.cs` - Llama a `await context.SeedAsync()` en Development

**Nota:** Actualmente con issue de autenticación local (DefaultAzureCredential intenta Managed Identity).
**Workaround:** Usar scripts SQL manuales con Azure CLI token (método actual funcionando).

## 📋 Próximos Pasos

### Fase 1: Testing End-to-End

1. **Probar endpoint de generación de cartera**

   ```bash
   POST /api/cartera/generar
   {
     "periodo": "2026-01",
     "centroCostoId": "<guid>",
     "tiposAfiliacion": [1, 2] // Full Color + Rockets
   }
   ```

2. **Verificar creación de CuentasPorCobrar**
   - Query: `SELECT * FROM CuentasPorCobrar WHERE Periodo = '2026-01'`
   - Validar que se crearon 25 registros (15 Full Color + 10 Rockets)
   - Validar `ValorEsperadoCOP = 20000` para Enero
   - Validar `ValorEsperadoCOP = 25000` para Febrero

3. **Probar Frontend integrado**
   - Levantar frontend: `npm run dev` (puerto 3000)
   - Levantar backend: `dotnet run` (puerto 7030)
   - Navegar a `/cartera/generar`
   - Generar cartera para 2026-01 y 2026-02
   - Verificar respuestas y datos en DB

### Fase 2: Configuración para Desarrollo Local

**Problema actual:** `DefaultAzureCredential` en `appsettings.Development.json` intenta Managed Identity (Azure Arc) y falla en local.

**Soluciones:**

**Opción A: Usar AzureCliCredential explícitamente**

```csharp
// En InfrastructureServicesConfiguration.cs
services.AddDbContext<LamaDbContext>(options =>
{
    if (env.IsDevelopment())
    {
        var credential = new AzureCliCredential();
        var token = credential.GetToken(
            new TokenRequestContext(new[] { "https://database.windows.net//.default" })
        );

        var connString = configuration.GetConnectionString("DefaultConnection");
        var connBuilder = new SqlConnectionStringBuilder(connString);

        options.UseSqlServer(connString, sqlOptions =>
        {
            sqlOptions.AccessToken(token.Token);
        });
    }
    else
    {
        // Production usa Managed Identity desde App Service
        options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection")
        );
    }
});
```

**Opción B: Connection String con Interactive Auth**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:lamaregionnorte-sql-server.database.windows.net,1433;Initial Catalog=LAMAMedellinContable;Authentication=Active Directory Interactive;Encrypt=True;"
  }
}
```

**⚠️ Nota:** Abre navegador para autenticarse cada vez que corre la app

**Opción C: Mantener scripts SQL manuales** (actual, funcional)

- Pro: Simple, directo, no requiere cambios de código
- Con: Requiere ejecución manual para seeding
- Recomendado para: Desarrollo inicial hasta deployment a Azure

### Fase 3: Deployment y CI/CD

1. **Azure App Service para Backend**
   - Configurar Managed Identity
   - Variable de entorno: `ASPNETCORE_ENVIRONMENT=Production`
   - Connection string usa `Authentication=Active Directory Default` (funciona en Azure)

2. **Azure Static Web Apps para Frontend**
   - Build command: `npm run build`
   - Output folder: `out` o `.next`
   - API routes via backend App Service

3. **GitHub Actions**
   - Workflow para backend: build + deploy a App Service
   - Workflow para frontend: build + deploy a SWA
   - Secrets: Azure credentials, connection strings

### Fase 4: Features Pendientes

- [ ] Endpoint GET para listar CuentasPorCobrar por período
- [ ] Endpoint GET para consultar deuda acumulada por miembro
- [ ] Endpoint POST para registrar pago (afecta CuentaPorCobrar + Banco)
- [ ] Frontend: Vista de cartera generada
- [ ] Frontend: Vista de estado de cuenta por miembro
- [ ] Frontend: Form de registro de pagos

## 🔧 Comandos Útiles

### Git

```bash
# Ver estado actual
git status

# Ver último commit
git log -1 --oneline

# Ver branch actual
git branch --show-current

# Ver commits recientes con archivos cambiados
git log --oneline --stat -5
```

### Backend

```bash
# Build
cd LAMAMedellin
dotnet build

# Run (Development)
cd src/LAMAMedellin.API
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run

# Crear migración
dotnet ef migrations add <NombreMigracion> --project src/LAMAMedellin.Infrastructure --startup-project src/LAMAMedellin.API

# Aplicar migración
dotnet ef database update --project src/LAMAMedellin.Infrastructure --startup-project src/LAMAMedellin.API
```

### Frontend

```bash
cd frontend
npm run dev          # Desarrollo (puerto 3000)
npm run build        # Build producción
npm run lint         # Linter
npm run type-check   # TypeScript check
```

### Azure SQL

```powershell
# Conectar y ejecutar query
$token = (az account get-access-token --resource https://database.windows.net/ --query accessToken -o tsv)
$cn = New-Object System.Data.SqlClient.SqlConnection
$cn.ConnectionString = "Server=tcp:lamaregionnorte-sql-server.database.windows.net,1433;Initial Catalog=LAMAMedellinContable;Encrypt=True;"
$cn.AccessToken = $token
$cn.Open()

# Ejecutar query
$cmd = $cn.CreateCommand()
$cmd.CommandText = "SELECT COUNT(*) FROM Miembros"
$result = $cmd.ExecuteScalar()
Write-Host "Total miembros: $result"
$cn.Close()
```

## 📚 Referencias

- [Clean Architecture en .NET](https://learn.microsoft.com/es-es/dotnet/architecture/modern-web-apps-azure/common-web-application-architectures)
- [Entity Framework Core](https://learn.microsoft.com/es-es/ef/core/)
- [Next.js App Router](https://nextjs.org/docs/app)
- [Azure SQL con Entra ID](https://learn.microsoft.com/es-es/azure/azure-sql/database/authentication-aad-overview)
- [CQRS Pattern con MediatR](https://github.com/jbogard/MediatR/wiki)

---

**Última sincronización:**

- **Backend:** Commit `6790d1c` - Refactoring + EF Core fixes + seeders
- **Scripts SQL:** Commit `b5b3ee0` - Seed de cuotas y miembros
- **Branch remoto:** `origin/docs/spec-v1` actualizado

✅ **Sistema listo para testing end-to-end**
