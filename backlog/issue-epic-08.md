## Objetivo
Permitir registrar USD como moneda origen (informativa) con COP como valor contable oficial, y manejar diferencia en cambio cuando aplique.

## Alcance
- Campos USD (monto, tasa, fecha, fuente, soporte) cuando MonedaOrigen=USD
- COP siempre como moneda funcional y contable
- Sin reexpresión mensual (no hay cuenta USD)
- Diferencia en cambio automática al liquidar CxP/CxC en USD si COP difiere

## Criterios de aceptación
- [ ] Reportes contables solo en COP
- [ ] USD requiere tasa/fuente/soporte obligatorios
- [ ] Diferencia en cambio a cuentas:
  - Ingresos por diferencia en cambio
  - Gastos por diferencia en cambio