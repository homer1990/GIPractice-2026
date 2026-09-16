# GI anatomy / semantic structure

## Rule

Do not research clinical anatomy by brute-force text search.

The application preserves what the clinician typed, but semantic meaning is stored through a small controlled GI vocabulary with aliases and relationships.

Example input:

```text
antrum-corpus
```

The client may recognize that as two canonical sites:

```text
STOMACH_ANTRUM
STOMACH_CORPUS
```

while the original `antrum-corpus` text remains available for labels/display.

## AnatomicalSite

`AnatomicalSite` contains:

- UUID identity;
- stable application code;
- display name;
- kind: organ / region / landmark;
- optional parent site;
- sort order.

The parent relationship allows hierarchical research. A query for `STOMACH` can include `STOMACH_ANTRUM`, `STOMACH_CORPUS`, `STOMACH_FUNDUS`, etc. without string matching.

`AnatomicalSiteAlias` contains spelling/language/common-name variants used by autocomplete and parsers. Aliases are input aids, not stored clinical semantics.

`GiAnatomyVocabulary` contains the initial practical seed set for esophagus, GEJ/Z-line/diaphragmatic impression, stomach regions, duodenum, terminal ileum and colon segments/flexures. It is intentionally not a home-grown full medical ontology.

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
GEJ                       39 cm from incisors
Diaphragmatic impression  42 cm from incisors
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
   -> alias/autocomplete/parser recognition
   -> suggested canonical concepts
   -> user acceptance/correction where semantic ambiguity exists
   -> stored relationships
```

Constrained fields such as biopsy collection site can be more assertive than free prose because their context strongly limits meaning. Free-text clinical and pathology parsing remains conservative, especially around negation and uncertainty.
