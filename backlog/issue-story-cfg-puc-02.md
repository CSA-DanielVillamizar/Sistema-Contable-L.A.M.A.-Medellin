# [Story][Phase 0] Consultar y navegar el Catálogo de Cuentas desde la interfaz

## Como
**Contador** o **Admin** de la Fundación L.A.M.A. Medellín

## Quiero
Ver el Catálogo de Cuentas completo en pantalla con su estructura jerárquica, filtrar por nivel o tipo, y saber qué cuentas están habilitadas para recibir asientos

## Para
- Verificar que el catálogo esté correcto antes de iniciar la operación contable.
- Identificar rápidamente las cuentas asentables disponibles al registrar un comprobante.
- Detectar si alguna cuenta falta o necesita ser completada con un auxiliar.

---

## Reglas de negocio

1. El catálogo se muestra ordenado por código numérico (orden natural del árbol PUC).
2. Las cuentas con `PermiteMovimiento = false` se presentan visualmente como **nodos agrupadores** (sin checkbox, en tono gris/negrita).
3. Las cuentas con `PermiteMovimiento = true` se presentan como **nodos hoja** (habilitadas para asiento).
4. Las cuentas con `ExigeTercero = true` muestran un indicador visible (ej. ícono de persona o etiqueta "Exige Tercero").
5. La **naturaleza** (Débito / Crédito) es visible en cada fila.
6. No se exponen cuentas con `IsDeleted = true`.

---

## Criterios de aceptación

### Vista de listado completo
- [ ] Al navegar a `/contabilidad/cuentas` el Contador ve la tabla con todas las cuentas del catálogo.
- [ ] La tabla muestra: Código, Descripción, Nivel, Naturaleza, PermiteMovimiento (ícono), ExigeTercero (ícono).
- [ ] Las cuentas están ordenadas por código (orden ascendente).
- [ ] El listado incluye las 34 cuentas sembradas inicialmente (Patrimonio 3xxx, Ingresos 4xxx, Gastos 5xxx, Costos 6xxx).

### Filtro por asentables
- [ ] Existe un botón/toggle "Solo asentables" que filtra las cuentas con `PermiteMovimiento = true`.
- [ ] Al activarlo, solo se muestran 14 cuentas (las hojas del árbol).
- [ ] Al desactivarlo, vuelven a mostrarse las 34 cuentas.

### Detalle de cuenta
- [ ] Al hacer clic en una cuenta se puede ver su información completa incluyendo la cuenta padre.
- [ ] Si la cuenta tiene `ExigeTercero = true`, se muestra la leyenda: *"Esta cuenta requiere identificación de tercero para información exógena DIAN"*.

### Accesibilidad y protección
- [ ] La ruta `/contabilidad/cuentas` requiere autenticación (redirige a login si no hay sesión).
- [ ] Un usuario sin sesión activa no puede acceder al catálogo.

### Errores
- [ ] Si el API retorna error, se muestra mensaje amigable: *"No fue posible cargar el catálogo de cuentas. Intente recargar la página."*

---

## Datos y campos del DTO

```typescript
// CuentaContableDto (retornado por GET /api/cuentas-contables)
{
  id: string;             // UUID
  codigo: string;         // "410505"
  descripcion: string;    // "Cuotas de Afiliación (Nuevos)"
  nivel: number;          // 4 (Subcuenta)
  nivelNombre: string;    // "Subcuenta"
  naturalezaNombre: string; // "Credito" | "Debito"
  permiteMovimiento: boolean;
  exigeTercero: boolean;
  cuentaPadreId: string | null;
}
```

---

## Notas técnicas

### API endpoints disponibles
| Método | Ruta | Descripción |
|--------|------|-------------|
| `GET` | `/api/cuentas-contables` | Catálogo completo (34 cuentas) |
| `GET` | `/api/cuentas-contables/asentables` | Solo nodos hoja (`PermiteMovimiento = true`) |

### Frontend sugerido

```
Página: /contabilidad/cuentas  (App Router Next.js)
Componente principal: TablaCuentasContables (Server Component + hydration mínima)
Estado del servidor: TanStack Query
  - queryKey: ['contabilidad', 'cuentas', { soloAsentables }]
  - queryFn: GET /api/cuentas-contables | GET /api/cuentas-contables/asentables
```

### Diseño visual sugerido
- Tabla con columnas: Código | Descripción | Nivel | Naturaleza | Movimiento | Exige Tercero
- `PermiteMovimiento = false` → fila en peso `font-semibold`, texto slate-500 (no es clickeable para asiento)
- `PermiteMovimiento = true` → fila normal, icono ✅
- `ExigeTercero = true` → badge o icono de usuario con tooltip "Exige identificación de tercero (DIAN)"
- `Naturaleza = Debito` → etiqueta roja/naranja; `Naturaleza = Credito` → etiqueta verde/azul

### Permisos
- Roles con acceso de lectura: Admin, Contador, Tesorero
- Roles sin acceso: Junta (no requiere ver el PUC)
- Control de roles: pendiente hasta que se implemente RBAC interno (issue-story-0-3)

---

## Eventos y auditoría

| Acción | Debe auditar |
|--------|:------------:|
| Visualización del catálogo | No |
| Exportación del catálogo (futura) | Sí |

---

## Definición de Listo (Definition of Done)

- [ ] Página `/contabilidad/cuentas` implementada en Next.js App Router
- [ ] Componente `TablaCuentasContables` con React Query
- [ ] Toggle "Solo asentables" funcional
- [ ] Indicadores visuales de `ExigeTercero` y `PermiteMovimiento`
- [ ] Manejo de estados: loading, error, vacío
- [ ] Ruta protegida (requiere sesión activa)
- [ ] Sin hardcode de datos (todo viene del API)
- [ ] Lint y build pasando
- [ ] Aprobado en revisión de código

---

## Referencias

- **Épica padre:** `[Epic][Phase 0] Modelo base: Centros de costo + Medios de pago + Terceros + Mapeo contable` (issue-epic-03)
- **Depende de:** `[Story][Phase 0] Construir y precargar Catálogo de Cuentas PUC` (issue-story-cfg-puc-01) — el API ya está disponible
- **Desbloquea:** Selector de cuenta contable en formulario de comprobantes (Phase 1)
- **API implementada en:** `LAMAMedellin/src/LAMAMedellin.API/Controllers/CuentasContablesController.cs`

---

## Labels
`story` · `priority:high` · `effort:S` · `phase:0` · `area:accounting` · `frontend`

## Milestone
Phase 0 – Foundations

## Estimación de esfuerzo
**S (Small) – Medio día** (el API ya existe; es solo implementar la página y el componente de tabla)
