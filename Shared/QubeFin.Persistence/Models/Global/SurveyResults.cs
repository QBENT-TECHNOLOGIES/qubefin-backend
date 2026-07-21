using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Global
{
    public class SurveyResults
    {
        public Guid Id { get; set; }
        public string AssignmentNo { get; set; } = string.Empty;
        public DateOnly AssignmentDate { get; set; }
        public string SurveyType { get; set; } = string.Empty;
        public DateOnly TentativeSubmissionDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalCount { get; set; }
    }
}
