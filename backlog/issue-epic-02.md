## Objetivo
Tener infraestructura mínima costo-sostenible en Azure para web, api, SQL, Blob, Key Vault y observabilidad.

## Alcance (In / Out)
**Incluye**
- App Service (api)
- Hosting Next.js (Static Web Apps o App Service Node según decisión)
- Azure SQL
- Storage Account Blob (soportes)
- Key Vault + Managed Identity
- Application Insights + alertas básicas

**No incluye**
- Alta disponibilidad multi-región (fase futura)
- Service Bus/colas (solo si se requiere después)

## Entregables
- [ ] Recursos Azure creados (ideal IaC: Bicep/Terraform)
- [ ] Deploy mínimo (web + api)
- [ ] App Insights conectado y registrando trazas
- [ ] Blob listo para subida de soportes (con lifecycle sugerido)

## Criterios de aceptación
- [ ] Secretos fuera del código (Key Vault)
- [ ] Logs y trazas visibles en App Insights
- [ ] Backups SQL activos (por configuración Azure SQL)