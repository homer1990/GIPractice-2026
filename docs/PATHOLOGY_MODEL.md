# Pathology / biopsy tracking model

This model replaces the current Excel convention where an endoscopy number doubles as the biopsy identifier and each pathologist has a separate worksheet.

## Core principle

Do not conflate:

1. the physical biopsy container;
2. the courier parcel;
3. the pathology case for one endoscopy;
4. the pathology/billing charge.

They usually travel together, but they are not the same thing. Keeping them separate is what allows later assays to be charged in a future parcel without pretending the original biopsy container was shipped again.

## Relationships

```text
Patient
  |
  +-- hidden Encounter
         |
         +-- Endoscopy
                |
                +-- PathologyCase (0..1 initially)
                       |
                       +-- BiopsyContainer (1..n)
                       +-- PathologyReport (0..n)
                       +-- PathologyAssay (0..n)
                       +-- PathologyCharge (0..n)

Pathologist
  |
  +-- Parcel (0..n)
         |
         +-- ParcelContainer -> BiopsyContainer   (physical shipment)
         +-- PathologyCharge.BilledInParcelId     (financial settlement)
```

A `Pathologist` replaces the Excel-tab separation. The client can show one tab/filter/view per pathologist without partitioning the data into different tables.

## PathologyCase

One case groups the pathology work originating from one Endoscopy.

It stores workflow facts that belong to the endoscopy's biopsy set as a whole:

- EndoscopyId;
- urgent pathology flag;
- receipt requested;
- pathology-fee waiver reason (for example `Doctor`);
- assigned pathologist when known.

The case is the level at which the initial biopsy-processing price is calculated because the pricing rule is based on the number of containers belonging to one endoscopy.

## BiopsyContainer

Each physical tube/container gets:

- its own UUIDv7 database ID;
- a globally unique human-readable `LabelCode` suitable for a tube label;
- an ordinal within its pathology case for convenient ordering;
- anatomical site;
- collection timestamp;
- optional description.

`LabelCode` must **not** be derived from the Endoscopy ID. The database will enforce uniqueness.

A future human-readable format could be something like `BC-26-000123`, but the exact printed-label scheme should be chosen together with the label-printer workflow rather than hard-coded into the domain now.

## Parcel

A Parcel is the actual courier packet/shipment sent to one pathologist.

It stores:

- parcel number;
- pathologist;
- created/sent timestamps;
- courier/tracking details;
- courier cost;
- currency.

Courier cost is a property of the shipment itself. It is separate from pathology fees calculated per Endoscopy.

`ParcelContainer` records which physical biopsy containers were inside the parcel.

## Charges are a ledger

`PathologyCharge` is independent of physical shipment.

A charge records:

- the PathologyCase it belongs to;
- type (`InitialBiopsy`, `AdditionalAssay`, adjustment);
- calculated amount according to the pricing rules;
- actually charged amount after waiver/adjustment;
- currency;
- waiver reason;
- pricing-policy code/version;
- container-count snapshot where applicable;
- optional related container;
- optional related assay;
- the Parcel in which the charge was billed (`BilledInParcelId`).

This is deliberately a historical snapshot. Opening an old parcel must never recalculate it using today's prices.

### Example initial pricing rule

For the currently described example policy:

- base charge: EUR 10;
- up to 2 containers included;
- if there are more than 2 containers, add EUR 10;
- then add EUR 5 for every container above 2.

Therefore 5 containers produce:

```text
10 base
+ 10 >2-container surcharge
+ 3 x 5 additional containers
= 35 EUR
```

The code implements this as configurable `BiopsyPricingPolicy`, not as a permanent hard-coded business constant.

If the pathology case is waived because the patient is a doctor, both facts are retained:

- `CalculatedAmount = 35.00`
- `ChargedAmount = 0.00`
- `WaiverReason = Doctor`

That preserves the actual price calculation while making the professional courtesy explicit.

## Additional assays

A later assay (for example a histochemical test) belongs to the original `PathologyCase` and may optionally identify the specific `BiopsyContainer` used.

It creates its own `PathologyCharge`.

Crucially, that charge starts with no `BilledInParcelId`. When the next parcel is prepared, the outstanding charge can be attached financially to that parcel without adding the old container to `ParcelContainer`.

This gives us the real workflow:

```text
Parcel 100
  physically contains BC-001, BC-002
  bills initial Case A charge

weeks later: additional assay requested on BC-001
  creates Assay X
  creates outstanding Charge X

Parcel 101
  physically contains unrelated new containers
  bills their normal charges
  ALSO bills Charge X from old Case A
  does NOT claim BC-001 is physically in Parcel 101
```

## Pathology reports

Reports belong to the `PathologyCase`, not to the Parcel.

Multiple reports are allowed:

- initial pathology report;
- addendum after an additional assay;
- later corrected/supplemental report if needed.

A report may optionally reference the assay that caused the addendum.

## Biopsy handover protocol

The protocol should be generated from a Parcel and contain two logical sections.

### A. Physical handover

Group the physical `ParcelContainer` records by PathologyCase/Endoscopy and show:

- endoscopy/patient identification needed by the existing protocol;
- all container label codes and anatomical sites;
- whether the case is urgent;
- whether a receipt is requested;
- whether pathology fees are waived because the patient is a doctor;
- calculated amount and actually charged amount for the initial biopsy work.

### B. Additional charges carried forward

List `PathologyCharge` records billed in this Parcel whose originating containers/case were sent previously, for example additional assays.

This keeps the handover document financially complete without corrupting physical chain-of-custody.

## Why this replaces Excel cleanly

- Every tube has one globally unique identity.
- Every tube remains traceable back to one Endoscopy.
- Every physical shipment is independently traceable.
- Pathologists are data, not spreadsheet tabs.
- Initial pricing can depend on container count per Endoscopy.
- Professional-courtesy/free cases remain auditable.
- Receipt requests remain attached to the relevant pathology case.
- Additional assays can be billed later without moving the original sample in the data model.
- Historical prices remain historical.
