using GIPractice.Core.Abstractions;
using GIPractice.Core.Enums;

namespace GIPractice.Core.Entities;

public class Endoscopy : BaseEntity
{
    public EndoscopyType Type { get; set; }
    public DateTime PerformedAtUtc { get; set; }
    public bool IsUrgent { get; set; }
    public string? Notes { get; set; }

    public int VisitId { get; set; }
    public Visit Visit { get; set; } = null!;

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    public ICollection<Observation> Observations { get; set; } = [];

    public List<BiopsyBottle> BiopsyBottles { get; set; } = [];

    public Report? Report { get; set; }

    public ICollection<EndoscopyMedia> MediaFiles { get; set; } = [];

    public decimal? BiopsiesCost { get; set; }
    public decimal? EndoscopyCost { get; set; }
    public bool IsPaid { get; set; }
    public bool IsPaidBiopsies { get; set; }
    public List<Diagnosis> Diagnoses { get; set; } = [];
}
