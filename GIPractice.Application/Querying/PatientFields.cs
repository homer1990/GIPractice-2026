using GIPractice.Core.Entities;

namespace GIPractice.Application.Querying;

/// <summary>Canonical patient fields. Define semantics here once and reuse them.</summary>
public static class PatientFields
{
    public static readonly QueryField<Patient, string> FirstName =
        new("patient.firstName", patient => patient.FirstName);

    public static readonly QueryField<Patient, string> LastName =
        new("patient.lastName", patient => patient.LastName);

    public static readonly QueryField<Patient, string> FathersName =
        new("patient.fathersName", patient => patient.FathersName);

    public static readonly QueryField<Patient, DateTime> BirthDate =
        new("patient.birthDate", patient => patient.BirthDay);

    // Same semantic field, different root entity. No duplicated mapping rule.
    public static readonly QueryField<Encounter, string> EncounterFirstName =
        FirstName.Through<Encounter>(encounter => encounter.Patient);

    public static readonly QueryField<Endoscopy, string> EndoscopyFirstName =
        FirstName.Through<Endoscopy>(endoscopy => endoscopy.Encounter.Patient);
}
