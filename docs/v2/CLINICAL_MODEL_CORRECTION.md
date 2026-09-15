# Clinical model correction

The v2 model distinguishes planning truth from clinical reality.

- An appointment has an immutable appointment type chosen when the appointment is created.
- Arrival does not freeze scheduling. A scheduled or arrived appointment may still be moved (for example because the patient is late or must wait after drinking water).
- Appointment type does not constrain the contents of the eventual encounter.
- An encounter is a clinical session/container, not a mutually exclusive kind.
- One encounter may contain several components at the same time, for example Endoscopy + ClinicalExam + Prescription.
- Walk-in encounters are first-class and need no appointment. This covers workflows such as HEINE/orthoscopy examinations performed primarily as walk-ins.
- INFAI-only encounters may coexist with the practice's exclusive clinical encounter. Any encounter containing at least one non-INFAI component occupies the exclusive clinical slot.
- Encounter components use EncounterId as their identity/foreign key and do not duplicate patient, appointment, timing, urgency, or encounter identity.

This correction supersedes the earlier one-EncounterKind/one-detail assumption.