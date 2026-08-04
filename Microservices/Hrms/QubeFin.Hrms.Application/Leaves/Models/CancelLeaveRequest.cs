using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.Leaves.Models
{
    public class CancelLeaveRequest
    {
        public Guid Id { get; set; }
        public string? Reason { get; set; }
    }
}
