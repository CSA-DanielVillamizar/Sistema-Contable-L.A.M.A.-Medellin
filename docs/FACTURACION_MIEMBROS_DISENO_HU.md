# Diseño breve y Historias de Usuario - Facturación de Miembros

## 1) Módulo de Parametrización de Cartera

### Objetivo
Permitir que el Tesorero configure el valor vigente de la cuota mensual, con trazabilidad auditable soportada por acta de asamblea.

### Diseño funcional
- El sistema conserva histórico por vigencias (no se sobreescribe el registro anterior).
- Toda modificación exige `ActaAsamblea`.
- Solo usuarios con rol Tesorero (o autorizado) pueden crear una nueva vigencia.

### Estructura sugerida: `ConfiguracionCarteraCuota`
| Campo | Tipo | Regla |
|---|---|---|
| Id | uniqueidentifier | PK |
| ValorCuotaMensualCOP | decimal(18,2) | Obligatorio, > 0 |
| PeriodoVigenciaDesde | char(7) | Formato `YYYY-MM`, obligatorio |
| PeriodoVigenciaHasta | char(7) | `YYYY-MM`, opcional |
| ActaAsamblea | nvarchar(50) | Obligatorio |
| FechaActa | date | Obligatorio |
| MotivoCambio | nvarchar(300) | Opcional |
| UsuarioModificacion | nvarchar(100) | Obligatorio |
| FechaModificacion | datetime2 | Obligatorio |
| IsDeleted | bit | Soft delete |

Regla de consistencia: no debe existir traslape de periodos vigentes para la misma configuración.

---

## 2) Conceptos de Cartera y Naturaleza Contable

### Tabla sugerida: `ConceptosFacturacion`
| Campo | Tipo | Regla |
|---|---|---|
| Id | uniqueidentifier | PK |
| Codigo | nvarchar(50) | Unico. Valores iniciales: `CUOTA_MENSUAL`, `RENOVACION_INTERNACIONAL` |
| Nombre | nvarchar(150) | Obligatorio |
| ClaseContable | tinyint | 2=Pasivo, 4=Ingreso |
| CuentaContableId | uniqueidentifier | FK a `CuentasContables` |
| MonedaBase | nvarchar(3) | `COP` o `USD` |
| ValorBase | decimal(18,2) | Obligatorio, > 0 |
| EsRecaudoTerceros | bit | Para separar ingreso propio vs recaudo para terceros |
| RequiereTRM | bit | Obligatorio para conceptos en USD |
| Activo | bit | Estado logico |
| IsDeleted | bit | Soft delete |

### Parametros iniciales recomendados
1. `CUOTA_MENSUAL`
   - `ClaseContable = 4`
   - `MonedaBase = COP`
   - `EsRecaudoTerceros = false`
   - Cuenta sugerida: ingreso por cuotas de sostenimiento (Clase 4).

2. `RENOVACION_INTERNACIONAL`
   - `ClaseContable = 2`
   - `MonedaBase = USD`
   - `ValorBase = 20.00`
   - `EsRecaudoTerceros = true`
   - `RequiereTRM = true`
   - Cuenta sugerida: pasivo por recaudos para terceros (Clase 2).

Esta separación evita inflar ingresos fiscales de la ESAL local ante DIAN cuando el capítulo actúa solo como intermediario.

---

## 3) Gestión Multimoneda (cobro de 20 USD en diciembre)

### Regla de negocio
- En diciembre, al facturar `RENOVACION_INTERNACIONAL`, se liquida `20 USD`.
- El sistema convierte a COP con TRM del momento del pago/cobro.
- Moneda funcional y de reporte: COP.

### Datos obligatorios cuando el concepto sea en USD
- `MontoMonedaOrigen` (USD)
- `TasaCambioUsada`
- `FechaTasaCambio`
- `FuenteTasaCambio`
- `MontoCOP`

### Diferencia en cambio
Si el COP estimado y el COP real difieren al recaudar o liquidar:
- Diferencia positiva: registrar ingreso por diferencia en cambio.
- Diferencia negativa: registrar gasto por diferencia en cambio.

El asiento debe conservar referencia al documento origen para auditoría.

---

## 4) Historias de Usuario

### HU-01 - Parametrizar cuota mensual con acta
**Como** Tesorero  
**Quiero** actualizar el valor de la cuota mensual vigente  
**Para** aplicar lo aprobado por asamblea con trazabilidad auditable.

**Criterios de aceptación**
- No se guarda el cambio sin `ActaAsamblea`.
- Si falta `ActaAsamblea`, el sistema muestra validación y no permite guardar.
- No se permiten valores `<= 0`.
- Se registra histórico por vigencia sin borrar el anterior.
- Se guarda usuario y fecha de modificación.

### HU-02 - Administrar conceptos de facturacion con naturaleza contable
**Como** Contador/Tesorero  
**Quiero** configurar `ConceptosFacturacion`  
**Para** mapear cada cobro al tratamiento contable correcto NIIF/DIAN.

**Criterios de aceptación**
- `CUOTA_MENSUAL` usa clase contable 4 (ingreso).
- `RENOVACION_INTERNACIONAL` usa clase contable 2 (pasivo/recaudo para terceros).
- `Codigo` es unico.
- No se permite guardar sin cuenta contable valida.

### HU-03 - Cobrar renovación internacional en diciembre (USD)
**Como** Tesorero  
**Quiero** generar el cobro de renovacion internacional por 20 USD en diciembre  
**Para** recaudar y transferir a L.A.M.A. Internacional sin reconocerlo como ingreso propio.

**Criterios de aceptación**
- El cobro se activa en diciembre.
- Exige TRM, fecha y fuente de tasa.
- Registra moneda origen USD y conversión a COP.
- El asiento principal usa cuenta de clase 2.

### HU-04 - Reconocer diferencia en cambio
**Como** Contador  
**Quiero** que el sistema registre automáticamente la diferencia en cambio  
**Para** reflejar correctamente el resultado cambiario.

**Criterios de aceptación**
- Si hay diferencia entre COP calculado y COP real, se genera asiento automático.
- La diferencia positiva va a ingreso por diferencia en cambio.
- La diferencia negativa va a gasto por diferencia en cambio.
- El asiento queda vinculado al comprobante o recaudo origen.
