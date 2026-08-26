using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Global.Application.OrganizationUnits.Models
{
    public class DesignationRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid PostId { get; set; }
        public Guid OrganizationUnitId { get; set; }
        public Guid RoleId { get; set; }
        public Guid SalaryGradeId { get; set; }
        public bool IsActive { get; set; }
    }
}
