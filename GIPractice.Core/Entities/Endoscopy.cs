using GIPractice.Core.Abstractions;
using GIPractice.Core.Enums;

namespace GIPractice.Core.Entities;

public class Endoscopy : BaseEntity
{
    public int EncounterId { get; set; }
    public Encounter Encounter { get; set; } = null!;

    public EndoscopyType Type { get; set; }
    public string? Notes { get; set; }

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
