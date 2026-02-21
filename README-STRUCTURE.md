LAMA Medellín Accounting System

A comprehensive accounting platform for LAMA Colombia Foundation's Medellín chapter, built with modern software architecture and cloud-native technologies.

## Repository Structure

This repository is organized into clear, purposeful directories to maintain clarity and organization:

```
Tesoreriaygerenciade-negocios/
│
├── src/                        Source code and development projects
│   ├── LAMAMedellin/          .NET 8 backend solution (Clean Architecture)
│   │   ├── src/               All project sources (Domain, Application, Infrastructure, API)
│   │   └── tests/             Unit and integration tests
│   └── scripts/               Project-specific automation scripts
│
├── docs/                       Development documentation
│   ├── BACKEND-SETUP.md       Backend architecture and setup guide
│   ├── ARCHITECTURE.md        System design and patterns
│   ├── guides/                Development guides (setup, contributing, deployment)
│   └── diagrams/              Architecture diagrams and visual documentation
│
├── governance/                 Project administration and management
│   ├── backlog-tracking/      Issue snapshots and backlog status
│   ├── process-docs/          Standards, guides, and checklists
│   └── templates/             Reference templates for issues
│
├── backlog/                    Content files for GitHub issues
│   ├── epics/                 Epic issue body content
│   └── stories/               Story issue body content
│
├── tools/                      Repository utilities and configuration
│   ├── scripts/               Global automation scripts
│   └── docker/                Docker configuration (future)
│
├── .github/                    GitHub-specific configuration
│   ├── ISSUE_TEMPLATE/        Issue templates
│   └── workflows/             CI/CD pipelines (future)
│
└── README.md                   This file
```

## Quick Start

### For Developers

1. Clone the repository
2. Follow [Local Development Setup](docs/guides/LOCAL-SETUP.md)
3. Read [Backend Setup Guide](docs/BACKEND-SETUP.md)
4. Check [Contributing Guide](docs/guides/CONTRIBUTING.md)

Backend build:

```powershell
cd src/LAMAMedellin
dotnet build
dotnet run --project src/LAMAMedellin.API
```

### For Project Managers

1. Review [Governance README](governance/README.md)
2. Check [Backlog Status](governance/backlog-tracking/BACKLOG-STATUS.md)
3. Understand [Issue Creation Guide](governance/process-docs/ISSUE-CREATION-GUIDE.md)

## Key Sections

**Development** (src/ + docs/)

- All production code lives in `src/`
- All development documentation lives in `docs/`
- See [src/README.md](src/README.md) for code structure
- See [docs/README.md](docs/README.md) for documentation index

**Governance** (governance/ + backlog/)

- Project administration and backlog tracking in `governance/`
- Issue content files in `backlog/`
- Issue definitions are synced with [GitHub Issues](https://github.com/CSA-DanielVillamizar/Sistema-Contable-L.A.M.A.-Medellin/issues)

**Configuration** (.github/ + tools/)

- GitHub templates and workflows in `.github/`
- Global automation scripts and tools in `tools/`

## Technology Stack

- **Backend**: ASP.NET Core 8.0 Web API, C#
- **Architecture**: Clean Architecture with CQRS pattern
- **Database**: Azure SQL Server with Entity Framework Core
- **Cloud**: Microsoft Azure
- **Authentication**: Microsoft Entra External ID
- **Version Control**: Git & GitHub

## Key Principles

**Clear Organization**: Every file has a purpose. No guessing what's what.

**Separation of Concerns**:

- Code ≠ Documentation
- Development ≠ Administration
- Project-specific ≠ Global configuration

**Professional Standards**:

- Spanish for business logic and domain concepts
- English for infrastructure and platform terms
- No emojis in code or documentation
- Clean Architecture patterns enforced

**Version Control**:

- All documentation and process files are committed to git
- Backlog snapshots provide historical audit trail
- All changes are traceable

## Development Workflow

1. Pick an issue from [GitHub Issues](https://github.com/CSA-DanielVillamizar/Sistema-Contable-L.A.M.A.-Medellin/issues)
2. Create a feature branch: `feature/issue-XXX-description`
3. Follow [Contributing Guide](docs/guides/CONTRIBUTING.md)
4. Submit pull request with linked issue
5. Pass code review (see [Review Checklist](governance/process-docs/REVIEW-CHECKLIST.md))
6. Merge to main

## Documentation Index

**Getting Started**

- [Backend Setup](docs/BACKEND-SETUP.md) - Architecture and development workflow
- [Local Development](docs/guides/LOCAL-SETUP.md) - Environment configuration
- [Contributing](docs/guides/CONTRIBUTING.md) - Code standards and git workflow

**System Design**

- [Architecture Overview](docs/ARCHITECTURE.md) - System design and patterns
- [Code Standards](governance/process-docs/CODE-STANDARDS.md) - Conventions and best practices

**Project Management**

- [Governance Overview](governance/README.md) - Project administration
- [Backlog Status](governance/backlog-tracking/BACKLOG-STATUS.md) - Current sprint status
- [Issue Creation Guide](governance/process-docs/ISSUE-CREATION-GUIDE.md) - How to write issues

## Support & Communication

- Issues & Bugs: [GitHub Issues](https://github.com/CSA-DanielVillamizar/Sistema-Contable-L.A.M.A.-Medellin/issues)
- Documentation: See [docs/README.md](docs/README.md)
- Questions: Create an [issue](https://github.com/CSA-DanielVillamizar/Sistema-Contable-L.A.M.A.-Medellin/issues/new) with `question` label

## License

Proprietary - LAMA Colombia Foundation

---

Last updated: February 2026
Repository: [CSA-DanielVillamizar/Sistema-Contable-L.A.M.A.-Medellin](https://github.com/CSA-DanielVillamizar/Sistema-Contable-L.A.M.A.-Medellin)
