## Como
Admin

## Quiero
Gestionar roles internos y asignarlos a usuarios

## Para
Aplicar segregación de funciones y control interno.

## Reglas de negocio
- Roles definidos: Admin, Operador, Tesorero, Contador, Junta.
- Cambios de rol siempre auditados.

## Criterios de aceptación
- [ ] Admin puede asignar/quitar roles a usuarios
- [ ] Queda auditoría: quién, cuándo, rol anterior/nuevo, motivo (opcional)
- [ ] La API aplica autorización por rol en endpoints

## Auditoría
- Debe auditar: Sí (asignación/remoción)