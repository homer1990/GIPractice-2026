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
- `c9ab937cb2e1ceb28e0a33b2790391520a45b890` — first local compiler-feedback fixes: Qt no-keywords, QML inheritance and current KI18n QML setup.

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
- stable codes do not change by language;
- Greek aliases intentionally include clinically common mixed-language forms such as `GEJ`, `D2`, `corpus`, `antrum` and `TI`.

The initial seed set covers practical upper/lower GI anatomy and landmarks. `ObservedLandmark` stores measured source facts; derivable distances are calculated from observations.

## Client anatomy vocabulary

`client/src/clinical/AnatomyVocabulary.{h,cpp}` is a QtCore-only in-memory lookup layer. It provides localized display lookup, exact alias/code resolution, autocomplete suggestions and hierarchy access.

Fallback order is requested locale -> same language -> Greek -> English -> canonical code. Recognition is case-insensitive, accent-insensitive and punctuation/separator-normalized.

`client/src/clinical/AnatomySuggestionModel.{h,cpp}` is the thin `QAbstractListModel` adapter for QML. Presentation roles are localized; the canonical code is retrieved explicitly through `codeAt(row)` only when a suggestion is accepted.

Because KDE builds define `QT_NO_KEYWORDS`, Qt meta-object code uses `Q_SIGNALS` and `Q_EMIT` rather than the disabled `signals` / `emit` keywords.

`AnatomySuggestionModel` is intentionally not `final`: `qmlRegisterType<T>()` internally creates a QML wrapper subclass.

Neither layer mutates clinical data or silently accepts parser suggestions.

## Qt/KF6 client build skeleton

The client is a real CMake target.

Top-level client CMake requires:

- CMake 3.20+;
- ECM 6+;
- Qt6 Core/Gui/Qml/Quick/QuickControls2/Widgets;
- KF6 CoreAddons/I18n/QQC2DesktopStyle;
- Kirigami QML module;
- C++23.

`client/src/CMakeLists.txt` builds `gipractice-client`, includes the anatomy vocabulary/model sources, creates QML module `net.gmanthos.gipractice`, defines translation domain `gipractice`, and links both `KF6::I18n` and `KF6::I18nQml`.

`client/src/main.cpp` sets up QApplication, KDE desktop Quick Controls style, GPLv3 KAboutData, KI18n application domain, QML registration and `KLocalization::setupLocalizedContext(&engine)` before loading the QML module.

The obsolete `KLocalizedContext` setup was removed after the first local build warned that KF 6.8+ should use `KLocalization::setupLocalizedContext()` / `KF6::I18nQml`.

`client/src/qml/Main.qml` remains a minimal Kirigami application window until compilation is clean.

Local validation command:

```bash
cmake -S client -B build/client -G Ninja
cmake --build build/client
./build/client/src/gipractice-client
```

## Endoscopy / pathology summary

Structured Endoscopy data includes procedure type/indication/priority, preparation/sedation, outcome, extent, findings, termination reasons, timeline, impression/recommendations and media. No interventional snare/ablation/clip framework is planned.

Each `BiopsyContainer` has unique identity/label, exact collection-site text, canonical site links and optional external-release metadata. Practice-managed containers enter Parcel/ParcelContainer, handover, billing and managed pathology-report workflow; external-release containers do not.

Initial biopsy processing is calculated per Endoscopy from only the containers from that Endoscopy physically present in the Parcel being billed. Under the example policy: 5 billable => EUR 35; 3 billable => EUR 25.

Practice-managed pathology reports retain the exact source DOCX outside SQL with SHA-256 while extracted text/assets support search, deduplication and future annotations.

## Next exact development slice

Re-run the local Qt/KF6 build after commit `c9ab937cb2e1ceb28e0a33b2790391520a45b890` and fix any remaining real compiler/link/QML startup error first.

Once the shell builds and starts, the next small client slice is the first anatomy autocomplete QML control wired to `AnatomySuggestionModel`.

Persistence remains a separate later slice: SQLite schema/migration + anatomy vocabulary seeding + concrete stores.

Do not implement free-text parser/NLP yet.

## Validation

First local build reached C++ compilation and exposed only integration issues in `AnatomySuggestionModel` and KI18n setup; those fixes are committed. A successful rebuild has not yet been observed, so no client compile success is claimed yet. The assistant environment still has no configured Qt/KDE client build toolchain and no .NET SDK.
