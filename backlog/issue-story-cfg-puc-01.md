# [Story][Phase 0] Construir y precargar Catálogo de Cuentas (PUC adaptado ESAL – NIIF Grupo III)

## Como
**Contador** de la Fundación L.A.M.A. Medellín

## Quiero
Que el sistema disponga de un Catálogo de Cuentas (PUC) propio, jerárquico, precargado con las cuentas de Patrimonio, Ingresos, Gastos y Costos acordes al marco normativo NIIF para Microempresas (Grupo III), desde el arranque del sistema

## Para
Arrancar la contabilidad formal sin necesidad de importar un archivo externo, garantizando que:
- Los asientos contables solo puedan registrarse en cuentas habilitadas para movimiento (*nodos hoja*).
- El catálogo refleje la naturaleza jurídica de la fundación (ESAL vigilada por la Gobernación de Antioquia).
- Cada cuenta que lo exija obligue a identificar un tercero (NIT/Cédula), cumpliendo el requerimiento de información exógena DIAN.
- Los cierres anuales utilicen las cuentas 3205 (Excedente del Ejercicio) y 3210 (Déficit del Ejercicio) en lugar de "Utilidades", tal como lo exige el modelo contable ESAL.

---

## Contexto normativo

| Campo | Valor |
|---|---|
| Marco normativo | NIIF para Microempresas (Grupo III) |
| Tamaño de empresa | Micro |
| Responsabilidad tributaria | 42 – Obligado a llevar contabilidad |
| Inspección, vigilancia y control | Gobernación de Antioquia |
| Otras responsabilidades activas | 05-Renta, 07-Retención en la Fuente, 48-IVA, 14-Información Exógena |
| Actividades económicas (CIIU) | 9319, 9329, 7990, 5630 |

> Como la fundación **no está vigilada por una Superintendencia estricta**, no está obligada a un PUC predeterminado por el Estado. Sin embargo, utiliza la codificación numérica tradicional del antiguo PUC comercial para facilitar el cumplimiento tributario.

---

## Reglas de negocio

1. **Estructura jerárquica de 5 niveles** determinada por la longitud del código numérico:
   - Nivel 1 – Clase: 1 dígito (ej. `3`)
   - Nivel 2 – Grupo: 2 dígitos (ej. `31`)
   - Nivel 3 – Cuenta: 4 dígitos (ej. `3105`)
   - Nivel 4 – Subcuenta: 6 dígitos (ej. `310505`)
   - Nivel 5 – Auxiliar: 8+ dígitos (ej. `31050501`)

2. **Solo los nodos hoja** (`PermiteMovimiento = true`) pueden recibir asientos contables. Clases, Grupos y Cuentas son únicamente agrupadores.

3. **Naturaleza de la cuenta** (`Naturaleza = DEBITO | CREDITO`): define si el saldo de la cuenta crece por débitos o por créditos.

4. **Exige Tercero** (`ExigeTercero = true`): toda línea de asiento sobre esta cuenta debe identificar al tercero (NIT o Cédula). Obligatorio para la información exógena DIAN.

5. **Relación padre–hijo**: cada cuenta conoce su cuenta padre (mediante `CuentaPadreId`), lo que permite sumarizar saldos jerárquicamente para estados financieros.

6. **Cuentas de cierre anual ESAL**:
   - `3205 – Excedente del Ejercicio` (si Ingresos > Gastos+Costos) → naturaleza **Crédito**
   - `3210 – Déficit del Ejercicio` (si Gastos+Costos > Ingresos) → naturaleza **Débito**
   - Estas cuentas reemplazan conceptualmente "Utilidad/Pérdida del Ejercicio" del PUC comercial.

7. **Adaptación de nombres patrimoniales**: se usa "Fondo Social" (31) en lugar de "Capital Social", y "Patrimonio Institucional" (3) en lugar de "Patrimonio".

8. **Codificación de ingresos por actividad**: se crean cuentas de ingresos alineadas a las actividades CIIU de la fundación (rodadas, eventos, merchandising, donaciones).

---

## Catálogo de cuentas a precargar

### Clase 3 – Patrimonio Institucional

| Código | Descripción | Naturaleza | Permite Movimiento | Exige Tercero |
|--------|-------------|:----------:|:------------------:|:-------------:|
| `3` | PATRIMONIO INSTITUCIONAL | Crédito | No | No |
| `31` | Fondo Social | Crédito | No | No |
| `3105` | Aportes de Fundadores | Crédito | No | No |
| `310505` | Aportes en Dinero | Crédito | **Sí** | **Sí** |
| `310510` | Aportes en Especie | Crédito | **Sí** | **Sí** |
| `3115` | Fondo de Destinación Específica | Crédito | No | No |
| `311505` | Reserva para proyectos misionales | Crédito | **Sí** | No |
| `32` | Resultados del Ejercicio (No Utilidades) | Crédito | No | No |
| `3205` | Excedente del Ejercicio | Crédito | **Sí** | No |
| `3210` | Déficit del Ejercicio | **Débito** | **Sí** | No |

### Clase 4 – Ingresos

| Código | Descripción | Naturaleza | Permite Movimiento | Exige Tercero |
|--------|-------------|:----------:|:------------------:|:-------------:|
| `4` | INGRESOS | Crédito | No | No |
| `41` | Ingresos de Actividades Ordinarias | Crédito | No | No |
| `4105` | Aportes y Cuotas de Sostenimiento | Crédito | No | No |
| `410505` | Cuotas de Afiliación (Nuevos) | Crédito | **Sí** | **Sí** |
| `410510` | Cuotas de Sostenimiento (Mensualidad) | Crédito | **Sí** | **Sí** |
| `4110` | Ingresos por Eventos y Actividades | Crédito | No | No |
| `411005` | Inscripciones a Rodadas y Eventos | Crédito | **Sí** | **Sí** |
| `411010` | Venta de Merchandising (Parches, etc.) | Crédito | **Sí** | No |
| `4115` | Donaciones Recibidas | Crédito | No | No |
| `411505` | Donaciones No Condicionadas (Libres) | Crédito | **Sí** | **Sí** |
| `411510` | Donaciones Condicionadas (Proyectos) | Crédito | **Sí** | **Sí** |

### Clase 5 – Gastos Administrativos

| Código | Descripción | Naturaleza | Permite Movimiento | Exige Tercero |
|--------|-------------|:----------:|:------------------:|:-------------:|
| `5` | GASTOS ADMINISTRATIVOS | Débito | No | No |
| `51` | Operación y Administración | Débito | No | No |
| `5105` | Gastos de Representación | Débito | No | No |
| `510505` | Reuniones de Junta Directiva | Débito | **Sí** | No |
| `5110` | Honorarios y Servicios | Débito | No | No |
| `511005` | Honorarios Contables y Legales | Débito | **Sí** | **Sí** |

### Clase 6 – Costos de Proyectos Misionales

| Código | Descripción | Naturaleza | Permite Movimiento | Exige Tercero |
|--------|-------------|:----------:|:------------------:|:-------------:|
| `6` | COSTOS DE PROYECTOS MISIONALES | Débito | No | No |
| `61` | Costos de Eventos y Rodadas | Débito | No | No |
| `6105` | Logística de Eventos | Débito | No | No |
| `610505` | Alquiler de Espacios / Permisos | Débito | **Sí** | **Sí** |
| `610510` | Alimentación y Refrigerios | Débito | **Sí** | **Sí** |
| `610515` | Reconocimientos y Trofeos | Débito | **Sí** | **Sí** |

> **Nota:** Las clases 1 (Activos) y 2 (Pasivos) se agregarán en una siguiente iteración; son de estructura estándar y no requieren adaptación conceptual.

---

## Criterios de aceptación

### Estructura del catálogo
- [x] El sistema almacena `Código`, `Descripción`, `Naturaleza`, `PermiteMovimiento`, `ExigeTercero` y `CuentaPadreId` por cada cuenta.
- [x] El nivel jerárquico (`Clase`, `Grupo`, `Cuenta`, `Subcuenta`, `Auxiliar`) se deriva automáticamente de la longitud del código numérico; no se persiste.
- [x] El código es único en la base de datos (índice único sobre `Codigo`).
- [x] El código acepta solo dígitos numéricos y longitudes válidas (1, 2, 4, 6, u 8+).
- [x] Cada cuenta (excepto las de Clase/raíz) tiene referencia correcta a su cuenta padre.

### Datos iniciales
- [x] Las 34 cuentas listadas arriba están disponibles desde el primer arranque del sistema en el entorno de desarrollo (seed idempotente).
- [x] Las cuentas `3205` y `3210` existen y tienen la naturaleza y movimiento correctos para el cierre anual ESAL.
- [x] Las cuentas `411510`, `410505`, `410510`, `411005`, `411505`, `511005`, `610505`, `610510`, `610515`, `310505`, `310510` tienen `ExigeTercero = true`.

### API REST (consulta)
- [x] `GET /api/cuentas-contables` retorna el catálogo completo ordenado por código con todos los campos del DTO.
- [x] `GET /api/cuentas-contables/asentables` retorna solo las cuentas con `PermiteMovimiento = true`, para usar en selectores de comprobantes.
- [x] Ambos endpoints requieren autenticación (`[Authorize]`).
- [x] La respuesta incluye: `Id`, `Codigo`, `Descripcion`, `Nivel` (int), `NivelNombre` (string), `NaturalezaNombre` (string), `PermiteMovimiento`, `ExigeTercero`, `CuentaPadreId`.

### Integridad
- [ ] El sistema **no permite** guardar un asiento contable en una cuenta con `PermiteMovimiento = false`. *(Pendiente: aplica al módulo de Comprobantes – Phase 1)*
- [ ] El sistema **no permite** guardar un asiento en cuenta con `ExigeTercero = true` si no se especifica el tercero. *(Pendiente: aplica al módulo de Comprobantes – Phase 1)*
- [x] El catálogo tiene soft delete: una cuenta puede marcarse como inactiva sin eliminarla físicamente.

### Auditoría
- [ ] Los cambios manuales al catálogo (alta/modificación/desactivación) quedan registrados con usuario, fecha y valor anterior/nuevo. *(Pendiente: audit trail transversal – Fase siguiente)*

---

## Datos y modelo de persistencia

```
Tabla: CuentasContables
┌──────────────────────┬────────────────────────┬──────────────────────────────────┐
│ Campo                │ Tipo SQL               │ Restricciones                    │
├──────────────────────┼────────────────────────┼──────────────────────────────────┤
│ Id                   │ uniqueidentifier       │ PK, NOT NULL                     │
│ Codigo               │ nvarchar(20)           │ NOT NULL, UNIQUE                 │
│ Descripcion          │ nvarchar(300)          │ NOT NULL                         │
│ Naturaleza           │ int                    │ NOT NULL (1=Débito, 2=Crédito)   │
│ PermiteMovimiento    │ bit                    │ NOT NULL                         │
│ ExigeTercero         │ bit                    │ NOT NULL                         │
│ CuentaPadreId        │ uniqueidentifier       │ FK→CuentasContables(Id), NULL    │
│ IsDeleted            │ bit                    │ NOT NULL, DEFAULT 0              │
└──────────────────────┴────────────────────────┴──────────────────────────────────┘

Índices:
- IX_CuentasContables_Codigo (UNIQUE)
- IX_CuentasContables_CuentaPadreId
FK OnDelete: RESTRICT (no se puede eliminar un padre con hijos activos)
```

---

## Notas técnicas

### Capas implementadas

| Capa | Artefacto | Descripción |
|------|-----------|-------------|
| Domain | `CuentaContable.cs` | Entidad raíz; valida código, descripción, auto-resuelve nivel |
| Domain | `NaturalezaCuenta.cs` | Enum `Debito=1 / Credito=2` |
| Domain | `NivelCuenta.cs` | Enum `Clase / Grupo / Cuenta / Subcuenta / Auxiliar` |
| Application | `ICuentaContableRepository` | Interfaz de acceso a datos |
| Application | `GetCatalogoCuentasQuery` | Query CQRS con filtro `SoloAsentables` |
| Application | `CuentaContableDto` | DTO de salida (Id, Codigo, Descripcion, Nivel, Naturaleza, PermiteMovimiento, ExigeTercero, CuentaPadreId) |
| Infrastructure | `CuentaContableConfiguration` | Configuración Fluent API EF Core (FK self-ref, índice único) |
| Infrastructure | `CuentaContableRepository` | Implementación repositorio |
| Infrastructure | `CuentaContableSeeder` | Precarga idempotente de las 34 cuentas |
| Infrastructure | `20260224181213_AddCuentasContables` | Migración EF Core |
| API | `CuentasContablesController` | `GET /api/cuentas-contables` + `GET /api/cuentas-contables/asentables` |

### Consideraciones de diseño

- **Nivel calculado, no persistido**: derivar el nivel de la longitud del código evita inconsistencias entre la longitud y el nivel guardado. El campo `Nivel` en la entidad es una propiedad calculada (`=>`) y está configurado como `Ignored` en EF Core.
- **Seed idempotente**: el seeder verifica `context.CuentasContables.AnyAsync()` antes de insertar, lo que lo hace seguro para ejecutarse múltiples veces sin duplicar datos.
- **Construcción del árbol en dos pasadas**: el seeder instancia primero todas las entidades y luego resuelve padres usando un diccionario `Codigo → CuentaContable`, garantizando la integridad del árbol sin dependencias de orden.

### API: Permisos
- Ambos endpoints requieren JWT de Entra External ID (`[Authorize]`).
- No hay restricción de rol específica todavía (pendiente RBAC interno).

### Pendiente para Phase 1 (Comprobantes)
- Validar `PermiteMovimiento = true` al crear líneas de asiento.
- Validar `ExigeTercero = true` → tercero obligatorio en la línea.
- El cierre anual deberá acumular saldos de Clase 4 menos Clases 5 y 6, y depositar el resultado en `3205` o `3210`.

---

## Eventos y auditoría

| Acción | Debe auditar | Estado |
|--------|:------------:|--------|
| Precarga inicial del catálogo (seed) | No (es automático en dev) | ✅ Implementado |
| Consulta del catálogo | No | ✅ Implementado |
| Alta manual de nueva cuenta | Sí (quién/cuándo/código/descripción) | ⬜ Pendiente |
| Modificación de descripción/naturaleza | Sí | ⬜ Pendiente |
| Desactivación (soft delete) de cuenta | Sí | ⬜ Pendiente |

---

## Definición de Listo (Definition of Done)

- [x] Código implementado y compila sin errores ni warnings
- [x] Entidad de dominio con validaciones completas
- [x] 26 tests unitarios nuevos: 22 de entidad + 4 de handler — **38/38 passing**
- [x] Migración EF Core generada y snapshot actualizado
- [x] Seeder idempotente con las 34 cuentas iniciales
- [x] API expone dos endpoints protegidos
- [x] 0 alertas CodeQL
- [x] Sin credenciales ni secretos en el código
- [x] Sigue estándares Clean Architecture del proyecto
- [ ] Endpoint documentado con Swagger/OpenAPI *(pendiente habilitación de Swagger en el proyecto)*
- [ ] Audit trail para cambios manuales *(sprint posterior)*
- [ ] Restricción `PermiteMovimiento` en módulo Comprobantes *(Phase 1)*

---

## Referencias

- **Épica padre:** `[Epic][Phase 0] Modelo base: Centros de costo + Medios de pago + Terceros + Mapeo contable` (issue-epic-03)
- **Historia relacionada:** `[Story][Phase 0] Importar PUC ESAL desde archivo entregado por contador` (issue-story-1-1) — esta historia es la continuación manual vía UI
- **Historia que desbloquea:** `[Story][Phase 1] Registrar comprobantes contables balanceados (debe=haber)` (issue-story-1-3)
- **Implementación:** `LAMAMedellin/src/LAMAMedellin.Domain/Entities/CuentaContable.cs`
- **Seeder:** `LAMAMedellin/src/LAMAMedellin.Infrastructure/Persistence/Seeders/CuentaContableSeeder.cs`
- **Migración:** `LAMAMedellin/src/LAMAMedellin.Infrastructure/Migrations/20260224181213_AddCuentasContables.cs`
- **BRD/SRS:** `docs/docs_BRD-SRS_Version2.md` — Sección 7.4 (RF-CFG-05, RF-CFG-06)

---

## Labels
`story` · `priority:high` · `effort:L` · `phase:0` · `area:accounting` · `area:infra`

## Milestone
Phase 0 – Foundations

## Estimación de esfuerzo
**L (Large) – 3 días** (diseño del modelo, implementación completa, seeder, tests, revisión normativa)
