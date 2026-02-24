Epic Issue Template

Copy this template when creating a new epic.

```markdown
Title: EPIC: [Phase Number] - [Main Initiative]

## Description
Brief summary of this epic's scope and objectives.

## Objectives
What are the main goals of this epic?
- Goal 1
- Goal 2
- Goal 3

## Problem Context
Why are we building this? What business need does it address?

## Scope
What is INCLUDED in this epic?
- Feature 1
- Feature 2
- Feature 3

### Out of Scope
What is explicitly NOT included?
- Feature A
- Feature B

## Technical Approach
High-level technical direction (no implementation details).

## Dependencies
- Other epics or work that must complete first
- External systems or services
- Resources needed

## Acceptance Criteria
The epic is complete when:
- [ ] All child stories are implemented and tested
- [ ] Documentation is updated
- [ ] Performance requirements are met
- [ ] Security review completed
- [ ] User acceptance testing passed

## Success Metrics
How will we measure success?
- Metric 1: Target value
- Metric 2: Target value

## Stories
Will be linked via GitHub's issue relations

## Milestone
Phase X

## Labels
epic, priority-[level], phase-[number]
```

## Example

```markdown
Title: EPIC: Phase 1 - Contabilidad general (PUC, comprobantes, libros, cierres)

## Description
Implementation of general accounting functionality following Colombian accounting standards and LAMA governance policies.

## Objectives
- Establish canonical chart of accounts (PUC) for the organization
- Record all monetary transactions as accounting vouchers
- Generate required financial statements
- Implement period closing procedures
- Maintain complete audit trail

## Problem Context
The organization needs to maintain compliance with Colombian accounting regulations (NIIF for SMEs) and internal governance policies. All financial transactions must flow through a proper general ledger system.

## Scope
INCLUDED:
- Chart of accounts (PUC) management
- Accounting vouchers (comprobantes) creation and management
- Financial books (Mayor, Diario, de Bancos)
- Period closing procedures
- Journal entry reversal

NOT IN SCOPE:
- Tax reporting (handled separately in Phase 5)
- Budget forecasting
- Cost accounting
- Multi-entity consolidation

## Technical Approach
- Domain-driven design for accounting entities
- Event sourcing for voucher history (future)
- Clean Architecture patterns
- CQRS for financial queries

## Dependencies
- Phase 0 must complete (IAM, Infrastructure, Base Models)
- CentroCosto definitions from Base Models
- User roles and permissions

## Acceptance Criteria
- [ ] All 8 related stories implemented and tested
- [ ] Financial statements generate correctly per Colombian GAAP
- [ ] Audit trail captures all changes
- [ ] Period closing prevents posting after close
- [ ] API documentation complete
- [ ] Performance: Financial queries <500ms for 1M transactions

## Success Metrics
- System handles 1M+ transactions without performance degradation
- All regulatory reports generate correctly
- Audit trail shows 100% traceability
- User acceptance testing: 95%+ pass rate

## Labels
epic, priority-high, phase-1
```
