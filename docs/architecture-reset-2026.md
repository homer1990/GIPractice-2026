# 2026 architecture reset

## Clinical aggregate

`Encounter` is the root clinical event. `Visit`, `Endoscopy`, `Exam` and `InfaiTest` are one-to-one detail records and no longer carry independent patient/date/appointment state.

This is intentional: a patient can arrive without an appointment, and an appointment may become exactly one clinical encounter. The scheduler operates on appointments and encounters as peer timeline items.

## Database providers

The runtime model must remain relational-provider neutral. Supported provider keys are:

- `SqlServer` / `MSSQL`
- `SQLite`
- `PostgreSQL`
- `MySQL`
- `MariaDB`

Do not put provider-specific SQL expressions or column types in entity configurations unless they are isolated behind provider-specific migrations.

## Migrations

The migrations currently in `GIPractice.Infrastructure/Migrations` describe the pre-Encounter model and are retained only as historical migration data until the upgrade path for existing databases is explicitly designed and tested.

The API no longer calls `Migrate()` on normal service startup. Use `db-pending`, `db-check` and, only after reviewing the target database, `db-migrate`.

A new provider-aware migration baseline/upgrade path must be generated before this branch is used against production data.

## Secrets

No JWT signing key belongs in `appsettings*.json`. Configure `Jwt:SigningKey` through environment variables, a service secret store, or the future configurator. Any key previously committed to the repository must be considered compromised and rotated.
