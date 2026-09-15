# GIPractice v2 Architecture

## Principles

1. The database is authoritative for persisted clinical state.
2. The desktop client never connects to SQL directly; it talks only to the API.
3. Domain state is normalized once. Secondary clinical tables do not duplicate Patient, Appointment, timestamps or urgency from Encounter.
4. Read models are projected directly for their API use-case instead of passing through generic CRUD/mapping layers.
5. Write workflows are explicit commands with validation and transactional boundaries.
6. Database-specific code is isolated to provider configuration, migrations and exceptional dialect features.
7. No generated CRUD module tree. A feature exists only when it has real behavior.

## Technology

### Server

- .NET 10 LTS
- ASP.NET Core
- LINQ to DB for typed SQL/query composition
- FluentMigrator for schema migrations
- YAML configuration
- Generic Host so the same executable can run interactively, under systemd, or as a Windows Service
- Separate CLI executable reusing the same application/infrastructure assemblies

### Desktop

- C++
- Qt 6
- Qt Quick / QML
- KDE Frameworks 6
- Kirigami
- KI18n for translations
- KConfig for client preferences
- Breeze/KDE-native appearance via the desktop style
- QNetworkAccessManager-based API layer

## Server projects

- `GIPractice.Domain`: entities, value objects, invariants; no SQL/web/UI dependencies.
- `GIPractice.Application`: use-cases, commands, queries, authorization boundaries and interfaces.
- `GIPractice.Infrastructure`: SQL providers, LINQ to DB mappings, migrations, filesystem/certificate implementations.
- `GIPractice.Api`: HTTP transport, authentication, request/response contracts and hosting.
- `GIPractice.Cli`: database/configuration/certificate/service administration commands.

Dependencies point inward only:

`Api -> Application <- Infrastructure`

`Cli -> Application <- Infrastructure`

`Application -> Domain`

`Infrastructure -> Domain + Application`

## Planning truth vs clinical reality

Appointment and Encounter deliberately model different facts.

An Appointment records the plan. Its **AppointmentType is immutable** once the appointment is created. The scheduled time and duration remain operational scheduling data and may still change after the patient has arrived when real workflow requires it (for example, delay after drinking water or because the patient is running late).

An Encounter records what actually happened. AppointmentType does not constrain Encounter contents.

Walk-ins have no appointment at all.

## Encounter model

`Encounter` is the prime clinical-session record, but it has no mutually-exclusive clinical kind.

Common state lives only on Encounter:

- EncounterId
- PatientId
- optional AppointmentId
- StartUtc
- EndUtc
- Status
- RequiresExclusiveSlot
- IsUrgent
- Notes
- audit/concurrency metadata

Clinical components use `EncounterId` as both primary key and foreign key:

- `Visits`
- `Endoscopies`
- `ClinicalExams`
- `Prescriptions`
- `InfaiTests`

Different component tables may coexist for the same EncounterId. Examples:

- Endoscopy + ClinicalExam + Prescription
- walk-in Endoscopy + ClinicalExam (for example a HEINE/orthoscopy workflow)
- Visit + Prescription
- INFAI only

This is composition, not inheritance and not a Visit -> Endoscopy hierarchy.

A single component type occurs at most once per encounter in the initial model. If a future workflow genuinely requires repeated same-type components, that component will receive its own child identity rather than weakening Encounter semantics globally.

Consequences:

- no duplicated PatientId on Endoscopy/Visit/Exam/Prescription/INFAI;
- no duplicated appointment link;
- no duplicated performed/start date;
- no possibility for an Endoscopy to claim a different patient than its Encounter;
- a single real clinical session can contain all work actually performed;
- one timeline query can combine every clinical session cheaply.

### Exclusive clinical slot

The existing practice-wide concurrency rule is represented as an operational property of the encounter, not as an EncounterKind.

- INFAI-only encounters do not require the exclusive slot.
- An encounter containing any non-INFAI component requires the exclusive slot.
- A composite encounter still claims only one slot regardless of how many components it contains.

The slot is claimed transactionally through `practice_state.active_encounter_id`.

## Appointment model

Appointments represent planned work, not clinical truth.

An Appointment has:

- immutable AppointmentType;
- scheduled start and duration, which may be changed while Scheduled or Arrived;
- lifecycle status;
- urgency and notes.

An Appointment can be:

- scheduled;
- arrived;
- moved while still arrived;
- cancelled;
- resolved into an Encounter;
- no-show.

Walk-in encounters have no AppointmentId.

Resolving an appointment is one transaction: validate current state, create Encounter + all actual component rows, link the Appointment, then transition the Appointment to Resolved.

The eventual Encounter may contain more, fewer, or different clinical components than were implied by AppointmentType. The appointment type remains unchanged as historical planning truth.

## IDs

New v2 tables use UUIDv7 identifiers generated by the server. They are portable across supported SQL engines and roughly time ordered without relying on vendor-specific identity columns.

## Persistence

LINQ to DB supplies typed SQL generation and projections. Persistence models may map directly to simple domain records where there is no impedance mismatch; complex aggregates receive explicit repository methods.

There is no generic repository, no `EfStore`, and no DTO-to-entity converter layer.

Reads should normally be expressed as:

`query filters -> SQL projection -> response record`

Writes should normally be expressed as:

`validated request -> application command -> domain transition -> transaction`

## Mapping policy

The previous proposed `IMappableField<T>` hierarchy is not carried over as a framework.

Reusable semantic fields are represented by strongly typed values/enums/IDs and composable query expressions. API read DTOs are projected directly from queries. Write DTOs are converted explicitly at the application boundary. If repeated mechanical mapping later becomes measurable duplication, a source generator may be introduced locally rather than making mapping itself a domain abstraction.

## Database support

Target providers:

- SQLite
- PostgreSQL
- SQL Server
- MySQL / MariaDB

Provider-neutral schema features are the default. Provider-specific optimizations must be optional and isolated behind capability checks.

Migrations are authored through FluentMigrator and exercised against every provider in CI before a release is considered database-portable.

## Configuration

Human configuration is YAML. Environment variables and CLI switches may override secrets/runtime values.

Configuration owns:

- listener URLs;
- database provider + connection parameters;
- TLS/certificate policy;
- media/document storage paths;
- authentication settings;
- logging;
- practice identity/settings.

Secrets are never written into source-controlled defaults.

## Qt/KDE client

The client shell uses Kirigami navigation with these primary destinations:

- Home
- Schedule
- Patients
- Endoscopies
- Practice
- Settings

The client contains presentation state and local preferences only. Clinical business rules remain server-side.

Large lists use server-side search, ordering and pagination. The UI may debounce input, but it never loads an entire clinical table merely to filter locally.

Client C++ exposes models/services to QML; QML owns presentation/layout. Network DTOs do not become QML business objects directly.

## Legacy policy

Legacy projects remain in the branch temporarily for reference. New v2 projects must not reference them. Once parity has been demonstrated for a slice, the corresponding legacy area can be deleted deliberately.
