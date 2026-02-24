Backlog Status & History

This file maintains a snapshot of the current backlog state, updated whenever bulk synchronization occurs.

## Current Status

Last synchronization: February 24, 2026

Total Issues: 53

- Epics: 13 (Open)
- Stories: 40 (Open)
- Bugs: 0
- Enhancements: 0

## Canonical Backlog Distribution

Phase 0 (Foundation): 10 stories *(+2 historias PUC ESAL agregadas 2026-02-24)*
Phase 1 (Core): 20 stories
Phase 2 (Advanced): 5 stories
Phase 3 (Social): 3 stories
Phase 4 (Business): 2 stories

## Cambios recientes

### 2026-02-24 – Catálogo de Cuentas (PUC adaptado ESAL)
Se agregaron 2 nuevas historias para documentar el requerimiento de Catálogo de Cuentas:

| Archivo | Título | Épica padre | Estado |
|---------|--------|-------------|--------|
| `backlog/issue-story-cfg-puc-01.md` | Construir y precargar Catálogo de Cuentas (PUC ESAL – NIIF Grupo III) | issue-epic-03 | ✅ Implementado (Phase 0) |
| `backlog/issue-story-cfg-puc-02.md` | Consultar y navegar el Catálogo de Cuentas desde la interfaz | issue-epic-03 | ⬜ Pendiente (Phase 0) |

La historia `cfg-puc-01` está **completamente implementada** en la rama `copilot/protect-main-branch`:
- Entidad `CuentaContable` con 5 niveles jerárquicos, `PermiteMovimiento`, `ExigeTercero`, `Naturaleza`
- 34 cuentas sembradas (Patrimonio 3xxx, Ingresos 4xxx, Gastos 5xxx, Costos 6xxx)
- API: `GET /api/cuentas-contables` y `GET /api/cuentas-contables/asentables`
- 26 tests unitarios nuevos (38/38 passing)

## File Organization

Individual issue definitions are stored in this directory:

- `issue-epic-*.md` - Epic issue definitions
- `issue-story-*.md` - Story issue definitions

These files are maintained in version control to provide:

1. Historical audit trail of backlog changes
2. Reference snapshots for tracking and reporting
3. Offline backlog access without GitHub API calls

## Synchronization

The canonical backlog is defined in `scripts/create_github_backlog.ps1` and synchronized to GitHub Issues via automation.

To view the live backlog, visit:
<https://github.com/CSA-DanielVillamizar/Tesoreriaygerenciade-negocios/issues>

To resync the backlog to canonical state, run:

```powershell
Set-Location "Tesoreriaygerenciade-negocios"
powershell -ExecutionPolicy Bypass -File .\scripts\create_github_backlog.ps1 `
  -Repo "CSA-DanielVillamizar/Tesoreriaygerenciade-negocios" `
  -BacklogPath "C:\Path\To\backlog" `
  -ResetCatalog
```

## Navigation

- [Back to Governance](../README.md)
- [Issue Creation Guide](../process-docs/ISSUE-CREATION-GUIDE.md)
- [GitHub Issues](https://github.com/CSA-DanielVillamizar/Tesoreriaygerenciade-negocios/issues)
