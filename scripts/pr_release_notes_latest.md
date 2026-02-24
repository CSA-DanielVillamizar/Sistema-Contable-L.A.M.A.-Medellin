Release notes (docs/spec-v1)

- Backend: flujo Registrar Ingreso implementado end-to-end (Application + Infrastructure + API).
- API: endpoint protegido POST /api/transacciones/ingreso con manejo global de errores en ProblemDetails.
- Frontend: pantalla Registro de Ingreso implementada con Zod + React Hook Form + React Query.
- Build fixes: provider global de React Query y ajuste de tipos Zod input/output para TS estricto.
- Estado branch: alineado en docs/spec-v1, último commit 6f80830.

QA checklist sugerido

- [x] Build backend (Application/Infrastructure/API) sin errores.
- [x] Build frontend (`next build`) exitoso.
- [x] Flujo UI incluye validación condicional para USD.
- [x] Manejo de errores API en frontend mostrando mensaje de ProblemDetails.
- [ ] Validar integración completa con token real de Entra External ID en entorno de pruebas.
- [ ] Validar persistencia real en Azure SQL con datos de prueba.
