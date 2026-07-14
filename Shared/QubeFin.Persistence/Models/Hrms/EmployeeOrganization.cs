using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Persistence.Models.Hrms
{
    public class EmployeeOrganization
    {
        public Guid? OrganizationUnitId { get; set; }
        public string? OrganizationUnitName { get; set; }
        public TimeOnly? AttendanceInTime { get; set; }
        public TimeOnly? AttendanceOutTime { get; set; }

        public EmployeeOrganization()
        {

        }
        public EmployeeOrganization(Guid? organizationUnitId, string? organizationUnitName, TimeOnly? attendanceInTime, TimeOnly? attendanceOutTime)
        {
            OrganizationUnitId = organizationUnitId;
            OrganizationUnitName = organizationUnitName;
            AttendanceInTime = attendanceInTime;
            AttendanceOutTime = attendanceOutTime;
        }
    }
}



  
