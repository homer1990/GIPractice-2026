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
