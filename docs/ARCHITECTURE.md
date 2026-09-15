# Architecture

## 1. Keep the application smaller than the clinic

GIPractice is practice-management software for one gastroenterology practice, not a general medical-record framework.

Prefer explicit feature code over reusable architecture until repetition is real.

## 2. User-facing concepts

The user works with:

- Patient
- Appointment
- Endoscopy
- Clinical exam
- Prescription
- Visit
- INFAI
- later: biopsy/pathology/media/report objects required by those workflows

There is no Encounter screen, New Encounter command, Encounter editor or Encounter CRUD API.

## 3. Encounter is internal glue

The database contains an internal `encounters` row:

```text
encounters
----------
id
patient_id
appointment_id?    -- optional planning provenance
started_at_utc
ended_at_utc?
```

Its purpose is relational:

```text
encounter 1 --- * endoscopies
          1 --- * clinical_exams
          1 --- * prescriptions
          1 --- * visits
          1 --- * infai_tests
```

Each child has its own ID. We do not enforce one child of each kind because that is an unnecessary future restriction.

Clinical child rows do not duplicate PatientId or AppointmentId. Queries obtain those through Encounter.

## 4. How Encounter is used without exposing it

When the user chooses **New Endoscopy** for a patient or an appointment, the server transaction creates:

1. an internal Encounter row;
2. the Endoscopy row referencing it.

When the user adds an Exam or Prescription during that same clinical session, the client/service carries the internal session identifier and the server inserts the new row under the same Encounter.

That identifier is infrastructure/application context. It is not shown as a user-editable object in QML.

A walk-in Exam, Endoscopy, Prescription or Visit works identically except that the generated Encounter has no AppointmentId.

## 5. Appointment is planning, and planning is correctable

Appointment fields are ordinary editable data:

```text
id
patient_id
type_code
start_utc
duration_minutes
status
notes
```

Except for the Appointment ID itself, these fields are not treated as metaphysical invariants. Humans can enter the wrong patient, type or time.

After clinical work has been created, correcting the Appointment does not rewrite the Encounter or its clinical children. The Appointment records the corrected planning record; the clinical session remains separate truth.

Audit/revision history will record material corrections when that subsystem is added.

## 6. Clinical patient correction

Encounter is the single patient link for all clinical children in that session. If a clinical session was assigned to the wrong patient, a privileged correction changes the Encounter patient once and records the correction in audit history. Child rows require no mass update.

## 7. Server organization

Use one .NET 10 executable project.

```text
GIPractice.Server/
  Patients/
  Scheduling/
  Clinical/
  Data/
  Configuration/
  Api/
```

Do not create Domain/Application/Infrastructure assemblies merely to enforce conceptual layers. Add another assembly only when there is a concrete deployment or dependency reason.

Data access remains typed SQL. LINQ to DB is the intended query/data-access layer; FluentMigrator is the intended migration layer. Database support target remains SQLite, PostgreSQL, SQL Server and MySQL/MariaDB.

## 8. Client organization

C++ / Qt 6 / Qt Quick / KDE Frameworks 6 / Kirigami.

QML sees user concepts, never Encounter. C++ services may retain an opaque clinical-session key while a workflow is open.

Examples of UI commands:

- New Endoscopy
- New Exam
- New Prescription
- New Visit
- New INFAI
- Patient Arrived
- Move Appointment

Not:

- New Encounter
- Edit Encounter
- Encounter Details

## 9. Avoid premature rules

Do not add a global active-Encounter table, generic EncounterPlan hierarchy, generic repository, mapping framework, event bus or one-row-per-component restriction before an actual workflow requires it.

Business rules should be implemented at the operation that needs them and promoted into reusable abstractions only after repetition proves the abstraction useful.
