# Rewrite state

Branch: `v3-simple-clinical-core`

Current checkpoints:

- `2bce4015716570286332c213ec02c5715fc8ab36` — clean-tree restart.
- `621752dd9b58969f6e1c6853d31480238601e6c6` — hidden clinical-session workflow; no user-facing Encounter.
- `09c9bca6a3cc2ee7fab64de1ddb2dcf08d752597` — clinical documentation/endoscopy/media decisions locked.
- `8b58a075008e141aa170548aa0e55ab0dc0a8f54` — biopsy-container / parcel / pathology billing model.
- `a177d2e7667253fac592983e33959652d3760e33` — double-endoscopy and split-container model.
- `ba5cc88b6ef6d8cc0969eceb7dbf90d0875fbd43` — parcel-based pricing and simplified external-release workflow.
- `91230b0368c84e3a40b2a39bb70bb4ce4576e32f` — controlled GI anatomy + searchable/source-preserving pathology report model.

## Fixed principles

1. Encounter is internal relational/session glue only; the UI never exposes Encounter CRUD.
2. One real clinical session may contain multiple Endoscopy records.
3. A double procedure is two independent Endoscopies sharing one hidden session/Appointment.
4. Appointment planning data may be corrected; corrections do not silently rewrite completed clinical work.
5. Free text is first-class where clinicians naturally think in prose; structure is added where it improves retrieval/research/workflow.
6. Automated text recognition may suggest codes/links but must not silently assert clinical meaning.
7. Clinical children have their own identities and may repeat within a session where the real workflow permits it.
8. Media/document source files live outside SQL; SQL stores relationships/metadata/hash/searchable derivatives.
9. No generic repository/mapping framework/CQRS/event bus or generic hospital-EHR abstraction.
10. Researchable anatomy uses canonical concepts/relationships, not brute-force label-text searching.

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

- `StartEndoscopyAsync(...)` — creates a hidden clinical session and first Endoscopy.
- `AddEndoscopyAsync(session, ...)` — adds another Endoscopy to the same session.

Each Endoscopy keeps independent anatomy, findings, completion state, media and pathology.

## History / clinical text

- `PatientHistoryEntry` stores longitudinal history.
- `PatientHistoryKind.ExternalReport` is used for outside reports that later reach the practice.
- Current symptoms/HPI stay on the hidden clinical session.
- `ClinicalExam` is text-first with optional assessment.
- `ClinicalTextAnnotation` may carry suggested/confirmed diagnosis, finding, symptom, medication, anatomy or patient-reference semantics.
- Parser/NLP is not implemented yet.

## Controlled GI anatomy

`AnatomicalSite` is the canonical research concept. It has:

- UUID identity;
- stable code;
- display name;
- kind: organ / region / landmark;
- optional parent;
- sort order.

`AnatomicalSiteAlias` stores spelling/language/common-name variants used for recognition/autocomplete, not as clinical semantics.

`GiAnatomyVocabulary` provides the initial practical seed set for:

- esophagus + upper/middle/distal regions;
- GEJ, Z-line, diaphragmatic impression;
- stomach/cardias/fundus/corpus/incisura/antrum/pylorus;
- duodenum/bulb/D2;
- terminal ileum;
- colon segments, flexures, ileocecal valve and rectum;
- English/Greek/common aliases.

`ObservedLandmark` stores measured source observations such as GEJ and diaphragmatic-impression positions relative to incisors/anal verge. Derivable distances are calculated from those observations rather than stored as the only fact.

See `docs/ANATOMY_MODEL.md`.

## Endoscopy

Structured data includes procedure type/indication/priority, start/end, preparation/sedation, outcome, maximal extent, anatomical findings, termination reasons, timeline, impression/recommendations and linked media.

No snare-polypectomy/ablation/clip/interventional framework is planned for this practice.

## Pathology / biopsy tracking

### PathologyCase

One `PathologyCase` belongs to one Endoscopy and groups pathology/billing facts originating from that Endoscopy. It stores urgency, receipt request and fee-waiver reason; it does not own one assigned pathologist.

### BiopsyContainer

Each physical tube has:

- UUIDv7 database ID;
- globally unique human-readable `LabelCode`;
- case-relative ordinal;
- exact `CollectionSiteText` entered by the user;
- collection time;
- optional description;
- optional external-release timestamp/note.

Researchable anatomy is represented by `BiopsyContainerSite` links to one or more canonical `AnatomicalSite` rows. Example: `antrum-corpus` remains the display text while semantics are `STOMACH_ANTRUM` + `STOMACH_CORPUS`.

### Practice-managed versus external release

Practice-managed containers enter Parcel/ParcelContainer, handover, billing and managed pathology-report workflow. Externally released tubes do not create a parallel external pathology subsystem and do not contribute to our Parcel billing.

Outside reports that later reach the practice are Patient history (`ExternalReport`).

### Parcel / charges / pricing

A Parcel is one physical courier shipment to one Pathologist. `PathologyCharge` is a historical financial ledger.

Initial biopsy processing is calculated **per Endoscopy from only the containers from that Endoscopy physically present in the Parcel being billed**.

Example policy:

- EUR 10 base;
- first 2 billable containers included;
- if >2, +EUR 10;
- +EUR 5 for each billable container above 2.

Thus 5 billable containers => EUR 35; 5 collected but only 3 parcelled => EUR 25.

A double procedure is priced independently per Endoscopy/PathologyCase. Doctor waiver retains normal calculated amount but charges zero.

Additional assays create separate charges that can be billed in a later Parcel without re-shipping the original container in the data model.

### Pathology reports / DOCX ingestion

`PathologyReport` is only for practice-managed reports.

The exact source document is retained outside SQL with:

- `OriginalStorageKey`;
- `OriginalSha256`;
- original filename/MIME type.

The importer also stores `ExtractedText` for searching, display and future annotations.

`PathologyReportAnnotation` supports parser-derived tissue type, diagnosis, finding, organism and anatomy concepts. Suggestions remain unconfirmed until accepted by a user.

Repeated embedded assets are content-addressed as `PathologyDocumentAsset` by SHA-256. `PathologyReportAsset` links reports to deduplicated signature/header/embedded-image assets. `PathologyReportTemplate` may retain reusable pathologist header text + canonical signature asset for normalized presentation, but never replaces the exact original DOCX.

`PathologyReportContainer` explicitly states which submitted containers each managed report covers.

See `docs/PATHOLOGY_MODEL.md`.

## Media policy

- AV1 default video codec target;
- AVIF default display-still format;
- source-faithful original retained when available;
- derivatives reference source;
- SHA-256 stored for integrity;
- media can link to Endoscopy or finding.

## Next exact development slice

1. Add the real SQLite schema/migration, including anatomy vocabulary/aliases, landmark observations, biopsy-container site links and pathology-document source/asset/annotation tables.
2. Seed the initial `GiAnatomyVocabulary` into SQLite with stable unique codes/aliases.
3. Add concrete SQLite stores.
4. Add tests proving:
   - one hidden session can contain both colonoscopy and gastroscopy;
   - each Endoscopy has independent PathologyCase/pricing;
   - `antrum-corpus` can preserve raw text while storing two canonical site links;
   - parent-site queries can find child-region biopsy sites without text search;
   - observed GEJ/diaphragmatic positions preserve source facts for derived measurements;
   - containers have globally unique IDs/labels;
   - external-release containers are excluded from Parcel billing;
   - 5 collected / 3 parcelled => EUR 25 under the example policy;
   - doctor waiver preserves calculated amount but charges zero;
   - managed report retains original DOCX identity/hash plus extracted text;
   - identical signature assets can be deduplicated by SHA-256;
   - report annotations remain distinguishable as suggested vs confirmed;
   - later assay charge can be billed in a later Parcel without re-shipping its container;
   - media derivative linkage/hash persists.
5. Add Appointment correction persistence.
6. Only after tests pass, expose HTTP endpoints and begin client screens/autocomplete/parser UX.

## Validation

Assistant environment still has no .NET SDK; no compiler/test success is claimed here. User has Rider/.NET locally and can provide real build feedback.
