# Backlog (Épicas y Historias) — para GitHub Issues

## Labels sugeridos
epic, story, task  
phase:0..phase:5  
area:iam, area:infra, area:accounting, area:treasury, area:members, area:donations, area:projects, area:business, area:reports  
priority:high|medium|low

## Milestones sugeridos
- Phase 0 - Foundations
- Phase 1 - MVP Contabilidad + Cuotas
- Phase 2 - Donaciones
- Phase 3 - Proyectos
- Phase 4 - Negocios
- Phase 5 - Tributario avanzado

## Épicas
1. [Epic][Phase 0] IAM Entra External ID + Roles internos + MFA
2. [Epic][Phase 0] Infra mínima Azure + Observabilidad + Key Vault + Blob
3. [Epic][Phase 0] Modelo base: Centros de costo + Medios de pago + Terceros + Mapeo contable
4. [Epic][Phase 1] Contabilidad general (PUC, comprobantes, libros, cierres)
5. [Epic][Phase 1] Tesorería bancarizada + recibos + anulaciones
6. [Epic][Phase 1] Cuotas miembros + CxC + mora + recaudo
7. [Epic][Phase 1] CxP proveedores (facturas, vencimientos, pagos)
8. [Epic][Phase 1] Multimoneda informativa USD + Diferencia en cambio
9. [Epic][Phase 2] Donaciones + campañas + certificados obligatorios
10. [Epic][Phase 3] Proyectos sociales + beneficiarios + consentimiento + rendición
11. [Epic][Phase 4] Negocios (inventario simple, compras/ventas, comprobante interno)
12. [Epic][Phase 5] Reportes tributarios base (exógena, beneficiarios finales)
13. [Epic][Phase X] Preparación para facturación electrónica (estructura/adapter) — NO implementar

## Historias (mínimo para iniciar)
- [Story][Phase 0] Integrar login OIDC con Entra External ID
- [Story][Phase 0] CRUD roles internos (Admin/Operador/Tesorero/Contador/Junta) con auditoría
- [Story][Phase 0] Parametrizar centros de costo CAPITULO/FUNDACION/PROYECTO/EVENTO
- [Story][Phase 0] Catálogo de medios de pago obligatorio en ingresos/egresos
- [Story][Phase 0] Importar PUC ESAL desde archivo entregado por contador
- [Story][Phase 1] Registrar comprobantes contables balanceados (debe=haber)
- [Story][Phase 1] Ejecutar cierre mensual con bloqueo (valida tesorero, ejecuta contador)
- [Story][Phase 1] Registrar cuota anual con mes inicio y soporte acta
- [Story][Phase 1] Generar obligaciones mensuales de cuotas (CxC)
- [Story][Phase 1] Registrar pago bancario y aplicar a obligaciones (anticipos permitidos)
- [Story][Phase 1] Registrar factura proveedor (CxP) y pago posterior cruzando obligación
- [Story][Phase 1] Multimoneda USD: capturar USD/COP/tasa/soporte
- [Story][Phase 1] Diferencia en cambio automática al liquidar CxP/CxC en USD
- [Story][Phase 2] Registrar donación y emitir certificado obligatorio con QR
- [Story][Phase 3] Beneficiarios con consentimiento obligatorio para PII
- [Story][Phase 4] Venta merch con comprobante interno PDF + QR
- [Story][Phase 5] Reporte estructurado exógena (exportable)