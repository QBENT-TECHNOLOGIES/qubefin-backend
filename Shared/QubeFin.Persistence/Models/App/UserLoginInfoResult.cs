namespace QubeFin.Persistence.Models.App
{
    /// <summary>
    /// One row of [Auth].[USP_GetUserLoginInfo]: the user columns repeat on every row and the
    /// organization-unit columns are null when the user has no attendance location.
    /// </summary>
    public class UserLoginInfoResult
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid? EmployeeId { get; set; }
        public string Employee { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public string? CompanyLogoUrl { get; set; }
        public bool IsMileageEnabled { get; set; }

        public Guid? OrganizationUnitId { get; set; }
        public string? OrganizationUnitName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public TimeOnly? AttendanceInTime { get; set; }
        public TimeOnly? AttendanceOutTime { get; set; }
        public int CheckRadiusInMeter { get; set; }
        public bool IsLoginRequired { get; set; }

        // Tracking: role (post name, 'Admin' for a super admin), unit, branches in scope (comma-separated) and whether they can create jobs
        public string? Role { get; set; }
        public string? UnitName { get; set; }
        public string? OrganisationUnitIds { get; set; }
        public bool CanCreateJobs { get; set; }
    }
}
