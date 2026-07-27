# Documento Maestro Final (BRD + SRS + Arquitectura + Backlog + Seguridad + Operación)
**Proyecto:** Plataforma Integral Fundación / Capítulo L.A.M.A. Medellín  
**Versión:** 1.0 (Cerrado)  
**Fecha:** 2026-02-20  
**Entidad legal contable/tributaria:** Fundación L.A.M.A. Medellín (única)  
**Identidad operativa:** Capítulo L.A.M.A. Medellín  

**Autenticación:** Microsoft Entra External ID (antes Azure AD B2C) + MFA Azure  
**Frontend:** Next.js (SSR opcional)  
**Backend:** ASP.NET Core 8 Web API  
**Datos:** Azure SQL Database  
**Archivos:** Azure Blob Storage  
**Secrets:** Azure Key Vault + Managed Identity  
**Observabilidad:** Application Insights + Log Analytics  
**Facturación electrónica:** No en fase inicial (preparado para integración futura)

---

## 0) Resumen ejecutivo
La Fundación L.A.M.A. Medellín requiere un sistema integral para: tesorería bancarizada, contabilidad formal (PUC ESAL), control de cuotas mensuales (regla asamblea anual y vigencia por periodo), manejo de CxC/CxP, donaciones con certificados obligatorios, proyectos sociales con beneficiarios e indicadores (Ley 1581/2012), y gerencia de negocios (inventario simple y merchandising).  
Debe garantizar segregación de funciones, auditoría, y cierre contable mensual con bloqueo de periodos. Se adopta un stack sostenible en costos en Azure: Next.js + ASP.NET Core API + Azure SQL + Blob + Entra External ID.

---

## 1) Identidad y marco institucional
### 1.1 ¿Qué somos?
- **Capítulo L.A.M.A. Medellín**: identidad mototurística, reconocimiento dentro de L.A.M.A. Internacional.
- **Fundación L.A.M.A. Medellín**: figura legal administrativa en Colombia para bancos, donaciones, proyectos y obligaciones DIAN.

### 1.2 Uso correcto del nombre (regla de comunicación)
| Situación | Nombre correcto |
|---|---|
| Eventos L.A.M.A., parches, uniformes, comunicación mototurística | Capítulo L.A.M.A. Medellín |
| Contratos, bancos, documentación legal, DIAN, proyectos sociales | Fundación L.A.M.A. Medellín |

**Regla:** el sistema opera legal/tributariamente como Fundación; internamente separa por centros de costo.

---

## 2) Caso de negocio (BRD)
### 2.1 Problema
- Gestión dispersa (Excel/soportes) con baja trazabilidad y alto riesgo (errores/fraude/pérdida de evidencia).
- Necesidad de contabilidad formal y reportes base para obligaciones DIAN.
- Necesidad misional: proyectos sociales con evidencia, beneficiarios e indicadores.
- Necesidad de sostenibilidad: cuotas + donaciones + merchandising.

### 2.2 Objetivos
1. Contabilidad formal completa (PUC ESAL, comprobantes, libros, cierres).
2. Bancarización 100% y trazabilidad por medio de pago.
3. Cuotas mensuales con reglas de asamblea (histórico y vigencia por periodo) + mora (CxC).
4. Cuentas por pagar proveedores (CxP) para control real de obligaciones.
5. Donaciones con certificado obligatorio y verificación pública.
6. Proyectos sociales con beneficiarios (consentimiento) + indicadores + rendición.
7. Negocios (merch) con inventario simple y documento interno de venta (sin FE inicial).
8. Control interno: segregación de funciones, auditoría, anulaciones intra-mes, reversos post-cierre.

### 2.3 Alcance (In / Out)
**Incluye**
- Tesorería bancaria (Bancolombia única)
- Contabilidad general (PUC ESAL)
- Cuotas de miembros + CxC
- CxP proveedores
- Donaciones + campañas + certificados obligatorios
- Proyectos sociales + beneficiarios + consentimiento + indicadores + rendición
- Gerencia de negocios (merch) + inventario simple
- Reportes contables y reportes base tributarios (exógena, beneficiarios finales)

**Excluye (fase inicial)**
- Nómina
- Facturación electrónica (se deja preparada para integración futura)

---

## 3) Gobernanza, roles y segregación de funciones
### 3.1 Roles internos (RBAC)
- **Admin**
- **Operador**
- **Tesorero**
- **Contador**
- **Junta**

### 3.2 RACI
- Registro diario: Operador / Tesorero
- Validación mensual: Tesorero
- Cierre oficial: Contador
- Reporte a Junta: mensual (Contador + Tesorero → Junta)

---

## 4) Reglas de negocio (obligatorias)

### 4.1 Bancarización 100% (sin caja)
- Todos los ingresos/egresos impactan contablemente **Banco** (Bancolombia).
- Campo obligatorio: **Medio de pago**.
- No existe caja física operativa.

**Medios de pago permitidos**
- Transferencia bancaria
- Consignación en efectivo
- Pago por corresponsal bancario
- QR / canal digital (futuro)

### 4.2 Banco único (fase inicial)
- Banco: Bancolombia, cuenta de ahorros Fundación.
- El diseño debe permitir múltiples cuentas a futuro, pero inicialmente 1 activa.

### 4.3 Centros de costo (CC) (obligatorio)
- `CAPITULO`
- `FUNDACION`
- `PROYECTO:<nombre>`
- `EVENTO:<nombre>` (opcional)

**Regla:** toda transacción financiera debe llevar CC.

### 4.4 Cuotas de miembros (asamblea anual)
- Cuota fija aprobada en asamblea de enero.
- Puede iniciar cobro desde un mes posterior (ej: feb).
- Valor aplicable se determina por **Periodo (YYYY-MM)**.
- Centro de costo por defecto: CAPITULO o FUNDACION según tipo afiliación del miembro.

### 4.5 Donaciones (certificado obligatorio)
- Todas las donaciones generan certificado (PDF + QR + verificación).

### 4.6 Beneficiarios y Ley 1581/2012
- Si se registra **nombre/documento/teléfono** → consentimiento obligatorio.
- Sin consentimiento → solo permitir código anónimo (y datos no sensibles).
- Control de acceso para PII y auditoría de cambios.

### 4.7 Cuentas por cobrar (CxC) y pagar (CxP)
- CxC: obligaciones mensuales de cuotas y otros cobros a terceros cuando aplique.
- CxP: facturas proveedor pendientes con pagos posteriores.

### 4.8 Anulación, reversos y cierres
- Anulación solo dentro del mismo mes contable + aprobación Tesorero + motivo obligatorio.
- Post-cierre: solo reverso vía comprobante contable de ajuste.
- Cierre contable mensual obligatorio: valida Tesorero, ejecuta Contador, bloquea periodo.

### 4.9 Facturación electrónica (no inicial, preparada)
- No se implementa en fase inicial.
- Ventas de merchandising: comprobante interno (PDF + QR).
- Preparar estructura para integración futura con proveedor DIAN (adapter, estados, numeración, impuestos parametrizables).

---

## 5) Multimoneda informativa (USD) — Definición oficial

### 5.1 Regla de negocio (USD informativo, COP funcional)
- **Moneda funcional y de reporte:** COP.
- La Fundación **no maneja cuenta bancaria en USD** en esta fase (no hay saldos permanentes en USD).
- Se habilita el registro de **moneda origen USD** únicamente para operaciones puntuales:
  - eventos internacionales,
  - pagos/membresías internacionales,
  - compras o ingresos ocasionales pactados en USD.
- **El valor contable oficial siempre será COP**, soportado por el **extracto bancario** (o documento oficial equivalente).
- **No se implementa reexpresión mensual automática**, ya que no existen saldos bancarios en USD.

### 5.2 Datos mínimos obligatorios cuando la moneda origen sea USD
Para cualquier transacción monetaria (ingreso/egreso, CxC/CxP, compra/venta, donación) en la que `MonedaOrigen = USD`, el sistema debe almacenar:
- `MonedaOrigen = USD`
- `MontoMonedaOrigen` (USD)
- `MontoCOP` (valor contable oficial en COP)
- `TasaCambioUsada` (TRM o tasa banco, solo para trazabilidad)
- `FechaTasaCambio`
- `FuenteTasaCambio`:
  - `TRM_SFC` (TRM oficial Superintendencia Financiera de Colombia)
  - `TASA_BANCO` (tasa de liquidación del banco según soporte)
  - `MANUAL_CON_SOPORTE` (solo si se adjunta soporte)
- Soporte obligatorio (extracto, comprobante, factura, confirmación de pago, etc.)

### 5.3 Tratamiento contable: diferencia en cambio
- Si en una operación con moneda origen USD existe diferencia entre:
  - el COP reconocido inicialmente (por ejemplo en una CxP/CxC registrada usando TRM), y
  - el COP real del pago/cobro soportado por extracto,
  entonces el sistema debe registrar la diferencia en:
  - **Ingresos por diferencia en cambio** (si resulta ganancia)
  - **Gastos por diferencia en cambio** (si resulta pérdida)

---

## 6) Stack y arquitectura (mínima sostenible en Azure)

### 6.1 Autenticación (Entra External ID)
- OIDC con Entra External ID.
- MFA configurable en Entra.
- No se almacenan contraseñas en la aplicación.

### 6.2 Stack recomendado
- **Frontend:** Next.js + TypeScript (SSR opcional)
- **Backend:** ASP.NET Core 8 Web API
- **Persistencia:** Azure SQL Database
- **Archivos:** Azure Blob Storage
- **Secrets:** Azure Key Vault + Managed Identity
- **Observabilidad:** Application Insights + Log Analytics

### 6.3 Despliegue recomendado (costo/operación)
- **API (.NET):** Azure App Service (Linux) plan bajo + autoscale.
- **Web (Next.js):**
  - preferido: Azure Static Web Apps si el modo SSR es compatible/limitado,
  - alternativa: App Service Node si SSR completo es necesario desde el día 1.
- **SQL:** Azure SQL serverless (si uso bajo) o S0 (si uso estable).
- **Blob:** lifecycle policies (hot→cool→archive) para reducir costo.
- **Key Vault:** secretos y certificados; acceso por Managed Identity.

### 6.4 Backups y restore
- SQL: backups automáticos + retención configurada.
- Blob: lifecycle + soft delete opcional.
- Runbook: procedimiento de restore probado periódicamente.

---

## 7) Requerimientos funcionales (SRS)

### 7.1 IAM (Entra + roles internos)
**RF-IAM-01** Login con email/password gestionado por Entra External ID.  
**RF-IAM-02** MFA configurable en Entra.  
**RF-IAM-03** La app no almacena contraseñas ni hashes localmente.  
**RF-IAM-04** Roles internos: Admin, Operador, Tesorero, Contador, Junta.  
**RF-IAM-05** Auditoría de asignación/cambio de roles internos.

**CA-IAM**
- CA-IAM-01: login solo vía Entra.
- CA-IAM-02: MFA según política.
- CA-IAM-03: BD local sin contraseñas.
- CA-IAM-04: autorización por roles internos en API y UI.

### 7.2 Configuración base
**RF-CFG-01** Parametrizar cuenta bancaria Bancolombia (y su cuenta contable asociada).  
**RF-CFG-02** Catálogo de medios de pago (obligatorio).  
**RF-CFG-03** Centros de costo y asociación a proyectos/eventos.  
**RF-CFG-04** Tipos de afiliación y CC por defecto.  
**RF-CFG-05** Importación PUC ESAL (contador provee).  
**RF-CFG-06** Mapeo contable por operación (configurable):
- Ingresos por Cuotas de Afiliación / Membresía
- Ingresos por Donaciones
- Ingresos por Venta de Merchandising
- Gastos por naturaleza (admin/operativos/eventos/proyectos/bancarios)
- Compra de inventario
- **Ingresos por diferencia en cambio**
- **Gastos por diferencia en cambio**

### 7.3 Multimoneda informativa (USD)
**RF-FX-01** Permitir `MonedaOrigen` (COP/USD) en transacciones monetarias; si USD exigir USD, COP, tasa, fecha, fuente y soporte.  
**RF-FX-02** Precargar TRM (SFC) como ayuda de captura (almacenando evidencia de tasa usada).  
**RF-FX-03** En pagos/cobros de CxP/CxC con moneda origen USD, registrar automáticamente diferencia en cambio si el COP difiere.

**CA-FX**
- CA-FX-01: si MonedaOrigen=USD, sin tasa/fecha/fuente/soporte no se guarda.
- CA-FX-02: reportes contables solo en COP.
- CA-FX-03: no reexpresión mensual.
- CA-FX-04: diferencia en cambio automática en liquidación de CxP/CxC en USD cuando aplique.

### 7.4 Contabilidad general
**RF-CONT-01** Importar/gestionar PUC ESAL.  
**RF-CONT-02** Comprobantes (ingreso/egreso/diario/ajuste/cierre) con numeración.  
**RF-CONT-03** Asientos balanceados debe=haber, CC obligatorio, tercero cuando aplique, soporte.  
**RF-CONT-04** Libros: diario, mayor, balance de prueba.  
**RF-CONT-05** Estados financieros: BG y ER por CC y consolidado.  
**RF-CONT-06** Cierre mensual: bloqueo total y control de ajustes.  
**RF-CONT-07** Ajustes post-cierre solo vía comprobante (sin editar origen).  
**RF-CONT-08** Reportes base tributarios (estructurados exportables): exógena y beneficiarios finales.

### 7.5 Tesorería bancaria
**RF-TES-01** Movimientos bancarios con soporte, CC y medio de pago.  
**RF-TES-02** Recibos PDF + QR + verificación pública.  
**RF-TES-03** Conciliación bancaria mensual.  
**RF-TES-04** Anulación intra-mes con aprobación tesorero y motivo.

### 7.6 CxC (cuotas y cartera)
**RF-CXC-01** Obligaciones mensuales por miembro activo y periodo.  
**RF-CXC-02** Pagos aplicables a obligaciones (anticipos permitidos).  
**RF-CXC-03** Mora + aging.  
**RF-CXC-04** (Opcional) CxC de terceros.

### 7.7 CxP (proveedores)
**RF-CXP-01** Facturas por pagar con soporte y vencimiento.  
**RF-CXP-02** Pago posterior cruzando obligación contra banco.  
**RF-CXP-03** Reporte vencidas/por vencer.

### 7.8 Miembros y cuotas
**RF-MEM-01** CRUD miembros, tipo afiliación y estado.  
**RF-MEM-02** Histórico cuotas: valor, aprobación, mes inicio, acta soporte.  
**RF-MEM-03** Reportes recaudo/mora/histórico.  
**RF-MEM-04** Asiento automático por pago: Banco vs Ingreso Cuotas.

### 7.9 Donaciones
**RF-DON-01** Campañas.  
**RF-DON-02** Donantes natural/jurídico.  
**RF-DON-03** Donación dinero/especie con soporte.  
**RF-DON-04** Certificado obligatorio (PDF + QR + verificación).  
**RF-DON-05** Reportes por campaña/donante/proyecto.  
**RF-DON-06** Asiento automático: Banco vs Ingreso Donaciones.

### 7.10 Proyectos sociales
**RF-PROY-01** Proyectos + presupuesto + cronograma + evidencias.  
**RF-PROY-02** Beneficiarios con consentimiento obligatorio para PII.  
**RF-PROY-03** Indicadores agregados.  
**RF-PROY-04** Egresos imputados a proyecto.  
**RF-PROY-05** Informe rendición (PDF/Excel).

### 7.11 Negocios (merch)
**RF-BIZ-01** Inventario simple (cantidades + valor unitario adquisición).  
**RF-BIZ-02** Compras (entrada inventario) con CxP o pago directo.  
**RF-BIZ-03** Ventas (salida inventario) con comprobante interno PDF + QR.  
**RF-BIZ-04** Reportes ventas/inventario/utilidad simple.

---

## 8) Requerimientos no funcionales (RNF)
- **RNF-SEC:** MFA, RBAC y hardening.
- **RNF-AUD:** auditoría de acciones críticas (cambios cuota, anulaciones, cierres, consentimiento).
- **RNF-PRIV:** cumplimiento Ley 1581/2012 (minimización, permisos, trazabilidad).
- **RNF-OPS:** backups/restore documentado.
- **RNF-COST:** arquitectura Azure sostenible con lifecycle y escalado.
- **RNF-OBS:** App Insights + logs estructurados + alertas básicas.

---

## 9) Matriz de permisos (resumen)
| Módulo | Admin | Operador | Tesorero | Contador | Junta |
|---|---:|---:|---:|---:|---:|
| Usuarios/roles internos | RW | - | - | - | - |
| Configuración | RW | R | R | R | - |
| Tesorería | R | RW | RW | R | R |
| Anulaciones | R | Solicita | Aprueba | R | - |
| CxC | R | RW | RW | R | R |
| CxP | R | RW | RW | R | R |
| Contabilidad | R | R | R | RW | R |
| Cierre mensual | - | - | Valida | Ejecuta | Ver |
| Donaciones | R | RW | RW | R | R |
| Proyectos/beneficiarios PII | R | RW (según permiso) | R | R | Agregado |
| Negocios | R | RW | R | R | R |

---

## 10) Roadmap
- **Phase 0:** IAM Entra + roles + auditoría base + import PUC + mapeo contable + catálogos
- **Phase 1 (MVP):** contabilidad + tesorería + cuotas + CxC/CxP + cierres + multimoneda informativa
- **Phase 2:** donaciones/certificados + campañas
- **Phase 3:** proyectos + beneficiarios + consentimiento + rendición
- **Phase 4:** negocios/inventario simple
- **Phase 5:** reportes tributarios base avanzados + evaluación FE futura (solo preparación)

---