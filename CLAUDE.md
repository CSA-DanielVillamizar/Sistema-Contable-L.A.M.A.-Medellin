# Convenciones del repositorio

## Commits

- **No incluir coautoria ni atribucion de herramientas en los mensajes de commit.**
  Nada de trailers `Co-Authored-By`, ni firmas del tipo "Generated with".
- **No dejar rastro de atribucion en el codigo, comentarios, documentacion ni
  scripts** del repositorio.
- Mensajes en espanol, sin tildes en el asunto, con prefijo de tipo
  (`feat:`, `fix:`, `security:`, `build:`, `refactor:`, `docs:`, `chore:`).
- El asunto describe el resultado; el cuerpo explica el porque y las
  consecuencias, no la lista de archivos.
- Commits tematicos: un commit por unidad coherente de cambio.

## Codigo

- Espanol para dominio y logica de negocio; ingles para terminos de
  infraestructura y plataforma.
- Sin emojis en codigo ni documentacion.
- Clean Architecture: Domain no depende de nada; Application define las
  interfaces; Infrastructure las implementa; API solo orquesta.

## Ejecucion local

Ver [README](README.md#ejecucion-local). Resumen: `docker compose up -d` para la
base, luego `dotnet run --project src/LAMAMedellin.API --no-launch-profile`
desde `LAMAMedellin/`, y `npm run dev` desde `frontend/`.

Los comandos de dotnet deben apuntar a `LAMAMedellin.slnx` de forma explicita.

## Migraciones

El historial se colapso en una unica migracion base (`Baseline`). Antes de
registrarla en produccion hay que verificar drift con
`scripts/sql/inventario-esquema.sql`. Detalle en
[docs/BACKEND-SETUP.md](docs/BACKEND-SETUP.md#historial-de-migraciones).

Nunca apuntar el entorno Development a la base de produccion: al arrancar,
la API aplica migraciones y siembra datos automaticamente.
