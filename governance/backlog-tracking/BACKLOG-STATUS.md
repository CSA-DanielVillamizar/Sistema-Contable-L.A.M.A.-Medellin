Backlog Status & History

This file maintains a snapshot of the current backlog state, updated whenever bulk synchronization occurs.

## Current Status

Last synchronization: February 20, 2026

Total Issues: 51

- Epics: 13 (Open)
- Stories: 38 (Open)
- Bugs: 0
- Enhancements: 0

## Canonical Backlog Distribution

Phase 0 (Foundation): 8 stories
Phase 1 (Core): 20 stories
Phase 2 (Advanced): 5 stories
Phase 3 (Social): 3 stories
Phase 4 (Business): 2 stories

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
