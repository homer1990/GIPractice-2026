# GIPractice v2 — Rewrite State

## Status

A clean rewrite has been authorized. The existing implementation is legacy reference only.

## Branch

`v2-clean-architecture`

Baseline: `dev` at `3492722a04630526145bfa25f97b9998c41faf0e`.

## Architectural decisions

- Do not repair the existing WPF/client architecture.
- The Windows/WPF client is retired.
- The new desktop client is Qt/KDE native.
- Prefer Qt 6 + KDE Frameworks 6 + Kirigami/Qt Quick for the application shell and reusable UI.
- Reuse business rules, domain knowledge, terminology and useful workflow ideas from the legacy code; do not mechanically port implementation structure.
- Build the new server/API as a small, explicit architecture rather than generated CRUD modules.
- `Encounter` is the primary clinical event. Visit, Endoscopy, ClinicalExam and INFAI are encounter-specific records.
- Appointments are planning records and may resolve into encounters; encounters may also be created without appointments.
- Keep the server SQL-backed and provider-agnostic where practical.
- The server must support daemon/service hosting and CLI administration.
- Configuration should be external and portable; YAML is the intended human-facing format.
- Preserve UTC at persistence/API boundaries and localize only at the UI boundary.
- Avoid DTO-specific mapping layers when straightforward projections/commands are sufficient.

## Legacy sources

The following are reference material only:

- existing `GIPractice.Wpf*`
- existing `GIPractice.Client`
- generated scaffold modules
- old API/store/controller implementations
- the January 2026 archive, which contains later unfinished Encounter consolidation work

Nothing from those sources is assumed correct until deliberately reintroduced.

## Initial v2 shape

Server side:

- `src/server/GIPractice.Domain`
- `src/server/GIPractice.Application`
- `src/server/GIPractice.Infrastructure`
- `src/server/GIPractice.Api`
- `src/server/GIPractice.Cli`
- `tests/...`

Desktop side:

- `src/client/` — C++/Qt/KDE application
- KDE-native configuration, localization and desktop integration
- API-only access to clinical data; no direct SQL from the client

## First implementation slice

1. Establish clean repository/project layout.
2. Define IDs, Patient, Appointment and Encounter aggregate boundaries.
3. Define Visit, Endoscopy, ClinicalExam and INFAI encounter extensions.
4. Define database provider abstraction and one reference provider for development/tests.
5. Build health/version endpoints and CLI service/config commands.
6. Scaffold the Qt/KDE shell with Home / Schedule / Patients / Endoscopies / Practice / Settings navigation.
7. Add tests before migrating any legacy behavior.

## Resume rule

Read this file first. Update it after every coherent batch of changes so an interrupted session can resume without reconstructing intent from chat history.
