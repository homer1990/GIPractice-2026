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
- `da26277f4d74d234ef254ee0295e3fdd87fc00ca` — first patient/appointment read API plus Qt HTTP transport.
- `292312eb781906f69064c8e35e60decc6b61e823` — removed stale duplicate pathology persistence rows; server then built and ran locally.
- `7cda0f5333d0eb7eafb98e6db76bb33656f448ee` — first real patient-search presentation model and QML page over the HTTP client.

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

## First read API contract

The first HTTP application contract is:

```text
GET /api/patients/search
GET /api/patients/{id}
GET /api/appointments?fromUtc=...&toUtc=...&patientId=...
```

Patient search supports first name, last name, father's name, birth-date range, page and page size. Appointment listing uses explicit UTC half-open bounds `[fromUtc, toUtc)` plus optional patient filtering.

`Data/InMemoryPracticeReadStore.cs` remains temporary synthetic development scaffolding. It exists only to prove contracts before SQLite.

## Qt HTTP transport

`client/src/api/PracticeApiClient.{h,cpp}` owns server URL handling, query serialization, `QNetworkAccessManager`, JSON parsing and explicit transport/HTTP/JSON errors. It has no QML dependency.

The API URL is provided through:

```bash
GIPRACTICE_API_URL=http://127.0.0.1:5070
```

The earlier startup-count smoke probe has now been removed. The real patient-search presentation model owns the first application calls.

## Patient search presentation

`client/src/patients/PatientSearchModel.{h,cpp}` is a QML-facing `QAbstractListModel` over `PracticeApiClient`.

It provides:

- criteria matching the current API: first name, last name, father's name, birth-date from/to;
- ISO date validation before transport;
- asynchronous loading/error state;
- 50-row paging with previous/next;
- stale-response suppression using a request serial;
- patient UUID retrieval only through `patientIdAt(row)` when a row is activated;
- presentation roles for localized UI: display name, first name, last name, father's name, birth date.

`client/src/qml/PatientSearchPage.qml` is now the main application surface. It triggers the initial search on load, shows criteria, loading/errors, results and paging, and emits a selected patient UUID without displaying it.

The next vertical slice is patient details using `GET /api/patients/{id}`.

## Clinical session / double endoscopy

`ClinicalWriteService` supports `StartEndoscopyAsync(...)` and `AddEndoscopyAsync(session, ...)`. Each Endoscopy keeps independent anatomy, findings, completion state, media and pathology while sharing the hidden session when appropriate.

## Controlled GI anatomy and localization

`GiAnatomyVocabulary` is Greek-first (`el-GR`) with English secondary localization and mixed clinical aliases such as `GEJ`, `D2`, `corpus`, `antrum` and `TI`.

The vocabulary must eventually become extensive for real GI practice, but breadth expansion remains deferred while the client/API spine is built.

`client/src/qml/AnatomyAutocompleteField.qml` has already run successfully locally. `main.cpp` still carries a deliberately small development-only anatomy vocabulary until the real vocabulary endpoint exists.

Compound multi-site parsing such as `άντρο-σώμα` is intentionally deferred.

## Endoscopy / pathology summary

Structured Endoscopy data includes procedure type/indication/priority, preparation/sedation, outcome, extent, findings, termination reasons, timeline, impression/recommendations and media.

Each `BiopsyContainer` has unique identity/label, exact collection-site text, canonical site links and optional external-release metadata. Practice-managed containers enter Parcel/ParcelContainer, handover, billing and managed pathology-report workflow; external-release containers do not.

Practice-managed pathology reports retain exact source DOCX files outside SQL with SHA-256 while extracted text/assets support search and future annotations.

## Next exact development slice

1. Locally compile/run the new `PatientSearchModel` + `PatientSearchPage`.
2. Fix any concrete C++ or QML errors from that build/run.
3. Add patient-details loading/selection using the already-existing `GET /api/patients/{id}` contract.
4. Then add selected-day appointment presentation over the existing appointment read contract.
5. After the basic read UI works, add create/correct Patient and Appointment writes.
6. Introduce SQLite behind the same server contracts after those flows are stable.

Do not implement broad free-text parser/NLP yet.

## Validation

Verified locally by the user:

- Qt/KF6/Kirigami shell builds and runs;
- anatomy autocomplete builds and runs;
- .NET server builds and starts on `127.0.0.1:5070`;
- Qt client reaches the ASP.NET server and parses the patient-search response successfully (`patient count: 3`).

Not yet verified locally:

- the new `PatientSearchModel` and `PatientSearchPage` slice at checkpoint `7cda0f5333d0eb7eafb98e6db76bb33656f448ee`.

The assistant environment has no configured Qt/KDE client build toolchain or .NET SDK, so no additional compile/test success is claimed.
