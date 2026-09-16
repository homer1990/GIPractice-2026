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

A real clinical session may contain more than one Endoscopy.

A common example is a double procedure:

```text
Appointment
  |
  +-- hidden Encounter / clinical session
         |
         +-- Colonoscopy
         |      |
         |      +-- PathologyCase A
         |             +-- containers...
         |
         +-- Gastroscopy
                |
                +-- PathologyCase B
                       +-- containers...
```

Colonoscopy and gastroscopy remain two independent Endoscopy records because their anatomy, findings, completion state, biopsies and pathology charges are independent. They simply share the same hidden clinical session and optional Appointment.

The old human convention such as `XXXA` / `XXXB` may still be useful in labels/reports, but it is presentation only. Database identity uses UUIDv7 and does not depend on that numbering scheme.

## Relationships

```text
Patient
  |
  +-- hidden Encounter
         |
         +-- Endoscopy (1..n per session)
                |
                +-- PathologyCase (0..1 per Endoscopy initially)
                       |
                       +-- BiopsyContainer (1..n)
                       |      |
                       |      +-- ParcelContainer / BiopsyTransfer
                       |
                       +-- PathologyReport (0..n)
                       |      +-- PathologyReportContainer (explicit coverage)
                       |
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

One case groups pathology work originating from one Endoscopy.

It stores workflow facts that belong to that endoscopy's biopsy set as a whole:

- EndoscopyId;
- urgent pathology flag;
- receipt requested;
- pathology-fee waiver reason (for example `Doctor`).

It deliberately does **not** own one assigned pathologist. Containers from the same Endoscopy may be split between destinations.

Example:

- urgent gastric container handed to the patient for their oncologist;
- routine colonic/gastrointestinal containers remain with the practice and later go to the normal pathologist.

Routing therefore belongs to physical containers/transfers, not to PathologyCase.

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

## Split routing / custody

`ParcelContainer` records normal physical membership in a courier parcel.

`BiopsyTransfer` records custody when a container leaves by another route, for example:

- handed to the patient;
- handed to another doctor/oncologist;
- delivered to another pathology destination.

This means one PathologyCase can have containers with different destinations without duplicating the Endoscopy or inventing separate cases for administrative reasons.

## Parcel

A Parcel is the actual courier packet/shipment sent to one pathologist.

It stores:

- parcel number;
- pathologist;
- created/sent timestamps;
- courier/tracking details;
- courier cost;
- currency.

Courier cost is a property of the shipment itself. It is separate from pathology fees calculated for biopsy work.

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

For a double procedure, the two Endoscopy records are priced independently because each has its own PathologyCase and container count.

Example:

```text
Colonoscopy: 5 containers -> its own calculated charge
Gastroscopy: 2 containers -> its own calculated charge
```

Whether a split-routing case is priced from all containers in the Endoscopy or only the containers actually processed by a particular pathologist is intentionally left to the pricing policy. Do not hard-code that until the real billing rule is confirmed.

If the pathology case is waived because the patient is a doctor, both facts are retained:

- `CalculatedAmount = 35.00`
- `ChargedAmount = 0.00`
- `WaiverReason = Doctor`

That preserves the actual price calculation while making the professional courtesy explicit.

## Additional assays

A later assay (for example a histochemical test) belongs to the original `PathologyCase` and may optionally identify the specific `BiopsyContainer` used.

It creates its own `PathologyCharge`.

Crucially, that charge starts with no `BilledInParcelId`. When the next parcel is prepared, the outstanding charge can be attached financially to that parcel without adding the old container to `ParcelContainer`.

## Pathology reports

Reports belong to the originating `PathologyCase`, not to the Parcel.

Because containers from one case may go to different destinations, report coverage is explicit through `PathologyReportContainer`. A report is therefore linked only to the containers it actually describes.

Multiple reports are allowed:

- initial pathology report;
- report from another destination/pathologist;
- addendum after an additional assay;
- later corrected/supplemental report if needed.

A report may optionally identify the pathologist and the assay that caused an addendum.

## Biopsy handover protocol

The protocol should be generated from a Parcel and contain two logical sections.

### A. Physical handover

Group the physical `ParcelContainer` records by PathologyCase/Endoscopy and show:

- endoscopy/patient identification needed by the existing protocol;
- all container label codes and anatomical sites;
- whether the case is urgent;
- whether a receipt is requested;
- whether pathology fees are waived because the patient is a doctor;
- calculated amount and actually charged amount for the relevant biopsy work.

For a double procedure, colonoscopy and gastroscopy appear as separate Endoscopy/pathology lines even though they came from the same appointment/session.

### B. Additional charges carried forward

List `PathologyCharge` records billed in this Parcel whose originating containers/case were sent previously, for example additional assays.

This keeps the handover document financially complete without corrupting physical chain-of-custody.

## Why this replaces Excel cleanly

- Every tube has one globally unique identity.
- Every tube remains traceable back to one specific Endoscopy.
- One appointment/session may contain multiple independent Endoscopies.
- Containers from one Endoscopy may be routed independently.
- Reports explicitly identify which containers they cover.
- Every physical shipment is independently traceable.
- Pathologists are data, not spreadsheet tabs.
- Initial pricing can be calculated independently for each Endoscopy.
- Professional-courtesy/free cases remain auditable.
- Receipt requests remain attached to the relevant pathology case.
- Additional assays can be billed later without moving the original sample in the data model.
- Historical prices remain historical.
