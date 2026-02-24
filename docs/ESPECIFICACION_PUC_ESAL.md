# Especificación del Catálogo de Cuentas PUC Adaptado para ESAL
## Fundación L.A.M.A. Medellín

**Documento:** `ESPECIFICACION_PUC_ESAL.md`  
**Versión:** 1.0  
**Fecha:** 2026-02-24  
**Autor:** Daniel Villamizar (Fundador / Representante Legal)  
**Estado:** Aprobado — implementado parcialmente (ver sección 5)

---

## Índice

1. [Contexto institucional y marco normativo](#1-contexto-institucional-y-marco-normativo)  
2. [Épica: Contabilidad Formal (PUC ESAL)](#2-épica-contabilidad-formal-puc-esal)  
3. [Historia de usuario: Catálogo de Cuentas precargado](#3-historia-de-usuario-catálogo-de-cuentas-precargado)  
4. [Especificación técnica del PUC](#4-especificación-técnica-del-puc)  
   - 4.1 Reglas de negocio para la arquitectura  
   - 4.2 Estructura completa de cuentas ESAL  
   - 4.3 Consideraciones técnicas adicionales  
5. [Estado de implementación](#5-estado-de-implementación)  
6. [Trazabilidad de requerimientos](#6-trazabilidad-de-requerimientos)

---

## 1. Contexto institucional y marco normativo

La Fundación L.A.M.A. Medellín tiene la siguiente clasificación contable y tributaria:

| Atributo | Valor |
|---|---|
| **Grupo NIIF** | **Grupo III — NIIF para Microempresas** |
| **Tamaño de empresa** | Micro |
| **Responsabilidad tributaria** | 42 — Obligado a llevar contabilidad |
| **Entidad de inspección, vigilancia y control** | Gobernación de Antioquia |
| **Otras responsabilidades activas** | 05 — Renta, 07 — Retención en la Fuente, 48 — IVA, 14 — Información Exógena |
| **Actividades económicas (CIIU)** | 9319 · 9329 · 7990 · 5630 |
| **Naturaleza jurídica** | ESAL (Entidad Sin Ánimo de Lucro) |

Esta combinación de factores determina el manejo de las cuentas de la siguiente manera:

### 1.1 Marco normativo aplicable

Al estar en el **Grupo III**, la fundación debe llevar su contabilidad exclusivamente bajo el **marco técnico de NIIF para Microempresas**.

### 1.2 Flexibilidad del PUC

Como es una fundación vigilada por la **Gobernación de Antioquia** (y no por una superintendencia estricta como la Supersolidaria), **no tiene que regirse por un catálogo único predeterminado e inmodificable** por parte del Estado.

### 1.3 Estructuración a la medida

La fundación debe construir su propio **Catálogo de Cuentas**. La mejor práctica es utilizar la **codificación numérica tradicional del antiguo PUC comercial** para facilitar el cumplimiento de las responsabilidades tributarias que tiene activas. Mantener esta estructura de códigos será indispensable para:

- Presentar el **impuesto de renta** (responsabilidad 05)
- Declarar **retención en la fuente** (07)
- Liquidar **IVA** (48)
- Generar los reportes de **información exógena** (14)

### 1.4 Adaptación de conceptos ESAL

Se deben ajustar los nombres de las cuentas patrimoniales:

| PUC Comercial (original) | PUC ESAL (adaptado) |
|---|---|
| Capital Social | Fondo Social |
| Utilidad del Ejercicio | Excedente del Ejercicio |
| Pérdida del Ejercicio | Déficit del Ejercicio |
| Patrimonio | Patrimonio Institucional |

Adicionalmente, se deben crear cuentas de ingresos que reflejen el recaudo por las actividades económicas específicas que se desarrollen (actividades deportivas, recreativas o de turismo registradas bajo los códigos CIIU **9319, 9329, 7990 y 5630**).

---

## 2. Épica: Contabilidad Formal (PUC ESAL)

> **Archivo de referencia en backlog:** `backlog/issue-epic-04.md`

### Título
`[Epic][Phase 0 → Phase 1] Catálogo de Cuentas + Contabilidad Formal (PUC ESAL, comprobantes, libros, cierres)`

### Objetivo
Implementar el motor contable completo de la Fundación L.A.M.A. Medellín: desde el diseño y carga del Catálogo de Cuentas (PUC adaptado ESAL) hasta la generación de comprobantes balanceados, libros contables, estados financieros básicos y cierre mensual con bloqueo de periodos.

### Alcance — Incluye

- **Catálogo de Cuentas (PUC):** diseño, carga inicial, consulta y gestión (alta/baja/modificación).
- **Comprobantes contables:** tipos Ingreso, Egreso, Diario, Ajuste, Cierre. Asientos con Debe = Haber obligatorio.
- **Imputación por Centro de Costo:** obligatoria en toda línea de asiento.
- **Libros contables:** Libro Diario, Libro Mayor, Balance de Prueba.
- **Estados financieros básicos:** Balance General (BG) y Estado de Resultados (ER).
- **Cierre mensual:** flujo aprobado por Tesorero → ejecutado por Contador → bloqueo del periodo.
- **Ajustes post-cierre:** vía comprobante de ajuste (nunca editando el origen).
- **Cierre anual ESAL:** saldo neto de Clases 4 − (5 + 6) → cuenta 3205 (Excedente) o 3210 (Déficit).

### Alcance — Excluye

- Facturación electrónica (estructura preparada, no implementar en esta fase)
- Integración con DIAN en línea (solo exportación de archivos para exógena)
- Reportes tributarios avanzados (Phase 5)

### Criterios de aceptación a nivel épica

- [ ] Ningún comprobante se contabiliza si `Debe ≠ Haber`
- [ ] Todo asiento exige `CuentaContableId` con `PermiteMovimiento = true`
- [ ] Un periodo cerrado no puede recibir nuevos asientos ni ediciones
- [ ] Los ajustes post-cierre quedan auditados (quién, cuándo, motivo)
- [ ] El cierre anual genera asiento automático a `3205` o `3210`
- [ ] El catálogo es consultable en UI con filtro por asentables / nivel / naturaleza

### Dependencias

| Depende de | Bloquea |
|---|---|
| Centros de costo configurados (issue-epic-03) | Comprobantes (no puede haber asiento sin CC) |
| Banco configurado (issue-story-0-8) | Ingreso/egreso bancario |
| Mapeo contable por operación (issue-story-0-6) | Generación automática de asientos desde transacciones |

### Milestones

| Fase | Descripción |
|---|---|
| **Phase 0 – Sprint 2** | Catálogo de cuentas precargado + consulta en UI ← *esta especificación* |
| **Phase 1 – Sprint 3** | Comprobantes + validación Debe=Haber + integración con transacciones |
| **Phase 1 – Sprint 4** | Libros contables + BG/ER básicos |
| **Phase 1 – Sprint 5** | Cierre mensual + bloqueo + cierre anual ESAL |

### Labels
`epic` · `priority:high` · `phase:0` · `phase:1` · `area:accounting`

---

## 3. Historia de usuario: Catálogo de Cuentas precargado

> **Archivos de referencia en backlog:** `backlog/issue-story-cfg-puc-01.md` (backend) y `backlog/issue-story-cfg-puc-02.md` (frontend)

---

### Historia 1 de 2 — Backend y datos iniciales

**ID:** `STORY-CFG-PUC-01`  
**Estado:** ✅ Implementado

#### Como
**Contador** de la Fundación L.A.M.A. Medellín

#### Quiero
Que el sistema disponga de un Catálogo de Cuentas (PUC) propio, jerárquico y precargado con las cuentas de Patrimonio, Ingresos, Gastos y Costos acordes al marco NIIF para Microempresas (Grupo III), desde el primer arranque del sistema

#### Para
- Arrancar la contabilidad formal sin importar un archivo externo.
- Garantizar que los asientos solo se registren en cuentas habilitadas (nodos hoja).
- Reflejar la naturaleza jurídica de la fundación (ESAL, Gobernación de Antioquia).
- Obligar la identificación de terceros en cuentas que exige la DIAN para información exógena.
- Usar las cuentas `3205` y `3210` en el cierre anual en lugar de "Utilidades".

#### Criterios de aceptación

| # | Criterio | Estado |
|---|---|:---:|
| 1 | El sistema almacena Código, Descripción, Naturaleza, PermiteMovimiento, ExigeTercero, CuentaPadreId | ✅ |
| 2 | El nivel jerárquico se deriva de la longitud del código; no se persiste | ✅ |
| 3 | El código es único en BD | ✅ |
| 4 | Solo acepta códigos numéricos con longitudes 1, 2, 4, 6 u 8+ | ✅ |
| 5 | Cada cuenta tiene referencia correcta a su cuenta padre | ✅ |
| 6 | Las 34 cuentas están disponibles desde el primer arranque (seed idempotente) | ✅ |
| 7 | `GET /api/cuentas-contables` retorna el catálogo completo | ✅ |
| 8 | `GET /api/cuentas-contables/asentables` retorna solo nodos hoja | ✅ |
| 9 | Ambos endpoints requieren autenticación JWT | ✅ |
| 10 | Sistema no permite asiento en cuenta con `PermiteMovimiento = false` | ⬜ Phase 1 |
| 11 | Sistema no permite asiento sin tercero cuando `ExigeTercero = true` | ⬜ Phase 1 |

---

### Historia 2 de 2 — Frontend (consulta y navegación)

**ID:** `STORY-CFG-PUC-02`  
**Estado:** ✅ Implementado

#### Como
**Contador** o **Admin** de la Fundación L.A.M.A. Medellín

#### Quiero
Ver el Catálogo de Cuentas completo en pantalla con su estructura jerárquica, filtrar por cuentas asentables, e identificar visualmente qué cuentas exigen tercero

#### Para
- Verificar que el catálogo esté correcto antes de iniciar la operación contable.
- Encontrar rápidamente la cuenta correcta al registrar un comprobante.

#### Criterios de aceptación

| # | Criterio | Estado |
|---|---|:---:|
| 1 | Al navegar a `/contabilidad/cuentas` se muestra la tabla de cuentas | ✅ |
| 2 | La tabla muestra: Código, Descripción, Nivel, Naturaleza, Movimiento, Tercero | ✅ |
| 3 | Las cuentas están ordenadas por código (orden ascendente) | ✅ |
| 4 | Toggle "Solo cuentas asentables" filtra a `PermiteMovimiento = true` | ✅ |
| 5 | Cuentas agrupadoras se muestran en tono diferente (negrilla / fondo gris) | ✅ |
| 6 | `ExigeTercero = true` muestra ícono ⚠ con tooltip DIAN | ✅ |
| 7 | Si el API falla, se muestra mensaje de error amigable | ✅ |
| 8 | El acceso rápido "Catálogo de Cuentas" aparece en el dashboard | ✅ |

---

## 4. Especificación técnica del PUC

> Esta es la especificación original entregada por el representante legal / contador de la fundación, que sirvió como insumo para el diseño del modelo de datos.

---

### 4.1 Reglas de negocio para la arquitectura del sistema

Para el modelo de datos de la tabla `CuentasContables`, los desarrolladores deben contemplar los siguientes atributos y restricciones:

#### Estructura jerárquica (árbol)

El código numérico define el nivel de profundidad:

| Nivel | Nombre | Dígitos | Ejemplo |
|---|---|---|---|
| 1 | Clase | 1 dígito | `3` — Patrimonio |
| 2 | Grupo | 2 dígitos | `31` — Fondo Social |
| 3 | Cuenta | 4 dígitos | `3105` — Aportes de Fundadores |
| 4 | Subcuenta | 6 dígitos | `310505` — Cuotas de Afiliación |
| 5 | Auxiliar | 8+ dígitos | `31050501` — Afiliaciones L.A.M.A. Medellín |

#### Nodos hoja (asentables)

Solo los niveles más profundos (Auxiliares, o en este catálogo las Subcuentas que son los nodos más profundos) pueden recibir asientos contables (transacciones). Las Clases, Grupos y Cuentas funcionan **únicamente como nodos agrupadores** para sumarizar saldos.

Se requiere el campo booleano: **`PermiteMovimiento` (True / False)**

#### Naturaleza de la cuenta

Campo `Naturaleza` tipo ENUM: **`DEBITO` | `CREDITO`**

Define si la cuenta aumenta su saldo con débitos o créditos.

#### Requerimiento de terceros

Campo booleano: **`ExigeTercero` (True / False)**

En las ESAL es vital para reportar información exógena a la DIAN. Toda línea de asiento sobre una cuenta con `ExigeTercero = true` debe incluir el NIT o Cédula del tercero.

---

### 4.2 Estructura completa de cuentas ESAL

> Set de datos inicial que debe precargarse en el sistema. Se omiten temporalmente **Activos (1)** y **Pasivos (2)** por ser de estructura estándar comercial, para enfocarnos en la lógica estructural que cambia drásticamente en el **Patrimonio (3)**, **Ingresos (4)** y **Gastos / Costos (5 y 6)** de la Fundación.

#### Clase 3 — Patrimonio Institucional

| Código | Descripción | Nivel Jerárquico | Naturaleza | Asentable | Exige Tercero |
|--------|-------------|:----------------:|:----------:|:---------:|:-------------:|
| `3` | PATRIMONIO INSTITUCIONAL | Clase (1) | Crédito | **Falso** | Falso |
| `31` | Fondo Social | Grupo (2) | Crédito | **Falso** | Falso |
| `3105` | Aportes de Fundadores | Cuenta (3) | Crédito | **Falso** | Falso |
| `310505` | Aportes en Dinero | Subcuenta (4) | Crédito | **Verdadero** | **Verdadero** |
| `310510` | Aportes en Especie | Subcuenta (4) | Crédito | **Verdadero** | **Verdadero** |
| `3115` | Fondo de Destinación Específica | Cuenta (3) | Crédito | **Falso** | Falso |
| `311505` | Reserva para proyectos misionales | Subcuenta (4) | Crédito | **Verdadero** | Falso |
| `32` | Resultados del Ejercicio (No Utilidades) | Grupo (2) | Crédito | **Falso** | Falso |
| `3205` | Excedente del Ejercicio | Cuenta (3) | Crédito | **Verdadero** | Falso |
| `3210` | Déficit del Ejercicio | Cuenta (3) | **Débito** | **Verdadero** | Falso |

#### Clase 4 — Ingresos

| Código | Descripción | Nivel Jerárquico | Naturaleza | Asentable | Exige Tercero |
|--------|-------------|:----------------:|:----------:|:---------:|:-------------:|
| `4` | INGRESOS | Clase (1) | Crédito | **Falso** | Falso |
| `41` | Ingresos de Actividades Ordinarias | Grupo (2) | Crédito | **Falso** | Falso |
| `4105` | Aportes y Cuotas de Sostenimiento | Cuenta (3) | Crédito | **Falso** | Falso |
| `410505` | Cuotas de Afiliación (Nuevos) | Subcuenta (4) | Crédito | **Verdadero** | **Verdadero** |
| `410510` | Cuotas de Sostenimiento (Mensualidad) | Subcuenta (4) | Crédito | **Verdadero** | **Verdadero** |
| `4110` | Ingresos por Eventos y Actividades | Cuenta (3) | Crédito | **Falso** | Falso |
| `411005` | Inscripciones a Rodadas y Eventos | Subcuenta (4) | Crédito | **Verdadero** | **Verdadero** |
| `411010` | Venta de Merchandising (Parches, etc.) | Subcuenta (4) | Crédito | **Verdadero** | Falso |
| `4115` | Donaciones Recibidas | Cuenta (3) | Crédito | **Falso** | Falso |
| `411505` | Donaciones No Condicionadas (Libres) | Subcuenta (4) | Crédito | **Verdadero** | **Verdadero** |
| `411510` | Donaciones Condicionadas (Proyectos) | Subcuenta (4) | Crédito | **Verdadero** | **Verdadero** |

#### Clase 5 — Gastos Administrativos

| Código | Descripción | Nivel Jerárquico | Naturaleza | Asentable | Exige Tercero |
|--------|-------------|:----------------:|:----------:|:---------:|:-------------:|
| `5` | GASTOS ADMINISTRATIVOS | Clase (1) | Débito | **Falso** | Falso |
| `51` | Operación y Administración | Grupo (2) | Débito | **Falso** | Falso |
| `5105` | Gastos de Representación | Cuenta (3) | Débito | **Falso** | Falso |
| `510505` | Reuniones de Junta Directiva | Subcuenta (4) | Débito | **Verdadero** | Falso |
| `5110` | Honorarios y Servicios | Cuenta (3) | Débito | **Falso** | Falso |
| `511005` | Honorarios Contables y Legales | Subcuenta (4) | Débito | **Verdadero** | **Verdadero** |

#### Clase 6 — Costos de Proyectos Misionales

| Código | Descripción | Nivel Jerárquico | Naturaleza | Asentable | Exige Tercero |
|--------|-------------|:----------------:|:----------:|:---------:|:-------------:|
| `6` | COSTOS DE PROYECTOS MISIONALES | Clase (1) | Débito | **Falso** | Falso |
| `61` | Costos de Eventos y Rodadas | Grupo (2) | Débito | **Falso** | Falso |
| `6105` | Logística de Eventos | Cuenta (3) | Débito | **Falso** | Falso |
| `610505` | Alquiler de Espacios / Permisos | Subcuenta (4) | Débito | **Verdadero** | **Verdadero** |
| `610510` | Alimentación y Refrigerios | Subcuenta (4) | Débito | **Verdadero** | **Verdadero** |
| `610515` | Reconocimientos y Trofeos | Subcuenta (4) | Débito | **Verdadero** | **Verdadero** |

> **Nota:** Las clases **1 (Activos)** y **2 (Pasivos)** se agregarán en una iteración posterior; son de estructura estándar comercial y no requieren adaptación conceptual para ESAL.

---

### 4.3 Consideraciones técnicas adicionales para el Back-End

#### Cierre contable anual

El sistema debe incluir un procedimiento (Stored Procedure o job de fin de año) que:

1. Tome los saldos acumulados de la **Clase 4** (Ingresos).
2. Reste los saldos de la **Clase 5** (Gastos) y **Clase 6** (Costos).
3. Deposite la diferencia en:
   - **`3205` — Excedente del Ejercicio** si el resultado es positivo (Ingresos > Gastos + Costos)
   - **`3210` — Déficit del Ejercicio** si el resultado es negativo (Gastos + Costos > Ingresos)

> **Importante:** en lugar de enviar este saldo a una cuenta de "Utilidad" (como en el PUC comercial), el código debe usar las cuentas ESAL `3205` / `3210`.

#### Manejo de donaciones condicionadas y Centros de Costo

Las donaciones que exigen ejecución en un proyecto específico (ej. cuenta `411510`) requieren un seguimiento paralelo mediante un sistema de **Centros de Costo**.

Se recomienda estructurar una tabla relacional secundaria `CentrosCosto` que pueda asociarse a cualquier transacción en las cuentas de **Clase 4** y **Clase 6**, para que la fundación pueda reportar exactamente cuánto dinero ingresó y se gastó por un evento específico.

> Este requerimiento ya está cubierto en la entidad `CentroCosto` implementada (`TipoCentroCosto`: CAPITULO / FUNDACION / PROYECTO / EVENTO). La vinculación con los asientos contables se completará en Phase 1.

#### Modelo de datos sugerido para `CuentasContables`

```sql
CREATE TABLE CuentasContables (
    Id             UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    Codigo         NVARCHAR(20)     NOT NULL,  -- Solo dígitos; longitudes: 1, 2, 4, 6, 8+
    Descripcion    NVARCHAR(300)    NOT NULL,
    Naturaleza     INT              NOT NULL,  -- 1=Débito, 2=Crédito
    PermiteMovimiento BIT           NOT NULL,  -- True = nodo hoja (asentable)
    ExigeTercero   BIT              NOT NULL,  -- True = NIT/Cédula obligatorio en asiento
    CuentaPadreId  UNIQUEIDENTIFIER NULL,      -- FK self-referential
    IsDeleted      BIT              NOT NULL DEFAULT 0,

    CONSTRAINT UQ_CuentasContables_Codigo UNIQUE (Codigo),
    CONSTRAINT FK_CuentasContables_Padre
        FOREIGN KEY (CuentaPadreId) REFERENCES CuentasContables(Id)
        ON DELETE NO ACTION
);

-- Índices
CREATE INDEX IX_CuentasContables_CuentaPadreId ON CuentasContables (CuentaPadreId);
```

> **Nota de diseño:** El campo `Nivel` (Clase/Grupo/Cuenta/Subcuenta/Auxiliar) **NO se persiste** en la base de datos; se calcula a partir de la longitud del `Codigo`. Esto evita inconsistencias entre el código almacenado y el nivel declarado.

---

## 5. Estado de implementación

| Componente | Archivo | Estado |
|---|---|:---:|
| Enum `NaturalezaCuenta` | `Domain/Enums/NaturalezaCuenta.cs` | ✅ |
| Enum `NivelCuenta` | `Domain/Enums/NivelCuenta.cs` | ✅ |
| Entidad `CuentaContable` | `Domain/Entities/CuentaContable.cs` | ✅ |
| Interfaz repositorio | `Application/Interfaces/ICuentaContableRepository.cs` | ✅ |
| Query `GetCatalogoCuentas` | `Application/Features/Contabilidad/Queries/…` | ✅ |
| Config EF Core | `Infrastructure/Configurations/CuentaContableConfiguration.cs` | ✅ |
| Repositorio concreto | `Infrastructure/Repositories/CuentaContableRepository.cs` | ✅ |
| Seeder (34 cuentas) | `Infrastructure/Seeders/CuentaContableSeeder.cs` | ✅ |
| Migración BD | `Migrations/20260224181213_AddCuentasContables.cs` | ✅ |
| Controller API | `API/Controllers/CuentasContablesController.cs` | ✅ |
| Hook React Query | `frontend/src/features/contabilidad/hooks/useCuentasContables.ts` | ✅ |
| Tabla UI | `frontend/src/features/contabilidad/components/TablaCuentasContables.tsx` | ✅ |
| Página `/contabilidad/cuentas` | `frontend/src/app/contabilidad/cuentas/page.tsx` | ✅ |
| Acceso rápido en dashboard | `frontend/src/app/page.tsx` | ✅ |
| Tests unitarios (26 tests) | `Application.Tests/Domain/Entities/CuentaContableTests.cs` | ✅ |
| Validación `PermiteMovimiento` en asientos | Pendiente — Phase 1 | ⬜ |
| Validación `ExigeTercero` en asientos | Pendiente — Phase 1 | ⬜ |
| Cierre anual ESAL (3205 / 3210) | Pendiente — Phase 1 Sprint 5 | ⬜ |
| Clases 1 (Activos) y 2 (Pasivos) en catálogo | Pendiente — Phase 1 | ⬜ |
| Audit trail para cambios manuales al catálogo | Pendiente — transversal | ⬜ |

### Cómo acceder

**Desde el sistema (UI):**  
`Inicio → Catálogo de Cuentas` *(acceso rápido en el dashboard)*  
O directamente: `/contabilidad/cuentas`

**Desde el API:**
```
GET /api/cuentas-contables            → Catálogo completo (34 cuentas)
GET /api/cuentas-contables/asentables → Solo nodos hoja (14 cuentas asentables)
```

---

## 6. Trazabilidad de requerimientos

| Requerimiento (SRS) | Historia | Archivo implementación | Estado |
|---|---|---|:---:|
| RF-CFG-05: Importar/cargar PUC ESAL | STORY-CFG-PUC-01 | `CuentaContableSeeder.cs` | ✅ Precargado |
| RF-CFG-05: Marcar cuentas de movimiento | STORY-CFG-PUC-01 | `CuentaContable.PermiteMovimiento` | ✅ |
| RF-CFG-05: Jerarquía padre–hijo | STORY-CFG-PUC-01 | `CuentaContable.CuentaPadreId` | ✅ |
| RF-CFG-06: Naturaleza DEBITO/CREDITO | STORY-CFG-PUC-01 | `NaturalezaCuenta` enum | ✅ |
| RF-CFG-06: ExigeTercero para exógena | STORY-CFG-PUC-01 | `CuentaContable.ExigeTercero` | ✅ |
| RF-CONT-01: Consultar catálogo en UI | STORY-CFG-PUC-02 | `TablaCuentasContables.tsx` | ✅ |
| RF-CONT-02: Comprobantes balanceados | STORY-1-3 (pendiente) | — | ⬜ Phase 1 |
| RF-CONT-06: Cierre mensual | STORY-1-2 (pendiente) | — | ⬜ Phase 1 |
| Cierre anual ESAL 3205/3210 | STORY-CFG-PUC-01 (CA-ANUAL) | — | ⬜ Phase 1 Sprint 5 |

---

*Documento generado el 2026-02-24. Para cambios en el catálogo de cuentas, coordinar con el Contador y actualizar el seeder + esta especificación.*
