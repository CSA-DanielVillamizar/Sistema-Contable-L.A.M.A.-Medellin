Pull Request Review Checklist

Use this checklist when reviewing pull requests to ensure quality and consistency.

## Code Quality

- [ ] Code follows [Code Standards](CODE-STANDARDS.md)
- [ ] No hardcoded values (use configuration)
- [ ] No console.log/Debug.WriteLine left in code
- [ ] Error handling is appropriate
- [ ] No magic numbers or unclear logic
- [ ] Comments explain "why", not "what"

## Architecture & Design

- [ ] Changes respect Clean Architecture layers
- [ ] No circular dependencies
- [ ] Dependencies injected properly
- [ ] Database queries are efficient (no N+1 queries)
- [ ] CQRS pattern followed for commands/queries
- [ ] No direct EF Core usage outside Infrastructure layer

## Database Changes

- [ ] Migrations are included and tested
- [ ] Soft delete considered for financial entities
- [ ] No breaking schema changes without migration plan
- [ ] Multimoneda fields populated if USD used

## Security

- [ ] No hardcoded credentials or secrets
- [ ] No sensitive data in logs
- [ ] Input validation on server-side
- [ ] Authentication checks present where needed
- [ ] No SQL injection possibilities

## Tests

- [ ] Unit tests included for logic changes
- [ ] Tests pass locally and in CI/CD
- [ ] Edge cases covered
- [ ] Integration tests added for data layer changes
- [ ] Test names follow convention: Method_Scenario_ExpectedResult

## Documentation

- [ ] README updated if needed
- [ ] Architecture documentation reflects changes
- [ ] Code comments added for complex logic
- [ ] Commit messages are clear and reference issues
- [ ] PR description explains the change and rationale

## Business Logic

- [ ] Business rules properly validated
- [ ] Edge cases handled (null values, zero amounts, etc.)
- [ ] CentroCostoId validation for monetary transactions
- [ ] Soft deletes used instead of physical deletion
- [ ] Multimoneda handling correct if applicable

## Language & Conventions

- [ ] Business logic in Spanish
- [ ] Infrastructure in English
- [ ] No emojis in code or comments
- [ ] Variable names clear and meaningful
- [ ] Consistent with existing codebase

## Performance

- [ ] No N+1 database queries
- [ ] Async/await used appropriately
- [ ] Large data sets handled efficiently
- [ ] No unnecessary memory allocations

## Special Cases

### New Feature

- [ ] Acceptance criteria met (from issue)
- [ ] User stories understood and implemented
- [ ] Edge cases considered
- [ ] Documentation updated

### Bug Fix

- [ ] Root cause identified and fixed (not just symptom)
- [ ] Test added to prevent regression
- [ ] Performance impact assessed

### Refactoring

- [ ] No functional changes
- [ ] All tests pass
- [ ] Readability improved
- [ ] No performance regression

## Approval Criteria

The PR can be merged when:

1. All checks pass
2. At least one approval from code owner
3. All conversations resolved
4. CI/CD pipeline green
5. No breaking changes without migration plan

---

**Reviewer**: ___________
**Date**: ___________
**Notes**: ___________
