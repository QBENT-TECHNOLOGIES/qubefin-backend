using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence.Entities;

namespace QubeFin.Persistence;

public partial class QubeFinDataContext : DbContext
{
    public QubeFinDataContext()
    {
    }

    public QubeFinDataContext(DbContextOptions<QubeFinDataContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TblAdministrativeUnit> TblAdministrativeUnits { get; set; }

    public virtual DbSet<TblAdministrativeUnitType> TblAdministrativeUnitTypes { get; set; }

    public virtual DbSet<TblAttendance> TblAttendances { get; set; }

    public virtual DbSet<TblCompany> TblCompanies { get; set; }

    public virtual DbSet<TblCreditOrganization> TblCreditOrganizations { get; set; }

    public virtual DbSet<TblDepartment> TblDepartments { get; set; }

    public virtual DbSet<TblDesignation> TblDesignations { get; set; }

    public virtual DbSet<TblDesignationRole> TblDesignationRoles { get; set; }

    public virtual DbSet<TblEmployee> TblEmployees { get; set; }

    public virtual DbSet<TblEmployeeDesignation> TblEmployeeDesignations { get; set; }

    public virtual DbSet<TblEmployeeDocument> TblEmployeeDocuments { get; set; }

    public virtual DbSet<TblEmployeeEmployment> TblEmployeeEmployments { get; set; }

    public virtual DbSet<TblEmployeeQualification> TblEmployeeQualifications { get; set; }

    public virtual DbSet<TblEmployeeReference> TblEmployeeReferences { get; set; }

    public virtual DbSet<TblFinancialInstitute> TblFinancialInstitutes { get; set; }

    public virtual DbSet<TblFinancialYear> TblFinancialYears { get; set; }

    public virtual DbSet<TblHoliday> TblHolidays { get; set; }

    public virtual DbSet<TblMenu> TblMenus { get; set; }

    public virtual DbSet<TblOrganizationUnit> TblOrganizationUnits { get; set; }

    public virtual DbSet<TblOrganizationUnitType> TblOrganizationUnitTypes { get; set; }

    public virtual DbSet<TblPermission> TblPermissions { get; set; }

    public virtual DbSet<TblPoliceStation> TblPoliceStations { get; set; }

    public virtual DbSet<TblPost> TblPosts { get; set; }

    public virtual DbSet<TblPostOffice> TblPostOffices { get; set; }

    public virtual DbSet<TblRole> TblRoles { get; set; }

    public virtual DbSet<TblRoleMenu> TblRoleMenus { get; set; }

    public virtual DbSet<TblRolePermission> TblRolePermissions { get; set; }

    public virtual DbSet<TblSalaryComponent> TblSalaryComponents { get; set; }

    public virtual DbSet<TblSalaryComponentCategory> TblSalaryComponentCategories { get; set; }

    public virtual DbSet<TblSalaryGrade> TblSalaryGrades { get; set; }

    public virtual DbSet<TblSalaryStructure> TblSalaryStructures { get; set; }

    public virtual DbSet<TblSalaryStructureComponent> TblSalaryStructureComponents { get; set; }

    public virtual DbSet<TblSystemValue> TblSystemValues { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<TblUserDevice> TblUserDevices { get; set; }

    public virtual DbSet<TblUserMenu> TblUserMenus { get; set; }

    public virtual DbSet<TblUserSession> TblUserSessions { get; set; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblAdministrativeUnit>(entity =>
        {
            entity.ToTable("Tbl_AdministrativeUnit", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.AdministrativeUnitType).WithMany(p => p.TblAdministrativeUnits)
                .HasForeignKey(d => d.AdministrativeUnitTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_AdministrativeUnit_Tbl_AdministrativeUnitType");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Tbl_AdministrativeUnit_Tbl_AdministrativeUnit");
        });

        modelBuilder.Entity<TblAdministrativeUnitType>(entity =>
        {
            entity.ToTable("Tbl_AdministrativeUnitType", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Category).HasMaxLength(10);
            entity.Property(e => e.Icon).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<TblAttendance>(entity =>
        {
            entity.ToTable("Tbl_Attendance", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.OrganizationUnit).WithMany(p => p.TblAttendances)
                .HasForeignKey(d => d.OrganizationUnitId)
                .HasConstraintName("FK_Tbl_Attendance_Tbl_OrganizationUnit");
        });

        modelBuilder.Entity<TblCompany>(entity =>
        {
            entity.ToTable("Tbl_Company", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<TblCreditOrganization>(entity =>
        {
            entity.ToTable("Tbl_CreditOrganization", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<TblDepartment>(entity =>
        {
            entity.ToTable("Tbl_Department", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<TblDesignation>(entity =>
        {
            entity.ToTable("Tbl_Designation", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.OrganizationUnit).WithMany(p => p.TblDesignations)
                .HasForeignKey(d => d.OrganizationUnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Designation_Tbl_OrganizationUnit");

            entity.HasOne(d => d.Post).WithMany(p => p.TblDesignations)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Designation_Tbl_Post");
        });

        modelBuilder.Entity<TblDesignationRole>(entity =>
        {
            entity.ToTable("Tbl_DesignationRole", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Designation).WithMany(p => p.TblDesignationRoles)
                .HasForeignKey(d => d.DesignationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_DesignationRole_Tbl_Designation");

            entity.HasOne(d => d.Role).WithMany(p => p.TblDesignationRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_DesignationRole_Tbl_Role");
        });

        modelBuilder.Entity<TblEmployee>(entity =>
        {
            entity.ToTable("Tbl_Employee", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BankAccountType).HasMaxLength(10);
            entity.Property(e => e.BankBranch).HasMaxLength(50);
            entity.Property(e => e.BankHolderName).HasMaxLength(50);
            entity.Property(e => e.BloodGroup)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Caste).HasMaxLength(50);
            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.CreatedDate).HasColumnType("datetime");
            entity.Property(e => e.DisablityType).HasMaxLength(50);
            entity.Property(e => e.EmergencyContactMobile1)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.EmergencyContactMobile2)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.EmergencyContactName1).HasMaxLength(50);
            entity.Property(e => e.EmergencyContactName2).HasMaxLength(50);
            entity.Property(e => e.EmergencyContactRelation1).HasMaxLength(20);
            entity.Property(e => e.EmergencyContactRelation2).HasMaxLength(20);
            entity.Property(e => e.EmployementType).HasMaxLength(10);
            entity.Property(e => e.FatherName).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(30);
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.HowYouKnow).HasMaxLength(200);
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(30);
            entity.Property(e => e.MaritalStatus)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.MiddleName).HasMaxLength(30);
            entity.Property(e => e.MobileNo).HasMaxLength(10);
            entity.Property(e => e.MotherName).HasMaxLength(50);
            entity.Property(e => e.Nationality).HasMaxLength(25);
            entity.Property(e => e.OfficialEmail).HasMaxLength(100);
            entity.Property(e => e.PermanentHouseNo).HasMaxLength(20);
            entity.Property(e => e.PermanentLandMark).HasMaxLength(100);
            entity.Property(e => e.PermanentOwnerShipOfHouse).HasMaxLength(20);
            entity.Property(e => e.PermanentPinCode)
                .HasMaxLength(6)
                .IsFixedLength();
            entity.Property(e => e.PermanentRoadName).HasMaxLength(50);
            entity.Property(e => e.PersonalEmail).HasMaxLength(100);
            entity.Property(e => e.PresentHouseNo).HasMaxLength(20);
            entity.Property(e => e.PresentLandMark).HasMaxLength(100);
            entity.Property(e => e.PresentOwnerShipOfHouse).HasMaxLength(10);
            entity.Property(e => e.PresentPinCode)
                .HasMaxLength(6)
                .IsFixedLength();
            entity.Property(e => e.PresentRoadName).HasMaxLength(50);
            entity.Property(e => e.Religion).HasMaxLength(25);
            entity.Property(e => e.Salutation).HasMaxLength(20);
            entity.Property(e => e.SeparationDate).HasComputedColumnSql("(dateadd(year,(60),[DateOfBirth]))", false);

            entity.HasOne(d => d.Bank).WithMany(p => p.TblEmployees)
                .HasForeignKey(d => d.BankId)
                .HasConstraintName("FK_Tbl_Employee_Tbl_FinancialInstitute");

            entity.HasOne(d => d.Company).WithMany(p => p.TblEmployees)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Tbl_Employee_Tbl_Company");

            entity.HasOne(d => d.Department).WithMany(p => p.TblEmployees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Employee_Tbl_Department");

            entity.HasOne(d => d.OrganizationUnit).WithMany(p => p.TblEmployees)
                .HasForeignKey(d => d.OrganizationUnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Employee_Tbl_OrganizationUnit");

            entity.HasOne(d => d.PermanentAdministrativeUnit).WithMany(p => p.TblEmployeePermanentAdministrativeUnits)
                .HasForeignKey(d => d.PermanentAdministrativeUnitId)
                .HasConstraintName("FK_Tbl_Employee_Tbl_AdministrativeUnit");

            entity.HasOne(d => d.PresentAdministrativeUnit).WithMany(p => p.TblEmployeePresentAdministrativeUnits)
                .HasForeignKey(d => d.PresentAdministrativeUnitId)
                .HasConstraintName("FK_Tbl_Employee_Tbl_AdministrativeUnit1");
        });

        modelBuilder.Entity<TblEmployeeDesignation>(entity =>
        {
            entity.ToTable("Tbl_EmployeeDesignation", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Designation).WithMany(p => p.TblEmployeeDesignations)
                .HasForeignKey(d => d.DesignationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_EmployeeDesignation_Tbl_Designation");

            entity.HasOne(d => d.Employee).WithMany(p => p.TblEmployeeDesignations)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_EmployeeDesignation_Tbl_Employee");
        });

        modelBuilder.Entity<TblEmployeeDocument>(entity =>
        {
            entity.ToTable("Tbl_EmployeeDocument", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DocumentCategory).HasMaxLength(20);
            entity.Property(e => e.DocumentName).HasMaxLength(50);
            entity.Property(e => e.DocumentNo).HasMaxLength(50);
            entity.Property(e => e.FileName).HasMaxLength(150);
            entity.Property(e => e.FileNo).HasMaxLength(200);
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.UploadedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.TblEmployeeDocuments)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_EmployeeDocument_Tbl_Employee");
        });

        modelBuilder.Entity<TblEmployeeEmployment>(entity =>
        {
            entity.ToTable("Tbl_EmployeeEmployment", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Designation).HasMaxLength(100);
            entity.Property(e => e.EmployerName).HasMaxLength(100);
            entity.Property(e => e.ExpCertFileName).HasMaxLength(100);
            entity.Property(e => e.ExpCertFileNo).HasMaxLength(50);
            entity.Property(e => e.JobTitle).HasMaxLength(200);
            entity.Property(e => e.LastDrawnSalary).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.NocFileName).HasMaxLength(100);
            entity.Property(e => e.NocFileNo).HasMaxLength(50);

            entity.HasOne(d => d.Employee).WithMany(p => p.TblEmployeeEmployments)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_EmployeeEmployment_Tbl_Employee");
        });

        modelBuilder.Entity<TblEmployeeQualification>(entity =>
        {
            entity.ToTable("Tbl_EmployeeQualification", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AcademicStream).HasMaxLength(50);
            entity.Property(e => e.DocFileName).HasMaxLength(100);
            entity.Property(e => e.DocFileNo).HasMaxLength(50);
            entity.Property(e => e.GradeOrMarks).HasMaxLength(5);
            entity.Property(e => e.SchoolOrCollege).HasMaxLength(100);
            entity.Property(e => e.Specialization).HasMaxLength(50);
            entity.Property(e => e.UniversityOrBoard).HasMaxLength(100);

            entity.HasOne(d => d.Employee).WithMany(p => p.TblEmployeeQualifications)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_EmployeeQualification_Tbl_Employee");
        });

        modelBuilder.Entity<TblEmployeeReference>(entity =>
        {
            entity.ToTable("Tbl_EmployeeReference", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.HowDoYouKnow).HasMaxLength(200);
            entity.Property(e => e.Mobile).HasMaxLength(10);
            entity.Property(e => e.Occupation).HasMaxLength(100);
            entity.Property(e => e.PersonName).HasMaxLength(100);

            entity.HasOne(d => d.Employee).WithMany(p => p.TblEmployeeReferences)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_EmployeeReference_Tbl_Employee");
        });

        modelBuilder.Entity<TblFinancialInstitute>(entity =>
        {
            entity.ToTable("Tbl_FinancialInstitute", "Finance");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<TblFinancialYear>(entity =>
        {
            entity.ToTable("Tbl_FinancialYear", "Finance");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AssessmentYear).HasMaxLength(10);
            entity.Property(e => e.Caption).HasMaxLength(20);
        });

        modelBuilder.Entity<TblHoliday>(entity =>
        {
            entity.ToTable("Tbl_Holiday", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(50);
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<TblMenu>(entity =>
        {
            entity.ToTable("Tbl_Menu", "Auth");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Icon).HasMaxLength(20);
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Target).HasMaxLength(100);

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Tbl_Menu_Tbl_Menu");
        });

        modelBuilder.Entity<TblOrganizationUnit>(entity =>
        {
            entity.ToTable("Tbl_OrganizationUnit", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.OrganizationUnitType).WithMany(p => p.TblOrganizationUnits)
                .HasForeignKey(d => d.OrganizationUnitTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_OrganizationUnit_Tbl_OrganizationUnitType");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Tbl_OrganizationUnit_Tbl_OrganizationUnit");
        });

        modelBuilder.Entity<TblOrganizationUnitType>(entity =>
        {
            entity.ToTable("Tbl_OrganizationUnitType", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<TblPermission>(entity =>
        {
            entity.ToTable("Tbl_Permission", "Auth");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AccessFunction)
                .HasMaxLength(30)
                .HasDefaultValue("");
            entity.Property(e => e.AccessToken)
                .HasMaxLength(10)
                .HasDefaultValue("");
            entity.Property(e => e.Name)
                .HasMaxLength(41)
                .HasComputedColumnSql("(([AccessFunction]+'.')+[AccessToken])", false);
        });

        modelBuilder.Entity<TblPoliceStation>(entity =>
        {
            entity.ToTable("Tbl_PoliceStation", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.District).WithMany(p => p.TblPoliceStations)
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_PoliceStation_Tbl_AdministrativeUnit");
        });

        modelBuilder.Entity<TblPost>(entity =>
        {
            entity.ToTable("Tbl_Post", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<TblPostOffice>(entity =>
        {
            entity.ToTable("Tbl_PostOffice", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Pincode)
                .HasMaxLength(6)
                .IsFixedLength();
        });

        modelBuilder.Entity<TblRole>(entity =>
        {
            entity.ToTable("Tbl_Role", "Auth");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<TblRoleMenu>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.MenuId });

            entity.ToTable("Tbl_RoleMenu", "Auth");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Menu).WithMany(p => p.TblRoleMenus)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_RoleMenu_Tbl_Menu");

            entity.HasOne(d => d.Role).WithMany(p => p.TblRoleMenus)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_RoleMenu_Tbl_Role");
        });

        modelBuilder.Entity<TblRolePermission>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.PermissionId }).HasName("PK_Tbl_RolePermission_1");

            entity.ToTable("Tbl_RolePermission", "Auth");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Permission).WithMany(p => p.TblRolePermissions).HasForeignKey(d => d.PermissionId);

            entity.HasOne(d => d.Role).WithMany(p => p.TblRolePermissions).HasForeignKey(d => d.RoleId);
        });

        modelBuilder.Entity<TblSalaryComponent>(entity =>
        {
            entity.ToTable("Tbl_SalaryComponent", "Payroll");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IsCtccomponent).HasColumnName("IsCTCComponent");
            entity.Property(e => e.IsEsiapplicable).HasColumnName("IsESIApplicable");
            entity.Property(e => e.IsPfapplicable).HasColumnName("IsPFApplicable");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.Category).WithMany(p => p.TblSalaryComponents)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_SalaryComponent_Tbl_SalaryComponentCategory");
        });

        modelBuilder.Entity<TblSalaryComponentCategory>(entity =>
        {
            entity.ToTable("Tbl_SalaryComponentCategory", "Payroll");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<TblSalaryGrade>(entity =>
        {
            entity.ToTable("Tbl_SalaryGrade", "Payroll");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Code).HasMaxLength(10);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<TblSalaryStructure>(entity =>
        {
            entity.ToTable("Tbl_SalaryStructure", "Payroll");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.GrossAmount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Remarks).HasMaxLength(100);

            entity.HasOne(d => d.Grade).WithMany(p => p.TblSalaryStructures)
                .HasForeignKey(d => d.GradeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_SalaryStructure_Tbl_SalaryGrade");
        });

        modelBuilder.Entity<TblSalaryStructureComponent>(entity =>
        {
            entity.ToTable("Tbl_SalaryStructureComponent", "Payroll");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CalculationMethod).HasMaxLength(30);
            entity.Property(e => e.FixedAmount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.Formula).HasMaxLength(500);
            entity.Property(e => e.Percentage).HasColumnType("numeric(9, 2)");

            entity.HasOne(d => d.SalaryComponent).WithMany(p => p.TblSalaryStructureComponents)
                .HasForeignKey(d => d.SalaryComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_SalaryStructureComponent_Tbl_SalaryComponent");

            entity.HasOne(d => d.SalaryStructure).WithMany(p => p.TblSalaryStructureComponents)
                .HasForeignKey(d => d.SalaryStructureId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_SalaryStructureComponent_Tbl_SalaryStructure");
        });

        modelBuilder.Entity<TblSystemValue>(entity =>
        {
            entity.HasKey(e => new { e.SysKey, e.SysVal });

            entity.ToTable("Tbl_SystemValue", "Global");

            entity.Property(e => e.SysKey).HasMaxLength(30);
            entity.Property(e => e.SysVal).HasMaxLength(100);
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.ToTable("Tbl_User", "Auth");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.MfaSecret).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(500);
            entity.Property(e => e.UserName).HasMaxLength(30);

            entity.HasOne(d => d.Employee).WithMany(p => p.TblUsers)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Tbl_User_Tbl_Employee");
        });

        modelBuilder.Entity<TblUserDevice>(entity =>
        {
            entity.ToTable("Tbl_UserDevice", "Auth");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AssignDate).HasColumnType("datetime");
            entity.Property(e => e.DeviceId).HasMaxLength(100);
            entity.Property(e => e.ReleaseDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<TblUserMenu>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.MenuId });

            entity.ToTable("Tbl_UserMenu", "Auth");

            entity.Property(e => e.CreatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Menu).WithMany(p => p.TblUserMenus)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_UserMenu_Tbl_Menu");

            entity.HasOne(d => d.User).WithMany(p => p.TblUserMenus)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_UserMenu_Tbl_User");
        });

        modelBuilder.Entity<TblUserSession>(entity =>
        {
            entity.ToTable("Tbl_UserSession", "Auth");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AccessToken).HasMaxLength(4000);
            entity.Property(e => e.DeviceId).HasMaxLength(100);
            entity.Property(e => e.RefreshToken).HasMaxLength(50);
            entity.Property(e => e.SessionToken).HasMaxLength(50);
            entity.Property(e => e.UserAgent).HasMaxLength(500);

            entity.HasOne(d => d.User).WithMany(p => p.TblUserSessions).HasForeignKey(d => d.UserId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
