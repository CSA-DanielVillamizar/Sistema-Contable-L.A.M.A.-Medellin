# Frontend ERP - L.A.M.A. Medellin

Frontend principal del ERP contable para el capitulo L.A.M.A. Medellin, construido con Next.js App Router, React y TypeScript.

## Stack tecnico

- Next.js 16 (App Router)
- React 19 + TypeScript estricto
- TailwindCSS 4
- TanStack React Query para estado de servidor
- react-hot-toast para feedback global no bloqueante

## Estructura App Shell

La aplicacion usa un App Shell consistente en todo el sistema:

- Sidebar fijo para navegacion modular (Cartera, Miembros, Eventos, Tesoreria, etc.)
- Navbar superior para contexto de usuario y navegacion secundaria
- Contenedor de contenido con scroll independiente en `main`

Este patron se define en `src/app/layout.tsx` y garantiza una experiencia continua entre modulos sin cambios bruscos de estructura visual.

## Patron UX: Master-Detail (Split-Screen)

Los modulos operativos usan una distribucion master-detail para acelerar trabajo administrativo:

- Panel izquierdo (master): listado, filtros y seleccion de registros
- Panel derecho (detail): detalle operativo, acciones de edicion, trazabilidad o formularios

Aplicado en:

- Cartera: lista de cuentas por cobrar + accion de pago y estado
- Miembros: directorio + hoja de vida del miembro seleccionado
- Merchandising: catalogo tabular + detalle de producto y kardex

Beneficios clave:

- Menos navegacion innecesaria
- Contexto permanente del registro activo
- Flujo de trabajo mas rapido para operaciones repetitivas

## Manejo de Estado y Datos

### React Query

Toda la lectura/mutacion de datos de negocio usa hooks de React Query.

Patron de refresco en vivo:

- Listados: `['eventos']`, `['miembros']`, `['cartera', 'cuentas-por-cobrar']`, etc.
- Detalles por entidad: `['evento', id]` y llaves equivalentes por modulo
- En cada mutacion exitosa se hace `invalidateQueries` para actualizar la UI sin recargar la pagina

### Feedback global con toasts

Se integra `react-hot-toast` a nivel global mediante `GlobalToaster` en `layout.tsx`.

Casos ya instrumentados:

- Registro exitoso de eventos
- Registro exitoso de pagos en cartera

Resultado:

- Confirmaciones inmediatas para usuario final
- Menos incertidumbre en operaciones criticas
- Experiencia consistente en todos los modulos

## Estados globales de UX

Implementacion transversal de App Router:

- `src/app/loading.tsx`: estado global de carga con spinner centrado
- `src/app/error.tsx`: boundary global de errores con mensaje amigable y boton de reintento

Con esto, la app evita pantallas en blanco y mantiene continuidad operativa ante errores temporales.

## Scripts

Desde la carpeta `frontend`:

```bash
npm run dev
npm run build
npx tsc --noEmit
```

## Objetivo de arquitectura

Consolidar una experiencia ERP moderna, consistente y mantenible, con:

- navegacion estable (App Shell)
- operacion eficiente (Master-Detail)
- datos sincronizados en vivo (React Query)
- feedback claro al usuario (Toasts + Loading/Error globales)
