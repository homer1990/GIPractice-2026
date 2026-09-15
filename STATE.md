# Rewrite state

Branch: `v3-simple-clinical-core`

## Why this branch exists

The previous implementation and the first v2 rewrite both accumulated abstractions faster than the actual practice workflows justified. This branch restarts from the minimum model that fits the real clinic.

## Fixed principles

1. The UI never exposes Encounter as something the user creates, edits, searches or navigates to.
2. Encounter is an internal relational/session row used to bind clinical records that happened together.
3. The user creates real objects: Appointment, Endoscopy, ClinicalExam, Prescription, Visit and INFAI.
4. Starting a clinical object automatically creates the internal Encounter when needed.
5. Adding another clinical object during the same session reuses the same internal Encounter automatically.
6. Clinical child tables have their own IDs and an `encounter_id` foreign key. They are not limited to one row of each type.
7. Appointment records describe planning. Their patient, type, start and duration can be corrected when entered incorrectly.
8. Correcting an Appointment does not silently rewrite clinical records that already happened.
9. Encounter holds the patient identity for the clinical session, so child clinical rows do not duplicate PatientId.
10. Corrections to finalized clinical data will eventually be revision/audit operations rather than hidden overwrites.

## Deliberately not designed yet

- generic mapping framework
- generic repository
- event bus / CQRS framework
- Encounter CRUD/API/UI
- hard global active-Encounter singleton
- medication-order subsystem
- pathology/media/report internals beyond the requirements needed by the next feature slice

## Server shape

One .NET 10 project: `server/GIPractice.Server`.

Feature folders, not architectural projects:

- `Patients`
- `Scheduling`
- `Clinical`
- `Data`
- `Configuration`
- `Api`

Dependencies are kept obvious by code organization and tests instead of assembly proliferation.

## Client shape

Qt 6 + KDE Frameworks 6 + Kirigami. The C++ client service layer may carry internal clinical-session identifiers, but QML/UI never presents Encounter as a domain object.

## Next slice

1. Define the minimal relational schema and row mappings.
2. Implement Appointment corrections without artificial immutability.
3. Implement Endoscopy/Exam/Prescription/Visit/INFAI creation with automatic hidden Encounter creation/reuse.
4. Add focused SQLite tests for those workflows.
5. Only then add HTTP endpoints and the Schedule UI.
