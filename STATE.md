# Rewrite state

Branch: `v3-simple-clinical-core`

Current checkpoints:

- `2bce4015716570286332c213ec02c5715fc8ab36` — clean-tree restart.
- `621752dd9b58969f6e1c6853d31480238601e6c6` — hidden clinical-session workflow; no user-facing Encounter.
- `09c9bca6a3cc2ee7fab64de1ddb2dcf08d752597` — clinical documentation/endoscopy/media decisions locked.
- `8b58a075008e141aa170548aa0e55ab0dc0a8f54` — biopsy-container / parcel / pathology billing model.
- `4b8f35cd2186e5dce29ef7291581ca7eddcb6e65` — split biopsy routing + double-endoscopy support.

## Fixed principles

1. Encounter is internal relational/session glue only; the UI never exposes Encounter CRUD.
2. One real clinical session may contain multiple Endoscopy records.
3. A double procedure is modeled as two Endoscopies (for example colonoscopy + gastroscopy) sharing one hidden session/Appointment, not as one special combined Endoscopy.
4. Appointment planning data may be corrected; corrections do not silently rewrite completed clinical work.
5. Free text is first-class where clinicians naturally think in prose; structured data is added where it improves retrieval/research/workflow.
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

`ClinicalWriteService` now supports both:

- `StartEndoscopyAsync(...)` — creates a hidden clinical session and its first Endoscopy.
- `AddEndoscopyAsync(session, ...)` — adds another independent Endoscopy to the same session.

This supports a double procedure cleanly:

```text
Appointment
  -> hidden Encounter/session
       -> Colonoscopy
       -> Gastroscopy
```

Each Endoscopy keeps independent anatomy, findings, completion state, media and pathology.

## History / clinical text

- `PatientHistoryEntry` stores longitudinal history.
- Current symptoms/HPI stay on the hidden clinical session.
- `ClinicalExam` is text-first with optional assessment.
- `ClinicalTextAnnotation` may carry suggested/confirmed diagnosis, finding, symptom, medication, anatomy or patient-reference semantics.
- Parser/NLP is not implemented yet.

## Endoscopy

Structured data currently includes:

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

One `PathologyCase` belongs to one Endoscopy and groups pathology work originating from that Endoscopy.

It stores:

- EndoscopyId;
- creation time;
- urgent-pathology flag;
- receipt-requested flag;
- fee-waiver reason (`Doctor`, etc.).

It deliberately does **not** own an assigned pathologist. Containers from one Endoscopy may be routed to different destinations.

### BiopsyContainer

Each physical tube has:

- UUIDv7 database ID;
- globally unique human-readable `LabelCode`;
- case-relative ordinal;
- anatomical site;
- collection time;
- optional description.

The printed label is not derived from Endoscopy ID. Old `XXXA` / `XXXB` style numbering may still be presented for convenience, but it is not identity.

### Split routing

Normal courier shipment is represented only by:

- `Parcel`
- `ParcelContainer`

Exceptional direct handover is represented by `BiopsyTransfer`, for example:

- urgent tube handed to the patient for their oncologist;
- tube handed directly to another doctor/third party.

This allows one Endoscopy's containers to split between destinations without splitting or duplicating the Endoscopy itself.

### Reports

`PathologyReport` belongs to the originating PathologyCase, but because one case may split between destinations, report coverage is explicit through `PathologyReportContainer`.

A report therefore states exactly which containers it covers and may identify its Pathologist.

### Parcel

A Parcel is one physical courier packet to one Pathologist and stores:

- parcel number;
- recipient Pathologist;
- created/sent timestamps;
- courier/tracking information;
- courier cost/currency.

### Charges

`PathologyCharge` is a financial ledger independent of physical shipment.

It stores historical calculated/charged amounts, waiver, pricing policy/version, container-count snapshot, optional assay/container and optional `BilledInParcelId`.

A later assay can therefore be billed in the next Parcel without pretending the original container was shipped again.

### Pricing and doubles

`BiopsyPricingPolicy` is configurable and snapshots its result in the charge.

For the described example policy, five containers calculate to EUR 35.

In a double procedure, colonoscopy and gastroscopy are priced independently because each is a separate Endoscopy with its own PathologyCase/container count.

If containers from one Endoscopy are split across destinations, the exact billing count rule (all containers vs only those processed by a given pathologist) is intentionally **not hard-coded yet**. The charge model supports either policy; confirm the real billing rule before SQL/tests freeze it.

### Handover protocol

The biopsy handover protocol is generated from Parcel data, not maintained separately.

It will show physical containers grouped by Endoscopy/PathologyCase plus urgency, doctor/free status, receipt request and calculated/charged amounts. Additional carried-forward assay charges appear separately so physical custody and billing remain distinct.

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
   - each Endoscopy has an independent PathologyCase and pricing calculation;
   - containers have globally unique IDs/labels;
   - containers from one case can split between normal Parcel routing and direct handover;
   - reports can cover explicit subsets of containers;
   - five-container pricing example = EUR 35;
   - doctor waiver preserves calculated amount but charges zero;
   - later assay charge can be billed in a later Parcel without re-shipping its container;
   - urgent/receipt flags survive into handover query data;
   - media derivative linkage/hash persists.
4. Add Appointment correction persistence.
5. Only after tests pass, expose HTTP endpoints and begin client screens.

## Validation

Assistant environment still has no .NET SDK; no compiler/test success is claimed here. User has Rider/.NET locally and can provide real build feedback.
