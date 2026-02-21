Issue Creation Guide

How to create well-formed issues that communicate clearly to the team.

## Issue Types

### Epic

A major feature or initiative that spans multiple sprints and requires breaking into stories.

Use when: Creating a large, phase-based initiative (e.g., "Phase 1 - Core Accounting Features")

**Components**:

- Title with "EPIC: " prefix
- Clear description of scope and objectives
- Acceptance criteria
- Related stories (linked via GitHub)
- Milestone (phase assignment)

[Epic Template](../../governance/templates/epic-template.md)

### Story

A single, implementable feature that can be completed in one sprint.

Use when: Breaking down an epic or adding specific functionality

**Components**:

- Title with clear user perspective
- User story format: "As a [role], I want [action], so that [benefit]"
- Acceptance criteria (testable)
- Technical notes (if applicable)
- Estimate (T-shirt sizing: XS, S, M, L, XL)
- Assignee (who's working on it)
- Milestone (sprint/phase)

[Story Template](../../governance/templates/story-template.md)

### Bug

Something that's broken or not working as designed.

Use when: Reporting unexpected behavior or defects

**Components**:

- Clear title: "Bug: [Component] - [What's broken]"
- Reproduction steps
- Expected behavior
- Actual behavior
- Screenshots/logs (if applicable)
- Severity: Critical / High / Medium / Low

### Enhancement

Improvement to existing functionality.

Use when: Requesting improvements to current features

**Components**:

- Clear title: "Enhancement: [Component] - [Improvement]"
- Current behavior
- Proposed behavior
- Benefits/rationale
- Breaking changes (yes/no)

## Title Guidelines

**Good Titles**

- Specific and descriptive
- Use business language when appropriate
- ~60 characters max

**Examples**

- "EPIC: Phase 1 - Contabilidad general" ✓
- "Registrar transacciones bancarias" ✓
- "Bug: API returns 500 when monto is null" ✓
- "Enhancement: Add pagination to transaction list" ✓

**Bad Titles**

- "Fix stuff" ✗
- "EPIC: Feature" ✗
- "Doesn't work" ✗
- "Random issue about the system" ✗

## Description Guidelines

Write descriptions that others can understand without needing to ask you.

**What to Include**

- Problem statement / What needs to be done
- Why it matters (business impact)
- Scope boundaries (what's in/out of scope)
- Any dependencies or prerequisites
- Relevant context or links

**Format**

```markdown
## Description
Brief summary of what this issue is about.

## Problem / Context
Why are we doing this? What problem are we solving?

## Solution / Approach
How should this be implemented? (optional, can be in comments)

## Acceptance Criteria
- [ ] Criteria 1 - specific and testable
- [ ] Criteria 2 - use action verbs
- [ ] Criteria 3 - clear definition of done

## Technical Notes
Any specific implementation requirements or considerations.

## References
Links to related issues, PRs, or documentation.
```

## Labels

Apply these labels to organize issues:

**Type**

- `epic` - Large initiative
- `story` - Implementable feature
- `bug` - Defect
- `enhancement` - Improvement
- `documentation` - Doc changes
- `chore` - Maintenance work

**Priority**

- `priority-critical` - Blocks other work
- `priority-high` - Important but not blocking
- `priority-medium` - Standard priority
- `priority-low` - Nice to have

**Status**

- `needs-review` - Waiting for feedback
- `in-progress` - Someone is working on it
- `blocked` - Waiting for something else
- `ready-for-development` - Approved and ready to start

**Effort**

- `effort-xs` - 1-2 hours
- `effort-s` - Half day
- `effort-m` - 1-2 days
- `effort-l` - 3-5 days
- `effort-xl` - More than a week

## Milestones

Issues are assigned to development phases:

- **Phase 0**: Foundation (IAM, Infrastructure, Base Models)
- **Phase 1**: Core Accounting (General Ledger, Treasury, Receivables, Payables)
- **Phase 2**: Advanced Features (Donations, Projects)
- **Phase 3**: Social Management
- **Phase 4**: Business Operations
- **Phase 5**: Reporting & Compliance

## Acceptance Criteria

Write acceptance criteria that are:

- Testable (you can verify when they're done)
- Specific (no ambiguity)
- Independent (can be verified separately)
- Concise (one line each)

**Good Examples**

- [ ] User can register a cuota and it appears in the list
- [ ] Cuota amount must be greater than zero or API returns 400
- [ ] Created cuota has CentroCostoId populated

**Bad Examples**

- [ ] It works properly
- [ ] User is satisfied
- [ ] System is more efficient

## Creating an Issue

### Step 1: Choose Issue Type

Epic, Story, Bug, or Enhancement?

### Step 2: Write Title

Clear, descriptive, ~60 characters

### Step 3: Write Description

Use the format above - Problem, Solution, Acceptance Criteria

### Step 4: Add Labels

Select Type, Priority, and Effort labels

### Step 5: Assign Milestone

Which phase does this belong to?

### Step 6: Link Related Issues

Epics link to Stories. Stories link to parent Epic.

### Step 7: Review & Submit

Check for:

- Clear language
- Complete acceptance criteria
- Correct labels
- Appropriate milestone

## Examples

### Epic Example

```
Title: EPIC: Phase 1 - Contabilidad general (PUC, comprobantes, libros, cierres)

Description:
Implement core general accounting functionality following Colombian accounting standards.

Acceptance Criteria:
- [ ] Users can create and manage Plan Único de Cuentas (PUC)
- [ ] System records all monetary transactions as comprobantes
- [ ] Financial statements (daily, monthly, annual) generate correctly
- [ ] Period closing process implemented with Journal entry freezing
- [ ] Audit trail tracks all changes to accounting entries

Labels: epic, priority-high, phase-1
Milestone: Phase 1
```

### Story Example

```
Title: Registrar transacción bancaria con validación de CentroCosto

Description:
As an accountant, I want to record bank transactions directly,
so that I can keep cash accounts updated in real-time.

Technical Notes:
Must always require CentroCostoId per business rules.
Should use soft delete instead of hard delete.

Acceptance Criteria:
- [ ] User can enter transaction details (date, amount, beneficiary, CentroCosto)
- [ ] API rejects transactions missing CentroCostoId with 400 error
- [ ] Transaction appears in transaction list immediately after creation
- [ ] User can view transaction details
- [ ] Soft delete functionality works (transaction marked as deleted)
- [ ] API returns 404 for soft-deleted transactions

Labels: story, priority-high, effort-m, phase-1
Milestone: Phase 1
```

## Review Before Submitting

- [ ] Title is clear and specific
- [ ] Description follows the format
- [ ] Acceptance criteria are testable
- [ ] Labels are appropriate
- [ ] Milestone is assigned
- [ ] No duplicate issues exist
- [ ] Related issues are linked

---

Questions? Check [Governance Overview](../../governance/README.md) or ask in a comment.

Good issues = Good development!
