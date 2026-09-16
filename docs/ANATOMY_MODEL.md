# GI anatomy / semantic structure

## Rule

Do not research clinical anatomy by brute-force text search.

The application preserves what the clinician typed, but semantic meaning is stored through a small controlled GI vocabulary with aliases and relationships.

The vocabulary is **Greek-first but not Greek-bound**:

- stable concept codes are language-neutral;
- Greek (`el-GR`) is the primary/reference authored vocabulary for this practice;
- preferred display names are localized;
- aliases are localized and may include foreign-language terms or abbreviations that Greek clinicians actually use;
- future public installations can add languages without changing stored clinical relationships.

Example input:

```text
άντρο-corpus
```

The client may recognize that as two canonical sites:

```text
STOMACH_ANTRUM
STOMACH_CORPUS
```

while the original text remains available for labels/display.

## Three localization layers

Keep these separate.

### 1. UI text

Buttons, menus, validation messages and other application chrome belong to the Qt/KDE translation system. They do not belong in the clinical vocabulary tables.

### 2. Clinical vocabulary

Anatomical concepts use language-neutral codes with localized names and aliases.

Example:

```text
Code: STOMACH_ANTRUM

el-GR preferred: Άντρο στομάχου
el-GR aliases:   άντρο, αντρο, antrum, antral

en preferred:   Gastric antrum
en aliases:      antrum, gastric antrum, antral
```

The database relation stores the canonical concept, not whichever spelling happened to be typed.

### 3. User-authored clinical prose

Clinical notes, biopsy descriptions, pathology text and other authored prose remain exactly as entered. They are never internally translated merely to satisfy the structured model.

## AnatomicalSite

`AnatomicalSite` contains only language-neutral semantic information:

- UUID identity;
- stable application code;
- kind: organ / region / landmark;
- optional parent site;
- sort order.

Human-readable text is deliberately absent from the canonical object.

`AnatomicalSiteName` stores the preferred display name for a site and locale.

`AnatomicalSiteAlias` stores a site, locale and spelling/abbreviation/common-name variant used by autocomplete and parsers.

The parent relationship allows hierarchical research. A query for `STOMACH` can include `STOMACH_ANTRUM`, `STOMACH_CORPUS`, `STOMACH_FUNDUS`, etc. without string matching.

`GiAnatomyVocabulary` contains the initial practical seed set for esophagus, GEJ/Z-line/diaphragmatic impression, stomach regions, duodenum, terminal ileum and colon segments/flexures. Greek names are authored first; English names are bundled as a second localization.

Aliases intentionally reflect real clinical input rather than language purity. A Greek installation may therefore recognize values such as `GEJ`, `D2`, `corpus`, `antrum` and `TI` directly.

## Biopsy containers

A `BiopsyContainer` preserves `CollectionSiteText` exactly as entered.

Researchable anatomy is represented by zero or more `BiopsyContainerSite` links to canonical `AnatomicalSite` rows.

This permits combinations such as antrum + corpus without inventing a compound anatomy code and without relying on `LIKE '%antrum%'` queries.

## Landmarks and measurements

Relative anatomical measurements store the observed source facts through `ObservedLandmark`:

- Endoscopy;
- anatomical landmark;
- numeric position;
- unit;
- reference point such as incisors or anal verge.

Example:

```text
Γαστροοισοφαγική συμβολή  39 cm από τους τομείς
Διαφραγματικό εντύπωμα    42 cm από τους τομείς
```

The 3 cm separation is derived from those observations. The system should prefer storing observations and calculating derivable values rather than storing only the derived conclusion.

## Tissue / histology

Do not attach histological tissue type to the biopsy tube merely because that tissue is later found in it.

Collection records describe where tissue was taken and, where relevant, the endoscopic finding that prompted sampling. Histological facts belong to the pathology report/result.

Parsed pathology text may therefore carry `PathologyReportAnnotation` records for tissue type, diagnosis, finding, organism and anatomy. As with clinical-text annotations, parser suggestions are not confirmed facts until accepted by a user.

## Client behaviour

Expected client pattern:

```text
human input
   -> localized alias/autocomplete/parser recognition
   -> suggested canonical concepts
   -> user acceptance/correction where semantic ambiguity exists
   -> stored relationships
```

The client should use the configured UI locale for preferred display names but recognition may search aliases across useful locales. It should not require language detection before resolving a known concept.

Constrained fields such as biopsy collection site can be more assertive than free prose because their context strongly limits meaning. Free-text clinical and pathology parsing remains conservative, especially around negation and uncertainty.
