// GIPractice.Api/Models/PatientTimelineDtos.cs
using System;

namespace GIPractice.Api.Models
{
    public class PatientTimelineItemDto
    {
        public DateTime WhenUtc { get; set; }

        /// <summary>
        /// What kind of thing this is ("Appointment", "Visit", "Endoscopy", "Test", "InfaiTest", "Operation", "Treatment").
        /// </summary>
        public string Kind { get; set; } = string.Empty;

        /// <summary>
        /// Id of the underlying entity (Appointment.Id, Visit.Id, Endoscopy.Id, Test.Id, etc.).
        /// </summary>
        public int SourceId { get; set; }

        /// <summary>
        /// Short label to show in the UI (“Visit”, “Gastroscopy”, “H. pylori breath test”, etc.)
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Optional description / notes.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Optional extra info (Outcome, Urgent, results, etc.).
        /// </summary>
        public string? Extra { get; set; }
    }
}
