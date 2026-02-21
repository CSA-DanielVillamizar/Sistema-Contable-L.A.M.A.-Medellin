## Objetivo
Implementar catálogos y configuración base obligatoria para toda operación: centros de costo, medios de pago, terceros y mapeos contables.

## Alcance
- Centros de costo: CAPITULO/FUNDACION/PROYECTO/EVENTO
- Medios de pago (obligatorio)
- Terceros unificados (miembro/donante/cliente/proveedor)
- Mapeo contable por operación (cuotas/donaciones/merch/gastos naturaleza/inventario/diferencia cambio)

## Criterios de aceptación
- [ ] No se guarda transacción monetaria sin CC
- [ ] No se guarda transacción monetaria sin medio de pago
- [ ] Mapeos contables configurables (no hardcode)