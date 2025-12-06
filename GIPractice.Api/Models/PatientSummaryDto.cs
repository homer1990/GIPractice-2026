// File: GIPractice.Api/Models/PatientSummaryDtos.cs
using System;

namespace GIPractice.Api.Models
{
    public class PatientSummaryDto
    {
        public int PatientId { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FathersName { get; set; } = string.Empty;

        public string FullName => $"{LastName} {FirstName}";

        public string PersonalNumber { get; set; } = string.Empty;
        public DateTime BirthDay { get; set; }
        public int AgeYears { get; set; }

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }

        // Totals
        public int TotalAppointments { get; set; }
        public int TotalVisits { get; set; }
        public int TotalEndoscopies { get; set; }
        public int TotalDiagnoses { get; set; }
        public int TotalTests { get; set; }
        public int TotalInfaiTests { get; set; }
        public int TotalOperations { get; set; }
        public int TotalTreatments { get; set; }

        // Last activity
        public DateTime? LastAppointmentUtc { get; set; }
        public DateTime? LastVisitUtc { get; set; }
        public DateTime? LastEndoscopyUtc { get; set; }
        public DateTime? LastTestUtc { get; set; }
        public DateTime? LastInfaiTestUtc { get; set; }
        public DateTime? LastOperationUtc { get; set; }
        public DateTime? LastTreatmentStartUtc { get; set; }

        // Ongoing stuff
        public int ActiveTreatmentsCount { get; set; }

        // ---- Extra properties used by some endpoints ----

        /// <summary>
        /// Optional embedded basic patient DTO (used by some summary/dashboard endpoints).
        /// </summary>
        public PatientDto? Patient { get; set; }

        /// <summary>
        /// Optional short text of main diagnoses for quick display.
        /// </summary>
        public string? MainDiagnosesSummary { get; set; }

        /// <summary>
        /// Alias for ActiveTreatmentsCount so older/newer code can use either name.
        /// </summary>
        public int ActiveTreatments
        {
            get => ActiveTreatmentsCount;
            set => ActiveTreatmentsCount = value;
        }
    }
}
