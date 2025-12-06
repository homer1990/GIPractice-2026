using System.Collections.Generic;

namespace GIPractice.Api.Models
{
    public class PatientDashboardDto
    {
        public PatientSummaryDto Summary { get; set; } = null!;

        /// <summary>
        /// Recent timeline items (already ordered, usually descending).
        /// </summary>
        public List<PatientTimelineItemDto> Timeline { get; set; } = [];
    }
}
