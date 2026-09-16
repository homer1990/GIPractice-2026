# Clinical model decisions

This file records the deliberately small clinical model for the `v3-simple-clinical-core` branch.

## General rule

The software should follow the practice workflow, not force the clinician to fill a database schema.

Free text is first-class wherever clinicians naturally think and write in prose. Structured data is added where it improves retrieval, research, safety, or downstream workflow without making documentation slower.

Automation may suggest structure, codes, diagnoses, findings, or links. It must not silently assert clinical meaning. The user confirms semantic annotations.

## Encounter

`Encounter` is internal relational/session glue only. It binds records that happened to one patient during the same clinical session.

There is no Encounter page, create button, editor, search screen, or public CRUD concept.

The user works with Appointment, Endoscopy, Clinical Examination, Prescription, Visit and INFAI. The server creates/reuses the internal Encounter automatically.

## History

History is split in two.

### Longitudinal patient history

`PatientHistoryEntry` stores facts intended to remain part of the patient's longitudinal record, such as diagnoses, operations, medication, allergies, family history and social history.

Entries remain text-first and may optionally carry a coding system/code. Categories that later require real behavior may be promoted to dedicated models.

### Current/session history

Reason for attendance, current symptoms and history of the present illness belong to the current clinical session. They must not be silently copied into permanent patient history.

## Clinical examination

The initial clinical examination is a text document plus optional assessment.

The client may later recognize diagnoses, findings, symptoms, anatomy, medications and patient names and offer annotations. An annotation may carry an ICD/SNOMED/etc. code or a reference to another Patient record.

Patient references should render as readable linked names in the text while retaining the referenced PatientId underneath.

Text recognition must handle uncertainty and negation conservatively. Suggested annotations are not confirmed clinical facts until accepted by the user.

## Endoscopy

Endoscopy is more structured because anatomy, extent, findings, procedure outcome and specimens are intrinsically useful research data.

The initial structure includes:

- procedure type and indication;
- start/end time;
- preparation mode and preparation quality;
- sedation mode;
- procedure outcome: in progress, completed, limited or aborted;
- maximal extent reached;
- anatomical findings;
- termination reasons;
- a lightweight chronological event/timeline model;
- biopsy/specimen records with routine/urgent pathology priority;
- impression and recommendations;
- linked media.

There is intentionally no generic interventional-endoscopy framework. This practice does not perform snare polypectomy, ablation, clipping or similar tertiary-unit procedures. Biopsy is represented by the specimen it produces. Rare unusual actions can initially be recorded in timeline/narrative text.

### Findings and termination

A finding describes what was observed. A termination reason records why the examination was limited or abandoned. They may be linked.

Examples include retained food, stenosis/obstruction, unsafe inflammatory anatomy, inadequate preparation, pain, patient intolerance and sustained vagal bradycardia.

This preserves both the observed clinical fact and the operational consequence.

### Extent

`ExtentReachedCode` records the furthest anatomical point safely examined. A partial or aborted examination therefore remains researchable without parsing prose.

### Specimen urgency

Procedure urgency and pathology/specimen urgency are separate concepts. A scheduled routine endoscopy may produce an urgent biopsy because malignancy or another clinically serious condition is suspected, or because another treating team urgently needs the pathology result.

## Report generation

The printable endoscopy report is a rendering of structured findings, narrative descriptions, timeline/outcome, specimens, impression and recommendations. It is not the sole source of truth stored as one opaque report blob.

## Media

Media bytes are stored outside SQL. SQL stores identity, relationships, technical metadata, storage key and hash.

Default forward-looking formats:

- video codec: AV1;
- display still format: AVIF.

The canonical/source-faithful original is retained when available. Display/thumbnail derivatives may be generated from it. A media record carries a SHA-256 digest so the canonical file can be verified as unchanged.

Media can link to the Endoscopy as a whole or to a particular anatomical finding.

Container/MIME type and codec are recorded explicitly rather than inferred from a file extension.

## Design test

If collecting structured data makes ordinary documentation slower or more bureaucratic, reconsider the model. Structure should emerge naturally from the workflow or be suggested by the software, not turn the clinician into a database clerk.
