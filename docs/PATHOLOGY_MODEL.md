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
                       |      +-- ParcelContainer -> Parcel   (practice-managed)
                       |      +-- external release fields     (outside workflow)
                       |
                       +-- PathologyReport (0..n, practice-managed only)
                       |      +-- PathologyReportContainer
                       |
                       +-- PathologyAssay (0..n)
                       +-- PathologyCharge (0..n)

Pathologist
  |
  +-- Parcel (0..n)
         |
         +-- ParcelContainer -> BiopsyContainer
         +-- PathologyCharge.BilledInParcelId
```

A `Pathologist` replaces the Excel-tab separation. The client can show one tab/filter/view per pathologist without partitioning the data into different tables.

## PathologyCase

One case groups pathology work originating from one Endoscopy.

It stores workflow facts that belong to that endoscopy's biopsy set as a whole:

- EndoscopyId;
- urgent pathology flag;
- receipt requested;
- pathology-fee waiver reason (for example `Doctor`).

It deliberately does **not** own one assigned pathologist.

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

## Practice-managed versus external release

A BiopsyContainer always exists if the practice actually collected tissue into a physical tube.

There are then two paths.

### Practice-managed pathology

The container enters the normal workflow:

- it is added to a `Parcel` through `ParcelContainer`;
- the Parcel identifies the Pathologist;
- it participates in the handover protocol;
- it contributes to that Endoscopy's biopsy-price calculation for that Parcel;
- reports/assays/charges may be tracked by the pathology subsystem.

### Released externally

Sometimes a container is handed to the patient or another outside destination, for example an urgent biopsy the patient takes to their oncologist.

This does **not** create a second external pathology workflow in GIPractice.

The container simply records:

- `ExternalReleasedAtUtc`;
- optional `ExternalReleaseNote`.

It is not placed in our Parcel, does not contribute to our pathology charge and does not create an external Pathologist/Parcel/report hierarchy.

If an outside pathology report later comes back in a documented way, it is entered in the patient's longitudinal history as `PatientHistoryKind.ExternalReport`. In many real cases no structured result ever returns to the practice, and the model does not invent one.

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

`PathologyCharge` is independent of physical shipment history, but the initial biopsy-processing charge is calculated from physical Parcel membership.

A charge records:

- the PathologyCase it belongs to;
- type (`InitialBiopsy`, `AdditionalAssay`, adjustment);
- calculated amount according to the pricing rules;
- actually charged amount after waiver/adjustment;
- currency;
- waiver reason;
- pricing-policy code/version;
- billable-container-count snapshot where applicable;
- optional related container;
- optional related assay;
- the Parcel in which the charge was billed (`BilledInParcelId`).

This is deliberately a historical snapshot. Opening an old parcel must never recalculate it using today's prices.

## Initial biopsy pricing rule

The confirmed billing unit is:

> one Endoscopy, considering only the containers from that Endoscopy that are physically sent in the Parcel being billed.

Containers released externally are excluded.

For the currently described example policy:

- base charge: EUR 10;
- up to 2 billable containers included;
- if there are more than 2 billable containers, add EUR 10;
- then add EUR 5 for every billable container above 2.

Therefore five billable containers produce:

```text
10 base
+ 10 >2-container surcharge
+ 3 x 5 additional containers
= 35 EUR
```

But if five containers were collected and only three are sent through our Pathologist while two are released externally, the calculation uses **3 containers**:

```text
10 base
+ 10 >2-container surcharge
+ 1 x 5 additional container
= 25 EUR
```

The code implements this as configurable `BiopsyPricingPolicy`; the count passed to it explicitly means the billable containers from one PathologyCase/Endoscopy in one Parcel.

For a double procedure, the two Endoscopy records are priced independently because each has its own PathologyCase.

Example:

```text
Colonoscopy: 5 billable containers -> its own charge
Gastroscopy: 2 billable containers -> its own charge
```

If the pathology case is waived because the patient is a doctor, both facts are retained:

- normal `CalculatedAmount`;
- `ChargedAmount = 0.00`;
- `WaiverReason = Doctor`.

That preserves the actual price calculation while making the professional courtesy explicit.

## Additional assays

A later assay (for example a histochemical test) belongs to the original `PathologyCase` and may optionally identify the specific practice-managed `BiopsyContainer` used.

It creates its own `PathologyCharge`.

Crucially, that charge starts with no `BilledInParcelId`. When the next parcel is prepared, the outstanding charge can be attached financially to that parcel without adding the old container to `ParcelContainer`.

## Pathology reports

`PathologyReport` is only for reports received through the practice-managed pathology workflow.

Because a case may contain externally released containers as well as practice-managed containers, report coverage is explicit through `PathologyReportContainer`. A report links only to containers it actually describes.

Multiple managed reports are allowed:

- initial pathology report;
- addendum after an additional assay;
- later corrected/supplemental report if needed.

Outside reports are not modeled here. If one later reaches the practice, it is recorded in patient history as an external report.

## Biopsy handover protocol

The protocol should be generated from a Parcel and contain two logical sections.

### A. Physical handover

Group the physical `ParcelContainer` records by PathologyCase/Endoscopy and show:

- endoscopy/patient identification needed by the existing protocol;
- all container label codes and anatomical sites;
- whether the case is urgent;
- whether a receipt is requested;
- whether pathology fees are waived because the patient is a doctor;
- calculated amount and actually charged amount for the **containers in this Parcel**.

Externally released containers are absent because they are not part of this handover.

For a double procedure, colonoscopy and gastroscopy appear as separate Endoscopy/pathology lines even though they came from the same appointment/session.

### B. Additional charges carried forward

List `PathologyCharge` records billed in this Parcel whose originating containers/case were sent previously, for example additional assays.

This keeps the handover document financially complete without corrupting physical chain-of-custody.

## Why this replaces Excel cleanly

- Every tube has one globally unique identity.
- Every tube remains traceable back to one specific Endoscopy.
- One appointment/session may contain multiple independent Endoscopies.
- Practice-managed and externally released containers can coexist under the same Endoscopy without creating parallel external-lab infrastructure.
- Every physical practice-managed shipment is independently traceable.
- Pathologists are data, not spreadsheet tabs.
- Initial pricing uses exactly the containers from that Endoscopy that our pathologist is receiving in that Parcel.
- Professional-courtesy/free cases remain auditable.
- Receipt requests remain attached to the relevant pathology case.
- Additional assays can be billed later without moving the original sample in the data model.
- Historical prices remain historical.
- External reports, when they happen to return, fit naturally into Patient history instead of forcing incomplete outside workflows into the pathology subsystem.
