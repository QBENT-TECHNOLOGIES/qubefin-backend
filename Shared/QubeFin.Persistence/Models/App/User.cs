namespace QubeFin.Persistence.Models.App;

public class User
{
    public Guid Id { get; private set; }
    public string UserName { get; private set; } = null!;
    public string Password { get; private set; } = null!;
    public Guid? EmployeeId { get; private set; }
    public bool IsActive { get; private set; }
    public bool IsSuperAdmin { get; private set; }
    public bool HasMfaEnabled { get; private set; }
    public string MfaSecret { get; private set; } = null!;
    public bool IsAttendanceOptional { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? LastModifiedOn { get; private set; }
    public Guid? LastModifiedBy { get; private set; }
    public string? EmployeeName { get; set; }

    private User() { }

    public User(Guid id, string username, string password, Guid? employeeId, bool isActive, bool isSuperAdmin, bool hasMfaEnabled, string mfaSecret, bool isAttendanceOptional,
        Guid createdBy, DateTime createdOn, Guid? lastModifiedBy, DateTime? lastModifiedOn, string? employeeName)
    {
        Id = id;
        UserName = username;
        Password = password;
        EmployeeId = employeeId;
        IsActive = isActive;
        IsSuperAdmin = isSuperAdmin;
        HasMfaEnabled = hasMfaEnabled;
        MfaSecret = mfaSecret;
        IsAttendanceOptional = isAttendanceOptional;
        CreatedBy = createdBy;
        CreatedOn = createdOn;
        LastModifiedBy = lastModifiedBy;
        LastModifiedOn = lastModifiedOn;
        EmployeeName = employeeName;
    }

    public static User Create(Guid id, string username, string password, Guid? employeeId, string mfaSecret, bool isAttendanceOptional, Guid createdBy)
    {
        var user = new User
        {
            Id = id,
            UserName = username,
            Password = password,
            EmployeeId = employeeId,
            MfaSecret = mfaSecret,
            IsAttendanceOptional = isAttendanceOptional,
            IsActive = true,
            IsSuperAdmin = false,
            CreatedBy = createdBy,
            CreatedOn = DateTime.Now
        };

        return user;
    }

    public void Update(string username, bool isActive, bool hasMfaEnabled, bool isAttendanceOptional, Guid modifiedBy)
    {
        UserName = username;
        IsActive = isActive;
        HasMfaEnabled = hasMfaEnabled;
        IsAttendanceOptional = isAttendanceOptional;
        LastModifiedBy = modifiedBy;
        LastModifiedOn = DateTime.Now;
    }

    public void EnableMfa()
    {
        HasMfaEnabled = true;
        //MfaRegisterTime = DateTime.Now;
    }
}
