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

    public virtual DbSet<TblAccountGroup> TblAccountGroups { get; set; }

    public virtual DbSet<TblAccountHead> TblAccountHeads { get; set; }

    public virtual DbSet<TblAccountLedger> TblAccountLedgers { get; set; }

    public virtual DbSet<TblAdministrativeUnit> TblAdministrativeUnits { get; set; }

    public virtual DbSet<TblAdministrativeUnitType> TblAdministrativeUnitTypes { get; set; }

    public virtual DbSet<TblAnswer> TblAnswers { get; set; }

    public virtual DbSet<TblApplicationStep> TblApplicationSteps { get; set; }

    public virtual DbSet<TblApplicatoinDocument> TblApplicatoinDocuments { get; set; }

    public virtual DbSet<TblAttendance> TblAttendances { get; set; }

    public virtual DbSet<TblBranchSurvey> TblBranchSurveys { get; set; }

    public virtual DbSet<TblCalenderYear> TblCalenderYears { get; set; }

    public virtual DbSet<TblCoBorrower> TblCoBorrowers { get; set; }

    public virtual DbSet<TblCompany> TblCompanies { get; set; }

    public virtual DbSet<TblCompanyBankAccount> TblCompanyBankAccounts { get; set; }

    public virtual DbSet<TblCreditDataAccount> TblCreditDataAccounts { get; set; }

    public virtual DbSet<TblCreditDataAddress> TblCreditDataAddresses { get; set; }

    public virtual DbSet<TblCreditDataAlert> TblCreditDataAlerts { get; set; }

    public virtual DbSet<TblCreditDataDependent> TblCreditDataDependents { get; set; }

    public virtual DbSet<TblCreditDataIdentity> TblCreditDataIdentities { get; set; }

    public virtual DbSet<TblCreditDataIncome> TblCreditDataIncomes { get; set; }

    public virtual DbSet<TblCreditDatum> TblCreditData { get; set; }

    public virtual DbSet<TblCreditOrganization> TblCreditOrganizations { get; set; }

    public virtual DbSet<TblDepartment> TblDepartments { get; set; }

    public virtual DbSet<TblDesignation> TblDesignations { get; set; }

    public virtual DbSet<TblDesignationGradeMapping> TblDesignationGradeMappings { get; set; }

    public virtual DbSet<TblDesignationRole> TblDesignationRoles { get; set; }

    public virtual DbSet<TblEmployee> TblEmployees { get; set; }

    public virtual DbSet<TblEmployeeAttendanceException> TblEmployeeAttendanceExceptions { get; set; }

    public virtual DbSet<TblEmployeeDesignation> TblEmployeeDesignations { get; set; }

    public virtual DbSet<TblEmployeeDocument> TblEmployeeDocuments { get; set; }

    public virtual DbSet<TblEmployeeEmployment> TblEmployeeEmployments { get; set; }

    public virtual DbSet<TblEmployeeGrossSalary> TblEmployeeGrossSalaries { get; set; }

    public virtual DbSet<TblEmployeeQualification> TblEmployeeQualifications { get; set; }

    public virtual DbSet<TblEmployeeReference> TblEmployeeReferences { get; set; }

    public virtual DbSet<TblFinancialInstitute> TblFinancialInstitutes { get; set; }

    public virtual DbSet<TblFinancialYear> TblFinancialYears { get; set; }

    public virtual DbSet<TblGroup> TblGroups { get; set; }

    public virtual DbSet<TblHoliday> TblHolidays { get; set; }

    public virtual DbSet<TblKycDocument> TblKycDocuments { get; set; }

    public virtual DbSet<TblLeaveEvent> TblLeaveEvents { get; set; }

    public virtual DbSet<TblLeaveRequest> TblLeaveRequests { get; set; }

    public virtual DbSet<TblLeaveRequestDocument> TblLeaveRequestDocuments { get; set; }

    public virtual DbSet<TblLeaveRequestEvent> TblLeaveRequestEvents { get; set; }

    public virtual DbSet<TblLeaveStatus> TblLeaveStatuses { get; set; }

    public virtual DbSet<TblLeaveTransaction> TblLeaveTransactions { get; set; }

    public virtual DbSet<TblLeaveType> TblLeaveTypes { get; set; }

    public virtual DbSet<TblLoanApplication> TblLoanApplications { get; set; }

    public virtual DbSet<TblLoanApplicationWorkflow> TblLoanApplicationWorkflows { get; set; }

    public virtual DbSet<TblLoanApplicationWorkflowDocument> TblLoanApplicationWorkflowDocuments { get; set; }

    public virtual DbSet<TblLoanApplicationWorkflowStep> TblLoanApplicationWorkflowSteps { get; set; }

    public virtual DbSet<TblLoanProduct> TblLoanProducts { get; set; }

    public virtual DbSet<TblLoanProductQuestion> TblLoanProductQuestions { get; set; }

    public virtual DbSet<TblLoanProductStep> TblLoanProductSteps { get; set; }

    public virtual DbSet<TblLoanSettingParameter> TblLoanSettingParameters { get; set; }

    public virtual DbSet<TblLoanWorkflow> TblLoanWorkflows { get; set; }

    public virtual DbSet<TblLoanWorkflowDocument> TblLoanWorkflowDocuments { get; set; }

    public virtual DbSet<TblLoanWorkflowStep> TblLoanWorkflowSteps { get; set; }

    public virtual DbSet<TblMember> TblMembers { get; set; }

    public virtual DbSet<TblMemberAddress> TblMemberAddresses { get; set; }

    public virtual DbSet<TblMemberDocument> TblMemberDocuments { get; set; }

    public virtual DbSet<TblMenu> TblMenus { get; set; }

    public virtual DbSet<TblOrganizationUnit> TblOrganizationUnits { get; set; }

    public virtual DbSet<TblOrganizationUnitType> TblOrganizationUnitTypes { get; set; }

    public virtual DbSet<TblPayRoll> TblPayRolls { get; set; }

    public virtual DbSet<TblPayRollComponent> TblPayRollComponents { get; set; }

    public virtual DbSet<TblPermission> TblPermissions { get; set; }

    public virtual DbSet<TblPoliceStation> TblPoliceStations { get; set; }

    public virtual DbSet<TblPost> TblPosts { get; set; }

    public virtual DbSet<TblPostOffice> TblPostOffices { get; set; }

    public virtual DbSet<TblQuestion> TblQuestions { get; set; }

    public virtual DbSet<TblRole> TblRoles { get; set; }

    public virtual DbSet<TblRoleMenu> TblRoleMenus { get; set; }

    public virtual DbSet<TblRolePermission> TblRolePermissions { get; set; }

    public virtual DbSet<TblSalaryComponent> TblSalaryComponents { get; set; }

    public virtual DbSet<TblSalaryComponentCategory> TblSalaryComponentCategories { get; set; }

    public virtual DbSet<TblSalaryGrade> TblSalaryGrades { get; set; }

    public virtual DbSet<TblSalaryStructure> TblSalaryStructures { get; set; }

    public virtual DbSet<TblSalaryStructureComponent> TblSalaryStructureComponents { get; set; }

    public virtual DbSet<TblSurvey> TblSurveys { get; set; }

    public virtual DbSet<TblSurveyAssigned> TblSurveyAssigneds { get; set; }

    public virtual DbSet<TblSurveyCommittee> TblSurveyCommittees { get; set; }

    public virtual DbSet<TblSurveyCommitteeEvaluation> TblSurveyCommitteeEvaluations { get; set; }

    public virtual DbSet<TblSurveyDocument> TblSurveyDocuments { get; set; }

    public virtual DbSet<TblSystemValue> TblSystemValues { get; set; }

    public virtual DbSet<TblUser> TblUsers { get; set; }

    public virtual DbSet<TblUserDevice> TblUserDevices { get; set; }

    public virtual DbSet<TblUserMenu> TblUserMenus { get; set; }

    public virtual DbSet<TblUserSession> TblUserSessions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TblAccountGroup>(entity =>
        {
            entity.ToTable("Tbl_AccountGroup", "Finance");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Code).HasMaxLength(10);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);

            entity.HasOne(d => d.AccHead).WithMany(p => p.TblAccountGroups)
                .HasForeignKey(d => d.AccHeadId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_AccountGroup_Tbl_AccountHead");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .HasConstraintName("FK_Tbl_AccountGroup_Tbl_AccountGroup1");
        });

        modelBuilder.Entity<TblAccountHead>(entity =>
        {
            entity.ToTable("Tbl_AccountHead", "Finance");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<TblAccountLedger>(entity =>
        {
            entity.ToTable("Tbl_AccountLedger", "Finance");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Alias).HasMaxLength(100);
            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.OpeningBalance)
                .HasDefaultValue(0m, "DF_Tbl_AccountLedger_OpeningBalance")
                .HasColumnType("numeric(18, 2)");
        });

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

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblAdministrativeUnitCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_AdministrativeUnit_Tbl_User");

            entity.HasOne(d => d.LastModifiedByNavigation).WithMany(p => p.TblAdministrativeUnitLastModifiedByNavigations)
                .HasForeignKey(d => d.LastModifiedBy)
                .HasConstraintName("FK_Tbl_AdministrativeUnit_Tbl_User1");

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

        modelBuilder.Entity<TblAnswer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Tbl_HouseVisitAnswer");

            entity.ToTable("Tbl_Answer", "Loan");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Descripton).HasMaxLength(100);

            entity.HasOne(d => d.Question).WithMany(p => p.TblAnswers)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_HouseVisitAnswer_Tbl_HouseVisitQuestion");
        });

        modelBuilder.Entity<TblApplicationStep>(entity =>
        {
            entity.ToTable("Tbl_ApplicationStep", "Loan");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Color).HasMaxLength(10);
            entity.Property(e => e.Component).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(50);
        });

        modelBuilder.Entity<TblApplicatoinDocument>(entity =>
        {
            entity.ToTable("Tbl_ApplicatoinDocument", "Loan");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Sequence).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<TblAttendance>(entity =>
        {
            entity.ToTable("Tbl_Attendance", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.InTimeLat).HasColumnType("numeric(9, 6)");
            entity.Property(e => e.InTimeLong).HasColumnType("numeric(9, 6)");
            entity.Property(e => e.OutTimeLat).HasColumnType("numeric(9, 6)");
            entity.Property(e => e.OutTimeLong).HasColumnType("numeric(9, 6)");

            entity.HasOne(d => d.OrganizationUnit).WithMany(p => p.TblAttendances)
                .HasForeignKey(d => d.OrganizationUnitId)
                .HasConstraintName("FK_Tbl_Attendance_Tbl_OrganizationUnit");
        });

        modelBuilder.Entity<TblBranchSurvey>(entity =>
        {
            entity.ToTable("Tbl_BranchSurvey", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AccessibilityByMotorCycle).HasMaxLength(10);
            entity.Property(e => e.AdministrativeStatus).HasMaxLength(30);
            entity.Property(e => e.AgriculturePercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ApproxPortfolio).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.AreaVisitedPhysically).HasMaxLength(10);
            entity.Property(e => e.Atms).HasColumnName("ATMs");
            entity.Property(e => e.AutoTotoAvailability).HasMaxLength(10);
            entity.Property(e => e.AverageFamilySize).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.BusConnectivity).HasMaxLength(100);
            entity.Property(e => e.BusConnectivityAvailable).HasMaxLength(10);
            entity.Property(e => e.BusinessRisk).HasMaxLength(20);
            entity.Property(e => e.CollectionRisk).HasMaxLength(20);
            entity.Property(e => e.CommunalIssuesRisk).HasMaxLength(20);
            entity.Property(e => e.CompetitionRisk).HasMaxLength(20);
            entity.Property(e => e.CompetitorVerificationCompleted).HasMaxLength(10);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.CycloneRisk).HasMaxLength(20);
            entity.Property(e => e.DigitalPaymentAcceptance).HasMaxLength(100);
            entity.Property(e => e.DistanceFromDistrictHeadquarters).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.DistanceFromExistingWeGrowBranch).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.DrinkingWaterAvailability).HasMaxLength(100);
            entity.Property(e => e.DroughtRisk).HasMaxLength(20);
            entity.Property(e => e.ElectricitySupply).HasMaxLength(100);
            entity.Property(e => e.EstimatedCollectionEfficiency).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.EstimatedLoanPortfolioPotential).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.EstimatedNumberOfJlgsCentres).HasColumnName("EstimatedNumberOfJLGsCentres");
            entity.Property(e => e.ExistingCustomersContacted).HasMaxLength(10);
            entity.Property(e => e.ExpectedMonthlyDisbursement).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.FemalePopulationPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.FloodRisk).HasMaxLength(20);
            entity.Property(e => e.FraudRisk).HasMaxLength(20);
            entity.Property(e => e.GeoTag).HasMaxLength(200);
            entity.Property(e => e.Gpsverified)
                .HasMaxLength(10)
                .HasColumnName("GPSVerified");
            entity.Property(e => e.InternetAvailability).HasMaxLength(100);
            entity.Property(e => e.Jlgpotential).HasColumnName("JLGPotential");
            entity.Property(e => e.LandslideRisk).HasMaxLength(20);
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Latitude).HasColumnType("decimal(10, 8)");
            entity.Property(e => e.LeanSeason).HasMaxLength(200);
            entity.Property(e => e.LiteracyRate).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.LocalReferencesVerified).HasMaxLength(10);
            entity.Property(e => e.Longitude).HasColumnType("decimal(11, 8)");
            entity.Property(e => e.MainCrop).HasMaxLength(200);
            entity.Property(e => e.MigrationRisk).HasMaxLength(20);
            entity.Property(e => e.MigrationTrend).HasMaxLength(100);
            entity.Property(e => e.MinorityPopulationPercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.MobileNetworkCoverage).HasMaxLength(100);
            entity.Property(e => e.MultipleLendingRisk).HasMaxLength(20);
            entity.Property(e => e.NameOfInstitution).HasMaxLength(200);
            entity.Property(e => e.NearestLandmark).HasMaxLength(200);
            entity.Property(e => e.OtherIncomeGeneratingActivities).HasMaxLength(500);
            entity.Property(e => e.OverallEconomicCondition).HasMaxLength(20);
            entity.Property(e => e.Parpercent)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("PARPercent");
            entity.Property(e => e.PeakBusinessSeason).HasMaxLength(200);
            entity.Property(e => e.PhotographsAttached).HasMaxLength(10);
            entity.Property(e => e.PinCode)
                .HasMaxLength(6)
                .IsUnicode(false);
            entity.Property(e => e.PoliticalDisturbanceRisk).HasMaxLength(20);
            entity.Property(e => e.PortfolioYear1).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PortfolioYear2).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PortfolioYear3).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProposedOperationalArea).HasMaxLength(200);
            entity.Property(e => e.PublicTransportAvailability).HasMaxLength(100);
            entity.Property(e => e.RailConnectivity).HasMaxLength(10);
            entity.Property(e => e.RailwayConnectivity).HasMaxLength(100);
            entity.Property(e => e.Recommendation).HasMaxLength(30);
            entity.Property(e => e.RoadAccessibility).HasMaxLength(10);
            entity.Property(e => e.RoadCondition).HasMaxLength(20);
            entity.Property(e => e.SafetyOfArea).HasMaxLength(100);
            entity.Property(e => e.ScheduledCastePercent).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ScheduledTribePercent).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.AdministrativeUnit).WithMany(p => p.TblBranchSurveys)
                .HasForeignKey(d => d.AdministrativeUnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_BranchSurvey_Tbl_AdministrativeUnit");

            entity.HasOne(d => d.Survey).WithMany(p => p.TblBranchSurveys)
                .HasForeignKey(d => d.SurveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_BranchSurvey_Tbl_Survey");
        });

        modelBuilder.Entity<TblCalenderYear>(entity =>
        {
            entity.ToTable("Tbl_CalenderYear", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Caption).HasMaxLength(20);
        });

        modelBuilder.Entity<TblCoBorrower>(entity =>
        {
            entity.ToTable("Tbl_CoBorrower", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AadharNo).HasMaxLength(12);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.FirstName).HasMaxLength(30);
            entity.Property(e => e.FullName)
                .HasMaxLength(92)
                .HasComputedColumnSql("((([FirstName]+' ')+isnull([MiddleName]+' ',''))+[LastName])", false);
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(30);
            entity.Property(e => e.MiddleName).HasMaxLength(30);
            entity.Property(e => e.MobileNo).HasMaxLength(20);

            entity.HasOne(d => d.Member).WithMany(p => p.TblCoBorrowers)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_CoBorrower_Tbl_Member");
        });

        modelBuilder.Entity<TblCompany>(entity =>
        {
            entity.ToTable("Tbl_Company", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<TblCompanyBankAccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Tbl_CompanyBankFinanceount");

            entity.ToTable("Tbl_CompanyBankAccount", "Finance");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AccountNo).HasMaxLength(20);
            entity.Property(e => e.BranchName).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.IfscCode).HasMaxLength(20);
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");

            entity.HasOne(d => d.FinancialInstitute).WithMany(p => p.TblCompanyBankAccounts)
                .HasForeignKey(d => d.FinancialInstituteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_CompanyBankAccount_Tbl_FinancialInstitute");

            entity.HasOne(d => d.Ledger).WithMany(p => p.TblCompanyBankAccounts)
                .HasForeignKey(d => d.LedgerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_CompanyBankAccount_Tbl_AccountLedger");
        });

        modelBuilder.Entity<TblCreditDataAccount>(entity =>
        {
            entity.ToTable("Tbl_CreditDataAccounts", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AccountNumber).HasMaxLength(50);
            entity.Property(e => e.CloseDate).HasColumnType("datetime");
            entity.Property(e => e.CurrentBalance).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.DisburseAmount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.InstallAmount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.Institution).HasMaxLength(200);
            entity.Property(e => e.KeyPersonName).HasMaxLength(100);
            entity.Property(e => e.KeyPersonRel).HasMaxLength(50);
            entity.Property(e => e.LastPayment).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.LastPaymentDate).HasColumnType("datetime");
            entity.Property(e => e.LoanCategory).HasMaxLength(100);
            entity.Property(e => e.LoanCycleId).HasMaxLength(50);
            entity.Property(e => e.LoanPurpose).HasMaxLength(100);
            entity.Property(e => e.NoOfActiveAcc).HasDefaultValue(0, "DF_Tbl_CreditDataAccounts_NoOfActiveAcc_1");
            entity.Property(e => e.NoOfClosedAcc).HasDefaultValue(0, "DF_Tbl_CreditDataAccounts_NoOfClosedAcc_1");
            entity.Property(e => e.NoOfPastDueAcc).HasDefaultValue(0, "DF_Tbl_CreditDataAccounts_NoOfPastDueAcc_1");
            entity.Property(e => e.NoOfWrittenOffAcc).HasDefaultValue(0, "DF_Tbl_CreditDataAccounts_NoOfWrittenOffAcc_1");
            entity.Property(e => e.NomineeName).HasMaxLength(100);
            entity.Property(e => e.NomineeRel).HasMaxLength(50);
            entity.Property(e => e.OpenDate).HasColumnType("datetime");
            entity.Property(e => e.PastDueAmount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.RepaymentFreq).HasMaxLength(50);
            entity.Property(e => e.ReportDate).HasColumnType("datetime");
            entity.Property(e => e.SanctionAmount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.SanctionDate).HasColumnType("datetime");
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.TotalAcc).HasDefaultValue(0, "DF_Tbl_CreditDataAccounts_TotalAcc_1");
            entity.Property(e => e.WriteoffAmount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.WriteoffDate).HasColumnType("datetime");
            entity.Property(e => e.WriteoffReason).HasMaxLength(200);

            entity.HasOne(d => d.CreditData).WithMany(p => p.TblCreditDataAccounts)
                .HasForeignKey(d => d.CreditDataId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_CreditDataAccounts_Tbl_CreditData");
        });

        modelBuilder.Entity<TblCreditDataAddress>(entity =>
        {
            entity.ToTable("Tbl_CreditDataAddress", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.ReportDate).HasColumnType("datetime");
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.CreditData).WithMany(p => p.TblCreditDataAddresses)
                .HasForeignKey(d => d.CreditDataId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_CreditDataAddress_Tbl_CreditData");
        });

        modelBuilder.Entity<TblCreditDataAlert>(entity =>
        {
            entity.ToTable("Tbl_CreditDataAlert", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AlertMsg).HasMaxLength(500);
            entity.Property(e => e.Sequence).ValueGeneratedOnAdd();

            entity.HasOne(d => d.CreditData).WithMany(p => p.TblCreditDataAlerts)
                .HasForeignKey(d => d.CreditDataId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_CreditDataAlert_Tbl_CreditData");
        });

        modelBuilder.Entity<TblCreditDataDependent>(entity =>
        {
            entity.ToTable("Tbl_CreditDataDependents", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Relation).HasMaxLength(50);

            entity.HasOne(d => d.CreditData).WithMany(p => p.TblCreditDataDependents)
                .HasForeignKey(d => d.CreditDataId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_CreditDataDependents_Tbl_CreditData");
        });

        modelBuilder.Entity<TblCreditDataIdentity>(entity =>
        {
            entity.ToTable("Tbl_CreditDataIdentities", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.IdentityNo).HasMaxLength(50);
            entity.Property(e => e.IdentityType).HasMaxLength(50);
            entity.Property(e => e.ReportDate).HasColumnType("datetime");

            entity.HasOne(d => d.CreditData).WithMany(p => p.TblCreditDataIdentities)
                .HasForeignKey(d => d.CreditDataId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_CreditDataIdentities_Tbl_CreditData");
        });

        modelBuilder.Entity<TblCreditDataIncome>(entity =>
        {
            entity.ToTable("Tbl_CreditDataIncomes", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AssetOwnership).HasMaxLength(50);
            entity.Property(e => e.MonthlyExpense).HasMaxLength(30);
            entity.Property(e => e.MonthlyIncome).HasMaxLength(30);
            entity.Property(e => e.Occupation).HasMaxLength(50);
            entity.Property(e => e.ReportDate).HasColumnType("datetime");

            entity.HasOne(d => d.CreditData).WithMany(p => p.TblCreditDataIncomes)
                .HasForeignKey(d => d.CreditDataId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_CreditDataIncomes_Tbl_CreditData");
        });

        modelBuilder.Entity<TblCreditDatum>(entity =>
        {
            entity.ToTable("Tbl_CreditData", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Age).HasMaxLength(10);
            entity.Property(e => e.ConsumerName).HasMaxLength(100);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DocFilename).HasMaxLength(100);
            entity.Property(e => e.DocumentId).HasMaxLength(50);
            entity.Property(e => e.ErrorCode).HasMaxLength(20);
            entity.Property(e => e.ErrorMessage).HasMaxLength(300);
            entity.Property(e => e.Gender).HasMaxLength(20);
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.MfiScore).HasDefaultValue(0, "DF_Tbl_CreditData_MfiScore");
            entity.Property(e => e.NoOfActiveAcc).HasDefaultValue(0, "DF_Tbl_CreditData_NoOfActiveAcc");
            entity.Property(e => e.NoOfClosedAcc).HasDefaultValue(0, "DF_Tbl_CreditData_NoOfClosedAcc");
            entity.Property(e => e.NoOfPastDueAcc).HasDefaultValue(0, "DF_Tbl_CreditData_NoOfPastDueAcc");
            entity.Property(e => e.NoOfWrittenOffAcc).HasDefaultValue(0, "DF_Tbl_CreditData_NoOfWrittenOffAcc");
            entity.Property(e => e.OrganizationName).HasMaxLength(100);
            entity.Property(e => e.TotalAcc).HasDefaultValue(0, "DF_Tbl_CreditData_TotalAcc");
            entity.Property(e => e.TotalBalanceAmount)
                .HasDefaultValue(0m, "DF_Tbl_CreditData_TotalBalanceAmount")
                .HasColumnType("numeric(18, 0)");
            entity.Property(e => e.TotalMonthlyInstAmount)
                .HasDefaultValue(0m, "DF_Tbl_CreditData_TotalMonthlyInstAmount")
                .HasColumnType("numeric(18, 2)");
            entity.Property(e => e.TotalPastDueAmount)
                .HasDefaultValue(0m, "DF_Tbl_CreditData_TotalPastDueAmount")
                .HasColumnType("numeric(18, 2)");
            entity.Property(e => e.TotalSanctionedAmount)
                .HasDefaultValue(0m, "DF_Tbl_CreditData_TotalSanctionedAmount")
                .HasColumnType("numeric(18, 2)");
            entity.Property(e => e.TotalWritoffAmount)
                .HasDefaultValue(0m, "DF_Tbl_CreditData_TotalWritoffAmount")
                .HasColumnType("numeric(18, 2)");
            entity.Property(e => e.XmlDocFileName).HasMaxLength(200);

            entity.HasOne(d => d.CoBorrower).WithMany(p => p.TblCreditData)
                .HasForeignKey(d => d.CoBorrowerId)
                .HasConstraintName("FK_Tbl_CreditData_Tbl_CoBorrower");

            entity.HasOne(d => d.CreditOrganization).WithMany(p => p.TblCreditData)
                .HasForeignKey(d => d.CreditOrganizationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_CreditData_Tbl_CreditOrganization");

            entity.HasOne(d => d.Member).WithMany(p => p.TblCreditData)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_Tbl_CreditData_Tbl_Member");
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

        modelBuilder.Entity<TblDesignationGradeMapping>(entity =>
        {
            entity.ToTable("Tbl_DesignationGradeMapping", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
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
            entity.Property(e => e.Esiipno)
                .HasMaxLength(20)
                .HasColumnName("ESIIPNo");
            entity.Property(e => e.FatherName).HasMaxLength(50);
            entity.Property(e => e.FirstName).HasMaxLength(30);
            entity.Property(e => e.FullName)
                .HasMaxLength(92)
                .HasComputedColumnSql("((([FirstName]+' ')+isnull([MiddleName]+' ',''))+[LastName])", false);
            entity.Property(e => e.Gender).HasMaxLength(10);
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
            entity.Property(e => e.UniversalAccountNo).HasMaxLength(50);

            entity.HasOne(d => d.Bank).WithMany(p => p.TblEmployees)
                .HasForeignKey(d => d.BankId)
                .HasConstraintName("FK_Tbl_Employee_Tbl_FinancialInstitute");

            entity.HasOne(d => d.Company).WithMany(p => p.TblEmployees)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK_Tbl_Employee_Tbl_Company");

            entity.HasOne(d => d.Department).WithMany(p => p.TblEmployees)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_Tbl_Employee_Tbl_Department");

            entity.HasOne(d => d.OrganizationUnit).WithMany(p => p.TblEmployees)
                .HasForeignKey(d => d.OrganizationUnitId)
                .HasConstraintName("FK_Tbl_Employee_Tbl_OrganizationUnit");

            entity.HasOne(d => d.PermanentAdministrativeUnit).WithMany(p => p.TblEmployeePermanentAdministrativeUnits)
                .HasForeignKey(d => d.PermanentAdministrativeUnitId)
                .HasConstraintName("FK_Tbl_Employee_Tbl_AdministrativeUnit");

            entity.HasOne(d => d.PresentAdministrativeUnit).WithMany(p => p.TblEmployeePresentAdministrativeUnits)
                .HasForeignKey(d => d.PresentAdministrativeUnitId)
                .HasConstraintName("FK_Tbl_Employee_Tbl_AdministrativeUnit1");
        });

        modelBuilder.Entity<TblEmployeeAttendanceException>(entity =>
        {
            entity.ToTable("Tbl_EmployeeAttendanceException", "Payroll");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ApprovedOn).HasColumnType("datetime");
            entity.Property(e => e.NegativeElcount).HasColumnName("NegativeELCount");
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

        modelBuilder.Entity<TblEmployeeGrossSalary>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Tbl_EmployeeGrossSalary", "Payroll");

            entity.Property(e => e.GrossSalary).HasColumnType("numeric(18, 2)");
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

        modelBuilder.Entity<TblGroup>(entity =>
        {
            entity.ToTable("Tbl_Group", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(500);
            entity.Property(e => e.Code)
                .HasMaxLength(4000)
                .HasComputedColumnSql("(format([BranchCode],'000')+format([CodeVal],'00000'))", false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.CurrentStatus)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.LandMark).HasMaxLength(100);
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Latitude).HasMaxLength(100);
            entity.Property(e => e.Longitude).HasMaxLength(100);
            entity.Property(e => e.MobileNo).HasMaxLength(15);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PhotoScan).HasMaxLength(500);
            entity.Property(e => e.PinCode)
                .HasMaxLength(6)
                .IsFixedLength();
            entity.Property(e => e.VerifiedOn).HasColumnType("datetime");
            entity.Property(e => e.VerifiedRemarks).HasMaxLength(200);

            entity.HasOne(d => d.AdministrativeUnit).WithMany(p => p.TblGroups)
                .HasForeignKey(d => d.AdministrativeUnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Group_Tbl_AdministrativeUnit");

            entity.HasOne(d => d.Designation).WithMany(p => p.TblGroups)
                .HasForeignKey(d => d.DesignationId)
                .HasConstraintName("FK_Tbl_Group_Tbl_Designation");
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

        modelBuilder.Entity<TblKycDocument>(entity =>
        {
            entity.ToTable("Tbl_KycDocument", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<TblLeaveEvent>(entity =>
        {
            entity.ToTable("Tbl_LeaveEvent", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.ApplicantStatusText).HasMaxLength(50);
            entity.Property(e => e.EventButtonText).HasMaxLength(50);
            entity.Property(e => e.EventStatusText).HasMaxLength(50);
            entity.Property(e => e.StatusColor).HasMaxLength(10);
        });

        modelBuilder.Entity<TblLeaveRequest>(entity =>
        {
            entity.ToTable("Tbl_LeaveRequest", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Address).HasMaxLength(100);
            entity.Property(e => e.Reason).HasMaxLength(500);
            entity.Property(e => e.RequestDate).HasColumnType("datetime");
            entity.Property(e => e.TotalDays).HasComputedColumnSql("(datediff(day,[FromDate],[ToDate])+(1))", false);

            entity.HasOne(d => d.CalenderYear).WithMany(p => p.TblLeaveRequests)
                .HasForeignKey(d => d.CalenderYearId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LeaveRequest_Tbl_CalenderYear");

            entity.HasOne(d => d.StatusNavigation).WithMany(p => p.TblLeaveRequests)
                .HasForeignKey(d => d.Status)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveRequest_Status");
        });

        modelBuilder.Entity<TblLeaveRequestDocument>(entity =>
        {
            entity.ToTable("Tbl_LeaveRequestDocuments", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DocumentFileNo).HasMaxLength(200);
            entity.Property(e => e.DocumentFileSeq).HasMaxLength(200);
            entity.Property(e => e.DocumentType).HasMaxLength(100);
            entity.Property(e => e.UploadedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<TblLeaveRequestEvent>(entity =>
        {
            entity.ToTable("Tbl_LeaveRequestEvent", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.EventDate).HasPrecision(0);
            entity.Property(e => e.Remarks).HasMaxLength(500);
        });

        modelBuilder.Entity<TblLeaveStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tbl_Leav__3214EC0775329299");

            entity.ToTable("Tbl_LeaveStatus", "Hrms");

            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<TblLeaveTransaction>(entity =>
        {
            entity.ToTable("Tbl_LeaveTransaction", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.LeaveCredit).HasColumnType("numeric(5, 3)");
            entity.Property(e => e.LeaveDebit).HasColumnType("numeric(5, 3)");
            entity.Property(e => e.Remarks).HasMaxLength(200);

            entity.HasOne(d => d.Employee).WithMany(p => p.TblLeaveTransactions)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LeaveTransaction_Tbl_Employee");

            entity.HasOne(d => d.LeaveType).WithMany(p => p.TblLeaveTransactions)
                .HasForeignKey(d => d.LeaveTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LeaveTransaction_Tbl_LeaveType");
        });

        modelBuilder.Entity<TblLeaveType>(entity =>
        {
            entity.ToTable("Tbl_LeaveType", "Hrms");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Alias).HasMaxLength(5);
            entity.Property(e => e.ConcurrencyStamp)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(50);
        });

        modelBuilder.Entity<TblLoanApplication>(entity =>
        {
            entity.ToTable("Tbl_LoanApplication", "Loan");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AdjustedAmount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.ApplicationAmount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.ApplicationNo).HasMaxLength(20);
            entity.Property(e => e.ApprovedFlatRate).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.ApprovedInterestRate).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.AprovedAmount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.BorCoBorRelation).HasMaxLength(50);
            entity.Property(e => e.BpiAmount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.CersaiCgstamount)
                .HasComputedColumnSql("(([CersaiCharge]*[CersaiCGSTPercent])/(100))", false)
                .HasColumnType("numeric(38, 6)")
                .HasColumnName("CersaiCGSTAmount");
            entity.Property(e => e.CersaiCgstpercent)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("CersaiCGSTPercent");
            entity.Property(e => e.CersaiCharge).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.CersaiSgstamount)
                .HasComputedColumnSql("(([CersaiCharge]*[CersaiSGSTPercent])/(100))", false)
                .HasColumnType("numeric(38, 6)")
                .HasColumnName("CersaiSGSTAmount");
            entity.Property(e => e.CersaiSgstpercent)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("CersaiSGSTPercent");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DisbursedAmount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.DisbursedOn).HasColumnType("datetime");
            entity.Property(e => e.DocumentCharge).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.DownloadFileName).HasMaxLength(30);
            entity.Property(e => e.EnachComplete).HasMaxLength(10);
            entity.Property(e => e.InspectionChargeAmount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.InspectionChargePercent).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.InstallmentInYear).HasColumnType("numeric(2, 1)");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.LoanApprovedDate).HasColumnType("datetime");
            entity.Property(e => e.LoginFees).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.MortalityCoverageAmount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.MortalityCoveragePercent).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.Neftno)
                .HasMaxLength(50)
                .HasColumnName("NEFTNo");
            entity.Property(e => e.OngikarnamaPhoto).HasMaxLength(500);
            entity.Property(e => e.PaymentSchedule)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.ProcessingFeeAmount)
                .HasComputedColumnSql("(([AprovedAmount]*[ProcessingFeePercent])/(100))", false)
                .HasColumnType("numeric(38, 6)");
            entity.Property(e => e.ProcessingFeeCgstamount)
                .HasComputedColumnSql("(((([AprovedAmount]*[ProcessingFeePercent])/(100))*[ProcessingFeeCGSTPercent])/(100))", false)
                .HasColumnType("numeric(38, 6)")
                .HasColumnName("ProcessingFeeCGSTAmount");
            entity.Property(e => e.ProcessingFeeCgstpercent)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("ProcessingFeeCGSTPercent");
            entity.Property(e => e.ProcessingFeeIgstamount)
                .HasComputedColumnSql("(((([AprovedAmount]*[ProcessingFeePercent])/(100))*[ProcessingFeeIGSTPercent])/(100))", false)
                .HasColumnType("numeric(38, 6)")
                .HasColumnName("ProcessingFeeIGSTAmount");
            entity.Property(e => e.ProcessingFeeIgstpercent)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("ProcessingFeeIGSTPercent");
            entity.Property(e => e.ProcessingFeePercent).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.ProcessingFeeSgstamount)
                .HasComputedColumnSql("(((([AprovedAmount]*[ProcessingFeePercent])/(100))*[ProcessingFeeSGSTPercent])/(100))", false)
                .HasColumnType("numeric(38, 6)")
                .HasColumnName("ProcessingFeeSGSTAmount");
            entity.Property(e => e.ProcessingFeeSgstpercent)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("ProcessingFeeSGSTPercent");
            entity.Property(e => e.ProposedEmi)
                .HasColumnType("numeric(18, 2)")
                .HasColumnName("ProposedEMI");
            entity.Property(e => e.Remarks).HasMaxLength(500);
            entity.Property(e => e.TotalCharges).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.TotalMortalityAmount)
                .HasComputedColumnSql("([MortalityCoverageAmount]*case when [CoBorrowerMortalityApplied]=(1) then (2) else (1) end)", false)
                .HasColumnType("numeric(29, 2)");
            entity.Property(e => e.UdyamRegNo).HasMaxLength(50);

            entity.HasOne(d => d.BorrowerAddress).WithMany(p => p.TblLoanApplicationBorrowerAddresses)
                .HasForeignKey(d => d.BorrowerAddressId)
                .HasConstraintName("FK_Tbl_LoanApplication_Tbl_MemberAddress");

            entity.HasOne(d => d.BorrowerCreditData).WithMany(p => p.TblLoanApplicationBorrowerCreditData)
                .HasForeignKey(d => d.BorrowerCreditDataId)
                .HasConstraintName("FK_Tbl_LoanApplication_Tbl_CreditData");

            entity.HasOne(d => d.Borrower).WithMany(p => p.TblLoanApplications)
                .HasForeignKey(d => d.BorrowerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanApplication_Tbl_Member");

            entity.HasOne(d => d.CoBorrowerAddress).WithMany(p => p.TblLoanApplicationCoBorrowerAddresses)
                .HasForeignKey(d => d.CoBorrowerAddressId)
                .HasConstraintName("FK_Tbl_LoanApplication_Tbl_MemberAddress1");

            entity.HasOne(d => d.CoBorrowerCreditData).WithMany(p => p.TblLoanApplicationCoBorrowerCreditData)
                .HasForeignKey(d => d.CoBorrowerCreditDataId)
                .HasConstraintName("FK_Tbl_LoanApplication_Tbl_CreditData1");

            entity.HasOne(d => d.CoBorrower).WithMany(p => p.TblLoanApplications)
                .HasForeignKey(d => d.CoBorrowerId)
                .HasConstraintName("FK_Tbl_LoanApplication_Tbl_CoBorrower");

            entity.HasOne(d => d.CompanyAccount).WithMany(p => p.TblLoanApplications)
                .HasForeignKey(d => d.CompanyAccountId)
                .HasConstraintName("FK_Tbl_LoanApplication_Tbl_CompanyBankAccount");

            entity.HasOne(d => d.Designation).WithMany(p => p.TblLoanApplications)
                .HasForeignKey(d => d.DesignationId)
                .HasConstraintName("FK_Tbl_LoanApplication_Tbl_Designation");

            entity.HasOne(d => d.Group).WithMany(p => p.TblLoanApplications)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanApplication_Tbl_Group");

            entity.HasOne(d => d.LoanProduct).WithMany(p => p.TblLoanApplications)
                .HasForeignKey(d => d.LoanProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanApplication_Tbl_LoanProduct");

            entity.HasOne(d => d.OrganizationUnit).WithMany(p => p.TblLoanApplications)
                .HasForeignKey(d => d.OrganizationUnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanApplication_Tbl_OrganizationUnit");
        });

        modelBuilder.Entity<TblLoanApplicationWorkflow>(entity =>
        {
            entity.ToTable("Tbl_LoanApplicationWorkflow", "Loan");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Otp).HasMaxLength(6);
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.WorkflowComment).HasMaxLength(500);

            entity.HasOne(d => d.Designation).WithMany(p => p.TblLoanApplicationWorkflows)
                .HasForeignKey(d => d.DesignationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanApplicationWorkflow_Tbl_Designation");

            entity.HasOne(d => d.LoanApplication).WithMany(p => p.TblLoanApplicationWorkflows)
                .HasForeignKey(d => d.LoanApplicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanApplicationWorkflow_Tbl_LoanApplication");

            entity.HasOne(d => d.User).WithMany(p => p.TblLoanApplicationWorkflows)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Tbl_LoanApplicationWorkflow_Tbl_User");

            entity.HasOne(d => d.Workflow).WithMany(p => p.TblLoanApplicationWorkflows)
                .HasForeignKey(d => d.WorkflowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanApplicationWorkflow_Tbl_LoanWorkflow");
        });

        modelBuilder.Entity<TblLoanApplicationWorkflowDocument>(entity =>
        {
            entity.ToTable("Tbl_LoanApplicationWorkflowDocument", "Loan");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.FileName).HasMaxLength(500);
            entity.Property(e => e.FileSequence).HasMaxLength(500);
            entity.Property(e => e.UploadOn).HasColumnType("datetime");

            entity.HasOne(d => d.Document).WithMany(p => p.TblLoanApplicationWorkflowDocuments)
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanApplicationWorkflowDocument_Tbl_ApplicatoinDocument");

            entity.HasOne(d => d.LoanApplicationWorkflow).WithMany(p => p.TblLoanApplicationWorkflowDocuments)
                .HasForeignKey(d => d.LoanApplicationWorkflowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanApplicationWorkflowDocument_Tbl_LoanApplicationWorkflow");
        });

        modelBuilder.Entity<TblLoanApplicationWorkflowStep>(entity =>
        {
            entity.ToTable("Tbl_LoanApplicationWorkflowStep", "Loan");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CompletedOn).HasColumnType("datetime");

            entity.HasOne(d => d.ApplicationStep).WithMany(p => p.TblLoanApplicationWorkflowSteps)
                .HasForeignKey(d => d.ApplicationStepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanApplicationWorkflowStep_Tbl_ApplicationStep");

            entity.HasOne(d => d.LoanApplicationWorkflow).WithMany(p => p.TblLoanApplicationWorkflowSteps)
                .HasForeignKey(d => d.LoanApplicationWorkflowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanApplicationWorkflowStep_Tbl_LoanApplicationWorkflow");
        });

        modelBuilder.Entity<TblLoanProduct>(entity =>
        {
            entity.ToTable("Tbl_LoanProduct", "Loan");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Alias).HasMaxLength(20);
            entity.Property(e => e.BpiGlaccount).HasColumnName("BpiGLAccount");
            entity.Property(e => e.CbinspectionGlaccount).HasColumnName("CBInspectionGLAccount");
            entity.Property(e => e.CersaiGlaccount).HasColumnName("CersaiGLAccount");
            entity.Property(e => e.Cgstglaccount).HasColumnName("CGSTGLAccount");
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DocumentGlaccount).HasColumnName("DocumentGLAccount");
            entity.Property(e => e.Igstglaccount).HasColumnName("IGSTGLAccount");
            entity.Property(e => e.InsuranceGlaccount).HasColumnName("InsuranceGLAccount");
            entity.Property(e => e.InterestGlaccount).HasColumnName("InterestGLAccount");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.LoanRecoveryGlaccount).HasColumnName("LoanRecoveryGLAccount");
            entity.Property(e => e.LoanWriteOffGlaccount).HasColumnName("LoanWriteOffGLAccount");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PaymentSchedule)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.PrincipalGlaccount).HasColumnName("PrincipalGLAccount");
            entity.Property(e => e.ProcessingFeesGlaccount).HasColumnName("ProcessingFeesGLAccount");
            entity.Property(e => e.ProductCode).HasMaxLength(50);
            entity.Property(e => e.RoundoffGlaccount).HasColumnName("RoundoffGLAccount");
            entity.Property(e => e.Sgstglaccount).HasColumnName("SGSTGLAccount");

            entity.HasOne(d => d.AccountGroup).WithMany(p => p.TblLoanProducts)
                .HasForeignKey(d => d.AccountGroupId)
                .HasConstraintName("FK_Tbl_LoanProduct_Tbl_AccountGroup");
        });

        modelBuilder.Entity<TblLoanProductQuestion>(entity =>
        {
            entity.HasKey(e => new { e.LoanProductId, e.QuestionId, e.PostId });

            entity.ToTable("Tbl_LoanProductQuestion", "Loan");

            entity.HasOne(d => d.LoanProduct).WithMany(p => p.TblLoanProductQuestions)
                .HasForeignKey(d => d.LoanProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanProductQuestion_Tbl_LoanProduct");

            entity.HasOne(d => d.Post).WithMany(p => p.TblLoanProductQuestions)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanProductHVQuestion_Tbl_Post");

            entity.HasOne(d => d.Question).WithMany(p => p.TblLoanProductQuestions)
                .HasForeignKey(d => d.QuestionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanProductQuestion_Tbl_HouseVisitQuestion");
        });

        modelBuilder.Entity<TblLoanProductStep>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Tbl_LoanProductApplicationStep");

            entity.ToTable("Tbl_LoanProductStep", "Loan");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.ApplicationStep).WithMany(p => p.TblLoanProductSteps)
                .HasForeignKey(d => d.ApplicationStepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanProductApplicationStep_Tbl_ApplicationStep");

            entity.HasOne(d => d.LoanProduct).WithMany(p => p.TblLoanProductSteps)
                .HasForeignKey(d => d.LoanProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanProductStep_Tbl_LoanProduct");
        });

        modelBuilder.Entity<TblLoanSettingParameter>(entity =>
        {
            entity.ToTable("Tbl_LoanSettingParameter", "Loan");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CersaiCgstpercent)
                .HasColumnType("numeric(9, 2)")
                .HasColumnName("CersaiCGSTPercent");
            entity.Property(e => e.CersaiCharge).HasColumnType("numeric(9, 2)");
            entity.Property(e => e.CersaiSgstpercent)
                .HasColumnType("numeric(9, 2)")
                .HasColumnName("CersaiSGSTPercent");
            entity.Property(e => e.DocumentCharge).HasColumnType("numeric(9, 2)");
            entity.Property(e => e.Factor).HasMaxLength(50);
            entity.Property(e => e.InspectionChargePercent).HasColumnType("numeric(9, 2)");
            entity.Property(e => e.InstallmentPeriod).HasMaxLength(200);
            entity.Property(e => e.MaxInterestRate).HasColumnType("numeric(9, 2)");
            entity.Property(e => e.MaxLoanAmount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.MaxLoginFees)
                .HasDefaultValue(0m, "DF_Tbl_LoanSettingParameter_MaxLoginFees")
                .HasColumnType("numeric(18, 2)");
            entity.Property(e => e.MinInterestRate).HasColumnType("numeric(9, 2)");
            entity.Property(e => e.MinLoanAmount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.MinLoginFees)
                .HasDefaultValue(0m, "DF_Tbl_LoanSettingParameter_MinLoginFees")
                .HasColumnType("numeric(18, 2)");
            entity.Property(e => e.MortalityCoveragePercent).HasColumnType("numeric(9, 2)");
            entity.Property(e => e.ProcessingFeeCgstpercent)
                .HasColumnType("numeric(9, 2)")
                .HasColumnName("ProcessingFeeCGSTPercent");
            entity.Property(e => e.ProcessingFeeIgstpercent)
                .HasColumnType("numeric(9, 2)")
                .HasColumnName("ProcessingFeeIGSTPercent");
            entity.Property(e => e.ProcessingFeePercent).HasColumnType("numeric(18, 4)");
            entity.Property(e => e.ProcessingFeeSgstpercent)
                .HasColumnType("numeric(9, 2)")
                .HasColumnName("ProcessingFeeSGSTPercent");

            entity.HasOne(d => d.LoanProduct).WithMany(p => p.TblLoanSettingParameters)
                .HasForeignKey(d => d.LoanProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanSettingParameter_Tbl_LoanProduct");
        });

        modelBuilder.Entity<TblLoanWorkflow>(entity =>
        {
            entity.ToTable("Tbl_LoanWorkflow", "Loan");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.WorkflowStatus).HasMaxLength(100);

            entity.HasOne(d => d.LoanProduct).WithMany(p => p.TblLoanWorkflows)
                .HasForeignKey(d => d.LoanProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanWorkflow_Tbl_LoanProduct");

            entity.HasOne(d => d.Post).WithMany(p => p.TblLoanWorkflows)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanWorkflow_Tbl_Post");
        });

        modelBuilder.Entity<TblLoanWorkflowDocument>(entity =>
        {
            entity.ToTable("Tbl_LoanWorkflowDocument", "Loan");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Document).WithMany(p => p.TblLoanWorkflowDocuments)
                .HasForeignKey(d => d.DocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanWorkflowDocument_Tbl_ApplicatoinDocument");

            entity.HasOne(d => d.Workflow).WithMany(p => p.TblLoanWorkflowDocuments)
                .HasForeignKey(d => d.WorkflowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanWorkflowDocument_Tbl_LoanWorkflow");
        });

        modelBuilder.Entity<TblLoanWorkflowStep>(entity =>
        {
            entity.ToTable("Tbl_LoanWorkflowStep", "Loan");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.ApplicationStep).WithMany(p => p.TblLoanWorkflowSteps)
                .HasForeignKey(d => d.ApplicationStepId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanWorkflowStep_Tbl_ApplicationStep");

            entity.HasOne(d => d.Workflow).WithMany(p => p.TblLoanWorkflowSteps)
                .HasForeignKey(d => d.WorkflowId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_LoanWorkflowStep_Tbl_LoanWorkflow");
        });

        modelBuilder.Entity<TblMember>(entity =>
        {
            entity.ToTable("Tbl_Member", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AadharNo).HasMaxLength(12);
            entity.Property(e => e.Caste)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.CkycNumber).HasMaxLength(50);
            entity.Property(e => e.Code)
                .HasMaxLength(4000)
                .HasComputedColumnSql("(format([BranchCode],'000')+format([CodeVal],'000000'))", false);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.FirstName).HasMaxLength(30);
            entity.Property(e => e.FullName)
                .HasMaxLength(104)
                .HasComputedColumnSql("((isnull([Salutaion]+' ','')+(([FirstName]+' ')+isnull([MiddleName]+' ','')))+isnull([LastName]+' ',''))", false);
            entity.Property(e => e.Gender)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GuardianName).HasMaxLength(200);
            entity.Property(e => e.GuardianRelation).HasMaxLength(50);
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.LastName).HasMaxLength(30);
            entity.Property(e => e.MaritalStatus)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.MiddleName).HasMaxLength(30);
            entity.Property(e => e.MobileNo).HasMaxLength(20);
            entity.Property(e => e.MotherName).HasMaxLength(200);
            entity.Property(e => e.Occupation)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.PanNo).HasMaxLength(10);
            entity.Property(e => e.Qualification)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Religion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Salutaion).HasMaxLength(10);
            entity.Property(e => e.SpouseName).HasMaxLength(100);

            entity.HasOne(d => d.Designation).WithMany(p => p.TblMembers)
                .HasForeignKey(d => d.DesignationId)
                .HasConstraintName("FK_Tbl_Member_Tbl_Designation");

            entity.HasOne(d => d.Group).WithMany(p => p.TblMembers)
                .HasForeignKey(d => d.GroupId)
                .HasConstraintName("FK_Tbl_Member_Tbl_Group");
        });

        modelBuilder.Entity<TblMemberAddress>(entity =>
        {
            entity.ToTable("Tbl_MemberAddress", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.HouseNo).HasMaxLength(100);
            entity.Property(e => e.Latitude).HasColumnType("numeric(18, 16)");
            entity.Property(e => e.Longitude).HasColumnType("numeric(18, 16)");
            entity.Property(e => e.NearestLandmark).HasMaxLength(100);
            entity.Property(e => e.PinCode).HasMaxLength(10);
            entity.Property(e => e.PostOffice).HasMaxLength(100);
            entity.Property(e => e.Sequence).ValueGeneratedOnAdd();
            entity.Property(e => e.StreetName).HasMaxLength(100);

            entity.HasOne(d => d.AdministrativeUnit).WithMany(p => p.TblMemberAddresses)
                .HasForeignKey(d => d.AdministrativeUnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_MemberAddress_Tbl_AdministrativeUnit");

            entity.HasOne(d => d.Member).WithMany(p => p.TblMemberAddresses)
                .HasForeignKey(d => d.MemberId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_MemberAddress_Tbl_Member");

            entity.HasOne(d => d.PoliceStation).WithMany(p => p.TblMemberAddresses)
                .HasForeignKey(d => d.PoliceStationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_MemberAddress_Tbl_PoliceStation");
        });

        modelBuilder.Entity<TblMemberDocument>(entity =>
        {
            entity.ToTable("Tbl_MemberDocument", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.BackSideFileName).HasMaxLength(200);
            entity.Property(e => e.BackSideFileSequence).HasMaxLength(200);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.DocumentNo).HasMaxLength(50);
            entity.Property(e => e.FontSideFileName).HasMaxLength(200);
            entity.Property(e => e.FontSideFileSequence).HasMaxLength(200);
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Remarks).HasMaxLength(200);

            entity.HasOne(d => d.CoBorrower).WithMany(p => p.TblMemberDocuments)
                .HasForeignKey(d => d.CoBorrowerId)
                .HasConstraintName("FK_Tbl_MemberDocument_Tbl_CoBorrower");

            entity.HasOne(d => d.KycDocument).WithMany(p => p.TblMemberDocuments)
                .HasForeignKey(d => d.KycDocumentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_MemberDocument_Tbl_KycDocument");

            entity.HasOne(d => d.Member).WithMany(p => p.TblMemberDocuments)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK_Tbl_MemberDocument_Tbl_Member");
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

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblMenuCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Menu_Tbl_User");

            entity.HasOne(d => d.LastModifiedByNavigation).WithMany(p => p.TblMenuLastModifiedByNavigations)
                .HasForeignKey(d => d.LastModifiedBy)
                .HasConstraintName("FK_Tbl_Menu_Tbl_User1");

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

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblOrganizationUnitCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_OrganizationUnit_Tbl_User");

            entity.HasOne(d => d.LastModifiedByNavigation).WithMany(p => p.TblOrganizationUnitLastModifiedByNavigations)
                .HasForeignKey(d => d.LastModifiedBy)
                .HasConstraintName("FK_Tbl_OrganizationUnit_Tbl_User1");

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
            entity.Property(e => e.Icon).HasMaxLength(30);
            entity.Property(e => e.Name).HasMaxLength(30);
        });

        modelBuilder.Entity<TblPayRoll>(entity =>
        {
            entity.ToTable("Tbl_PayRoll", "Payroll");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Company).WithMany(p => p.TblPayRolls)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_PayRoll_Tbl_Company");

            entity.HasOne(d => d.Designation).WithMany(p => p.TblPayRolls)
                .HasForeignKey(d => d.DesignationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_PayRoll_Tbl_Designation");

            entity.HasOne(d => d.Employee).WithMany(p => p.TblPayRolls)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_PayRoll_Tbl_Employee");

            entity.HasOne(d => d.FinYear).WithMany(p => p.TblPayRolls)
                .HasForeignKey(d => d.FinYearId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Fk_Tbl_FinancialYear_Tbl_Payroll");

            entity.HasOne(d => d.OrganizationUnit).WithMany(p => p.TblPayRolls)
                .HasForeignKey(d => d.OrganizationUnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_PayRoll_Tbl_OrganizationUnit");
        });

        modelBuilder.Entity<TblPayRollComponent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Tbl_PayrollComponent");

            entity.ToTable("Tbl_PayRollComponent", "Payroll");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("numeric(18, 2)");
            entity.Property(e => e.Percentage).HasColumnType("numeric(9, 2)");

            entity.HasOne(d => d.PayRoll).WithMany(p => p.TblPayRollComponents)
                .HasForeignKey(d => d.PayRollId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_PayRollComponent_Tbl_PayRoll");

            entity.HasOne(d => d.SalaryComponent).WithMany(p => p.TblPayRollComponents)
                .HasForeignKey(d => d.SalaryComponentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_PayRollComponent_Tbl_SalaryComponent");
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

        modelBuilder.Entity<TblQuestion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Tbl_HouseVisitQuestion");

            entity.ToTable("Tbl_Question", "Loan");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AnswerType).HasMaxLength(50);
            entity.Property(e => e.Category).HasMaxLength(30);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
        });

        modelBuilder.Entity<TblRole>(entity =>
        {
            entity.ToTable("Tbl_Role", "Auth");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(30);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TblRoleCreatedByNavigations)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Role_Tbl_User");

            entity.HasOne(d => d.LastModifiedByNavigation).WithMany(p => p.TblRoleLastModifiedByNavigations)
                .HasForeignKey(d => d.LastModifiedBy)
                .HasConstraintName("FK_Tbl_Role_Tbl_User1");
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

        modelBuilder.Entity<TblSurvey>(entity =>
        {
            entity.ToTable("Tbl_Survey", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AssignmentNo).HasMaxLength(50);
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");
            entity.Property(e => e.ProposedArea).HasMaxLength(200);
            entity.Property(e => e.Sequence).ValueGeneratedOnAdd();
            entity.Property(e => e.SurveyType).HasMaxLength(50);

            entity.HasOne(d => d.AdministrativeUnit).WithMany(p => p.TblSurveys)
                .HasForeignKey(d => d.AdministrativeUnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_Survey_Tbl_AdministrativeUnit");
        });

        modelBuilder.Entity<TblSurveyAssigned>(entity =>
        {
            entity.ToTable("Tbl_SurveyAssigned", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.AssignedDate).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.TblSurveyAssigneds)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_SurveyAssigned_Tbl_Employee");

            entity.HasOne(d => d.Survey).WithMany(p => p.TblSurveyAssigneds)
                .HasForeignKey(d => d.SurveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_SurveyAssigned_Tbl_Survey");
        });

        modelBuilder.Entity<TblSurveyCommittee>(entity =>
        {
            entity.ToTable("Tbl_SurveyCommittee", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CreatedOn).HasColumnType("datetime");
            entity.Property(e => e.LastModifiedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Employee).WithMany(p => p.TblSurveyCommittees)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_SurveyCommittee_Tbl_Employee");
        });

        modelBuilder.Entity<TblSurveyCommitteeEvaluation>(entity =>
        {
            entity.ToTable("Tbl_SurveyCommitteeEvaluation", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Comments).HasMaxLength(500);
            entity.Property(e => e.EvaluationDate).HasColumnType("datetime");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.Employee).WithMany(p => p.TblSurveyCommitteeEvaluations)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_SurveyCommitteeEvaluation_Tbl_Employee");

            entity.HasOne(d => d.Survey).WithMany(p => p.TblSurveyCommitteeEvaluations)
                .HasForeignKey(d => d.SurveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_SurveyCommitteeEvaluation_Tbl_Survey");
        });

        modelBuilder.Entity<TblSurveyDocument>(entity =>
        {
            entity.ToTable("Tbl_SurveyDocument", "Global");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.FileName).HasMaxLength(300);
            entity.Property(e => e.FileSequence).HasMaxLength(300);
            entity.Property(e => e.Sequence).ValueGeneratedOnAdd();

            entity.HasOne(d => d.Survey).WithMany(p => p.TblSurveyDocuments)
                .HasForeignKey(d => d.SurveyId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tbl_SurveyDocument_Tbl_Survey");
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
