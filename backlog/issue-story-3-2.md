## Como
GestorSocial / Operador

## Quiero
Registrar beneficiarios respetando Ley 1581 (consentimiento)

## Para
Cumplimiento legal y auditoría.

## Reglas de negocio
- Si se registra nombre/documento/teléfono => consentimiento obligatorio (Sí/No + fecha + medio + soporte opcional).
- Sin consentimiento => solo código anónimo.

## Criterios de aceptación
- [ ] Si se diligencia documento o teléfono o nombre => exige consentimiento=Sí + fecha + medio
- [ ] Si consentimiento=No => bloquea PII y solo permite código anónimo
- [ ] Cambios de consentimiento quedan auditados (quién/cuándo)