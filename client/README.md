# Qt/KDE client

The desktop client uses Qt 6, Qt Quick/QML, KDE Frameworks 6 and Kirigami.

The UI works with Patients, Appointments, Endoscopies, Exams, Prescriptions, Visits and INFAI.

Encounter is intentionally absent from the presentation model. A C++ service may keep an opaque internal clinical-session key while an editor/workflow is open, but QML must not expose Encounter as a screen, command, list item or editable object.

## Build skeleton

The first real client build target exists and has been compiled/launched locally.

Requirements are expressed by CMake and currently include:

- CMake 3.20+
- Extra CMake Modules (ECM) 6+
- Qt 6: Core, Gui, Qml, Quick, QuickControls2, Widgets
- KDE Frameworks 6: CoreAddons, I18n, I18nQml, QQC2DesktopStyle
- Kirigami QML module (`org.kde.kirigami`)
- C++23 compiler

Configure and build from the repository root:

```bash
cmake -S client -B build/client -G Ninja
cmake --build build/client
```

With KDE's CMake settings the executable is currently emitted under the build-tree `bin` directory:

```bash
./build/client/bin/gipractice-client
```

The application ID / QML URI is `net.gmanthos.gipractice`.

`main.cpp` establishes:

- `QApplication`;
- KDE `KAboutData` with GPLv3 license metadata;
- KI18n application domain (`gipractice`);
- `KLocalizedQmlContext` for QML translation;
- KDE desktop Qt Quick Controls style when no style was explicitly selected;
- QML registration for `AnatomySuggestionModel`;
- a `QQmlApplicationEngine` loading the `net.gmanthos.gipractice` QML module.

## Clinical vocabulary localization

The client must not treat English labels as anatomical identity.

Canonical anatomy is represented by language-neutral codes such as:

- `STOMACH_ANTRUM`
- `STOMACH_CORPUS`
- `GEJ`
- `DUODENUM_D2`

Localized names and aliases are presentation/input metadata. Greek (`el-GR`) is the primary authored/reference vocabulary for this installation; English is a supported fallback and future public installations may add other locales without changing stored clinical relationships.

`src/clinical/AnatomyVocabulary` is the client-side lookup abstraction. It deliberately has no HTTP, SQL or QML dependency. A later data source can populate it from the server/database while the lookup rules remain unchanged.

Current behavior:

- display-name fallback: requested locale -> same language -> Greek -> English -> canonical code;
- exact resolution by canonical code, localized preferred name or alias;
- autocomplete suggestions ranked by exact/prefix/substring match and locale relevance;
- accent-insensitive/case-insensitive matching;
- punctuation/separator normalization for recognition;
- hierarchy access through `childrenOf(parentCode)`.

The lookup layer returns canonical codes. It does not itself mutate clinical data or silently accept semantic parser suggestions.

## QML suggestion adapter

`src/clinical/AnatomySuggestionModel` is a thin `QAbstractListModel` over `AnatomyVocabulary`.

QML-facing roles are presentation-only:

- `displayName`
- `matchedText`
- `matchedLocale`
- `kind`
- `exactMatch`

The canonical concept code is deliberately not exposed as a normal model role. When a user accepts a suggestion, QML explicitly calls `codeAt(row)` to obtain the semantic identity that should be sent to the application/service layer.

The model owns its vocabulary entry copy and exposes `setEntries(...)` only to C++ code. This keeps QML from becoming the owner/source of clinical vocabulary data and avoids pointer-lifetime coupling to a future HTTP/SQLite provider.

The model exposes QML properties for:

- `query`
- `localeName`
- `limit`
- read-only suggestion `count`

Changing query/locale/limit refreshes suggestions. The default locale is Greek (`el-GR`).

## First anatomy autocomplete control

`src/qml/AnatomyAutocompleteField.qml` is the first reusable clinical input control.

It currently provides:

- localized anatomy suggestions while typing;
- keyboard navigation with Up/Down and Return/Enter;
- mouse selection;
- alias-match hint text;
- explicit acceptance of one canonical concept through `codeAt(row)`;
- a removable chip showing the localized preferred concept name;
- preservation of the exact raw text the user typed in `sourceText`;
- no canonical database code in ordinary presentation.

For the current smoke test, `main.cpp` supplies a deliberately small **development-only** vocabulary containing representative entries such as antrum, corpus, GEJ, diaphragmatic impression, D2 and sigmoid colon. This is not a second authoritative vocabulary. It exists only so the control can be exercised before server persistence/API transport exists and must be removed when the real server-provided vocabulary is wired in.

`Main.qml` currently hosts this control as a test surface. Example inputs include `άντρο`, `αντρο`, `corpus`, `GEJ`, `D2` and `sigmoid`.

The first control resolves one concept at a time. Compound input such as `άντρο-σώμα` -> two canonical sites is a later parser/multi-selection slice; it is not silently guessed by this component.

## Localization layers

Keep these concerns separate:

1. UI strings: translated through KDE/Qt localization (`KI18n` / `.po` catalogs).
2. Clinical vocabulary: locale-aware names/aliases attached to language-neutral concept codes.
3. User-authored clinical prose/raw collection text: preserved exactly as entered; never internally translated.

HTTP/persistence integration and free-text parser/NLP remain separate later slices.
