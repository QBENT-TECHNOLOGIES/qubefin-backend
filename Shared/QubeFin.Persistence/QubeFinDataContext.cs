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

    public virtual DbSet<TblDesignation> TblDesignations { get; set; }

    public virtual DbSet<TblDesignationRole> TblDesignationRoles { get; set; }

    public virtual DbSet<TblEmployee> TblEmployees { get; set; }

    public virtual DbSet<TblHoliday> TblHolidays { get; set; }

    public virtual DbSet<TblMenu> TblMenus { get; set; }

    public virtual DbSet<TblMenuGroup> TblMenuGroups { get; set; }

    public virtual DbSet<TblOrganizationUnit> TblOrganizationUnits { get; set; }

    public virtual DbSet<TblOrganizationUnitType> TblOrganizationUnitTypes { get; set; }

    public virtual DbSet<TblPost> TblPosts { get; set; }

    public virtual DbSet<TblRole> TblRoles { get; set; }

    public virtual DbSet<TblRoleMenu> TblRoleMenus { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<TblUserMenu> TblUserMenus { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblAdministrativeUnit>(entity =>
        {
            entity.ToTable("Tbl_AdministrativeUnit", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

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
            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<TblDesignation>(entity =>
        {
            entity.ToTable("Tbl_Designation", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

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
            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.FirstName).HasMaxLength(30);
            entity.Property(e => e.LastName).HasMaxLength(30);
            entity.Property(e => e.MiddleName).HasMaxLength(30);
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

            entity.HasOne(d => d.MenuGroup).WithMany(p => p.TblMenus)
                .HasForeignKey(d => d.MenuGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Menu_Tbl_MenuGroup");
        });

        modelBuilder.Entity<TblMenuGroup>(entity =>
        {
            entity.ToTable("Tbl_MenuGroup", "Auth");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Category).HasMaxLength(100);
            entity.Property(e => e.Icon).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Target).HasMaxLength(100);
        });

        modelBuilder.Entity<TblOrganizationUnit>(entity =>
        {
            entity.ToTable("Tbl_OrganizationUnit", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();

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

        modelBuilder.Entity<TblRole>(entity =>
        {
            entity.ToTable("Tbl_Role", "Auth");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(30);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<TblRoleMenu>(entity =>
        {
            entity.HasKey(e => new { e.RoleId, e.MenuId });

            entity.ToTable("Tbl_RoleMenu", "Auth");

            entity.HasOne(d => d.Menu).WithMany(p => p.TblRoleMenus)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_RoleMenu_Tbl_Menu");

            entity.HasOne(d => d.Role).WithMany(p => p.TblRoleMenus)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_RoleMenu_Tbl_Role");
        });

        modelBuilder.Entity<TblUser>(entity =>
        {
            entity.ToTable("Tbl_User", "Auth");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.MfaSecret).HasMaxLength(50);
            entity.Property(e => e.Password).HasMaxLength(500);
            entity.Property(e => e.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UserName).HasMaxLength(30);

            entity.HasOne(d => d.Employee).WithMany(p => p.TblUsers)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("FK_Tbl_User_Tbl_Employee");
        });

        modelBuilder.Entity<TblUserMenu>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.MenuId });

            entity.ToTable("Tbl_UserMenu", "Auth");

            entity.HasOne(d => d.Menu).WithMany(p => p.TblUserMenus)
                .HasForeignKey(d => d.MenuId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_UserMenu_Tbl_Menu");

            entity.HasOne(d => d.User).WithMany(p => p.TblUserMenus)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_UserMenu_Tbl_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
