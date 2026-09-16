# Rewrite state

Branch: `v3-simple-clinical-core`

Current checkpoints:

- `2bce4015716570286332c213ec02c5715fc8ab36` — clean-tree restart with one server project and no legacy projects in this branch.
- `621752dd9b58969f6e1c6853d31480238601e6c6` — hidden clinical-session workflow service; no user-facing Encounter operations.
- `09c9bca6a3cc2ee7fab64de1ddb2dcf08d752597` — clinical documentation/endoscopy/media decisions locked in `docs/CLINICAL_MODEL.md`.
- `64e1c29f2da6ba2b0bf6800a71bd1223022c7de2` — current clinical model/persistence-record checkpoint.

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
11. Free text remains first-class where clinicians naturally think in prose; structure is added only where it improves retrieval/research/workflow without making documentation slower.
12. Automated text recognition may suggest codes/links but must not silently assert clinical meaning.

## Server shape

One .NET 10 project: `server/GIPractice.Server`.

Feature folders, not architectural projects:

- `Patients`
- `Scheduling`
- `Clinical`
- `Data`
- `Configuration`
- `Api`

No generic repository, generic mapping framework, CQRS/event-bus framework or Encounter CRUD/API/UI.

## History model

History is split into two concepts.

- `PatientHistoryEntry` is longitudinal patient history: diagnosis, surgery, medication, allergy, family history, social history or other durable facts. Entries are text-first and may optionally carry code-system/code metadata.
- Current symptoms/history of present illness belongs to the hidden clinical session (`EncounterRow.HistoryText`) and is not automatically merged into permanent patient history.

## Clinical examination

`ClinicalExam` is currently:

- UUIDv7 ID;
- text-first `ClinicalDocument`;
- optional assessment.

`ClinicalDocument` can carry `ClinicalTextAnnotation` entries for diagnosis, finding, symptom, medication, anatomy, patient reference or other semantic marks.

Annotations may carry a code system/code and/or a referenced PatientId. `ConfirmedByUser` distinguishes accepted meaning from parser suggestions.

The parser itself is deliberately not implemented yet.

## Endoscopy model

Endoscopy now carries structured procedure data rather than an opaque `ReportJson` blob:

- type code;
- indication;
- procedure priority: routine / urgent / emergency;
- start/end;
- preparation mode and quality code;
- sedation mode;
- outcome: in progress / completed / limited / aborted;
- maximal extent reached;
- impression;
- recommendations.

Child records:

- `EndoscopyFinding`: anatomical site + narrative description + optional finding/severity code and size;
- `EndoscopyTerminationReason`: why the procedure was limited/aborted, optionally linked to a finding;
- `EndoscopyEvent`: lightweight preparation/sedation/procedure/recovery/debrief timeline;
- `EndoscopySpecimen`: anatomical source + routine/urgent pathology priority + reason, optionally linked to a finding;
- `ClinicalMedia`: media metadata/linkage.

There is intentionally no snare-polypectomy/ablation/clip/interventional-endoscopy framework. The practice does not perform those procedures. Biopsy is represented by the specimen it produces; rare actions can initially be recorded in the timeline/narrative.

The write service now has concrete operations to start an endoscopy, add findings, termination reasons, timeline events and specimens, and finish the endoscopy as completed/limited/aborted.

## Media policy

- Media bytes live outside SQL.
- SQL stores identity, endoscopy/finding relationship, storage key, MIME/container/codec metadata and SHA-256.
- Default video codec target: AV1.
- Default display-still format: AVIF.
- Preserve the canonical/source-faithful original when available; display/thumbnail derivatives may be generated from it.
- Media may link to the overall Endoscopy or to a specific finding.

## Report policy

The printable endoscopy report is generated from structured findings plus their narrative descriptions, procedure timeline/outcome, specimens, impression and recommendations. It is not the sole source of truth stored as one opaque report document.

## Client shape

Qt 6 + KDE Frameworks 6 + Kirigami. The C++ client service layer may carry internal clinical-session identifiers, but QML/UI never presents Encounter as a domain object.

The intended UX rule is: prose first where humans think in prose, structure where it naturally helps. Client-side recognition may underline/suggest ICD/SNOMED-like concepts or Patient links, but acceptance belongs to the user.

## Next exact development slice

1. Add the first real SQL schema/migration for Patient, PatientHistoryEntry, Appointment, hidden Encounter, ClinicalExam/annotations, Endoscopy/findings/termination/events/specimens/media, Prescription, Visit and INFAI.
2. Add a concrete SQLite-backed `IClinicalWriteStore` first.
3. Add focused tests proving:
   - an Endoscopy creates a hidden Encounter automatically;
   - a ClinicalExam and Prescription can reuse the same hidden session;
   - findings are stored by anatomical site;
   - limited/aborted procedures retain extent + termination reason;
   - urgent specimen priority is independent from procedure priority;
   - media derivatives can reference a canonical original and retain SHA-256 metadata.
4. Add Appointment correction persistence.
5. Only after those tests pass, expose the first HTTP endpoints and begin the Schedule/Endoscopy client screens.

## Validation

The available assistant execution environment still has no .NET SDK, so this branch has not been compiler/test validated there.

The user now has Rider/.NET locally and can provide the first real build feedback. Do not report the branch as passing until an actual build/test run is observed.
