## Como
Admin / Usuario del sistema

## Quiero
Iniciar sesión con Entra External ID (email/password) y MFA Azure

## Para
No almacenar contraseñas localmente y cumplir seguridad.

## Reglas de negocio
- Autenticación solo Entra.
- MFA configurable.

## Criterios de aceptación
- [ ] Login redirige a Entra y retorna sesión válida
- [ ] API valida tokens OIDC
- [ ] No hay contraseñas en BD local

## Notas técnicas
- Next.js + OIDC (MSAL/next-auth OIDC)
- API: JWT bearer con issuer/audience de Entra