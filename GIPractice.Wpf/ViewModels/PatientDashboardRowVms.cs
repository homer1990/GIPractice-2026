using System;

namespace GIPractice.Wpf.ViewModels;

// Right-side: "ΒΙΟΨΙΕΣ ΠΟΥ ΔΕΝ ΕΧΟΥΝ ΣΤΑΛΘΕΙ"
public sealed class PendingBiopsyVm
{
    public DateTime EndoscopyDateTime { get; set; }
    public bool IsUrgent { get; set; }
    public int HowManyBiopsyBottles { get; set; }
}

// Right-side: "ΠΑΘΟΛΟΓ/ΚΕΣ ΠΟΥ ΔΕΝ ΕΧΟΥΝ ΒΓΕΙ ΑΚΟΜΗ"
public sealed class PendingPathologyVm
{
    public DateTime EndoscopyDateTime { get; set; }
    public bool IsUrgent { get; set; }
    public int HowManyBiopsyBottles { get; set; }
}

// Right-side: "ΠΡΟΣΕΧΗ ΡΑΝΤΕΒΟΥ"
public sealed class PendingAppointmentVm
{
    public DateTime Date { get; set; }
    public bool IsUrgent { get; set; }
    public string AppointmentTypeName { get; set; } = "";
}

// Lower: "ΕΠΙΣΚΕΨΕΙΣ"
public sealed class VisitVm
{
    public DateTime Date { get; set; }
    public string Notes { get; set; } = "";
    public int DurationMinutes { get; set; }
    public string Procedures { get; set; } = "";
}



// Lower: "ΙΣΤΟΡΙΚΟ" (Encounters)
public sealed class EncounterHistoryVm
{
    public DateTime Start { get; set; }
    public DateTime? End { get; set; }

    // Generic encounter kind (e.g., Ιατρείο, Κλινική Επίσκεψη, Ενδοσκόπηση)
    public string EncounterTypeName { get; set; } = "";

    // If this encounter is an endoscopy, set this. UI will show this instead of EncounterTypeName.
    public string? EndoscopyTypeName { get; set; }

    public string DisplayType => !string.IsNullOrWhiteSpace(EndoscopyTypeName)
        ? EndoscopyTypeName!
        : EncounterTypeName;

    public string Notes { get; set; } = "";

    public int? DurationMinutes
        => End.HasValue ? (int)(End.Value - Start).TotalMinutes : null;
}
// Lower: "ΕΝΔΟΣΚΟΠΗΣΕΙΣ"
public sealed class EndoscopyVm
{
    public DateTime EndoscopyDateTime { get; set; }
    public bool IsUrgent { get; set; }
    public string Type { get; set; } = "";
    public int HowManyBiopsyBottles { get; set; }
}

// Lower: "ΠΑΘΟΛΟΓΟΑΝΑΤΟΜΙΚΕΣ"
public sealed class PathologyReportVm
{
    public DateTime EndoscopyDateTime { get; set; }
    public DateTime? ParcelSentDateTime { get; set; }
    public string ParcelId { get; set; } = "";
    public bool IsUrgent { get; set; }
    public string EndoscopyType { get; set; } = "";
    public int HowManyBiopsyBottles { get; set; }
}
