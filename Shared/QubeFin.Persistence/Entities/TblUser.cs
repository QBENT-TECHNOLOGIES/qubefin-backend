using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TblUser
{
    public Guid Id { get; set; }

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public Guid? EmployeeId { get; set; }

    public bool IsActive { get; set; }

    public bool IsSuperAdmin { get; set; }

    public bool HasMfaEnabled { get; set; }

    public string MfaSecret { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public virtual TblEmployee? Employee { get; set; }

    public virtual ICollection<TblLoanApplicationWorkflow> TblLoanApplicationWorkflows { get; set; } = new List<TblLoanApplicationWorkflow>();

    public virtual ICollection<TblOrganizationUnit> TblOrganizationUnitCreatedByNavigations { get; set; } = new List<TblOrganizationUnit>();

    public virtual ICollection<TblOrganizationUnit> TblOrganizationUnitLastModifiedByNavigations { get; set; } = new List<TblOrganizationUnit>();

    public virtual ICollection<TblUserMenu> TblUserMenus { get; set; } = new List<TblUserMenu>();

    public virtual ICollection<TblUserSession> TblUserSessions { get; set; } = new List<TblUserSession>();
}
