using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.App.Application.Users.Models
{
    public class UserLoginInfoResponse
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid? EmployeeId { get; set; }
        public string? Employee { get; set; }
        public string? Gender { get; set; }
        public string? EmployeeCode { get; set; }
        public string? Designation { get; set; }
        public string? CompanyLogoUrl { get; set; }
        public List<UserAccessOrganizationUnit> AccessOrganizationUnits { get; set; } = new List<UserAccessOrganizationUnit>();
    }
    public class UserAccessOrganizationUnit
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public TimeOnly? AttendanceInTime { get; set; }
        public TimeOnly? AttendanceOutTime { get; set; }
        public int CheckRadiusInMeter { get; set; }
    }
}
