using QubeFin.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.LeaveApproval.Models
{
    public class LeaveApprovalSearchRequest : SearchParam
    {
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public Guid? SearchEmployeeId { get; set; }
    }
}
