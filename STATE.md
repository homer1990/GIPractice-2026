# Rewrite state

Branch: `v3-simple-clinical-core`

Current checkpoints:

- `2bce4015716570286332c213ec02c5715fc8ab36` — clean-tree restart.
- `621752dd9b58969f6e1c6853d31480238601e6c6` — hidden clinical-session workflow; no user-facing Encounter.
- `09c9bca6a3cc2ee7fab64de1ddb2dcf08d752597` — clinical documentation/endoscopy/media decisions locked.
- `8b58a075008e141aa170548aa0e55ab0dc0a8f54` — biopsy-container / parcel / pathology billing model.
- `a177d2e7667253fac592983e33959652d3760e33` — double-endoscopy and split-container model.
- `ba5cc88b6ef6d8cc0969eceb7dbf90d0875fbd43` — confirmed parcel-based pricing and simplified external-release workflow.

## Fixed principles

1. Encounter is internal relational/session glue only; the UI never exposes Encounter CRUD.
2. One real clinical session may contain multiple Endoscopy records.
3. A double procedure is two independent Endoscopies (for example colonoscopy + gastroscopy) sharing one hidden session/Appointment.
4. Appointment planning data may be corrected; corrections do not silently rewrite completed clinical work.
5. Free text is first-class where clinicians naturally think in prose; structure is added only where it improves retrieval/research/workflow.
6. Automated text recognition may suggest codes/links but must not silently assert clinical meaning.
7. Clinical children have their own identities and may repeat within a session where the real workflow permits it.
8. Media bytes live outside SQL; SQL stores relationships/metadata/hash.
9. No generic repository/mapping framework/CQRS/event bus or generic hospital-EHR abstraction.

## Server shape

One .NET 10 project: `server/GIPractice.Server`.

Feature folders:

- `Patients`
- `Scheduling`
- `Clinical`
- `Pathology`
- `Data`
- `Configuration` (planned)
- `Api` (planned)

## Clinical session / double endoscopy

`ClinicalWriteService` supports:

- `StartEndoscopyAsync(...)` — creates a hidden clinical session and its first Endoscopy.
- `AddEndoscopyAsync(session, ...)` — adds another independent Endoscopy to the same session.

Thus a double procedure is:

```text
Appointment
  -> hidden Encounter/session
       -> Colonoscopy
       -> Gastroscopy
```

Each Endoscopy keeps independent anatomy, findings, completion state, media and pathology.

## History / clinical text

- `PatientHistoryEntry` stores longitudinal history.
- `PatientHistoryKind.ExternalReport` is used for outside reports that later reach the practice.
- Current symptoms/HPI stay on the hidden clinical session.
- `ClinicalExam` is text-first with optional assessment.
- `ClinicalTextAnnotation` may carry suggested/confirmed diagnosis, finding, symptom, medication, anatomy or patient-reference semantics.
- Parser/NLP is not implemented yet.

## Endoscopy

Structured data includes:

- procedure type/indication/priority;
- start/end;
- preparation/sedation;
- completed/limited/aborted outcome;
- maximal extent reached;
- anatomical findings;
- termination reasons;
- lightweight timeline;
- impression/recommendations;
- linked media.

No snare-polypectomy/ablation/clip/interventional framework is planned for this practice.

## Pathology / biopsy tracking

### PathologyCase

One `PathologyCase` belongs to one Endoscopy and groups pathology/billing facts originating from that Endoscopy.

It stores:

- EndoscopyId;
- creation time;
- urgent-pathology flag;
- receipt-requested flag;
- fee-waiver reason (`Doctor`, etc.).

It does not own an assigned pathologist.

### BiopsyContainer

Each physical tube has:

- UUIDv7 database ID;
- globally unique human-readable `LabelCode`;
- case-relative ordinal;
- anatomical site;
- collection time;
- optional description;
- optional `ExternalReleasedAtUtc` / `ExternalReleaseNote`.

The printed label is not derived from Endoscopy ID. Old `XXXA` / `XXXB` numbering may remain as presentation only.

### Practice-managed versus external release

Practice-managed containers enter the normal:

- `Parcel`
- `ParcelContainer`
- handover protocol
- pathology billing/report workflow.

Containers handed to a patient/oncologist/outside destination do **not** create a parallel external pathology subsystem. They simply record that they were released externally and do not enter our Parcel or billing calculation.

If an outside report later comes back, it is entered as `PatientHistoryKind.ExternalReport` rather than `PathologyReport`.

The earlier `BiopsyTransfer` abstraction is discarded.

### Reports

`PathologyReport` is only for practice-managed pathology reports. `PathologyReportContainer` explicitly states which submitted containers the report covers.

### Parcel

A Parcel is one physical courier packet to one Pathologist and stores:

- parcel number;
- recipient Pathologist;
- created/sent timestamps;
- courier/tracking information;
- courier cost/currency.

### Charges and confirmed pricing rule

`PathologyCharge` is a historical financial ledger. Initial biopsy processing is calculated **per Endoscopy from only the containers from that Endoscopy physically present in the Parcel being billed**.

Example:

```text
5 containers collected
2 released externally
3 sent to our pathologist
=> pricing count = 3
```

For the described policy:

- EUR 10 base;
- first 2 billable containers included;
- if >2 billable containers, +EUR 10;
- +EUR 5 for every billable container above 2.

Therefore:

- 5 billable containers => EUR 35;
- 3 billable containers => EUR 25.

`BiopsyPricingPolicy.Calculate(...)` now explicitly accepts `billableContainerCount`, and `ToInitialCharge(...)` requires the Parcel ID whose physical membership produced that count.

A double procedure is priced independently per Endoscopy/PathologyCase even though both Endoscopies share the same Appointment/session.

Doctor/professional-courtesy waiver retains the normal calculated amount but sets charged amount to zero.

### Additional assays

A later assay belongs to the original PathologyCase and may refer to a specific practice-managed BiopsyContainer.

It creates a separate outstanding PathologyCharge. That charge can be billed in a later Parcel without adding the old container to that Parcel's physical membership.

### Handover protocol

Generated from Parcel data, not maintained separately.

It shows physical containers grouped by Endoscopy/PathologyCase plus urgency, doctor/free status, receipt request and calculated/charged amounts for the containers actually in that Parcel. Additional carried-forward assay charges appear separately so physical custody and billing remain distinct.

See `docs/PATHOLOGY_MODEL.md`.

## Media policy

- AV1 default video codec target;
- AVIF default display-still format;
- source-faithful original retained when available;
- derivatives reference source;
- SHA-256 stored for integrity;
- media can link to Endoscopy or finding.

## Next exact development slice

1. Add real SQLite schema/migration for Patient, history, Appointment, hidden Encounter, ClinicalExam/annotations, Endoscopy/findings/termination/events/media, pathology models, Prescription, Visit and INFAI.
2. Add concrete SQLite stores.
3. Add tests proving:
   - one hidden session can contain both colonoscopy and gastroscopy;
   - each Endoscopy has an independent PathologyCase and price calculation;
   - containers have globally unique IDs/labels;
   - externally released containers are excluded from Parcel membership and billing;
   - 5 collected / 3 parcelled => pricing count 3 => EUR 25 under the example policy;
   - five parcelled containers => EUR 35;
   - doctor waiver preserves calculated amount but charges zero;
   - practice-managed reports cover explicit submitted containers;
   - outside reports are represented as Patient history rather than external pathology workflow;
   - later assay charge can be billed in a later Parcel without re-shipping its container;
   - urgent/receipt flags survive into handover query data;
   - media derivative linkage/hash persists.
4. Add Appointment correction persistence.
5. Only after tests pass, expose HTTP endpoints and begin client screens.

## Validation

Assistant environment still has no .NET SDK; no compiler/test success is claimed here. User has Rider/.NET locally and can provide real build feedback.
