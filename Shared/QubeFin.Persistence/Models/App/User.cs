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
    public DateTime CreatedOn { get; private set; }
    public Guid CreatedBy { get; private set; }
    public DateTime? LastModifiedOn { get; private set; }
    public Guid? LastModifiedBy { get; private set; }

    private User() { }

    public User(Guid id, string username, string password, Guid? employeeId, bool isActive, bool isSuperAdmin, bool hasMfaEnabled, string mfaSecret)
    {
        Id = id;
        UserName = username;
        Password = password;
        EmployeeId = employeeId;
        IsActive = isActive;
        IsSuperAdmin = isSuperAdmin;
        HasMfaEnabled = hasMfaEnabled;
        MfaSecret = mfaSecret;
    }

    public static User Create(Guid id, string username, string password, Guid? employeeId, string mfaSecret, Guid createdBy)
    {
        var user = new User
        {
            Id = id,
            UserName = username,
            Password = password,
            EmployeeId = employeeId,
            MfaSecret = mfaSecret,
            IsActive = true,
            IsSuperAdmin = false,
            CreatedBy = createdBy,
            CreatedOn = DateTime.Now
        };

        return user;
    }
}
