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
- `8ca0d9b5c22c905cb0216bab5475e7b2d5949189` — client anatomy vocabulary lookup/fallback abstraction.
- `4cd6a60c09a44572413822803df55655692c0bc4` — QML-facing anatomy suggestion model.
- `74b01bbd4e28150e4684e6d64cbd4fb58e42624d` — first real Qt/KF6/Kirigami client build skeleton.
- `42768eb5e3e8df9e355c04485bb40838d3ff75db` — compatible direct `KLocalizedQmlContext` setup; local client build subsequently launched successfully.
- `44a06552c2bbca902c30f4338754af53689df9d0` — first anatomy autocomplete QML control with raw-input preservation.
- `8a86e82d44ce66258ceee2e5d23579c027be96a7` — autocomplete QML runtime fixes; control subsequently ran successfully locally.
- `da26277f4d74d234ef254ee0295e3fdd87fc00ca` — first patient/appointment read API plus Qt HTTP transport and gated end-to-end patient-search smoke call.

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
13. Client concept acceptance must not destroy the exact source text the clinician typed; raw text and canonical semantic links are separate data.
14. Build the client/API vertical application spine before spending more time expanding frozen subsystems such as anatomy vocabulary breadth or compound parsing.

## Server shape

One .NET 10 project: `server/GIPractice.Server`.

Feature folders:

- `Patients`
- `Scheduling`
- `Clinical`
- `Pathology`
- `Data`
- `Api`
- `Configuration` (planned)

## First read API contract

The first real HTTP application contract is read-only and intentionally small:

```text
GET /api/patients/search
GET /api/patients/{id}
GET /api/appointments?fromUtc=...&toUtc=...&patientId=...
```

`GET /api/patients/search` supports current v3 Patient fields only:

- `firstName`
- `lastName`
- `fathersName`
- `birthDateFrom`
- `birthDateTo`
- `page` (default 1)
- `pageSize` (default 50, maximum 200)

The response is paged from the beginning because the real dataset is large. Current search matching for names is substring/case-insensitive; no fuzzy/NLP behavior is hidden inside this first contract.

Appointment listing uses explicit UTC bounds with half-open semantics `[fromUtc, toUtc)`, plus optional `patientId`. The client will derive selected-day UTC bounds rather than making the server assume a particular local timezone.

`server/GIPractice.Server/Api/ReadContracts.cs` defines the HTTP DTOs. `ReadEndpoints.cs` performs validation and maps domain objects to DTOs.

`Data/InMemoryPracticeReadStore.cs` is **temporary synthetic development scaffolding** used only to prove the contract before SQLite exists. It must be replaced, not grown into production persistence.

## First Qt HTTP transport

`client/src/api/PracticeApiClient.{h,cpp}` is the C++ transport boundary. It deliberately has no QML dependency and owns:

- base URL normalization;
- query serialization;
- `QNetworkAccessManager` GET requests;
- JSON parsing into C++ DTOs;
- network/HTTP/JSON error values.

Qt Network is now an explicit CMake dependency.

The API base URL is not hard-coded. For the temporary end-to-end smoke path, set:

```bash
GIPRACTICE_API_URL=http://127.0.0.1:5070
```

When set, `main.cpp` performs an empty paged patient search at startup and logs only the returned total count. This probe is temporary and will disappear once patient/appointment models own the calls.

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

`AnatomicalSite` is the language-neutral canonical research concept. Human-readable text is localized separately.

`GiAnatomyVocabulary` is Greek-first (`el-GR`) with English as a secondary localization. Stable codes do not change by language and Greek aliases intentionally include clinically common mixed-language forms such as `GEJ`, `D2`, `corpus`, `antrum` and `TI`.

The vocabulary must eventually become extensive for real GI practice, but breadth expansion is deliberately deferred while the client/API spine is built.

## Client anatomy vocabulary and autocomplete

`client/src/clinical/AnatomyVocabulary.{h,cpp}` is a QtCore-only lookup layer with localized display fallback, exact alias/code resolution, autocomplete suggestions and hierarchy access.

`client/src/clinical/AnatomySuggestionModel.{h,cpp}` is the QML-facing list-model adapter. Presentation roles are localized; canonical identity is retrieved explicitly through `codeAt(row)`.

`client/src/qml/AnatomyAutocompleteField.qml` is the first reusable clinical input control. It provides Greek-first suggestions, keyboard/mouse acceptance, alias hints, a removable localized selection chip and exact `sourceText` preservation. The control has run successfully on the user's local KF6 system.

`main.cpp` still supplies a deliberately small development-only anatomy vocabulary. It is temporary and must be removed when the server/database vocabulary endpoint is implemented.

Compound multi-site parsing such as `άντρο-σώμα` is intentionally deferred.

## Endoscopy / pathology summary

Structured Endoscopy data includes procedure type/indication/priority, preparation/sedation, outcome, extent, findings, termination reasons, timeline, impression/recommendations and media. No interventional snare/ablation/clip framework is planned.

Each `BiopsyContainer` has unique identity/label, exact collection-site text, canonical site links and optional external-release metadata. Practice-managed containers enter Parcel/ParcelContainer, handover, billing and managed pathology-report workflow; external-release containers do not.

Initial biopsy processing is calculated per Endoscopy from only the containers from that Endoscopy physically present in the Parcel being billed. Under the example policy: 5 billable => EUR 35; 3 billable => EUR 25.

Practice-managed pathology reports retain the exact source DOCX outside SQL with SHA-256 while extracted text/assets support search, deduplication and future annotations.

## Next exact development slice

First compile both sides of the new read-API slice locally and run the gated end-to-end smoke test.

Once that contract is proven, continue the vertical application spine:

1. `PatientSearchModel` / patient-search QML screen over `PracticeApiClient`;
2. patient details loading;
3. selected-day appointment model/list using explicit UTC bounds;
4. then add first write contracts (create/correct Patient and Appointment) before returning to deeper clinical workflows.

After the basic client/API flows are usable, introduce SQLite behind the same server contract, then replace development anatomy data with a server vocabulary endpoint and expand the vocabulary extensively.

Do not implement broad free-text parser/NLP yet.

## Validation

The Qt/KF6/Kirigami shell and anatomy autocomplete control have both been built and run successfully on the user's machine.

The new server read endpoints and `PracticeApiClient` transport have **not yet** been locally compiled or exercised together. The assistant environment has no .NET SDK or configured Qt/KDE build toolchain, so no compile/test success is claimed for this new slice.
