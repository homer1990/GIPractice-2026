# Qt/KDE client

The desktop client will use Qt 6, Qt Quick/QML, KDE Frameworks 6 and Kirigami.

The UI works with Patients, Appointments, Endoscopies, Exams, Prescriptions, Visits and INFAI.

Encounter is intentionally absent from the presentation model. A C++ service may keep an opaque internal clinical-session key while an editor/workflow is open, but QML must not expose Encounter as a screen, command, list item or editable object.

## Clinical vocabulary localization

The client must not treat English labels as anatomical identity.

Canonical anatomy is represented by language-neutral codes such as:

- `STOMACH_ANTRUM`
- `STOMACH_CORPUS`
- `GEJ`
- `DUODENUM_D2`

Localized names and aliases are presentation/input metadata. Greek (`el-GR`) is the primary authored/reference vocabulary for this installation; English is a supported fallback and future public installations may add other locales without changing stored clinical relationships.

`src/clinical/AnatomyVocabulary` is the first client-side lookup abstraction. It deliberately has no HTTP, SQL or QML dependency. A later data source can populate it from the server/database while the lookup rules remain unchanged.

Current behavior:

- display-name fallback: requested locale -> same language -> Greek -> English -> canonical code;
- exact resolution by canonical code, localized preferred name or alias;
- autocomplete suggestions ranked by exact/prefix/substring match and locale relevance;
- accent-insensitive/case-insensitive matching;
- punctuation/separator normalization for recognition;
- hierarchy access through `childrenOf(parentCode)`.

The lookup layer returns canonical codes. It does not itself mutate clinical data or silently accept semantic parser suggestions. QML/parser integration comes later.

## Localization layers

Keep these concerns separate:

1. UI strings: translated through KDE/Qt localization (`KI18n` / `.po` catalogs when the client shell is implemented).
2. Clinical vocabulary: locale-aware names/aliases attached to language-neutral concept codes.
3. User-authored clinical prose: preserved exactly as entered; never internally translated.

Client implementation beyond this vocabulary abstraction starts after the first server read/write contracts and persistence layer are stable.
