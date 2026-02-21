## Como
Usuario del sistema

## Quiero
Iniciar sesión con Microsoft Entra External ID (email/password) y MFA (Azure)

## Para
Autenticación segura sin contraseñas locales.

## Reglas de negocio
- Autenticación solo por Entra External ID.
- MFA configurable y administrado por Azure.

## Criterios de aceptación
- [ ] El login redirige a Entra y retorna sesión válida
- [ ] La API valida tokens (issuer/audience) y rechaza tokens inválidos/expirados
- [ ] No existe almacenamiento de contraseñas/hashes en BD local

## Datos / Campos
- Se almacena perfil local mínimo por usuario: `externalSubjectId (sub/oid)`, email, nombre (si llega), estado, fechas.

## Eventos y auditoría
- Debe auditar: Sí (inicio de sesión fallido/opcional)
- Eventos críticos: accesos no autorizados (logs)

## Notas técnicas
- Next.js: OIDC (MSAL o next-auth OIDC)
- API: JWT Bearer auth con configuración Entra