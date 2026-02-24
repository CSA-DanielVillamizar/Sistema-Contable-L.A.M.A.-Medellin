Project Governance & Administration

This directory contains project management artifacts, backlog tracking, and process documentation.

## Quick Navigation

**Backlog & Tracking**

- [Backlog Status](backlog-tracking/BACKLOG-STATUS.md) - Current snapshot of open issues
- [Issue Tracking](backlog-tracking/) - Epic and story files tracked in version control
- [Backlog Content](../backlog/) - Markdown content files for issue bodies

**Process & Standards**

- [Issue Creation Guide](process-docs/ISSUE-CREATION-GUIDE.md) - How to create well-formed issues
- [Code Standards](process-docs/CODE-STANDARDS.md) - Coding conventions and best practices
- [Review Checklist](process-docs/REVIEW-CHECKLIST.md) - Pull request review criteria

**Templates & Reference**

- [Epic Template](templates/epic-template.md) - Reference structure for epic issues
- [Story Template](templates/story-template.md) - Reference structure for story issues

## Directory Structure

```
governance/
├── README.md                               This file
├── backlog-tracking/
│   ├── BACKLOG-STATUS.md                  Current status snapshot
│   ├── issue-epic-*.md                    Individual epic definitions
│   └── issue-story-*.md                   Individual story definitions
├── process-docs/
│   ├── ISSUE-CREATION-GUIDE.md           How to write issues correctly
│   ├── CODE-STANDARDS.md                  Coding conventions
│   └── REVIEW-CHECKLIST.md               PR review standards
└── templates/
    ├── epic-template.md                   Epic issue structure
    └── story-template.md                  Story issue structure
```

## Key Principles

**Clarity**: Any team member should instantly know what's in each file and folder.

**Separation of Concerns**:

- Governance = admin, processes, backlog tracking
- Development = code, architecture, setup guides

**Version Control**: Issue files and process docs are committed to git for historical tracking and traceability.

**Automation**: Backlog synchronization runs via GitHub Actions and PowerShell scripts to maintain canonical state.

## Backlog Management

The backlog is managed through:

1. **Canonical Catalog**: Single source of truth in `scripts/create_github_backlog.ps1`
2. **GitHub Issues**: Live issues synced from the catalog
3. **Tracking Files**: Snapshots in `backlog-tracking/` for documentation

To synchronize the backlog with GitHub:

```powershell
Set-Location "Tesoreriaygerenciade-negocios"
powershell -ExecutionPolicy Bypass -File .\scripts\create_github_backlog.ps1 `
  -Repo "CSA-DanielVillamizar/Tesoreriaygerenciade-negocios" `
  -BacklogPath "C:\Path\To\backlog"
```

## For More Information

- Development setup: See [docs/README.md](../docs/README.md)
- Source code: See [src/README.md](../src/README.md)
- Project repository: [GitHub Repo](https://github.com/CSA-DanielVillamizar/Tesoreriaygerenciade-negocios)
