Repository Restructuring Summary

This document explains the new repository structure and how files were organized.

## What Changed

The repository has been reorganized into clear, purposeful directories following professional software development practices.

**Old Structure** (Mixed, unclear organization)

```
root/
├── docs/
├── scripts/
├── LAMAMedellin/          <- Backend mixed at root
├── backlog/
├── issue-epic-*.md        <- Mixed with other files
├── issue-story-*.md       <- Mixed with other files
└── ...
```

**New Structure** (Clear separation of concerns)

```
root/
├── src/                   <- All development code
├── docs/                  <- Development documentation
├── governance/            <- Project administration
├── backlog/               <- Issue content files
├── tools/                 <- Global utilities
├── .github/               <- GitHub configuration
└── README.md              <- Main project entry point
```

## Directory Purpose

**src/** - Development Code

- LAMAMedellin/ (Backend .NET solution)
- scripts/ (Project-specific automation)

Anyone looking for code knows to go here.

**docs/** - Development Documentation

- Architecture guides
- Setup instructions
- Development workflow documentation

Anyone learning the system starts here.

**governance/** - Project Administration

- Backlog tracking and status
- Process documentation
- Code standards and conventions
- Issue templates

Project managers and process people work here.

**backlog/** - Issue Content

- Epic descriptive files
- Story descriptive files
- Markdown content for GitHub issues

Administrative data storage.

**tools/** - Global Utilities

- Repository-wide scripts
- Docker configurations
- Git hooks

Infrastructure setup located here.

**.github/** - GitHub Configuration

- Issue templates
- GitHub Actions workflows (future)
- CODEOWNERS file

GitHub-specific configuration.

## Navigation Map

**For Developers**

1. Start: [README.md](README.md) overview
2. Setup: [docs/guides/LOCAL-SETUP.md](docs/guides/LOCAL-SETUP.md)
3. Learn: [docs/BACKEND-SETUP.md](docs/BACKEND-SETUP.md) + [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)
4. Contribute: [docs/guides/CONTRIBUTING.md](docs/guides/CONTRIBUTING.md)
5. Code: [src/README.md](src/README.md) for project structure

**For Project Managers**

1. Start: [governance/README.md](governance/README.md)
2. Backlog: [governance/backlog-tracking/](governance/backlog-tracking/)
3. Standards: [governance/process-docs/](governance/process-docs/)
4. Create Issue: [governance/process-docs/ISSUE-CREATION-GUIDE.md](governance/process-docs/ISSUE-CREATION-GUIDE.md)

**For DevOps/Infrastructure**

1. Setup: [tools/](tools/) for configurations
2. Deploy: [docs/guides/DEPLOYMENT.md](docs/guides/DEPLOYMENT.md)
3. Monitoring: See deployment guide

## Key Improvements

**1. Clarity**

- Developer sees "src/" and knows where code is
- Project manager sees "governance/" and knows where backlog is
- Anyone can navigate without asking questions

**2. Scalability**

- Multiple projects can go in src/ (backend, frontend, mobile)
- Multiple services can have separate governance subdirectories
- Structure grows horizontally without confusion

**3. Professional Standards**

- Follows industry best practices
- Matches GitHub/GitLab standard conventions
- Team members recognize organization immediately

**4. Separation of Concerns**

- Development artifacts separate from administration
- Code separate from configuration
- Business logic separate from tooling

**5. Version Control Efficiency**

- Easy to ignore unnecessary files (.gitignore)
- Clear commit scopes ("docs:", "src:", "governance:")
- Backlog snapshots maintained for audit trail

## Files Created

**Documentation**

- docs/README.md - Documentation index
- docs/ARCHITECTURE.md - System design overview
- docs/BACKEND-SETUP.md - Backend development guide (existing, moved to docs/)
- docs/guides/LOCAL-SETUP.md - Environment setup
- docs/guides/CONTRIBUTING.md - Contribution workflow
- docs/guides/DEPLOYMENT.md - Production deployment

**Governance**

- governance/README.md - Governance overview
- governance/BACKLOG-STATUS.md - Current backlog snapshot
- governance/process-docs/CODE-STANDARDS.md - Coding conventions
- governance/process-docs/ISSUE-CREATION-GUIDE.md - How to write issues
- governance/process-docs/REVIEW-CHECKLIST.md - PR review criteria
- governance/templates/epic-template.md - Epic issue template
- governance/templates/story-template.md - Story issue template

**Source Code**

- src/README.md - Source code overview
- src/LAMAMedellin/ - Backend solution (moved from root)
- src/scripts/ - Project-specific automation (moved from root)

**Project Root**

- README.md - Main project entry point
- README-STRUCTURE.md - This file (structure explanation)

## Migration Notes

**What Stayed in Place**

- .git/ - Git repository
- .github/ - GitHub templates/workflows
- backlog/ - Issue content files

**What Moved**

- LAMAMedellin/ → src/LAMAMedellin/
- scripts/ → src/scripts/ (select scripts)
- docs/BACKEND-SETUP.md → docs/BACKEND-SETUP.md (already in docs)

**What Was Created**

- Complete documentation hierarchy (docs/)
- Governance and standards structure (governance/)
- Navigation README files for each section
- Process documentation and templates

## Next Steps

1. **Bookmark Key Documents**
   - Developer README: `src/README.md`
   - Contributing Guide: `docs/guides/CONTRIBUTING.md`
   - Code Standards: `governance/process-docs/CODE-STANDARDS.md`

2. **Update Local References**
   - Clone scripts may need path updates
   - CI/CD pipelines need path adjustments
   - Documentation links validated

3. **Team Onboarding**
   - Share [README.md](README.md) with new team members
   - Direct developers to [docs/guides/LOCAL-SETUP.md](docs/guides/LOCAL-SETUP.md)
   - Direct PMs to [governance/README.md](governance/README.md)

4. **Continuous Improvement**
   - As new documentation needed, follow the hierarchy
   - As processes change, update governance/ docs
   - Maintain this as the single source of truth

## Rationale

**Why This Structure?**

This structure aligns with:

- **Industry Standards**: Matches Fortune 500 repos and open-source projects
- **Team Productivity**: Clear answers to "where do I X?" questions
- **Scalability**: Grows as project grows
- **Professional Development**: Meets stakeholder expectations
- **Best Practices**: Follows GitHub/Azure DevOps conventions

**Research & References**

- [Google C++ Style Guide - Repo Structure](https://google.github.io/styleguide/cppguide.html)
- [Apache Projects - Standard Directory Layout](https://infra.apache.org/)
- [GitHub Recommended Community Standards](https://docs.github.com/en/communities/setting-up-your-project-for-healthy-contributions)

---

Questions about the structure? Check [governance/README.md](governance/README.md) or create an issue.

Last updated: February 2026
