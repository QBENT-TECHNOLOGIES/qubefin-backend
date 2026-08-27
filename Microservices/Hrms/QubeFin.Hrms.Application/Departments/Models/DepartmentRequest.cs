using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.Departments.Models
{
    public class DepartmentRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
    }
}
