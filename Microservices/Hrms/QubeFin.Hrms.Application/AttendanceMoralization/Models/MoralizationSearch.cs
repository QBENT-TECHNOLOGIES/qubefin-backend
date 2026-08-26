using QubeFin.Persistence.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.AttendanceMoralization.Models
{
    public class MoralizationSearch : SearchParam
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? SearchOrganizationUnitId { get; set; }
        public Guid? EmployeeId { get; set; }
        public int? Status { get; set; }
    }
}
