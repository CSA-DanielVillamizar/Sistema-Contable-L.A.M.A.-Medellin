## Como
Admin / Equipo

## Quiero
Que el sistema aplique permisos por rol según la matriz aprobada

## Para
Evitar accesos indebidos y soportar auditoría.

## Criterios de aceptación
- [ ] Endpoint restringido devuelve 403 si el rol no tiene permiso
- [ ] UI oculta o deshabilita acciones no permitidas
- [ ] Junta no puede ver PII de beneficiarios

## Notas técnicas
- API: políticas/handlers por rol
- UI: guards y control de rutas/acciones