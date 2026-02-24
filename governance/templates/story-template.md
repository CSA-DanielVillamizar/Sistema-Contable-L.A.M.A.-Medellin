Story Issue Template

Copy this template when creating a new story.

```markdown
Title: [Action verb] [resource] - [outcome]

## User Story
As a [role/persona], I want [action], so that [business value].

## Description
Additional context or details about what needs to be built.

## Acceptance Criteria
How will we know this is done? Specific, testable outcomes.
- [ ] Criterion 1 - clear and measurable
- [ ] Criterion 2 - clear and measurable
- [ ] Criterion 3 - clear and measurable

## Technical Notes
- Implementation approach
- Architecture considerations
- Database changes needed
- Affected layers or components
- Testing strategy

## Definition of Done
- [ ] Code implemented and peer reviewed
- [ ] Unit tests written and passing
- [ ] Documentation updated
- [ ] No hardcoded secrets or sensitive data
- [ ] Follows Code Standards
- [ ] CentroCostoId validated (if financial)

## References
- Parent Epic: #XXX
- Related Issues: #YYY, #ZZZ
- Documentation: [link]

## Labels
story, priority-[level], effort-[size], phase-[number]

## Milestone
Phase X
```

## Example

```markdown
Title: Register bank transaction with validation

## User Story
As an accountant, I want to record bank transactions directly, so that our cash accounts stay updated in real-time.

## Description
Users need a simple interface to record bank deposits, withdrawals, and transfers. The system must validate that all transactions specify a CentroCosto (cost center) per business policy.

## Acceptance Criteria
- [ ] User can enter transaction details: date, amount, description, beneficiary, CentroCosto
- [ ] System validates amount > 0, otherwise returns 400 error
- [ ] System validates CentroCostoId is present, otherwise returns 400 error
- [ ] Successful transaction appears immediately in the transaction list
- [ ] User can view full transaction details
- [ ] Transaction can be soft-deleted (marked as deleted, not removed from DB)
- [ ] API correctly hides soft-deleted transactions from list queries
- [ ] Returned list includes pagination (10 per page)
- [ ] API documentation includes all endpoints

## Technical Notes
Implementation:
- Create RegistrarTransaccionCommand in Application layer
- Implement TransaccionBancaria entity in Domain layer
- Create ITransaccionRepository interface in Domain
- Implement repository in Infrastructure layer
- Create TransaccionesController in API layer

Database:
- Add Transaccion table with fields: Id, Monto, Descripcion, Beneficiario, CentroCostoId, Fecha, IsDeleted
- Add foreign key to CentroCosto table
- Add index on Fecha and CentroCostoId

Testing:
- Unit test: RegistrarTransaccionCommandHandler with valid/invalid inputs
- Unit test: TransaccionBancaria domain entity
- Integration test: Repository operations
- API test: POST/GET endpoints

## Definition of Done
- [ ] Code written following Code Standards
- [ ] All acceptance criteria verified
- [ ] Unit tests 100% passing locally
- [ ] Integration tests passing
- [ ] No hardcoded CentroCostoId defaults
- [ ] Soft delete implemented correctly
- [ ] Swagger/API docs generated
- [ ] Code reviewed and approved
- [ ] Ready for customer demo

## References
- Parent Epic: #15 (EPIC: Phase 1 - Contabilidad general)
- Related: #28 (CentroCosto management)
- Database schema: [link to docs]

## Labels
story, priority-high, effort-m, phase-1

## Milestone
Phase 1 - Sprint 3
```

## Tips

**Write Good Acceptance Criteria**
- Start with action verbs: User can, System validates, API returns
- Be specific about inputs/outputs
- Include both happy path and error cases
- Make them independently testable

**Estimate Effort**
- XS (Extra Small): 1-2 hours
- S (Small): Half day
- M (Medium): 1-2 days
- L (Large): 3-5 days
- XL (Extra Large): More than a week

**Clear Dependencies**
- Reference parent epic
- Link related stories
- Mention blocking issues
- Note external dependencies

---

Ready to create? [Issue Creation Guide](ISSUE-CREATION-GUIDE.md)
