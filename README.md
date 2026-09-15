# GIPractice

Clean rewrite of the gastroenterology practice system.

This branch intentionally starts from a small model instead of porting the legacy architecture.

## Shape

- `server/GIPractice.Server` — one .NET 10 server project
- `client` — Qt 6 / KDE Frameworks 6 / Kirigami desktop client
- `tests` — focused server tests
- `docs` — decisions and business rules

## Core rule

Users work with Patients, Appointments, Endoscopies, Exams, Prescriptions, Visits and INFAI records.

`Encounter` is not a user-facing entity. It is an internal database/session record used to bind clinical objects that happened together.

See `docs/ARCHITECTURE.md` and `STATE.md` before making structural changes.
