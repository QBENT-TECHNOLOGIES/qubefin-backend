using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Hrms;

    public class CandidateVerificationDto
    {
        public Guid CandidateId { get; set; }

        public string? AadharNumber { get; set; }
        public bool IsAadharValidated { get; set; }

        public string? VoterNumber { get; set; }
        public bool IsVoterValited { get; set; }

        public string? Pan { get; set; }
        public bool IsPanValidated { get; set; }

        public string MobileNo { get; set; } = string.Empty;
        public bool IsMobileValidated { get; set; }

        public string? Uan { get; set; }
        public bool IsUanVerified { get; set; }

        public bool IsCreditBureauChecked { get; set; }
        public string? CreditBureauReportLink { get; set; }

        public string OverallStatus { get; set; } = "Pending";
    }

