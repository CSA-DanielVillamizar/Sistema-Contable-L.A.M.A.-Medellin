# Scripts de automatización (GitHub Backlog)

## Requisitos

- PowerShell 5.1+
- GitHub CLI (`gh`) instalado
- Sesión iniciada: `gh auth login`
- Permisos para crear milestones, labels e issues en el repo

## Scripts

- `scripts/create_github_backlog.ps1`
- `scripts/create-foundation-backlog-extended.ps1`
- `scripts/bootstrap-foundation-backlog.ps1`
- `scripts/upsert_pr_comment.ps1`

Este script:

- Crea milestones (`Phase 0` a `Phase 5` y `Future / Backlog`) si no existen.
- Crea labels de tipo, fase, área y prioridad si no existen.
- Usa un catálogo canónico determinístico (51 issues: 13 épicas + 38 historias).
- Usa los `.md` del backlog como **body** y asigna labels/milestone exactos.
- En modo normal, solo crea los faltantes (si el título ya está abierto, lo omite).
- En modo reset, cierra todos los `epic/story` abiertos y reconstruye el catálogo completo.

## Crear backlog fundación (11 épicas + 50 historias)

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\create-foundation-backlog-extended.ps1 `
  -Repo "CSA-DanielVillamizar/Sistema-Contable-L.A.M.A.-Medellin"
```

El script crea labels, 11 épicas y un backlog extendido con historias iniciales (incluye cuota capitular COP 25.000 y renovación internacional USD 20).

## Bootstrap backlog + milestones Sprints

```powershell
gh auth login
pwsh ./scripts/bootstrap-foundation-backlog.ps1
```

Este script crea labels, milestones Sprint 1..6, 11 épicas y las historias extendidas asociadas.

## Uso rápido

Desde la raíz del repo:

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\create_github_backlog.ps1 \
  -Repo "CSA-DanielVillamizar/Sistema-Contable-L.A.M.A.-Medellin" \
  -BacklogPath "C:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\backlog" \
  -ResetCatalog
```

## Modo simulación (sin crear nada)

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\create_github_backlog.ps1 \
  -Repo "CSA-DanielVillamizar/Sistema-Contable-L.A.M.A.-Medellin" \
  -BacklogPath "C:\Users\DanielVillamizar\Sistema Contable L.A.M.A. Medellin\backlog" \
  -WhatIf
```

## Notas

- Si omites `-Repo`, el script intenta detectarlo desde `git remote origin`.
- `-ResetCatalog` es el modo recomendado para reconciliar y dejar el backlog exacto.

## Publicar/actualizar comentario en PR (REST)

Este script crea o actualiza un comentario del PR usando un `Marker` para poder reutilizar el mismo comentario en iteraciones sucesivas.

Ejemplo (actualiza o crea):

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\upsert_pr_comment.ps1 \
  -Repo "CSA-DanielVillamizar/Sistema-Contable-L.A.M.A.-Medellin" \
  -PullRequestNumber 1 \
  -BodyText "Release notes de prueba"
```

Ejemplo (solo verificar si existe):

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\upsert_pr_comment.ps1 \
  -Repo "CSA-DanielVillamizar/Sistema-Contable-L.A.M.A.-Medellin" \
  -PullRequestNumber 1 \
  -VerifyOnly
```
