# Rewrite state

Branch: `v3-simple-clinical-core`

Current checkpoints:

- `2bce4015716570286332c213ec02c5715fc8ab36` — clean-tree restart with one server project and no legacy projects in this branch.
- `621752dd9b58969f6e1c6853d31480238601e6c6` — hidden clinical-session workflow service; no user-facing Encounter operations.
- `09c9bca6a3cc2ee7fab64de1ddb2dcf08d752597` — clinical documentation/endoscopy/media decisions locked in `docs/CLINICAL_MODEL.md`.
- `64e1c29f2da6ba2b0bf6800a71bd1223022c7de2` — clinical model/persistence-record checkpoint.
- `8b58a075008e141aa170548aa0e55ab0dc0a8f54` — biopsy-container / parcel / pathology billing model documented.

## Fixed principles

1. The UI never exposes Encounter as something the user creates, edits, searches or navigates to.
2. Encounter is internal relational/session glue joining clinical work performed for one patient.
3. Free text is first-class where clinicians naturally think in prose; structure is added where it improves retrieval/research/workflow without making documentation slower.
4. Automated text recognition may suggest codes/links but must not silently assert clinical meaning.
5. Appointment planning data may be corrected when entered incorrectly; corrections do not silently rewrite completed clinical work.
6. Clinical child records have their own identities; they are not constrained to one row of each type per Encounter.
7. Media bytes live outside SQL; SQL stores identity/relationships/technical metadata and SHA-256.
8. There is no generic repository/mapping framework/CQRS/event-bus framework or Encounter CRUD/UI.

## Server shape

One .NET 10 project: `server/GIPractice.Server`.

Feature folders now include:

- `Patients`
- `Scheduling`
- `Clinical`
- `Pathology`
- `Data`
- `Configuration` (planned)
- `Api` (planned)

## History / clinical text

- `PatientHistoryEntry` stores longitudinal history such as diagnoses, surgery, medication, allergies, family history and social history.
- Current symptoms/HPI belong to the hidden clinical session and are not silently promoted into permanent patient history.
- `ClinicalExam` is text-first with optional assessment.
- `ClinicalTextAnnotation` can carry suggested/confirmed diagnosis, finding, symptom, medication, anatomy or patient-reference semantics with optional coding-system/code metadata.
- Patient references retain the linked PatientId under readable text.
- Parser/NLP is deliberately not implemented yet.

## Endoscopy

Structured procedure data currently includes:

- procedure type and indication;
- routine/urgent/emergency procedure priority;
- start/end;
- preparation mode/quality;
- sedation mode;
- completed/limited/aborted outcome;
- maximal extent reached;
- anatomical findings;
- termination reasons linked to findings where useful;
- lightweight procedure timeline;
- impression/recommendations;
- linked clinical media.

There is intentionally no snare-polypectomy/ablation/clip/interventional-endoscopy framework.

The earlier generic `EndoscopySpecimen` concept has been removed. Physical biopsy tubes are modeled explicitly as `BiopsyContainer` under the pathology feature.

## Pathology / biopsy tracking

The model separates four concepts that Excel previously conflated:

1. `PathologyCase` — one endoscopy's pathology work and billing context.
2. `BiopsyContainer` — one physical labeled tube/container with its own UUIDv7 and globally unique human-readable `LabelCode`.
3. `Parcel` — one physical courier shipment to one `Pathologist`; courier cost belongs here.
4. `PathologyCharge` — a financial ledger entry which can be billed in the current or a later parcel.

`Pathologist` is now data, not a separate spreadsheet tab. UI tabs/filters can still be provided without partitioning the database.

### PathologyCase

Initial model stores:

- EndoscopyId;
- creation time;
- urgent-pathology flag;
- receipt-requested flag;
- fee-waiver reason (`Doctor`, etc.);
- assigned pathologist when known.

### BiopsyContainer

Each physical container stores:

- UUIDv7 database ID;
- globally unique human-readable `LabelCode` (not derived from Endoscopy ID);
- case-relative ordinal;
- anatomical site;
- collection time;
- optional description.

Exact printed label-code format is intentionally not fixed until the label-printer workflow is designed.

### Parcel

Stores:

- parcel number;
- recipient Pathologist;
- created/sent timestamps;
- courier name/tracking number;
- courier cost and currency.

`ParcelContainer` tracks physical chain-of-custody: which containers were actually in that shipment.

### PathologyCharge

Charges are historical snapshots containing:

- PathologyCase;
- charge kind (`InitialBiopsy`, `AdditionalAssay`, adjustment);
- calculated amount;
- actually charged amount;
- currency;
- waiver reason;
- pricing-policy code/version;
- container-count snapshot when relevant;
- optional related container/assay;
- optional `BilledInParcelId`.

This permits a later assay to be billed in the **next** Parcel without falsely recording the original tube as physically re-shipped.

### Pricing

`BiopsyPricingPolicy` is implemented as configurable calculation logic.

For the example policy supplied by the practice:

- EUR 10 base;
- first 2 containers included;
- if >2 containers, +EUR 10 surcharge;
- +EUR 5 for every container above 2.

Thus 5 containers calculate to EUR 35.

For fee-waived cases (for example patient is a doctor), both values are retained:

- calculated amount remains the normal price;
- charged amount becomes zero;
- waiver reason is explicit.

Old charges never recalculate from future price rules.

### Additional assays

`PathologyAssay` belongs to the original PathologyCase and may optionally reference a specific BiopsyContainer.

The assay can generate a new `PathologyCharge` with no `BilledInParcelId`. That outstanding charge can later be attached financially to the next Parcel while the original container remains associated only with the parcel in which it was physically sent.

### Pathology reports

`PathologyReport` belongs to PathologyCase, not Parcel. Multiple reports/addenda are allowed and a report may reference the assay which produced the addendum.

### Biopsy handover protocol

The protocol is planned as a generated report from Parcel data with two sections:

A. Physical handover, grouped by PathologyCase/Endoscopy, showing containers/sites, urgency, receipt request, fee waiver/doctor status, calculated amount and charged amount.

B. Additional charges carried forward from earlier cases/assays and billed in this Parcel, without implying those old containers were physically included.

See `docs/PATHOLOGY_MODEL.md` for the full relationship explanation.

## Media policy

- default video codec target: AV1;
- default display-still format: AVIF;
- preserve source-faithful/canonical originals when available;
- derivatives reference their source;
- SHA-256 is stored for integrity;
- media can link to the Endoscopy or a particular finding.

## Next exact development slice

1. Add the first real SQLite schema/migration for Patient, PatientHistoryEntry, Appointment, hidden Encounter, ClinicalExam/annotations, Endoscopy/findings/termination/events/media, PathologyCase/BiopsyContainer/Pathologist/Parcel/ParcelContainer/Assay/Charge/Report, Prescription, Visit and INFAI.
2. Add concrete SQLite stores for clinical and pathology writes.
3. Add tests proving:
   - an Endoscopy creates its hidden Encounter automatically;
   - Exam/Prescription can reuse that session;
   - biopsy containers have globally unique IDs/label codes and remain tied to one Endoscopy via PathologyCase;
   - the 5-container example calculates to EUR 35;
   - Doctor waiver preserves calculated amount but charges EUR 0;
   - physical Parcel membership is independent from `PathologyCharge.BilledInParcelId`;
   - a later assay can be charged in a later Parcel without re-shipping the original container;
   - urgent pathology flag and receipt-requested flag survive into handover-protocol query data;
   - media derivatives retain original linkage and SHA-256 metadata.
4. Add Appointment correction persistence.
5. Only after those tests pass, expose HTTP endpoints and begin Schedule/Endoscopy/Pathology client screens.

## Validation

The assistant execution environment still has no .NET SDK, so these changes have not been compiler/test validated here.

The user has Rider/.NET locally and can provide real build feedback. Do not report the branch as passing until an actual build/test run is observed.
