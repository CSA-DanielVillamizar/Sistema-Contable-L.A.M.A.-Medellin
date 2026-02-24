## Como
Admin

## Quiero
Que cada usuario autenticado por Entra tenga un perfil interno

## Para
Asignar roles internos y auditar acciones.

## Criterios de aceptación
- [ ] Al primer login, se crea usuario interno si no existe (upsert)
- [ ] Se vincula por `sub`/`oid` (identificador estable)
- [ ] Se puede deshabilitar usuario interno (bloqueo de acceso aunque Entra autentique)

## Datos / Campos
- external_id (sub/oid), email, display_name, enabled, created_at, last_login_at

## Auditoría
- Debe auditar: Sí (creación/deshabilitación)