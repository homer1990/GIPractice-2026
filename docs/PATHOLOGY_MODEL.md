# Pathology / biopsy tracking model

This model replaces the current Excel convention where an endoscopy number doubles as the biopsy identifier and each pathologist has a separate worksheet.

## Core principle

Do not conflate:

1. the physical biopsy container;
2. the courier parcel;
3. the pathology case for one endoscopy;
4. the pathology/billing charge.

They usually travel together, but they are not the same thing. Keeping them separate is what allows split routing and later assays to be charged in a future parcel without pretending the original biopsy container was shipped again.

## Clinical-session relationship

A real clinical session may contain more than one Endoscopy. A double procedure is modeled as two independent Endoscopy records sharing one hidden clinical session/Appointment.

```text
Appointment
  |
  +-- hidden Encounter / clinical session
         |
         +-- Colonoscopy -> PathologyCase A -> containers...
         +-- Gastroscopy -> PathologyCase B -> containers...
```

The old human convention such as `XXXA` / `XXXB` may remain useful in labels/reports, but it is presentation only. Database identity uses UUIDv7.

## PathologyCase

One case groups pathology work originating from one Endoscopy. It stores:

- EndoscopyId;
- urgent pathology flag;
- receipt requested;
- pathology-fee waiver reason (for example `Doctor`).

It deliberately does not own one assigned pathologist.

## BiopsyContainer

Each physical tube/container gets:

- its own UUIDv7 database ID;
- a globally unique human-readable `LabelCode`;
- an ordinal within its pathology case;
- `CollectionSiteText`, preserving exactly what the user entered for the label/display;
- collection timestamp;
- optional description;
- optional external-release timestamp/note.

Researchable anatomy is **not** derived later by brute-force text search. `BiopsyContainerSite` links one container to one or more canonical `AnatomicalSite` rows.

Example:

```text
CollectionSiteText: "antrum-corpus"

BiopsyContainerSite -> STOMACH_ANTRUM
BiopsyContainerSite -> STOMACH_CORPUS
```

The original text remains visible, while research queries use canonical relationships.

See `docs/ANATOMY_MODEL.md` for the controlled GI anatomy vocabulary and landmark model.

## Practice-managed versus external release

A BiopsyContainer always exists if the practice actually collected tissue into a physical tube.

### Practice-managed pathology

The container enters the normal workflow:

- it is added to a `Parcel` through `ParcelContainer`;
- the Parcel identifies the Pathologist;
- it participates in the handover protocol;
- it contributes to that Endoscopy's biopsy-price calculation for that Parcel;
- reports/assays/charges may be tracked by the pathology subsystem.

### Released externally

If a tube is handed to the patient/oncologist/another outside destination, the container records `ExternalReleasedAtUtc` and an optional note. GIPractice does not create a parallel external pathology workflow.

If an outside report later comes back, it is recorded in longitudinal Patient history as `ExternalReport` rather than modeled as if the practice tracked that laboratory process.

## Parcel

A Parcel is the actual courier packet/shipment sent to one pathologist. It stores:

- parcel number;
- pathologist;
- created/sent timestamps;
- courier/tracking details;
- courier cost;
- currency.

`ParcelContainer` records physical membership of containers in the shipment.

## Charges are a ledger

`PathologyCharge` is independent of physical shipment history, but the initial biopsy-processing charge is calculated from physical Parcel membership.

A charge records:

- PathologyCase;
- charge kind;
- calculated amount;
- actually charged amount;
- currency;
- waiver reason;
- pricing-policy code/version;
- billable-container-count snapshot;
- optional related container/assay;
- Parcel in which the charge was billed.

Old charges are historical snapshots and never recalculate using future prices.

## Initial biopsy pricing rule

The confirmed billing unit is:

> one Endoscopy, considering only the containers from that Endoscopy that are physically sent in the Parcel being billed.

Containers released externally are excluded.

For the example policy:

- EUR 10 base;
- first 2 billable containers included;
- if >2, +EUR 10;
- +EUR 5 for every billable container above 2.

Five billable containers = EUR 35. If five were collected but only three enter our Parcel, the calculation uses three and produces EUR 25.

For a double procedure, colonoscopy and gastroscopy are priced independently because each is a separate Endoscopy/PathologyCase.

For waived cases (for example patient is a doctor), normal `CalculatedAmount` is retained, `ChargedAmount` becomes zero and the waiver reason is explicit.

## Additional assays

A later assay belongs to the original PathologyCase and may reference a specific practice-managed BiopsyContainer. It creates a separate charge which may be billed in a later Parcel without pretending the original tube was physically shipped again.

## Pathology reports

`PathologyReport` is only for reports received through the practice-managed pathology workflow.

### Preserve the exact source

The exact file received from the pathologist is retained outside SQL and referenced by:

- `OriginalStorageKey`;
- `OriginalSha256`;
- original filename;
- original MIME type.

For the current pathologist this will normally be the `.docx` attachment received by email. The source document is not reconstructed from extracted pieces and is not discarded after parsing.

### Searchable derived representation

The importer also extracts the report text into `ExtractedText`. This is what the client displays for fast search/annotation and what future research/NLP operates on.

`PathologyReportAnnotation` can later mark tissue type, diagnosis, finding, organism and anatomy. As with clinical-exam annotations, parser suggestions are not accepted clinical meaning until confirmed by a user.

Histological facts therefore belong to the pathology result, not to the biopsy-container collection metadata.

### Deduplicate repeated document assets

Embedded document assets are content-addressed by SHA-256 as `PathologyDocumentAsset`. Identical signature JPEGs or header images can therefore exist once in the derived asset store even if thousands of source DOCX files contain the same bytes.

`PathologyReportAsset` links a report to those extracted assets and records their role (signature/header image/embedded image/etc.).

`PathologyReportTemplate` may store reusable pathologist presentation metadata such as header text and the canonical signature asset. This is a convenience for normalized display/reporting only; it never replaces the original source document.

This intentionally permits both:

```text
exact original DOCX       retained verbatim
searchable extracted text stored separately
repeated signature/image  deduplicated in derived asset storage
```

### Report coverage

Because one case may contain externally released containers as well as practice-managed containers, report coverage is explicit through `PathologyReportContainer`. A managed report links only to the containers it actually describes.

Multiple managed reports are allowed: initial report, addendum after an assay, corrected/supplemental report, etc.

## Biopsy handover protocol

The protocol is generated from Parcel data rather than maintained separately.

### A. Physical handover

Group `ParcelContainer` rows by PathologyCase/Endoscopy and show:

- patient/endoscopy identification required by the protocol;
- container label codes and canonical/display anatomical sites;
- urgency;
- receipt requested;
- doctor/free status;
- calculated and charged amount for containers in this Parcel.

Externally released containers are absent because they are not part of this handover.

### B. Additional charges carried forward

List charges billed in this Parcel whose original case/container was sent previously, such as later histochemical assays.

This keeps billing complete without corrupting physical chain-of-custody.

## Why this replaces Excel cleanly

- Every tube has one globally unique identity.
- Every tube remains traceable to one specific Endoscopy.
- One appointment/session may contain multiple independent Endoscopies.
- Anatomy is researchable through canonical site relationships rather than label-text searches.
- Practice-managed and externally released containers can coexist without fake external-lab infrastructure.
- Every physical shipment is independently traceable.
- Pathologists are data, not spreadsheet tabs.
- Pricing uses exactly the containers from that Endoscopy that our pathologist receives in that Parcel.
- Free/doctor cases and receipt requests remain explicit.
- Later assays can be billed later without re-shipping samples in the data model.
- Source pathology DOCX files remain preserved while extracted text becomes searchable.
- Repeated signatures/assets can be deduplicated in the derived representation.
- Historical prices remain historical.
