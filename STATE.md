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
- `21a98fa88136304594547c3841529194e72dee22` — language-neutral anatomy concepts with Greek-first localized vocabulary.
- `8ca0d9b5c22c905cb0216bab5475e7b2d5949189` — client anatomy vocabulary lookup/fallback abstraction documented.
- `4cd6a60c09a44572413822803df55655692c0bc4` — QML-facing anatomy suggestion model documented.

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
11. Clinical concept codes are language-neutral. Greek (`el-GR`) is the primary authored clinical vocabulary for this installation, not an English model with Greek bolted on later.
12. UI translation, clinical-vocabulary localization and user-authored clinical prose are separate concerns.

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

## Controlled GI anatomy and localization

`AnatomicalSite` is the language-neutral canonical research concept. It contains UUID identity, stable code, kind, optional parent and sort order. Human-readable text is deliberately not stored on the canonical concept.

`AnatomicalSiteName` stores localized preferred display names. `AnatomicalSiteAlias` stores locale-aware spelling/abbreviation/common-name variants for recognition/autocomplete, never as semantic identity.

`GiAnatomyVocabulary` is Greek-first:

- `el-GR` is the primary/reference authored locale;
- English is bundled as a second localization;
- stable codes such as `STOMACH_ANTRUM`, `GEJ`, `DUODENUM_D2` and `SIGMOID_COLON` do not change by language;
- Greek aliases intentionally include clinically common mixed-language forms such as `GEJ`, `D2`, `corpus`, `antrum` and `TI`.

The initial seed set covers practical upper/lower GI anatomy and landmarks. `ObservedLandmark` stores measured source facts; derivable distances are calculated from observations.

See `docs/ANATOMY_MODEL.md`.

## Client anatomy vocabulary abstraction

`client/src/clinical/AnatomyVocabulary.{h,cpp}` is a QtCore-only in-memory lookup layer with no HTTP, SQL or QML dependency.

It accepts canonical anatomy entries plus localized names/aliases and provides:

- `displayName(code, locale)`;
- `resolveExact(text, locale)`;
- `suggest(text, locale, limit)`;
- `childrenOf(parentCode)`.

Display fallback order is:

1. requested locale;
2. same language;
3. Greek (`el-GR`);
4. English (`en`);
5. canonical code.

Recognition is case-insensitive, accent-insensitive and punctuation/separator-normalized. Suggestions rank exact matches before prefix/substring matches and favor terms in the requested locale. Canonical codes remain the returned semantic identity.

`client/src/clinical/AnatomySuggestionModel.{h,cpp}` is the thin `QAbstractListModel` adapter intended for QML autocomplete. It exposes localized presentation roles only:

- `displayName`;
- `matchedText`;
- `matchedLocale`;
- `kind`;
- `exactMatch`.

The canonical code is intentionally not a normal display role. QML explicitly calls `codeAt(row)` when the user accepts a suggestion. The adapter owns a copy of vocabulary entries, accepts updates only from C++ through `setEntries(...)`, and exposes QML properties for query, locale and suggestion limit. Default locale is Greek.

Neither vocabulary layer mutates clinical data or silently accepts parser suggestions.

## Endoscopy

Structured data includes procedure type/indication/priority, start/end, preparation/sedation, outcome, maximal extent, anatomical findings, termination reasons, timeline, impression/recommendations and linked media.

No snare-polypectomy/ablation/clip/interventional framework is planned for this practice.

## Pathology / biopsy tracking

One `PathologyCase` belongs to one Endoscopy and groups pathology/billing facts originating from that Endoscopy. It stores urgency, receipt request and fee-waiver reason; it does not own one assigned pathologist.

Each `BiopsyContainer` has UUIDv7 identity, globally unique label code, case-relative ordinal, exact collection-site text, collection time, optional description and optional external-release timestamp/note. Researchable anatomy is represented by `BiopsyContainerSite` links to canonical sites.

Practice-managed containers enter Parcel/ParcelContainer, handover, billing and managed pathology-report workflow. Externally released tubes do not create a parallel external pathology subsystem and do not contribute to our Parcel billing. Outside reports that later reach the practice are Patient history (`ExternalReport`).

Initial biopsy processing is calculated **per Endoscopy from only the containers from that Endoscopy physically present in the Parcel being billed**. Under the example policy, 5 billable containers => EUR 35; 5 collected but only 3 parcelled => EUR 25.

Additional assays create separate charges that can be billed in a later Parcel without re-shipping the original container in the data model.

### Pathology reports / DOCX ingestion

Practice-managed reports retain the exact source document outside SQL with storage key, SHA-256 and original filename/MIME, while extracted text is stored for searching/display/future annotations.

Repeated embedded assets are content-addressed/deduplicated by SHA-256. Parser-derived pathology annotations remain distinguishable as suggested vs confirmed.

See `docs/PATHOLOGY_MODEL.md`.

## Media policy

- AV1 default video codec target;
- AVIF default display-still format;
- source-faithful original retained when available;
- derivatives reference source;
- SHA-256 stored for integrity;
- media can link to Endoscopy or finding.

## Next exact development slice

Continue piecemeal.

The anatomy lookup and QML model boundary now exist. The next small slice should be the real client build skeleton (CMake + Qt/KF6 target) so these C++ files can actually be compiled before adding QML controls or HTTP integration.

After client compilation is established, continue with either:

1. the first small anatomy autocomplete QML control; or
2. the SQLite schema/migration and anatomy-vocabulary seed.

Do not implement free-text parser/NLP yet.

## Validation

The assistant environment has no configured Qt/KDE client build toolchain and no .NET SDK. No compiler/test success is claimed for these changes yet. The user has Rider/.NET locally for server validation; client compilation will need the Qt/KDE toolchain once the client build skeleton is added.
