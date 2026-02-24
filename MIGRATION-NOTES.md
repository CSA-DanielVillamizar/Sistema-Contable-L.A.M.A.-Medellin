Repository Migration Notes

This file documents the repository restructuring progress.

## Current Status

**Completed: Structure & Documentation**

- New directory structure created
- All documentation organized into docs/
- Governance/administration organized into governance/
- Professional README files created for each section
- Standards and templates documented

**Current Location (Being Phased Out)**

- LAMAMedellin/ at root level
- scripts/ at root level
- Various documentation files at root (docs_*.md)

**New Location (After Full Migration)**

- LAMAMedellin/ → src/LAMAMedellin/
- scripts/ → src/scripts/
- Documentation consolidated in docs/ and governance/

## Migration Steps (For Future)

### Phase 1: Parallel Structure (Current)

Both old and new structures exist simultaneously.

Developers can start using:

- `src/README.md` for navigation
- `docs/` for development guides
- `governance/` for standards and backlog

### Phase 2: Gradual Migration

As work continues:

- Update build/CI pipelines to reference src/ paths
- Move backend code via `git mv LAMAMedellin src/LAMAMedellin`
- Update scripts: `git mv scripts src/scripts`

### Phase 3: Cleanup

Once all references updated:

- Remove old root-level LAMAMedellin/ symlink
- Remove old scripts/ location
- Archive old docs_*.md files to governance/archive/

## Why Phased Approach

**Avoid Breaking Changes**

- Git history stays intact
- Scripts remain functional during transition
- Existing CI/CD pipelines keep working
- Team members adjust gradually

**Proper References**

- All references must be updated simultaneously
- PowerShell scripts need path adjustments
- GitHub workflows need configuration updates
- Build scripts need rebuild

## Files Needing Reference Updates

1. **Scripts**
   - `scripts/create_github_backlog.ps1` - Absolute paths
   - Any CI/CD configuration files

2. **Documentation**
   - Links in README files
   - Build instructions
   - Deployment guides

3. **Build Configuration**
   - dotnet build scripts
   - Docker build context
   - CI/CD pipeline definitions

## How to Move LAMAMedellin

When ready for full migration:

```powershell
# 1. Verify all tests pass
dotnet test

# 2. Commit any pending changes
git add .
git commit -m "Pre-migration checkpoint"

# 3. Move backend
git mv LAMAMedellin src/LAMAMedellin

# 4. Update references in documentation
# Search and replace all relative paths:
# LAMAMedellin/ → src/LAMAMedellin/
# .\LAMAMedellin → .\src\LAMAMedellin

# 5. Update scripts paths
# Edit scripts/create_github_backlog.ps1 for new paths

# 6. Test everything works
dotnet build src/LAMAMedellin/LAMAMedellin.slnx
dotnet test src/LAMAMedellin/LAMAMedellin.slnx

# 7. Commit migration
git commit -m "refactor: Move backend to src/ directory hierarchy"
```

## How to Move Scripts

```powershell
# 1. Copy scripts to new location
Copy-Item -Path .\scripts\* -Destination .\src\scripts\ -Recurse

# 2. Update references in scripts
# Edit all script paths to use new locations

# 3. Test scripts work from new location
powershell -ExecutionPolicy Bypass .\src\scripts\create_github_backlog.ps1

# 4. Once verified, remove old location
git rm -r scripts

# 5. Commit
git commit -m "refactor: Move project scripts to src/scripts"
```

## Documentation Cleanup

Archive old root-level documentation:

```
Old Files (can archive/delete):
- docs_ARCHITECTURE-AZURE.md → governance/archive/
- docs_BACKLOG.md → governance/archive/
- docs_BRD-SRS_Version2.md → governance/archive/
- docs_ISSUE-CREATION-GUIDE.md → governance/archive/ (already in governance/process-docs/)
```

## Timeline Recommendation

- **Now**: Use new structure for all new work
- **Next Sprint**: Document migration steps for team
- **2 Sprints**: Execute Phase 2 (gradual migration)
- **3 Sprints**: Complete Phase 3 (full cutover)

## Team Communication

When executing migration:

1. **Before**: Announce migration window to team
2. **During**: Update shared documentation with new paths
3. **After**: Verify all team members can build/run locally

## Questions or Issues

See:

- [STRUCTURE-GUIDE.md](STRUCTURE-GUIDE.md) - Why the new structure
- [README.md](README.md) - Quick navigation
- [src/README.md](src/README.md) - Development code structure
- [governance/README.md](governance/README.md) - Administration structure

---

Last updated: February 2026
Migration Status: Structure Created, Migration Pending
