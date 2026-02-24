## Objetivo
Autenticación vía Microsoft Entra External ID (OIDC) con MFA y autorización por roles internos (Admin/Operador/Tesorero/Contador/Junta) sin almacenar contraseñas en BD local.

## Alcance (In / Out)
**Incluye**
- Login OIDC Entra External ID
- MFA configurable en Entra
- Modelo de usuario interno vinculado por `sub/oid`
- CRUD roles internos y asignación con auditoría
- Middleware/guardas de autorización en API y UI

**No incluye**
- Gestión de contraseñas propia
- Roles gestionados directamente en Entra (se manejan internamente)

## Entregables
- [ ] App registrations (web/api) configuradas
- [ ] Login funcional end-to-end (web → Entra → api)
- [ ] Roles internos implementados y auditados
- [ ] Políticas por rol aplicadas

## Criterios de aceptación
- [ ] Ninguna contraseña se almacena en BD local
- [ ] MFA aplicable por política Entra
- [ ] Accesos bloqueados por rol según matriz de permisos

## Dependencias
- Depende de: EPIC 2 (infra mínima) parcialmente (Key Vault/Config)