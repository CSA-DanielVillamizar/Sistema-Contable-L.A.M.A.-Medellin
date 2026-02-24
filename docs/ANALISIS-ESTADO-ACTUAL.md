# Análisis del Estado Actual del Sistema
## Gap Analysis: Implementación vs. Documento Maestro (BRD + SRS + Arquitectura + Backlog + Seguridad + Operación)

**Proyecto:** Sistema Contable Integral – Fundación / Capítulo L.A.M.A. Medellín  
**Fecha del análisis:** 24-Feb-2026  
**Analista:** GitHub Copilot Agent (revisión exhaustiva del código fuente)  
**Versión del Documento Maestro referenciado:** 1.0 (2026-02-20)

---

## 0) ¿De qué se trata el código actual?

El repositorio contiene el inicio de una **plataforma de gestión financiero-contable** para la Fundación / Capítulo L.A.M.A. Medellín, desarrollada con el stack definido en el Documento Maestro:

| Capa | Tecnología | Estado |
|---|---|---|
| Backend API | ASP.NET Core 8 Web API + Clean Architecture | ✅ Scaffolding sólido |
| Frontend | Next.js 16 + TypeScript + App Router | ✅ Scaffolding sólido |
| Base de datos | Azure SQL (EF Core + migraciones) | ✅ Funcionando en Azure |
| Autenticación | Microsoft Entra External ID (MSAL) | ✅ Integrado |
| Estilo | TailwindCSS | ✅ Integrado |
| State/Fetching | TanStack Query (React Query) | ✅ Integrado |
| CQRS | MediatR + FluentValidation | ✅ Integrado |
| Errores | GlobalExceptionHandler → ProblemDetails | ✅ Integrado |
| Soft Delete | Flag `IsDeleted` en `BaseEntity` | ✅ Implementado |
| CI/CD | GitHub Actions (backend + frontend) | ✅ Configurado |

**En pocas palabras:** la arquitectura y los cimientos técnicos están muy bien construidos y alineados con la visión del Documento Maestro. Lo que existe es un **MVP parcial de Phase 1**, con módulos de tesorería básica, miembros y cartera (CxC de cuotas) funcionales, pero sin contabilidad formal, sin CxP, y sin ninguno de los módulos de fases 2-5.

---

## 1) Lo que está implementado ✅

### 1.1 Dominio (Domain Layer)

| Entidad / Artefacto | Descripción | Alineación SRS |
|---|---|---|
| `Banco` | NumeroCuenta, SaldoActual, AplicarIngreso(), AplicarEgreso() | RF-TES-01 parcial |
| `CentroCosto` | Nombre + TipoCentroCosto (Capitulo/Fundacion/Proyecto/Evento) | RF-CFG-03 ✅ |
| `Miembro` | Nombre, Apellidos, Documento, Email, Telefono, TipoAfiliacion, EstadoMiembro | RF-MEM-01 ✅ |
| `Transaccion` | MontoCOP, Fecha, TipoTransaccion, MedioPago, CentroCostoId, BancoId, Descripcion | RF-TES-01 parcial |
| `CuotaAsamblea` | Anio, ValorMensualCOP, MesInicioCobro, ActaSoporte | RF-MEM-02 parcial |
| `CuentaPorCobrar` | MiembroId, Periodo (YYYY-MM), ValorEsperadoCOP, SaldoPendienteCOP, Estado, AplicarAbono() | RF-CXC-01/02 ✅ |
| `TransaccionMultimoneda` (VO) | MonedaOrigen, MontoMonedaOrigen, TasaCambioUsada, FechaTasaCambio, FuenteTasaCambio | RF-FX-01 ✅ |
| `BaseEntity` | Id (Guid), IsDeleted, MarcarComoEliminado() (soft delete) | RNF-AUD parcial |
| Enums | EstadoCuentaPorCobrar, EstadoMiembro, FuenteTasaCambio, MedioPago, TipoAfiliacion, TipoCentroCosto, TipoTransaccion | RF-CFG-02/04 ✅ |

### 1.2 Application Layer (CQRS)

| Feature | Commands | Queries |
|---|---|---|
| Miembros | CreateMiembro, UpdateMiembro, DeleteMiembro (soft) | GetMiembros, GetMiembroById |
| Transacciones | RegistrarIngreso (+ USD), RegistrarEgreso (+ USD) | GetTransacciones, GetCatalogoBancos, GetCatalogoCentrosCosto |
| Cartera / CxC | GenerarObligacionesMensuales, GenerarCarteraMensual, RegistrarPago, RegistrarPagoCartera | GetCarteraPendiente |
| Dashboard | — | GetSaldosBancos, GetResumenCartera |
| Validators | Todos los commands tienen validadores FluentValidation | ✅ |
| Pipeline | ValidationBehavior en MediatR (fail-fast) | ✅ |
| Excepciones | ExcepcionNegocio (dominio) + GlobalExceptionHandler (API) | ✅ |

### 1.3 Infrastructure Layer

| Componente | Estado |
|---|---|
| LamaDbContext (EF Core) | ✅ con soft delete automático |
| Configuraciones EF (Fluent API) | ✅ para todas las entidades actuales |
| Repositorios concretos | ✅ Banco, CentroCosto, Miembro, CuotaAsamblea, CuentaPorCobrar, Transaccion |
| Migraciones | ✅ 3 migraciones aplicadas |
| Seed de desarrollo | ✅ Miembros (37) + CuotasAsamblea (2) + Bancos + CentrosCosto |
| Auth Azure SQL en dev | ✅ ChainedTokenCredential (CLI → DevCLI → PowerShell → Default) |
| Managed Identity (prod) | ✅ DefaultAzureCredential sin secrets en código |

### 1.4 API Layer (Controllers)

| Controller | Endpoints | Auth |
|---|---|---|
| MiembrosController | GET /api/miembros, GET /{id}, POST, PUT /{id}, DELETE /{id} | [Authorize] ✅ |
| TransaccionesController | GET /api/transacciones, GET /bancos, GET /centros-costo, POST /ingreso, POST /egreso | [Authorize] ✅ |
| CarteraController | POST /generar-mensual, GET /pendiente, POST /{id}/pago | [Authorize] ✅ |
| DashboardController | GET /api/dashboard/bancos, GET /api/dashboard/cartera | [Authorize] ✅ |
| WeatherForecastController | GET /WeatherForecast | ❌ Sin auth – template residual |

### 1.5 Frontend (Next.js)

| Módulo | Páginas / Componentes | Estado |
|---|---|---|
| Autenticación | AuthProvider (MSAL), TokenSync, loginRedirect, acquireTokenSilent | ✅ |
| Dashboard | `/` → saldo banco + cartera pendiente + accesos rápidos | ✅ |
| Transacciones | `/transacciones/ingreso`, `/transacciones/egreso`, `/transacciones/listado` | ✅ |
| Multimoneda USD | Checkbox + campos TRM + botón "Cargar TRM oficial" (API route interna) | ✅ RF-FX-01/02 |
| Miembros | `/miembros` (listado), `/miembros/nuevo` (crear/editar) | ✅ |
| Cartera | `/cartera/listado`, `/cartera/generar` | ✅ |
| API Route TRM | `/api/trm/actual` (consulta TRM SFC) | ✅ RF-FX-02 |
| Validación | Zod + React Hook Form en todos los formularios | ✅ |
| Estado servidor | TanStack Query (React Query) | ✅ |

### 1.6 Testing

| Suite | Tests | Stack |
|---|---|---|
| Application.Tests | 3 tests unitarios para RegistrarPagoCuotaCommandHandler | xUnit + Moq + FluentAssertions ✅ |
| API.Tests | 4 tests de integración para CarteraController (WebApplicationFactory + test auth) | xUnit + FluentAssertions ✅ |

### 1.7 CI/CD y Gobernanza

| Artefacto | Estado |
|---|---|
| `.github/workflows/backend-ci.yml` | Build + test .NET 8 en cada PR a main ✅ |
| `.github/workflows/frontend-ci.yml` | Lint + build Next.js en cada PR a main ✅ |
| `.github/CODEOWNERS` | Propietario asignado por área ✅ |
| `.github/settings.yml` | Branch protection declarativa (Probot Settings) ✅ |

---

## 2) Gap Analysis: Lo que falta ❌

### 2.1 Phase 0 — Fundaciones (parcialmente completo)

| Requerimiento | Estado | Descripción del Gap |
|---|---|---|
| RF-IAM-01 | ✅ | Login via Entra External ID funcionando |
| RF-IAM-02 | ✅ | MFA delegado a Entra |
| RF-IAM-03 | ✅ | Sin tabla de contraseñas local |
| **RF-IAM-04** | ❌ | **No existe CRUD de roles internos** (Admin/Operador/Tesorero/Contador/Junta). Solo hay `[Authorize]` genérico. Cualquier usuario autenticado puede hacer cualquier operación. |
| **RF-IAM-05** | ❌ | **Sin auditoría de cambio de roles internos** |
| RF-CFG-01 | ⚠️ | Banco existe como entidad, pero falta nombre descriptivo ("Bancolombia Ahorros") y asociación a cuenta contable PUC |
| RF-CFG-02 | ✅ | MedioPago como enum en dominio |
| RF-CFG-03 | ✅ | CentroCosto CRUD completo |
| RF-CFG-04 | ✅ | TipoAfiliacion en Miembro |
| **RF-CFG-05** | ❌ | **Sin importación de PUC ESAL**. No existe entidad `CuentaContable` ni catálogo de cuentas. Es el gap más crítico para contabilidad formal. |
| **RF-CFG-06** | ❌ | **Sin mapeo contable por operación**. No existe tabla de configuración que relacione tipo de operación ↔ cuentas PUC. |

### 2.2 Phase 1 — MVP Contabilidad + Cuotas

#### Contabilidad General (0% implementado)

| Requerimiento | Estado | Gap |
|---|---|---|
| **RF-CONT-01** | ❌ | Sin PUC ESAL: no hay entidad `CuentaContable`, sin catálogo de cuentas |
| **RF-CONT-02** | ❌ | Sin comprobantes contables formales. `Transaccion` es un movimiento bancario simple, no un comprobante con asientos de doble partida |
| **RF-CONT-03** | ❌ | Sin asientos balanceados (Debe = Haber). Sin entidad `AsientoContable` con líneas de débito/crédito |
| **RF-CONT-04** | ❌ | Sin libros contables (Libro Diario, Libro Mayor, Balance de Prueba) |
| **RF-CONT-05** | ❌ | Sin estados financieros (Balance General, Estado de Resultados) |
| **RF-CONT-06** | ❌ | Sin cierre contable mensual. Sin control de periodos bloqueados |
| **RF-CONT-07** | ❌ | Sin reversos post-cierre |
| **RF-CONT-08** | ❌ | Sin reportes tributarios base (Exógena, Beneficiarios Finales) |

> ⚠️ **Observación crítica:** La entidad `Transaccion` registra el movimiento de banco (débito/crédito de saldo), pero **NO genera asientos contables de doble partida**. Por ejemplo: un ingreso por cuota debería generar simultáneamente:  
> - **Débito** Banco (1105xx) COP 100.000  
> - **Crédito** Ingresos por Cuotas (4105xx) COP 100.000  
> Esto no existe. La contabilidad formal es el núcleo del sistema y está al 0%.

#### Tesorería (parcialmente implementado)

| Requerimiento | Estado | Gap |
|---|---|---|
| RF-TES-01 | ✅ | Movimientos bancarios con soporte, CC, medio de pago |
| **RF-TES-02** | ❌ | Sin recibos PDF + QR. Sin generación de documentos. Sin Azure Blob Storage para adjuntos |
| **RF-TES-03** | ❌ | Sin conciliación bancaria mensual |
| **RF-TES-04** | ❌ | Sin anulación intra-mes (no hay flujo de aprobación del Tesorero, ni campo `Motivo` de anulación, ni bloqueo post-cierre) |

#### CxC / Cuotas (mayormente implementado)

| Requerimiento | Estado | Gap |
|---|---|---|
| RF-CXC-01 | ✅ | Obligaciones mensuales por miembro activo y periodo |
| RF-CXC-02 | ✅ | Pagos con abono (anticipos posibles por la lógica de abono) |
| **RF-CXC-03** | ❌ | Sin cálculo de mora ni aging (sin campo `FechaVencimiento`, sin porcentaje de mora) |
| **RF-CXC-04** | ❌ | Sin CxC de terceros (solo miembros) |
| **RF-MEM-02** | ⚠️ | ActaSoporte en CuotaAsamblea es un string, no un archivo real en Blob Storage |
| **RF-MEM-03** | ❌ | Sin reportes de recaudo/mora/histórico exportables |
| **RF-MEM-04** | ⚠️ | `RegistrarPagoCartera` genera `Transaccion` en banco ✅, pero no genera asiento contable formal (doble partida) |

#### CxP Proveedores (0% implementado)

| Requerimiento | Estado | Gap |
|---|---|---|
| **RF-CXP-01** | ❌ | Sin entidad `FacturaProveedor` / `CuentaPorPagar` |
| **RF-CXP-02** | ❌ | Sin pago cruzando obligación vs. banco |
| **RF-CXP-03** | ❌ | Sin reportes de vencidas/por vencer |

#### Multimoneda USD (mayormente implementado)

| Requerimiento | Estado | Gap |
|---|---|---|
| RF-FX-01 | ✅ | MonedaOrigen, MontoMonedaOrigen, TasaCambioUsada, FechaTasaCambio, FuenteTasaCambio obligatorios cuando es USD |
| RF-FX-02 | ✅ | TRM oficial precargada desde API route Next.js |
| **RF-FX-03** | ❌ | Sin diferencia en cambio automática al liquidar CxP/CxC en USD (lógica de `CA-FX-04` no implementada) |

### 2.3 Phase 2 — Donaciones (0% implementado)

| Requerimiento | Estado | Gap |
|---|---|---|
| **RF-DON-01** | ❌ | Sin entidad `Campaña` |
| **RF-DON-02** | ❌ | Sin entidad `Donante` (persona natural/jurídica) |
| **RF-DON-03** | ❌ | Sin entidad `Donacion` (dinero/especie + soporte) |
| **RF-DON-04** | ❌ | Sin generación de certificado obligatorio (PDF + QR + verificación pública) |
| **RF-DON-05** | ❌ | Sin reportes por campaña/donante/proyecto |
| **RF-DON-06** | ❌ | Sin asiento automático Banco vs. Ingreso Donaciones |

### 2.4 Phase 3 — Proyectos Sociales (0% implementado)

| Requerimiento | Estado | Gap |
|---|---|---|
| **RF-PROY-01** | ❌ | Sin entidad `Proyecto` (presupuesto, cronograma, evidencias) |
| **RF-PROY-02** | ❌ | Sin entidad `Beneficiario` con consentimiento obligatorio para PII (Ley 1581/2012) |
| **RF-PROY-03** | ❌ | Sin indicadores de impacto agregados |
| **RF-PROY-04** | ❌ | Sin imputación de egresos a proyectos |
| **RF-PROY-05** | ❌ | Sin informe de rendición (PDF/Excel) |

> ⚠️ **Riesgo legal:** La Ley 1581/2012 exige gestión de consentimiento explícito para datos personales. Sin esto, el módulo de beneficiarios no se puede lanzar.

### 2.5 Phase 4 — Negocios / Merchandising (0% implementado)

| Requerimiento | Estado | Gap |
|---|---|---|
| **RF-BIZ-01** | ❌ | Sin entidad `Producto` / inventario simple |
| **RF-BIZ-02** | ❌ | Sin compras (entrada de inventario + CxP o pago directo) |
| **RF-BIZ-03** | ❌ | Sin ventas con comprobante interno PDF + QR |
| **RF-BIZ-04** | ❌ | Sin reportes de ventas/inventario/utilidad |

### 2.6 Phase 5 — Reportes Tributarios Avanzados (0% implementado)

| Requerimiento | Estado | Gap |
|---|---|---|
| **RF-CONT-08** | ❌ | Sin estructura de datos para exógena (Formato 1001, 1007, etc.) |
| — | ❌ | Sin exportación de beneficiarios finales |

---

## 3) Requerimientos No Funcionales (RNF)

| RNF | Estado | Observación |
|---|---|---|
| **RNF-SEC: MFA** | ✅ | Delegado 100% a Entra External ID |
| **RNF-SEC: RBAC** | ❌ | Solo `[Authorize]` genérico. Sin verificación de rol interno en ningún endpoint. Un "Junta" puede hacer cierres contables. |
| **RNF-SEC: Hardening** | ⚠️ | CORS configurado. HTTPS redirect activo. Falta eliminar `WeatherForecastController` (endpoint público sin utilidad). |
| **RNF-AUD** | ❌ | `IsDeleted` existe, pero no hay audit trail completo (sin columnas `CreatedAt`, `CreatedBy`, `UpdatedAt`, `UpdatedBy`, sin tabla `AuditLog` para acciones críticas como cambio de cuota, anulaciones, cierres). |
| **RNF-PRIV (Ley 1581)** | ❌ | Datos PII (Documento, Email, Telefono de `Miembro`) se exponen en API sin control de acceso por rol. No hay gestión de consentimiento. |
| **RNF-OPS: Backups** | ⚠️ | Azure SQL tiene backups automáticos. Sin runbook de restore documentado. |
| **RNF-COST** | ✅ | Stack Azure mínimo documentado. Sin lifecycle de Blob (no hay Blob aún). |
| **RNF-OBS** | ❌ | No hay Application Insights configurado en el código. Sin logs estructurados (solo ILogger básico). Sin alertas. |

---

## 4) Deuda Técnica Identificada

| Item | Prioridad | Descripción |
|---|---|---|
| `WeatherForecastController.cs` | 🔴 Alta | Template de scaffolding que nunca se eliminó. Endpoint HTTP sin autenticación (`[Authorize]` faltante). Debe eliminarse. |
| Duplicación RegistrarPago vs RegistrarPagoCartera | 🟡 Media | Hay dos handlers para registrar pago de cuota: `RegistrarPagoCuotaCommandHandler` (usa `GetDefaultAsync`) y `RegistrarPagoCarteraCommandHandler` (usa `GetByIdAsync` + genera `Transaccion`). Uno debería eliminarse. El más completo es `RegistrarPagoCarteraCommandHandler`. |
| Sin validación de duplicados en `CreateMiembro` | 🟡 Media | No se verifica si ya existe un miembro con el mismo `Documento` o `Email` antes de crear. `IMiembroRepository` tiene `GetByDocumentoAsync` / `GetByEmailAsync` pero el handler no los invoca. |
| `Banco.NumeroCuenta` sin nombre descriptivo | 🟡 Media | La entidad `Banco` solo tiene `NumeroCuenta`. Debería tener un campo `Nombre` ("Bancolombia Cuenta Ahorros") para mostrar en UI con sentido. |
| Sin paginación en listados | 🟡 Media | `GetMiembros`, `GetTransacciones`, `GetCarteraPendiente` retornan todos los registros. Con 37 miembros actuales es ok, pero no escala. |
| Sin auditoría temporal (`CreatedAt`, `UpdatedAt`) | 🟡 Media | `BaseEntity` solo tiene `Id` e `IsDeleted`. Los campos de auditoría temporal son estándar en sistemas contables. |
| `layout.tsx` con metadata genérica | 🟢 Baja | Título "Create Next App" en el `<title>` del HTML. Debe actualizarse a "Sistema Contable – L.A.M.A. Medellín". |
| Sin manejo de navegación global (sidebar/nav) | 🟢 Baja | No hay componente de navegación compartido. Cada página es aislada. El dashboard tiene accesos rápidos pero no hay un menú lateral persistente. |

---

## 5) Matriz de Cobertura General por Módulo

| Módulo / Área | Phase | % Estimado | Detalle |
|---|:---:|:---:|---|
| Arquitectura y Fundamentos | 0 | **95%** | Sólida. Solo falta Application Insights. |
| IAM + Roles Internos RBAC | 0 | **40%** | Entra ✅. CRUD roles internos ❌. Enforcement en API ❌. |
| Configuración Base (CC, bancos, medios pago) | 0 | **60%** | CC ✅. Bancos ⚠️. PUC ❌. Mapeo contable ❌. |
| Contabilidad Formal (PUC + comprobantes + libros + cierres) | 1 | **0%** | ❌ No iniciado |
| Tesorería Básica (ingresos/egresos bancarios) | 1 | **60%** | Registro ✅. PDF/QR ❌. Anulaciones ❌. Conciliación ❌. |
| Cuotas Miembros + CxC | 1 | **70%** | Generación ✅. Pagos ✅. Mora/aging ❌. Reportes ❌. |
| CxP Proveedores | 1 | **0%** | ❌ No iniciado |
| Multimoneda USD | 1 | **80%** | Captura ✅. TRM ✅. Diferencia en cambio ❌. |
| Miembros CRUD | 1 | **80%** | CRUD ✅. Validación duplicados ⚠️. Histórico cuotas ❌. |
| Donaciones + Certificados | 2 | **0%** | ❌ No iniciado |
| Proyectos Sociales + Beneficiarios | 3 | **0%** | ❌ No iniciado |
| Negocios / Inventario / Merch | 4 | **0%** | ❌ No iniciado |
| Reportes Tributarios (exógena) | 5 | **0%** | ❌ No iniciado |
| Auditoría de acciones críticas | Transversal | **10%** | Solo soft delete. Sin audit trail completo. |
| Observabilidad (App Insights + logs) | Transversal | **5%** | ILogger básico. Sin instrumentación. |
| Testing | Transversal | **15%** | 7 tests para 2 módulos. Sin cobertura de happy paths de ingreso/egreso. |

**Cobertura global estimada vs. Documento Maestro: ~30%**

---

## 6) Fortalezas del Código Actual

1. **Arquitectura limpia y consistente.** Clean Architecture bien aplicada: Domain no depende de EF Core ni ASP.NET, Application pura, Infrastructure aislada. Los 4 proyectos están correctamente referenciados.

2. **Multimoneda USD bien diseñada.** `TransaccionMultimoneda` como Value Object inmutable con validaciones, campos exactamente según la especificación (RF-FX-01), y API route de TRM integrada en el frontend.

3. **Lógica de cuotas correcta.** `CuotaAsamblea.GetVigentePorPeriodoAsync` implementa correctamente la regla de "vigencia por periodo" de la asamblea anual (RF-MEM-02).

4. **Soft delete global.** `BaseEntity` + `LamaDbContext.AplicarSoftDelete()` asegura que nada se borra físicamente sin código adicional.

5. **Frontend con MSAL bien integrado.** El `AuthProvider` maneja de forma robusta el flujo OIDC: loginRedirect, acquireTokenSilent, retry, error handling, y notificación a componentes via eventos custom.

6. **Validaciones FluentValidation en pipeline.** Cada command tiene su validator, y el `ValidationBehavior` en MediatR asegura fail-fast antes de ejecutar lógica de negocio.

7. **Seguridad de base robusta.** Sin credenciales en código, Managed Identity en producción, JWT con Entra en API, CORS restringido por origen.

---

## 7) Riesgos y Recomendaciones Prioritarias

### 🔴 Crítico — Bloquean el lanzamiento

| # | Riesgo | Acción recomendada |
|---|---|---|
| 1 | **Sin contabilidad formal.** El sistema no puede producir libros ni estados financieros para la DIAN. | Implementar `CuentaContable` (PUC ESAL), `Comprobante` y `AsientoContable` antes de cualquier fase 2+. |
| 2 | **Sin RBAC interno.** Cualquier usuario autenticado con Entra puede ejecutar cierres, anular transacciones, modificar cuotas. Viola la segregación de funciones del Documento Maestro (Sección 3). | Crear tabla `UsuarioRol` + middleware/policy de autorización por rol en cada endpoint. |
| 3 | **Sin cierre mensual.** Sin bloqueo de periodos, cualquier registro puede modificarse retroactivamente, comprometiendo la integridad contable. | Implementar `PeriodoContable` con estado (Abierto/Cerrado/Bloqueado). |

### 🟡 Importante — Afectan funcionalidad core

| # | Riesgo | Acción recomendada |
|---|---|---|
| 4 | Sin recibos/comprobantes PDF. Donantes, miembros y proveedores necesitan documento de soporte. | Integrar QuestPDF o iText en backend + Azure Blob Storage para almacenamiento. |
| 5 | Sin auditoría de acciones críticas. Cambios en cuotas, anulaciones y cierres deben quedar trazados. | Agregar `CreatedAt/By`, `UpdatedAt/By` en `BaseEntity` + tabla `AuditLog` para eventos críticos. |
| 6 | Sin paginación. Con crecimiento de miembros y transacciones, los listados se volverán lentos. | Implementar paginación cursor-based o offset en los queries de listado. |
| 7 | `WeatherForecastController` expuesto. Endpoint sin propósito en producción. | Eliminar `WeatherForecastController.cs` y `WeatherForecast.cs`. |

### 🟢 Mejoras menores

| # | Mejora |
|---|---|
| 8 | Agregar campo `Nombre` a entidad `Banco` para mayor claridad en UI. |
| 9 | Validar duplicados de `Documento` y `Email` en `CreateMiembroCommandHandler`. |
| 10 | Actualizar metadata de `layout.tsx` (título de la app). |
| 11 | Configurar Application Insights + logs estructurados (Serilog o Microsoft.Extensions.Logging estructurado). |
| 12 | Agregar navegación global (sidebar) al frontend. |
| 13 | Ampliar suite de tests: casos felices de RegistrarIngreso/Egreso, validaciones de dominio (CuentaPorCobrar, Transaccion). |

---

## 8) Roadmap Sugerido (próximos pasos en orden lógico)

```
Phase 0 (completar) ──────────────────────────────────────────────────────────
  Sprint 1:
    ✅ (Ya hecho) CI/CD + branch protection
    [ ] Eliminar WeatherForecastController
    [ ] Agregar CreatedAt/UpdatedAt a BaseEntity
    [ ] CRUD UsuarioRol + tabla en DB + policies de autorización
    [ ] Agregar nombre a entidad Banco
    [ ] Corregir duplicados en CreateMiembro

  Sprint 2:
    [ ] Importar PUC ESAL → entidad CuentaContable (padre/hijo, naturaleza, tipo)
    [ ] Mapeo contable configurable por tipo de operación
    [ ] PeriodoContable (Abierto/Cerrado/Bloqueado)

Phase 1 (completar core contable) ────────────────────────────────────────────
  Sprint 3:
    [ ] Entidad Comprobante + AsientoContable (numeración, debe=haber, CC obligatorio)
    [ ] Integrar asiento automático en RegistrarIngreso/Egreso y RegistrarPagoCartera
    [ ] Cierre mensual: validar Tesorero → ejecutar Contador → bloquear periodo

  Sprint 4:
    [ ] Libros contables (Diario, Mayor) — queries sobre AsientoContable
    [ ] Balance de prueba, BG y ER básicos
    [ ] Anulación intra-mes con aprobación + motivo

  Sprint 5:
    [ ] Entidad CuentaPorPagar (CxP proveedores)
    [ ] Flujo: factura → pago cruzado vs obligación → asiento automático
    [ ] Diferencia en cambio automática (RF-FX-03) en CxP/CxC USD
    [ ] Conciliación bancaria básica

  Sprint 6:
    [ ] Generación de recibos PDF + QR (QuestPDF)
    [ ] Azure Blob Storage para soportes
    [ ] Audit trail completo (AuditLog)
    [ ] Application Insights + logs estructurados

Phase 2 ──────────────────────────────────────────────────────────────────────
  Sprint 7-8:
    [ ] Donaciones (Campaña, Donante, Donacion, Certificado PDF+QR)

Phase 3 ──────────────────────────────────────────────────────────────────────
  Sprint 9-10:
    [ ] Proyectos sociales + Beneficiarios + Consentimiento (Ley 1581/2012)

Phase 4 ──────────────────────────────────────────────────────────────────────
  Sprint 11:
    [ ] Negocios / Inventario / Merchandising

Phase 5 ──────────────────────────────────────────────────────────────────────
  Sprint 12:
    [ ] Reportes tributarios base (Exógena, Beneficiarios Finales)
```

---

## 9) Resumen Ejecutivo

| Dimensión | Calificación | Comentario |
|---|:---:|---|
| Calidad arquitectónica | ⭐⭐⭐⭐⭐ | Clean Architecture, CQRS, validaciones, soft delete, Managed Identity: todo correcto. |
| Cobertura funcional | ⭐⭐ | ~30% del Documento Maestro. Falta contabilidad formal, CxP, donaciones, proyectos. |
| Seguridad operativa | ⭐⭐⭐ | Base sólida (Entra, JWT, CORS), pero sin RBAC interno ni auditoría. |
| Calidad de código | ⭐⭐⭐⭐ | Consistente, limpio, en español de negocio. Deuda técnica menor (template WeatherForecast). |
| Testing | ⭐⭐ | Solo 7 tests para 2 módulos. Cobertura muy baja. |
| Documentación | ⭐⭐⭐⭐ | Documento Maestro excelente. Docs en `/docs` y raíz útiles. |
| Listo para producción | ❌ | No. Falta RBAC, contabilidad formal, cierres y recibos como mínimo para MVP productivo. |

**El sistema tiene un excelente punto de partida arquitectónico y ya es utilizable como herramienta interna básica de registro de movimientos y gestión de miembros. Sin embargo, para cumplir con el Documento Maestro y operar legalmente como sistema contable formal de la Fundación, el trabajo restante más importante es la implementación del motor contable (PUC + comprobantes + asientos + cierres) y el sistema de RBAC interno.**
