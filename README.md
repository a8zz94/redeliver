# Redeliver

Reliable webhook delivery for .NET.

This is a learning and portfolio project exploring reliable delivery and failure handling. It is at an early stage and not yet functional. For production use, consider established options such as Svix.

## Tests

- `tests/Redeliver.AcceptanceTests`: Gherkin scenarios for behaviour across the system
- `tests/Redeliver.UnitTests`: fast tests for individual pieces of logic

```bash
dotnet test
```

Design decisions are recorded in `docs/decisions`.
