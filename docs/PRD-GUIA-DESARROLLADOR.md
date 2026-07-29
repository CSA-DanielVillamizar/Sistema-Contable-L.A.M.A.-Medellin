# Guía Integral del Desarrollador — Sistema Contable L.A.M.A. Medellín

> **Versión:** 2.0 | **Fecha:** 2026-07-28  
> **Audiencia:** Cualquier desarrollador que se incorpore al proyecto  
> **Propósito:** Entender de cero qué es este sistema, por qué existe, qué hace, cómo está construido y cuáles son sus reglas inamovibles.

---

## Tabla de Contenidos

1. [¿Qué es este sistema?](#1-qué-es-este-sistema)
2. [Contexto institucional](#2-contexto-institucional)
3. [El problema que resuelve](#3-el-problema-que-resuelve)
4. [Objetivos del sistema](#4-objetivos-del-sistema)
5. [Alcance — qué incluye y qué no](#5-alcance--qué-incluye-y-qué-no)
6. [Arquitectura técnica](#6-arquitectura-técnica)
7. [Estructura del repositorio](#7-estructura-del-repositorio)
8. [Módulos del sistema y estado actual](#8-módulos-del-sistema-y-estado-actual)
9. [Requerimientos funcionales (RF)](#9-requerimientos-funcionales-rf)
10. [Requerimientos no funcionales (RNF)](#10-requerimientos-no-funcionales-rnf)
11. [Historias de usuario completas](#11-historias-de-usuario-completas)
12. [Reglas de negocio inamovibles](#12-reglas-de-negocio-inamovibles)
13. [Modelo de datos clave](#13-modelo-de-datos-clave)
14. [Roles, RBAC y matriz de permisos](#14-roles-rbac-y-matriz-de-permisos)
15. [Seguridad y autenticación](#15-seguridad-y-autenticación)
16. [Multimoneda — USD informativo](#16-multimoneda--usd-informativo)
17. [Roadmap por fases](#17-roadmap-por-fases)
18. [Convenciones de código](#18-convenciones-de-código)
19. [Ejecución local](#19-ejecución-local)
20. [Glosario](#20-glosario)

---

## 1. ¿Qué es este sistema?

**Sistema Contable L.A.M.A. Medellín** es una plataforma integral web de gestión operativa, financiera y administrativa, construida exclusivamente para el **Capítulo L.A.M.A. Medellín** y su figura legal, la **Fundación L.A.M.A. Medellín**.

El sistema centraliza en un único software todo lo que antes se manejaba en hojas de cálculo dispersas, mensajes de WhatsApp y soportes físicos:

- Contabilidad formal bajo el PUC para Entidades Sin Ánimo de Lucro (ESAL).
- Tesorería bancaria 100% trazable.
- Cobro y seguimiento de cuotas de miembros (cartera).
- Cuentas por pagar a proveedores.
- Registro y certificación de donaciones.
- Gestión de proyectos sociales con beneficiarios y cumplimiento de Ley 1581/2012.
- Ventas de merchandising con inventario simple.
- Reportes tributarios base (información exógena DIAN, Registro Único de Beneficiarios).

**Stack tecnológico:**

| Capa | Tecnología |
|------|-----------|
| Frontend | Next.js 14 (App Router) + TypeScript + TailwindCSS |
| Backend | ASP.NET Core 8 Web API (Clean Architecture + CQRS) |
| Base de datos | Azure SQL Database |
| Almacenamiento de archivos | Azure Blob Storage |
| Autenticación | Microsoft Entra External ID (OIDC + MFA) |
| Secretos | Azure Key Vault + Managed Identity |
| Observabilidad | Application Insights + Log Analytics |

---

## 2. Contexto institucional

### 2.1 Dos identidades, una persona jurídica

| Identidad | Uso |
|-----------|-----|
| **Capítulo L.A.M.A. Medellín** | Identidad mototurística. Se usa en eventos, parches, uniformes, comunicación interna y pública del club. |
| **Fundación L.A.M.A. Medellín** | Figura legal. Se usa en contratos, cuentas bancarias, documentos DIAN, proyectos sociales y cualquier acto con terceros. |

El sistema opera legalmente bajo la **Fundación** pero separa internamente por **Centros de Costo** (CAPITULO, FUNDACION, PROYECTO:\<nombre\>, EVENTO:\<nombre\>).

### 2.2 ¿Qué es L.A.M.A.?

L.A.M.A. (siglas de su nombre internacional) es una organización mototurística. El capítulo de Medellín es uno de los capítulos locales y tiene obligaciones financieras tanto internas (cuotas de miembros, eventos) como externas (aportes a L.A.M.A. Internacional, proyectos sociales ante la comunidad).

### 2.3 Obligaciones que motivan el sistema

- Reportar correctamente ante la **DIAN** (entidad tributaria colombiana).
- Cumplir la **Ley 1581/2012** de protección de datos personales (Habeas Data) para proyectos con beneficiarios.
- Mantener **libros contables** formales exigibles ante cualquier ente de control.
- Garantizar **trazabilidad total** de fondos (bancarización obligatoria).

---

## 3. El problema que resuelve

Antes de este sistema, la organización enfrentaba:

| Problema | Impacto |
|---------|---------|
| Gestión en Excel sin control de versiones | Alta probabilidad de errores y pérdida de datos |
| Soportes físicos o en mensajes de WhatsApp | Imposibilidad de auditoría confiable |
| Sin contabilidad formal | Riesgo tributario ante DIAN |
| Sin certificados de donación | Incumplimiento del requisito legal para donantes |
| Sin registro de beneficiarios con consentimiento | Riesgo legal por Ley 1581/2012 |
| Control de cuotas manual | Mora no detectada, conflictos con miembros |
| Sin cierre contable | Imposibilidad de estados financieros confiables |

---

## 4. Objetivos del sistema

1. **Contabilidad formal completa:** PUC ESAL, comprobantes numerados, libros contables, cierres mensuales bloqueantes.
2. **Bancarización 100%:** toda transacción impacta la cuenta Bancolombia; cero caja física operativa.
3. **Cuotas de miembros trazables:** reglas de asamblea anual, histórico de valores por período, mora y cartera (CxC).
4. **Control de proveedores:** cuentas por pagar (CxP) con vencimientos y cruce contra banco.
5. **Donaciones certificadas:** certificado PDF + QR de verificación pública, obligatorio por ley para ESAL.
6. **Proyectos sociales con evidencia:** beneficiarios, consentimiento Habeas Data, indicadores y rendición de cuentas.
7. **Gerencia de negocios (merch):** inventario simple, compras y ventas con comprobante interno.
8. **Control interno robusto:** segregación de funciones, auditoría de acciones críticas, anulaciones controladas.

---

## 5. Alcance — qué incluye y qué no

### 5.1 Incluido

- Tesorería bancaria (Bancolombia, cuenta única inicial, diseño multi-cuenta)
- Contabilidad general bajo PUC ESAL
- Cuotas de miembros + Cuentas por Cobrar (CxC)
- Cuentas por Pagar (CxP) a proveedores
- Donaciones + campañas + certificados obligatorios
- Proyectos sociales + beneficiarios + consentimiento + indicadores + rendición
- Merchandising: inventario simple + ventas con comprobante interno
- Multimoneda informativa (USD → COP funcional)
- Reportes base tributarios exportables (exógena, RUB)
- Auditoría de cambios en entidades críticas

### 5.2 Excluido (fase inicial, preparado para el futuro)

| Característica | Estado |
|---------------|--------|
| Nómina | Fuera de alcance (sin fecha) |
| Facturación electrónica DIAN | No implementado; estructura adapter preparada |
| QR / canales digitales de pago | Planificado como futuro medio de pago |
| Múltiples cuentas bancarias | Diseño preparado; inicialmente 1 activa |

---

## 6. Arquitectura técnica

### 6.1 Diagrama de componentes

```
┌──────────────────────────────────────────────────────────────────┐
│                        AZURE CLOUD                               │
│                                                                  │
│  ┌─────────────────┐    ┌──────────────────┐                    │
│  │  Next.js 14      │    │  ASP.NET Core 8  │                    │
│  │  (App Router)    │───▶│  Web API         │                    │
│  │  Azure SWA /     │    │  Azure App       │                    │
│  │  App Service     │    │  Service (Linux) │                    │
│  └─────────────────┘    └────────┬─────────┘                    │
│                                  │                               │
│           ┌──────────────────────┼──────────────────┐           │
│           │                      │                  │           │
│  ┌────────▼───────┐  ┌──────────▼───────┐  ┌──────▼────────┐  │
│  │ Azure SQL DB   │  │ Azure Blob       │  │ Azure Key     │  │
│  │ (datos)        │  │ Storage          │  │ Vault         │  │
│  │                │  │ (soportes/docs)  │  │ (secretos)    │  │
│  └────────────────┘  └──────────────────┘  └───────────────┘  │
│                                                                  │
│  ┌──────────────────┐  ┌──────────────────────────────────────┐ │
│  │ Entra External ID│  │ Application Insights + Log Analytics │ │
│  │ (Auth + MFA)     │  │ (observabilidad)                     │ │
│  └──────────────────┘  └──────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────────────┘
```

### 6.2 Backend — Clean Architecture

```
LAMAMedellin/
├── Domain/          → Entidades, value objects, interfaces de repositorios.
│                      Cero dependencias externas (pura lógica de negocio).
├── Application/     → Casos de uso (CQRS: Commands + Queries).
│                      MediatR + FluentValidation Behaviors. Sin EF Core.
├── Infrastructure/  → EF Core, repositorios concretos, servicios Azure
│                      (Blob, Key Vault, etc.), migraciones.
└── API/             → Minimal APIs, middlewares, configuración DI.
```

**Flujo de una petición:**

```
HTTP Request
    → API (Minimal API endpoint)
    → MediatR.Send(Command/Query)
    → ValidationBehavior (FluentValidation — falla rápido)
    → CommandHandler / QueryHandler (Application)
    → IRepository (Domain interface)
    → Repository (Infrastructure / EF Core)
    → Azure SQL
```

**Manejo de errores:**

Todos los errores no controlados son capturados por un `GlobalExceptionMiddleware` que retorna siempre `ProblemDetails` (RFC 7807) con formato estandarizado.

### 6.3 Frontend — Next.js 14 App Router

- **Server Components** por defecto (SSR, sin JS en cliente innecesario).
- `'use client'` solo cuando se necesita interactividad (formularios, hooks).
- **TanStack Query** (React Query) para fetching y caché del estado del servidor.
- **TailwindCSS** para estilos.
- Autenticación via MSAL/NextAuth con proveedor OIDC de Entra External ID.

### 6.4 Seguridad — principios clave

- **Sin contraseñas locales:** autenticación 100% delegada a Entra External ID.
- **Managed Identity:** la API accede a SQL, Blob y Key Vault sin credenciales en código.
- **Soft Delete:** las entidades financieras nunca se eliminan físicamente.
- **Auditoría:** acciones críticas (anulaciones, cierres, cambio de cuotas, consentimientos) quedan registradas.

---

## 7. Estructura del repositorio

```
Sistema-Contable-L.A.M.A.-Medellin/
│
├── LAMAMedellin/                    # Backend .NET 8
│   ├── src/
│   │   ├── LAMAMedellin.Domain/     # Entidades puras, contratos, value objects
│   │   ├── LAMAMedellin.Application/# CQRS: Commands, Queries, Handlers, Validators
│   │   ├── LAMAMedellin.Infrastructure/ # EF Core, repos, servicios Azure, migraciones
│   │   └── LAMAMedellin.API/        # Minimal APIs, DI, middlewares, configuración
│   └── tests/
│       ├── LAMAMedellin.API.Tests/
│       └── LAMAMedellin.Application.Tests/
│
├── frontend/                        # Next.js 14 (TypeScript)
│   └── src/
│       ├── app/                     # Páginas y rutas (App Router)
│       └── components/              # Componentes reutilizables
│
├── docs/                            # Documentación del proyecto
│   ├── PRD-GUIA-DESARROLLADOR.md   # Este documento
│   ├── BRD-SRS_Version2.md         # Documento maestro de requerimientos
│   ├── ARCHITECTURE-AZURE.md       # Arquitectura y configuración Azure
│   ├── BACKLOG.md                  # Épicas e historias del backlog
│   ├── BACKEND-SETUP.md            # Configuración del backend
│   ├── DESPLIEGUE_AZURE.md         # Guía de despliegue en Azure
│   ├── ESPECIFICACION_PUC_ESAL.md  # Catálogo de cuentas ESAL
│   ├── IMPLEMENTATION-STATUS.md    # Estado de implementación por fase
│   └── MANUAL_USUARIO.md           # Manual de uso del sistema
│
├── backlog/                         # Issues de GitHub (épicas e historias)
│   ├── issue-epic-01.md .. issue-epic-13.md
│   └── issue-story-*.md
│
├── governance/                      # Estándares de código y proceso
├── scripts/
│   └── sql/                        # Migraciones SQL idempotentes
└── .github/
    └── copilot-instructions.md     # Instrucciones para el agente de IA
```

---

## 8. Módulos del sistema y estado actual

| # | Módulo | Descripción | Fase | Estado |
|---|--------|-------------|------|--------|
| 1 | **IAM** | Login Entra External ID + MFA + RBAC (5 roles) | Phase 0 | ✅ Completo |
| 2 | **Infra Azure** | Key Vault, Blob Storage, App Insights | Phase 0 | ✅ Completo |
| 3 | **Configuración base** | Centros de costo, medios de pago, PUC, mapeo contable | Phase 0 | ✅ Completo |
| 4 | **Contabilidad** | PUC ESAL, comprobantes, libros, cierres mensuales | Phase 1 | ✅ Completo |
| 5 | **Tesorería** | Movimientos bancarios, recibos PDF+QR, anulaciones | Phase 1 | ✅ Completo |
| 6 | **Cuotas / Cartera (CxC)** | Cuotas asamblea, obligaciones mensuales, mora | Phase 1 | ✅ Completo |
| 7 | **CxP Proveedores** | Facturas por pagar, vencimientos, pago cruzado | Phase 1 | ✅ Completo |
| 8 | **Multimoneda USD** | USD informativo, diferencia en cambio, TRM | Phase 1 | ✅ Completo |
| 9 | **Donaciones** | Campañas, certificados PDF+QR, asiento automático | Phase 2 | ✅ Completo |
| 10 | **Proyectos Sociales** | Beneficiarios, consentimiento, indicadores, rendición | Phase 3 | ✅ Completo |
| 11 | **Negocios (Merch)** | Inventario, compras, ventas, comprobante interno | Phase 4 | ✅ Completo |
| 12 | **Tributario avanzado** | Exógena DIAN, RUB (beneficiarios finales), auditoría | Phase 5 | ✅ Completo |
| 13 | **Facturación Electrónica** | Adapter/estructura preparada — NO implementar | Phase X | 🔲 Pendiente futuro |

> **Nota:** Al 28 de julio de 2026, el sistema se encuentra **funcionalmente completo** en todas sus fases comprometidas (Phase 0–5). Las actividades actuales corresponden a mantenimiento, refinamiento y preparación de nuevas épicas.

---

## 9. Requerimientos funcionales (RF)

### 9.1 IAM — Identidad y acceso

| ID | Requerimiento |
|----|--------------|
| RF-IAM-01 | Login con email/password gestionado por Entra External ID (OIDC). |
| RF-IAM-02 | MFA configurable en Entra (no en la app). |
| RF-IAM-03 | La aplicación NO almacena contraseñas ni hashes localmente en ninguna circunstancia. |
| RF-IAM-04 | Roles internos: Admin, Operador, Tesorero, Contador, Junta. Asignados en BD, no en Entra. |
| RF-IAM-05 | Auditoría de toda asignación o cambio de rol interno (quién, cuándo, desde qué rol anterior). |

**Criterios de aceptación IAM:**
- Login redirige a Entra y retorna JWT válido con claims del usuario.
- La API valida tokens OIDC (issuer + audience de Entra).
- No existe tabla de passwords en ningún esquema de la BD.
- Accesos bloqueados por rol según matriz de permisos (sección 14).

---

### 9.2 Configuración base

| ID | Requerimiento |
|----|--------------|
| RF-CFG-01 | Parametrizar cuenta bancaria Bancolombia con su cuenta contable PUC asociada. |
| RF-CFG-02 | Catálogo de medios de pago (transferencia bancaria, consignación en efectivo, corresponsal bancario, QR/digital). Obligatorio en todo ingreso/egreso. |
| RF-CFG-03 | CRUD de Centros de Costo: CAPITULO, FUNDACION, PROYECTO:\<nombre\>, EVENTO:\<nombre\>. |
| RF-CFG-04 | Tipos de afiliación de miembros y su centro de costo por defecto. |
| RF-CFG-05 | Importación del PUC ESAL desde Excel/CSV entregado por el contador, con validaciones (código único, jerarquía). |
| RF-CFG-06 | Mapeo contable configurable por tipo de operación (ej: débito y crédito para "pago de cuota", "donación", "venta merch", "diferencia en cambio"). |

---

### 9.3 Multimoneda informativa (USD)

| ID | Requerimiento |
|----|--------------|
| RF-FX-01 | Permitir `MonedaOrigen` (COP / USD) en cualquier transacción monetaria. Si es USD, exigir: monto USD, monto COP, tasa, fecha tasa, fuente y soporte adjunto. |
| RF-FX-02 | Precargar TRM (SFC) como ayuda de captura. Guardar evidencia de la tasa usada. |
| RF-FX-03 | En liquidación de CxP/CxC con moneda origen USD, registrar automáticamente asiento de diferencia en cambio si el COP difiere del valor inicial. |

---

### 9.4 Contabilidad general

| ID | Requerimiento |
|----|--------------|
| RF-CONT-01 | Importar y gestionar PUC ESAL (activar/inactivar cuentas, marcar movimiento/no movimiento). |
| RF-CONT-02 | Comprobantes contables (tipos: ingreso, egreso, diario, ajuste, cierre) con numeración consecutiva por tipo y período. |
| RF-CONT-03 | Asientos balanceados (suma débitos = suma créditos). Centro de costo obligatorio en cada línea. Tercero cuando aplique. Soporte adjunto obligatorio en comprobantes de ingreso/egreso. |
| RF-CONT-04 | Libros: Diario, Mayor, Balance de Prueba. |
| RF-CONT-05 | Estados financieros: Balance General (BG) y Estado de Resultados (ER), por CC y consolidado. |
| RF-CONT-06 | Cierre mensual: el Tesorero valida, el Contador ejecuta. Al cerrar, el período queda bloqueado (no se puede editar ningún comprobante del período). |
| RF-CONT-07 | Ajustes post-cierre solo mediante comprobante de ajuste en el período actual (nunca editar el comprobante original). |
| RF-CONT-08 | Reportes base tributarios exportables: Exógena DIAN y Registro Único de Beneficiarios (RUB). |

---

### 9.5 Tesorería bancaria

| ID | Requerimiento |
|----|--------------|
| RF-TES-01 | Registro de movimientos bancarios: fecha, valor, medio de pago, centro de costo, concepto y soporte adjunto (Blob). |
| RF-TES-02 | Generación de recibos PDF + código QR de verificación pública. |
| RF-TES-03 | Conciliación bancaria mensual (manual, con resumen de diferencias). |
| RF-TES-04 | Anulación de movimiento: solo dentro del mismo período contable, requiere aprobación del Tesorero y motivo obligatorio. |

---

### 9.6 Cuotas de miembros / CxC

| ID | Requerimiento |
|----|--------------|
| RF-CXC-01 | Registro de cuota anual aprobada en asamblea: valor, año, mes de inicio de cobro, acta soporte. |
| RF-CXC-02 | Generación de obligaciones mensuales (CuentasPorCobrar) para todos los miembros activos del tipo de afiliación seleccionado en un período dado. |
| RF-CXC-03 | Registro de pago bancario y aplicación contra obligaciones (permite anticipos y pago parcial). |
| RF-CXC-04 | Reporte de mora y aging (antigüedad de cartera) por miembro. |
| RF-CXC-05 | Asiento contable automático al pagar: débito Banco / crédito Ingresos por Cuotas. |

---

### 9.7 CxP Proveedores

| ID | Requerimiento |
|----|--------------|
| RF-CXP-01 | Registro de factura por pagar: proveedor, valor, fecha vencimiento, concepto, CC y soporte adjunto. |
| RF-CXP-02 | Registro de pago posterior: cruza la obligación contra el banco (asiento automático). |
| RF-CXP-03 | Reporte de cuentas vencidas y por vencer. |

---

### 9.8 Donaciones

| ID | Requerimiento |
|----|--------------|
| RF-DON-01 | CRUD de campañas de donación: nombre, meta, vigencia, estado, CC y proyecto asociado. |
| RF-DON-02 | Registro de donantes (persona natural o jurídica). |
| RF-DON-03 | Registro de donación: dinero o especie, con soporte adjunto. |
| RF-DON-04 | Certificado de donación obligatorio: generado automáticamente en PDF con código QR para verificación pública. Sin certificado, la donación no puede quedar en estado "confirmada". |
| RF-DON-05 | Reportes por campaña, por donante, por proyecto. |
| RF-DON-06 | Asiento contable automático: débito Banco / crédito Ingresos por Donaciones. |

---

### 9.9 Proyectos sociales

| ID | Requerimiento |
|----|--------------|
| RF-PROY-01 | CRUD de proyectos sociales: nombre, objetivo, presupuesto, cronograma, estado, CC. |
| RF-PROY-02 | Registro de beneficiarios: si se capturan datos PII (nombre, documento, teléfono, etc.), el consentimiento Habeas Data es **obligatorio**. Sin consentimiento, solo se permite código anónimo. |
| RF-PROY-03 | Indicadores agregados por proyecto (cuantitativos y cualitativos). |
| RF-PROY-04 | Imputación de egresos al proyecto (vincula el comprobante de egreso al proyecto). |
| RF-PROY-05 | Informe de rendición de cuentas exportable (PDF / Excel). |

---

### 9.10 Negocios — Merchandising

| ID | Requerimiento |
|----|--------------|
| RF-BIZ-01 | Catálogo de artículos con stock y valor unitario de adquisición. |
| RF-BIZ-02 | Registro de compra (entrada de inventario): con CxP o pago directo al banco. |
| RF-BIZ-03 | Registro de venta (salida de inventario): genera comprobante interno PDF + QR. |
| RF-BIZ-04 | Reportes: valorización de inventario, ventas, utilidad neta simple. |

---

## 10. Requerimientos no funcionales (RNF)

| ID | Categoría | Requerimiento |
|----|-----------|--------------|
| RNF-SEC-01 | Seguridad | MFA habilitado mediante política de Entra. |
| RNF-SEC-02 | Seguridad | RBAC aplicado en API (atributos de autorización) y en UI (guardas de ruta). |
| RNF-SEC-03 | Seguridad | Zero secrets en código o repositorio. Solo Azure Key Vault vía Managed Identity. |
| RNF-AUD-01 | Auditoría | Registro de auditoría en: cambio de cuota asamblea, anulaciones, cierres contables, asignación de roles, consentimientos. |
| RNF-AUD-02 | Auditoría | Los registros de auditoría son inmutables (append-only). |
| RNF-PRIV-01 | Privacidad | Cumplimiento Ley 1581/2012: minimización de datos PII, control de acceso por rol, trazabilidad de cambios en datos de beneficiarios. |
| RNF-OPS-01 | Operación | Backups automáticos de Azure SQL con retención configurable. |
| RNF-OPS-02 | Operación | Runbook de restore documentado y probado. |
| RNF-COST-01 | Costos | Arquitectura Azure optimizada: SQL Serverless o S0, Blob lifecycle, App Service plan bajo con autoscale. |
| RNF-OBS-01 | Observabilidad | Application Insights integrado con logs estructurados. |
| RNF-OBS-02 | Observabilidad | Alertas básicas configuradas (errores 5xx, latencia elevada, fallos de autenticación). |
| RNF-SOFT-01 | Integridad | Las entidades financieras (comprobantes, cuotas, donaciones, CxC, CxP, ventas) **nunca se eliminan físicamente**. Soft-delete con `IsDeleted` o `Anulado`. |

---

## 11. Historias de usuario completas

Las historias están organizadas por fase y épica. Para cada una se indica el actor, la acción deseada, el beneficio y los criterios de aceptación clave.

---

### PHASE 0 — Fundaciones e IAM

#### Épica 1: IAM Entra External ID + Roles internos

**Historia IAM-01 — Login con Entra External ID**
```
Como: Cualquier usuario del sistema
Quiero: Iniciar sesión con mi email y contraseña (gestionados por Entra) con MFA
Para: No almacenar credenciales localmente y cumplir con seguridad corporativa

Criterios de aceptación:
✅ El login redirige al flujo OIDC de Entra External ID
✅ Tras autenticación exitosa, la API recibe y valida el JWT de Entra
✅ MFA se activa según la política configurada en Entra
✅ No existe ninguna contraseña en la base de datos local
✅ Sesión expira correctamente y requiere re-autenticación

Auditoría: Sí — registrar accesos fallidos y exitosos
```

**Historia IAM-02 — CRUD de roles internos**
```
Como: Admin
Quiero: Asignar y modificar roles internos (Admin/Operador/Tesorero/Contador/Junta) a usuarios
Para: Controlar qué puede hacer cada persona en el sistema

Criterios de aceptación:
✅ Solo el Admin puede asignar/revocar roles
✅ Un usuario puede tener máximo un rol activo
✅ Todo cambio de rol queda en bitácora de auditoría (quién, cuándo, rol anterior, rol nuevo)
✅ Los guardas de autorización en API y UI respetan el rol asignado

Auditoría: Sí (obligatoria)
```

---

#### Épica 2: Infra mínima Azure

**Historia INF-01 — Configuración Key Vault + Managed Identity**
```
Como: Equipo de DevOps/Backend
Quiero: Que todos los secretos (connection strings, claves API) se lean desde Azure Key Vault
Para: No tener ninguna credencial en el código o en el repositorio

Criterios de aceptación:
✅ appsettings.json no contiene ningún secreto real
✅ La API accede a Key Vault usando Managed Identity (System Assigned)
✅ Los secretos se inyectan como IOptions<T> en la aplicación
✅ En Development, se usa AzureCliCredential; en Production, DefaultAzureCredential
```

**Historia INF-02 — Blob Storage para soportes**
```
Como: Operador / Tesorero
Quiero: Adjuntar archivos (soporte de pago, facturas, actas) a las transacciones
Para: Tener evidencia digital auditablealmacenada en la nube

Criterios de aceptación:
✅ Los archivos se suben a Azure Blob Storage, no a disco local
✅ El acceso a los archivos está controlado (URLs temporales con SAS token o acceso vía API)
✅ Existe política de lifecycle (hot → cool → archive) para control de costos
✅ Soft delete habilitado en Blob Storage
```

---

#### Épica 3: Modelo base — Configuración

**Historia CFG-01 — Importar PUC ESAL**
```
Como: Contador
Quiero: Importar el catálogo de cuentas PUC ESAL desde un archivo Excel/CSV
Para: Arrancar la contabilidad formal con la base oficial

Criterios de aceptación:
✅ Importación valida: código único, jerarquía de cuentas (grupo → cuenta → subcuenta → auxiliar)
✅ Se puede marcar cada cuenta como "de movimiento" o "de agrupación"
✅ Se pueden activar/inactivar cuentas individualmente
✅ Bitácora de importación: quién importó, cuándo y desde qué archivo

Auditoría: Sí
```

**Historia CFG-02 — Centros de costo**
```
Como: Admin
Quiero: Crear y gestionar centros de costo (CAPITULO, FUNDACION, PROYECTO:X, EVENTO:X)
Para: Segmentar la información financiera por área operativa

Criterios de aceptación:
✅ CRUD completo de centros de costo
✅ CC obligatorio en toda transacción financiera (validación en backend)
✅ Asociación de proyectos y eventos a su CC correspondiente
✅ Reporte de movimientos por CC
```

**Historia CFG-03 — Mapeo contable por operación**
```
Como: Contador
Quiero: Configurar qué cuentas PUC se debitan y acreditan para cada tipo de operación
Para: Que los asientos automáticos sean correctos sin intervención manual

Criterios de aceptación:
✅ Mapeos configurables para: cuotas, donaciones, ventas merch, compras, diferencia en cambio, ingresos/egresos genéricos
✅ El sistema valida que las cuentas mapeadas existan en el PUC activo
✅ Un cambio en el mapeo no afecta retroactivamente los asientos ya generados
```

---

### PHASE 1 — MVP Contabilidad, Tesorería y Cuotas

#### Épica 4: Contabilidad general

**Historia CONT-01 — Registrar comprobante contable**
```
Como: Contador / Operador
Quiero: Registrar comprobantes contables (ingreso/egreso/diario/ajuste/cierre)
Para: Mantener la contabilidad formal actualizada y balanceada

Criterios de aceptación:
✅ Un comprobante no se puede guardar si sum(débitos) ≠ sum(créditos)
✅ Cada línea del comprobante requiere: cuenta PUC, CC, valor, tipo (D/C)
✅ Los comprobantes de ingreso/egreso requieren soporte adjunto
✅ Numeración consecutiva por tipo de comprobante y período (ej: ING-2026-001)
✅ El comprobante queda en estado "borrador" hasta ser contabilizado

Auditoría: Sí
```

**Historia CONT-02 — Cierre mensual**
```
Como: Tesorero y Contador
Quiero: Ejecutar el cierre contable mensual
Para: Bloquear el período y generar estados financieros confiables

Criterios de aceptación:
✅ El Tesorero valida el período (confirma que los movimientos bancarios están completos)
✅ El Contador ejecuta el cierre (genera el comprobante de cierre y bloquea el período)
✅ Una vez cerrado, ningún comprobante del período puede ser editado ni anulado
✅ Los ajustes post-cierre se hacen con comprobante de ajuste en el período siguiente
✅ El sistema genera automáticamente: BG y ER al cierre

Auditoría: Sí (obligatoria)
```

**Historia CONT-03 — Libros contables**
```
Como: Contador / Junta
Quiero: Consultar el Libro Diario, Mayor y Balance de Prueba
Para: Cumplir con la obligación contable y revisar la salud financiera

Criterios de aceptación:
✅ Libro Diario: todos los comprobantes en orden cronológico con sus asientos
✅ Libro Mayor: movimientos agrupados por cuenta PUC
✅ Balance de Prueba: saldos de todas las cuentas con débitos, créditos y saldo neto
✅ Filtros por período y CC
✅ Exportable a Excel/PDF
```

---

#### Épica 5: Tesorería bancarizada

**Historia TES-01 — Registrar movimiento bancario**
```
Como: Operador / Tesorero
Quiero: Registrar ingresos y egresos de la cuenta Bancolombia
Para: Mantener el saldo bancario conciliado y la contabilidad actualizada

Criterios de aceptación:
✅ Campos obligatorios: fecha, valor, tipo (ingreso/egreso), medio de pago, CC, concepto, soporte adjunto
✅ El movimiento genera automáticamente el asiento contable correspondiente (Banco vs cuenta configurada)
✅ Se genera recibo PDF con QR de verificación pública
✅ Saldo bancario actualizado en tiempo real

Auditoría: Sí
```

**Historia TES-02 — Anular movimiento bancario**
```
Como: Operador (solicita) / Tesorero (aprueba)
Quiero: Anular un movimiento bancario erróneo dentro del mismo período
Para: Corregir errores sin alterar la integridad contable

Criterios de aceptación:
✅ Solo se puede anular dentro del mismo período contable (no cerrado)
✅ La anulación requiere aprobación explícita del Tesorero
✅ El motivo de anulación es obligatorio
✅ Se genera comprobante de reversión automático
✅ El comprobante original queda marcado como "Anulado" (soft-delete, no eliminado)

Auditoría: Sí (obligatoria — acción crítica)
```

---

#### Épica 6: Cuotas de miembros y CxC

**Historia MEM-01 — Gestionar miembros**
```
Como: Admin / Operador
Quiero: Crear, editar y desactivar miembros
Para: Mantener el padrón actualizado y controlar quién genera obligaciones de cuota

Criterios de aceptación:
✅ Campos: nombre, documento, tipo de afiliación, estado (Activo/Inactivo/Suspendido), CC por defecto
✅ Un miembro inactivo/suspendido no genera nuevas obligaciones de cuota
✅ El historial de afiliación se conserva (soft-delete de estados)
```

**Historia MEM-02 — Registrar cuota de asamblea**
```
Como: Admin / Tesorero
Quiero: Registrar el valor de cuota mensual aprobado en asamblea
Para: Que el sistema use el valor correcto según el período

Criterios de aceptación:
✅ Campos: año, mes de inicio de cobro, valor mensual COP, acta soporte adjunta
✅ El valor se aplica a todos los períodos >= mes de inicio del mismo año (o hasta nueva cuota)
✅ El sistema soporta múltiples cuotas históricas (ej: cuota de enero diferente a cuota de febrero)
✅ Auditoría de todo cambio de cuota

Regla clave: GetVigentePorPeriodoAsync(año, mes) → busca la cuota más reciente cuyo año < targetAño, O año = targetAño AND mesInicio <= targetMes.

Auditoría: Sí (obligatoria)
```

**Historia MEM-03 — Generar obligaciones mensuales (cartera)**
```
Como: Tesorero / Operador
Quiero: Generar las obligaciones de cuota para todos los miembros activos de un período
Para: Tener la cartera de cobro del mes actualizada

Criterios de aceptación:
✅ Selección de período (YYYY-MM) y tipos de afiliación a incluir
✅ Solo genera para miembros en estado Activo
✅ Usa el valor de cuota vigente para el período (regla de asamblea)
✅ No genera duplicados si ya existen obligaciones para ese período/miembro
✅ Cada obligación queda en estado "Pendiente"
```

**Historia MEM-04 — Registrar pago de cuota**
```
Como: Operador / Tesorero
Quiero: Registrar el pago de un miembro y aplicarlo a sus obligaciones pendientes
Para: Actualizar el estado de la cartera y generar el asiento contable

Criterios de aceptación:
✅ El pago se aplica a la(s) obligación(es) más antiguas primero (FIFO)
✅ Se permiten anticipos (pago de meses futuros)
✅ Se permiten pagos parciales
✅ Asiento automático: débito Banco / crédito Ingresos por Cuotas de Afiliación
✅ Actualización inmediata del estado de mora del miembro
```

---

#### Épica 7: CxP Proveedores

**Historia CXP-01 — Registrar factura por pagar**
```
Como: Operador / Tesorero
Quiero: Registrar una factura de proveedor pendiente de pago
Para: Controlar las obligaciones de la fundación y su vencimiento

Criterios de aceptación:
✅ Campos: proveedor/tercero, valor, fecha emisión, fecha vencimiento, CC, concepto, soporte adjunto
✅ La factura queda en estado "Pendiente"
✅ Asiento inicial: débito cuenta de gasto / crédito Cuentas por Pagar
```

**Historia CXP-02 — Pagar factura de proveedor**
```
Como: Tesorero
Quiero: Registrar el pago de una factura pendiente
Para: Cruzar la obligación contra el banco y cerrar la CxP

Criterios de aceptación:
✅ El pago vincula el movimiento bancario con la CxP
✅ Asiento automático: débito Cuentas por Pagar / crédito Banco
✅ Si la CxP fue en USD, registrar diferencia en cambio si el COP difiere
✅ La CxP pasa a estado "Pagada"
```

---

#### Épica 8: Multimoneda USD

**Historia FX-01 — Registrar transacción en USD**
```
Como: Operador / Tesorero
Quiero: Registrar una transacción cuyo origen fue en dólares estadounidenses
Para: Tener trazabilidad de la tasa usada y cumplir con los requerimientos de auditoría

Criterios de aceptación:
✅ Si MonedaOrigen = USD, son obligatorios: MontoUSD, MontoCOP, TasaCambioUsada, FechaTasaCambio, FuenteTasaCambio (TRM_SFC / TASA_BANCO / MANUAL_CON_SOPORTE) y soporte adjunto
✅ Sin alguno de estos campos, el sistema rechaza el guardado
✅ El valor contable oficial siempre es COP
✅ No existe reexpresión mensual automática de saldos en USD
✅ Los reportes contables muestran solo COP
```

---

### PHASE 2 — Donaciones

#### Épica 9: Donaciones y certificados

**Historia DON-01 — Crear campaña de donación**
```
Como: Admin / Operador
Quiero: Crear una campaña de donación con meta, vigencia y proyecto/CC asociado
Para: Organizar y reportar las donaciones por iniciativa

Criterios de aceptación:
✅ Campos: nombre, descripción, meta COP, fecha inicio, fecha fin, estado (Activa/Cerrada), CC, proyecto asociado (opcional)
✅ Una campaña cerrada no acepta nuevas donaciones
```

**Historia DON-02 — Registrar donación y emitir certificado**
```
Como: Operador / Tesorero
Quiero: Registrar una donación y emitir automáticamente el certificado
Para: Cumplir con la obligación legal de las ESAL de certificar toda donación

Criterios de aceptación:
✅ Campos: donante (natural/jurídico), tipo donación (dinero/especie), valor, campaña, soporte
✅ El certificado PDF se genera automáticamente con: datos del donante, valor, fecha, CC, logo institucional, QR de verificación
✅ El QR permite verificación pública sin exponer datos sensibles del donante
✅ Sin certificado emitido, la donación no puede quedar en estado "Confirmada"
✅ Asiento automático: débito Banco (si dinero) / crédito Ingresos por Donaciones

Auditoría: Sí
```

---

### PHASE 3 — Proyectos Sociales

#### Épica 10: Proyectos y beneficiarios

**Historia PROY-01 — Crear proyecto social**
```
Como: Admin / Operador
Quiero: Crear un proyecto social con presupuesto, cronograma y CC
Para: Tener visibilidad sobre los proyectos activos y sus recursos

Criterios de aceptación:
✅ Campos: nombre, objetivo, descripción, presupuesto, fecha inicio/fin, CC, estado
✅ Se pueden adjuntar documentos de respaldo del proyecto
```

**Historia PROY-02 — Registrar beneficiario con consentimiento**
```
Como: Operador (con permiso de PII)
Quiero: Registrar beneficiarios de un proyecto social
Para: Cumplir con los indicadores de impacto y con la Ley 1581/2012

Criterios de aceptación:
✅ Si se capturan datos PII (nombre, documento, teléfono, dirección), el consentimiento Habeas Data es OBLIGATORIO (campo booleano + fecha de firma)
✅ Sin consentimiento, solo se permite código anónimo + datos no sensibles (ej: categoría etaria, municipio)
✅ El acceso a datos PII de beneficiarios está restringido por rol
✅ Todo cambio en datos de beneficiarios queda en auditoría

Auditoría: Sí (obligatoria — Ley 1581/2012)
```

**Historia PROY-03 — Informe de rendición de cuentas**
```
Como: Admin / Contador / Junta
Quiero: Generar el informe de rendición de cuentas de un proyecto
Para: Presentar a la comunidad y a posibles donantes el impacto y uso de los recursos

Criterios de aceptación:
✅ El informe incluye: resumen del proyecto, beneficiarios (agregados, sin PII), egresos imputados, indicadores
✅ Exportable en PDF y Excel
✅ Los datos de beneficiarios individuales no aparecen en el PDF público
```

---

### PHASE 4 — Negocios / Merchandising

#### Épica 11: Inventario y ventas

**Historia BIZ-01 — Gestionar catálogo de artículos**
```
Como: Admin / Operador
Quiero: Crear y gestionar el catálogo de artículos de merchandising
Para: Controlar el inventario disponible para venta

Criterios de aceptación:
✅ Campos: nombre, descripción, unidad, valor unitario adquisición, stock actual
✅ El stock se actualiza automáticamente con cada compra y venta
✅ Alerta cuando el stock llega a 0 (o umbral configurable)
```

**Historia BIZ-02 — Registrar venta de merchandising**
```
Como: Operador
Quiero: Registrar la venta de artículos a un cliente
Para: Actualizar el inventario y generar el comprobante de venta interno

Criterios de aceptación:
✅ Carrito simple: artículo + cantidad
✅ Validación de stock antes de confirmar venta
✅ Campos obligatorios: medio de pago, CC
✅ Descuento de stock automático al confirmar
✅ Comprobante de venta PDF + QR de verificación (no es factura electrónica)
✅ Sin integración con DIAN en esta fase

Nota: El comprobante es de uso interno. La FE queda como preparación futura.
```

---

### PHASE 5 — Tributario Avanzado

#### Épica 12: Reportes tributarios

**Historia TRIB-01 — Reporte de información exógena**
```
Como: Contador / Admin
Quiero: Generar el reporte de información exógena para la DIAN
Para: Cumplir con la obligación tributaria de reportar pagos a terceros

Criterios de aceptación:
✅ Exportable en formato CSV compatible con especificación DIAN vigente
✅ Incluye: NIT/CC del tercero, concepto del pago, valor COP, período
✅ Filtros por año fiscal y CC
✅ Solo accesible por Contador / Admin
```

**Historia TRIB-02 — Reporte de beneficiarios finales (RUB)**
```
Como: Contador / Admin
Quiero: Generar el Registro Único de Beneficiarios (RUB)
Para: Cumplir con las obligaciones de transparencia de las ESAL

Criterios de aceptación:
✅ Exportable en formato CSV
✅ Datos requeridos: nombre/razón social, identificación, participación, tipo de beneficio
✅ Solo accesible por Contador / Admin
```

---

## 12. Reglas de negocio inamovibles

Estas reglas son **no negociables** y deben ser implementadas en cualquier código que se desarrolle para este sistema:

### RN-01: Bancarización 100%
> Todos los ingresos y egresos deben impactar la entidad `Banco` (Bancolombia). No existe caja física operativa. El campo `MedioPagoId` es obligatorio en toda transacción.

### RN-02: CentroCostoId obligatorio
> Toda transacción financiera debe llevar un `CentroCostoId` válido. Si falta, el backend rechaza la operación con error de validación.

### RN-03: Soft Delete financiero
> Las entidades financieras (comprobantes, movimientos, cuotas, donaciones, CxC, CxP, ventas) **nunca se eliminan físicamente** de la base de datos. Se marca `IsDeleted = true` o `Anulado = true`.

### RN-04: Cierre contable bloqueante
> Una vez cerrado un período contable, ningún comprobante de ese período puede ser creado, editado o anulado. Los ajustes se hacen en el período activo siguiente.

### RN-05: Anulación solo intra-mes
> Una anulación solo es posible si el movimiento pertenece al período contable **actualmente abierto**. Requiere aprobación del Tesorero y motivo obligatorio.

### RN-06: Certificado de donación obligatorio
> Ninguna donación puede quedar en estado "Confirmada" sin que exista un certificado PDF generado y su QR de verificación.

### RN-07: Consentimiento Habeas Data obligatorio para PII
> Si se captura nombre, documento, teléfono o cualquier dato personal identificable de un beneficiario, el consentimiento debe estar marcado como `true` con fecha de firma. Sin consentimiento, solo se permite código anónimo.

### RN-08: Moneda funcional es COP
> Todos los reportes contables, estados financieros y saldos se expresan en COP. El USD es informativo y siempre debe acompañarse de tasa, fecha, fuente y soporte.

### RN-09: Sin contraseñas locales
> Prohibición absoluta de almacenar contraseñas, hashes o datos de autenticación propios. La autenticación es 100% delegada a Microsoft Entra External ID.

### RN-10: Asientos balanceados
> Un comprobante contable no puede ser contabilizado si la suma de débitos no es igual a la suma de créditos. Esta validación se hace en el backend (no solo en el frontend).

---

## 13. Modelo de datos clave

### Entidades principales y sus relaciones

```
Miembro ──────────────────────── CuentaPorCobrar (CxC)
  │                                    │
  │ TipoAfiliacion                     │ CuotaAsamblea
  │ EstadoMiembro                      │ Periodo (YYYY-MM)
  │ IsDeleted                          │ ValorEsperadoCOP
  │                                    │ EstadoPago
  │                                    │
MovimientoBancario ─────────────── ComprobanteContable
  │                                    │
  │ CentroCostoId (oblig.)             │ Lineas de asiento
  │ MedioPagoId (oblig.)               │ (debe = haber)
  │ Soporte (Blob)                     │ PeriodoContable
  │ Anulado                            │ Anulado
  │
Donacion ──────────── Certificado ────── Campaña
  │                   (PDF + QR)          │
  │ Donante                               │ CentroCostoId
  │ TipoDonacion                          │ Meta COP
  │                                       │
ProyectoSocial ────── Beneficiario
  │                   │
  │ CC asignado       │ TieneConsentimientoHabeasData (bool)
  │ Presupuesto       │ FechaConsentimiento
  │                   │ (PII solo si consentimiento = true)
  │
ArticuloInventario ──── VentaMerchandising
  StockActual            │ ComprobantePDF + QR
  ValorAdquisicion       │ MedioPagoId
                         │ CentroCostoId

CuentaPorPagar (CxP) ──── MovimientoBancario (pago)
  Proveedor/Tercero          (cruce al pagar)
  FechaVencimiento
  Anulado
```

### Entidad `PeriodoContable`

```csharp
// Estado del período: Abierto, CerradoTesorero, Cerrado
// Un período Cerrado bloquea toda operación retroactiva
```

### Campos obligatorios de multimoneda (cuando MonedaOrigen = USD)

```
MonedaOrigen: "USD"
MontoMonedaOrigen: decimal (USD)
MontoCOP: decimal (valor contable oficial)
TasaCambioUsada: decimal
FechaTasaCambio: date
FuenteTasaCambio: enum { TRM_SFC, TASA_BANCO, MANUAL_CON_SOPORTE }
SoporteUrl: string (Blob URL) — OBLIGATORIO
```

---

## 14. Roles, RBAC y matriz de permisos

### Roles del sistema

| Rol | Descripción |
|-----|-------------|
| **Admin** | Gestiona usuarios/roles internos, configuración global del sistema. |
| **Operador** | Registra transacciones del día a día (movimientos, cuotas, donaciones, ventas). |
| **Tesorero** | Valida operaciones bancarias, aprueba anulaciones, valida cierre mensual. |
| **Contador** | Gestiona la contabilidad (PUC, comprobantes, cierres), genera reportes tributarios. |
| **Junta** | Vista de lectura de reportes consolidados. Sin capacidad de modificar datos. |

### Matriz de permisos

| Módulo | Admin | Operador | Tesorero | Contador | Junta |
|--------|:-----:|:--------:|:--------:|:--------:|:-----:|
| Usuarios / Roles internos | RW | — | — | — | — |
| Configuración (CC, medios de pago, PUC) | RW | R | R | R | — |
| Tesorería (movimientos bancarios) | R | RW | RW | R | R |
| Anulaciones | R | Solicita | Aprueba | R | — |
| CxC (cartera de cuotas) | R | RW | RW | R | R |
| CxP (proveedores) | R | RW | RW | R | R |
| Contabilidad (comprobantes, libros) | R | R | R | RW | R |
| Cierre mensual | — | — | Valida | Ejecuta | Ver |
| Donaciones | R | RW | RW | R | R |
| Proyectos sociales | R | RW | R | R | Agregado |
| Datos PII beneficiarios | R | RW (con permiso) | R | R | — |
| Negocios (inventario, ventas) | R | RW | R | R | R |
| Reportes tributarios | R | — | — | RW | — |

---

## 15. Seguridad y autenticación

### Flujo de autenticación

```
Usuario → Next.js (MSAL/NextAuth)
    → Redirige a Entra External ID
    → Entra valida credenciales + MFA
    → Retorna id_token + access_token (JWT)
    → Next.js almacena sesión
    → Cada llamada a la API incluye ******
    → API valida JWT (issuer, audience, firma)
    → Middleware de autorización verifica rol interno (desde BD)
    → Handler ejecuta la operación
```

### Configuración de desarrollo vs producción

**Development (local):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:...;Initial Catalog=LAMAMedellinContable;Encrypt=True;"
  }
}
// La API usa AzureCliCredential → requiere: az login
// NO incluir Authentication= en el connection string local
```

**Production (Azure App Service):**
```
ConnectionStrings__DefaultConnection="...;Authentication=Active Directory Default;Encrypt=True;"
// Usa Managed Identity automáticamente
// El App Service debe tener System Assigned Managed Identity habilitada
```

---

## 16. Multimoneda — USD informativo

### Resumen de la regla

- La **moneda funcional y de reporte es COP** en todos los estados financieros.
- La Fundación **no tiene cuenta bancaria en USD**.
- El USD se registra **solo informativamente** para operaciones puntuales (eventos internacionales, membresías, compras ocasionales).
- **No hay reexpresión mensual automática** (no hay saldos permanentes en USD).

### Cuándo se activa

- Cualquier transacción donde el pago/cobro original fue en USD.
- Ejemplos: pago de inscripción a rally internacional, membresía L.A.M.A. Internacional.

### Diferencia en cambio

Si una CxP fue registrada con TRM del día de la factura, pero se pagó con la tasa del banco al día del pago y existe diferencia:

- **Ganancia** → Ingreso por diferencia en cambio (cuenta PUC configurada).
- **Pérdida** → Gasto por diferencia en cambio (cuenta PUC configurada).

El asiento de diferencia en cambio se genera **automáticamente** al liquidar la CxP/CxC.

---

## 17. Roadmap por fases

| Fase | Nombre | Contenido | Estado |
|------|--------|-----------|--------|
| **Phase 0** | Fundaciones | IAM Entra + roles + auditoría base + PUC + centros de costo + medios de pago | ✅ Completada |
| **Phase 1** | MVP | Contabilidad + tesorería + cuotas + CxC/CxP + cierres + multimoneda | ✅ Completada |
| **Phase 2** | Donaciones | Campañas + donaciones + certificados obligatorios | ✅ Completada |
| **Phase 3** | Proyectos | Proyectos sociales + beneficiarios + consentimiento + rendición | ✅ Completada |
| **Phase 4** | Negocios | Inventario simple + compras/ventas merch + comprobante interno | ✅ Completada |
| **Phase 5** | Tributario | Exógena DIAN + RUB (beneficiarios finales) + auditoría de calidad tributaria | ✅ Completada |
| **Phase X** | FE futura | Estructura/adapter para facturación electrónica DIAN | 🔲 Solo preparación |

---

## 18. Convenciones de código

### Idioma por capa

| Capa | Idioma | Ejemplos |
|------|--------|---------|
| Dominio (entidades, comandos, reglas) | **Español técnico** | `RegistrarCuotaCommand`, `CuentaPorCobrar`, `CentrosDeCosto` |
| Infraestructura (servicios, patterns) | **Inglés** | `AzureBlobStorageService`, `GlobalExceptionMiddleware` |
| Frontend (componentes, páginas) | **Inglés** | `GenerarCarteraForm.tsx`, `DashboardPage.tsx` |

### Convenciones de .NET

- **Sin `.WithOpenApi()`** en Minimal APIs (causa errores con `RouteHandlerBuilder`).
- **`IOptions<T>`** para toda configuración (nunca hardcodear valores).
- **`ProblemDetails`** en todas las respuestas de error.
- **FluentValidation** como Behavior en el pipeline de MediatR (Fail Fast).
- **Repositorios específicos por Aggregate Root** (no repositorios genéricos).
- **EF Core solo en Infrastructure** — Domain y Application son puras.

### Testing

- Framework: **xUnit** + **Moq** + **FluentAssertions**
- Todo nuevo handler debe tener esqueleto de test unitario.
- Tests de integración para endpoints críticos (cierre, anulación, generación de cartera).

---

## 19. Ejecución local

### Prerrequisitos

- .NET 8 SDK
- Node.js 18+
- Azure CLI instalado y con `az login` activo (para autenticación a Azure SQL)
- Acceso a la suscripción Azure del proyecto

### Backend

```bash
cd LAMAMedellin
dotnet restore
dotnet build

# Ejecutar en modo Development
cd src/LAMAMedellin.API
dotnet run
# API disponible en https://localhost:7030
```

### Frontend

```bash
cd frontend
npm install
npm run dev          # Desarrollo — http://localhost:3000
npm run build        # Build producción
npm run lint         # Linter ESLint
npm run type-check   # TypeScript strict check
```

### Migraciones EF Core

```bash
# Crear nueva migración
dotnet ef migrations add <NombreMigracion> \
  --project src/LAMAMedellin.Infrastructure \
  --startup-project src/LAMAMedellin.API

# Aplicar migraciones
dotnet ef database update \
  --project src/LAMAMedellin.Infrastructure \
  --startup-project src/LAMAMedellin.API
```

### Variables de entorno necesarias en Development

```bash
# appsettings.Development.json (NO commitear datos reales)
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:<server>.database.windows.net,1433;Initial Catalog=LAMAMedellinContable;Encrypt=True;"
  },
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "<tenant-id>",
    "ClientId": "<client-id>"
  }
}
```

---

## 20. Glosario

| Término | Definición |
|---------|-----------|
| **PUC ESAL** | Plan Único de Cuentas para Entidades Sin Ánimo de Lucro. Catálogo de cuentas contables obligatorio en Colombia para fundaciones y ONGs. |
| **CxC** | Cuentas por Cobrar. En este sistema, principalmente las obligaciones de cuotas mensuales de miembros. |
| **CxP** | Cuentas por Pagar. Facturas de proveedores pendientes de pago. |
| **CC** | Centro de Costo. Segmento organizativo al que se imputa una transacción (CAPITULO, FUNDACION, PROYECTO:X, EVENTO:X). |
| **TRM** | Tasa Representativa del Mercado. Tasa oficial de cambio COP/USD publicada por la Superintendencia Financiera de Colombia. |
| **Cierre contable** | Proceso mensual que bloquea el período contable e impide modificaciones retroactivas. Requiere validación del Tesorero y ejecución del Contador. |
| **Soft Delete** | Marcado lógico de un registro como eliminado (`IsDeleted = true`) sin borrarlo físicamente de la base de datos. |
| **Habeas Data** | Derecho de las personas a conocer, actualizar y rectificar la información personal almacenada. En Colombia, regulado por la Ley 1581/2012. |
| **Exógena** | Información exógena: reporte anual que deben presentar ciertas entidades a la DIAN, detallando pagos realizados a terceros. |
| **RUB** | Registro Único de Beneficiarios. Reporte de transparencia que deben presentar las ESAL sobre quiénes son sus beneficiarios finales. |
| **ESAL** | Entidad Sin Ánimo de Lucro. Categoría jurídica colombiana que incluye fundaciones, asociaciones, etc. |
| **CQRS** | Command Query Responsibility Segregation. Patrón de arquitectura que separa las operaciones de escritura (Commands) de las de lectura (Queries). |
| **MediatR** | Librería .NET que implementa el patrón Mediator para desacoplar Commands/Queries de sus Handlers. |
| **Entra External ID** | Servicio de identidad de Microsoft (antes Azure AD B2C) para autenticación de usuarios externos con OIDC. |
| **Managed Identity** | Identidad administrada de Azure que permite a un servicio (ej: App Service) autenticarse ante otros servicios Azure (SQL, Blob, Key Vault) sin credenciales. |
| **ProblemDetails** | Estándar RFC 7807 para representar errores HTTP en APIs. Formato: type, title, status, detail, instance. |
| **DIAN** | Dirección de Impuestos y Aduanas Nacionales. Entidad tributaria de Colombia. |

---

*Documento generado el 2026-07-28. Para contribuir o actualizar, abrir un PR con los cambios correspondientes en `/docs/PRD-GUIA-DESARROLLADOR.md`.*
