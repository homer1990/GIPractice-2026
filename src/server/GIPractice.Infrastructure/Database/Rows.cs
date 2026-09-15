using LinqToDB.Mapping;

namespace GIPractice.Infrastructure.Database;

[Table("patients")]
internal sealed class PatientRow
{
    [PrimaryKey, Column("id"), NotNull] public string Id { get; set; } = null!;
    [Column("first_name"), NotNull] public string FirstName { get; set; } = null!;
    [Column("last_name"), NotNull] public string LastName { get; set; } = null!;
    [Column("fathers_name")] public string? FathersName { get; set; }
    [Column("birth_date")] public DateTime? BirthDate { get; set; }
    [Column("gender"), NotNull] public short Gender { get; set; }
}

[Table("appointments")]
internal sealed class AppointmentRow
{
    [PrimaryKey, Column("id"), NotNull] public string Id { get; set; } = null!;
    [Column("patient_id"), NotNull] public string PatientId { get; set; } = null!;
    [Column("kind"), NotNull] public short Kind { get; set; }
    [Column("scheduled_start_utc"), NotNull] public DateTime ScheduledStartUtc { get; set; }
    [Column("duration_minutes"), NotNull] public int DurationMinutes { get; set; }
    [Column("status"), NotNull] public short Status { get; set; }
    [Column("is_urgent"), NotNull] public bool IsUrgent { get; set; }
    [Column("notes")] public string? Notes { get; set; }
}

[Table("encounters")]
internal sealed class EncounterRow
{
    [PrimaryKey, Column("id"), NotNull] public string Id { get; set; } = null!;
    [Column("patient_id"), NotNull] public string PatientId { get; set; } = null!;
    [Column("kind"), NotNull] public short Kind { get; set; }
    [Column("started_at_utc"), NotNull] public DateTime StartedAtUtc { get; set; }
    [Column("ended_at_utc")] public DateTime? EndedAtUtc { get; set; }
    [Column("status"), NotNull] public short Status { get; set; }
    [Column("is_urgent"), NotNull] public bool IsUrgent { get; set; }
    [Column("notes")] public string? Notes { get; set; }
}

[Table("appointment_encounter_links")]
internal sealed class AppointmentEncounterLinkRow
{
    [PrimaryKey, Column("appointment_id"), NotNull] public string AppointmentId { get; set; } = null!;
    [Column("encounter_id"), NotNull] public string EncounterId { get; set; } = null!;
}

[Table("visits")]
internal sealed class VisitRow
{
    [PrimaryKey, Column("encounter_id"), NotNull] public string EncounterId { get; set; } = null!;
    [Column("kind"), NotNull] public short Kind { get; set; }
}

[Table("endoscopies")]
internal sealed class EndoscopyRow
{
    [PrimaryKey, Column("encounter_id"), NotNull] public string EncounterId { get; set; } = null!;
    [Column("endoscopy_type"), NotNull] public short EndoscopyType { get; set; }
    [Column("report_document_json")] public string? ReportDocumentJson { get; set; }
}

[Table("clinical_exams")]
internal sealed class ClinicalExamRow
{
    [PrimaryKey, Column("encounter_id"), NotNull] public string EncounterId { get; set; } = null!;
    [Column("has_serious_findings"), NotNull] public bool HasSeriousFindings { get; set; }
    [Column("clinical_notes")] public string? ClinicalNotes { get; set; }
}

[Table("infai_tests")]
internal sealed class InfaiTestRow
{
    [PrimaryKey, Column("encounter_id"), NotNull] public string EncounterId { get; set; } = null!;
    [Column("result"), NotNull] public short Result { get; set; }
    [Column("patient_contacted"), NotNull] public bool PatientContacted { get; set; }
    [Column("report_storage_key")] public string? ReportStorageKey { get; set; }
}

[Table("practice_state")]
internal sealed class PracticeStateRow
{
    [PrimaryKey, Column("id")] public int Id { get; set; }
    [Column("active_encounter_id")] public string? ActiveEncounterId { get; set; }
}
