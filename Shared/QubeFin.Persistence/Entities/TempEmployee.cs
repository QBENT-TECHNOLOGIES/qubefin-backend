using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class TempEmployee
{
    public string? CompanyId { get; set; }

    public string? EmpCode { get; set; }

    public string? OldCode { get; set; }

    public string? Title { get; set; }

    public string? Name { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? FatherName { get; set; }

    public string? Gender { get; set; }

    public string? MaritalStatus { get; set; }

    public DateOnly? DateOfJoining { get; set; }

    public DateOnly? TransferDate { get; set; }

    public DateOnly? ConfirmationDate { get; set; }

    public DateOnly? DateOfLeaving { get; set; }

    public DateOnly? LastWorkingDate { get; set; }

    public DateOnly? RetirementDate { get; set; }

    public string? EmailId { get; set; }

    public string? MobileNo { get; set; }

    public string? Status { get; set; }

    public string? PfNo { get; set; }

    public string? UanNo { get; set; }

    public string? EsicNo { get; set; }

    public string? Pran { get; set; }

    public string? Pan { get; set; }

    public string? AadharNo { get; set; }

    public string? City { get; set; }

    public string? SuperannuationId { get; set; }

    public DateOnly? StopPaymentFromDate { get; set; }

    public DateOnly? StopPaymentTillDate { get; set; }

    public string? BankName { get; set; }

    public string? BankBranchCode { get; set; }

    public string? IfscCode { get; set; }

    public string? BankAccountNo { get; set; }

    public string? ModeOfPayment { get; set; }

    public string? Area { get; set; }

    public string? Company { get; set; }

    public string? Department { get; set; }

    public string? Designation { get; set; }

    public string? Grade { get; set; }

    public string? Location { get; set; }

    public string? PostId { get; set; }

    public string? RoleId { get; set; }

    public decimal? BasicSalary { get; set; }

    public decimal? BonusCtc { get; set; }

    public decimal? Bonus { get; set; }

    public decimal? BonusProvision { get; set; }

    public decimal? CanteenDeduction { get; set; }

    public decimal? ClaimExpensePayment { get; set; }

    public decimal? ConveyanceAllowance { get; set; }

    public decimal? CtcTotal { get; set; }

    public string? Da { get; set; }

    public decimal? DriverReimbursement { get; set; }

    public decimal? EarlyGoingDeduction { get; set; }

    public decimal? EducationAllowance { get; set; }

    public decimal? EmployeeEsiCtc { get; set; }

    public decimal? EmployeePfCtc { get; set; }

    public decimal? EmployerEdliCtc { get; set; }

    public decimal? EmployerEsiCtc { get; set; }

    public decimal? EmployerPfAdminChargesCtc { get; set; }

    public decimal? EmployerPfCtc { get; set; }

    public decimal? EpsContributionManual { get; set; }

    public decimal? EpsContributionArrearsManual { get; set; }

    public decimal? EsicEmployeeContributionManual { get; set; }

    public decimal? EsicEmployeeContributionArrearsManual { get; set; }

    public decimal? ExGratia { get; set; }

    public decimal? ExgratiaProvision { get; set; }

    public decimal? FuelReimbursement { get; set; }

    public decimal? GratuityContributionCtc { get; set; }

    public decimal? GratuityAmountManual { get; set; }

    public decimal? GratuityProvision { get; set; }

    public decimal? HouseRentAllowance { get; set; }

    public decimal? HraPercentage { get; set; }

    public decimal? LateComingDeduction { get; set; }

    public string? LeaveEncashment { get; set; }

    public decimal? LeaveTravelAllowance { get; set; }

    public decimal? LtaReimbursement { get; set; }

    public string? LwfManual { get; set; }

    public decimal? MedicalAllowance { get; set; }

    public decimal? MedicalReimbursement { get; set; }

    public decimal? MinimumWage { get; set; }

    public decimal? MonthlyGross { get; set; }

    public decimal? MpfContributionManual { get; set; }

    public decimal? MpfContributionArrearsManual { get; set; }

    public decimal? NetPayCtc { get; set; }

    public decimal? OtherAllowance { get; set; }

    public decimal? OtherDeduction { get; set; }

    public string? OtherIncome { get; set; }

    public decimal? PrevMonthRoundOffRecovery { get; set; }

    public decimal? ProfTaxCtc { get; set; }

    public decimal? ProfTaxManual { get; set; }

    public decimal? SalaryAdvance { get; set; }

    public decimal? SpecialAllowance { get; set; }

    public string? TdsManual { get; set; }

    public decimal? TravelExpensePayment { get; set; }

    public decimal? VpfContributionAmount { get; set; }

    public decimal? VpfContributionArrearsAmount { get; set; }

    public decimal? VpfContribution { get; set; }

    public decimal? DaysInMonth { get; set; }

    public decimal? WorkableDays { get; set; }

    public decimal? DaysWorked { get; set; }

    public decimal? Lop { get; set; }

    public decimal? ArrearDays { get; set; }

    public decimal? LateLop { get; set; }

    public decimal? OvertimeHours { get; set; }

    public decimal? ArrearOvertimeHours { get; set; }

    public string? Currency { get; set; }

    public decimal? Gratuity { get; set; }

    public decimal? BasicSalary2 { get; set; }

    public string? Da2 { get; set; }

    public decimal? HouseRentAllowance2 { get; set; }

    public decimal? ConveyanceAllowance2 { get; set; }

    public decimal? SpecialAllowance2 { get; set; }

    public decimal? MedicalAllowance2 { get; set; }

    public decimal? EducationAllowance2 { get; set; }

    public decimal? LeaveTravelAllowance2 { get; set; }

    public decimal? OtherAllowance2 { get; set; }

    public decimal? ExGratia2 { get; set; }

    public decimal? Bonus2 { get; set; }

    public string? LeaveEncashment2 { get; set; }

    public string? OtherIncome2 { get; set; }

    public decimal? LateComingDeduction2 { get; set; }

    public decimal? EarlyGoingDeduction2 { get; set; }

    public decimal? LtaReimbursement2 { get; set; }

    public decimal? MedicalReimbursement2 { get; set; }

    public decimal? FuelReimbursement2 { get; set; }

    public decimal? DriverReimbursement2 { get; set; }

    public decimal? TravelExpensePayment2 { get; set; }

    public decimal? ClaimExpensePayment2 { get; set; }

    public decimal? BonusCurrentFy { get; set; }

    public decimal? ExgratiaCurrentFy { get; set; }

    public decimal? BonusPreviousFy { get; set; }

    public decimal? ExgratiaPreviousFy { get; set; }

    public decimal? TotalEarning { get; set; }

    public decimal? ArrearEarlyGoingLop { get; set; }

    public decimal? ArrearLateComingLop { get; set; }

    public decimal? ArrearOther { get; set; }

    public decimal? ArrearOvertime { get; set; }

    public decimal? ArrearPaidHolidays { get; set; }

    public decimal? BasicSalaryArrears { get; set; }

    public decimal? DaArrears { get; set; }

    public decimal? HouseRentAllowanceArrears { get; set; }

    public decimal? ConveyanceAllowanceArrears { get; set; }

    public decimal? SpecialAllowanceArrears { get; set; }

    public decimal? MedicalAllowanceArrears { get; set; }

    public decimal? EducationAllowanceArrears { get; set; }

    public decimal? LeaveTravelAllowanceArrears { get; set; }

    public decimal? OtherAllowanceArrears { get; set; }

    public decimal? ExGratiaArrears { get; set; }

    public decimal? BonusArrears { get; set; }

    public decimal? LeaveEncashmentArrears { get; set; }

    public decimal? OtherIncomeArrears { get; set; }

    public decimal? TravelExpensePaymentArrears { get; set; }

    public decimal? ClaimExpensePaymentArrears { get; set; }

    public decimal? LateComingDeductionArrears { get; set; }

    public decimal? EarlyGoingDeductionArrears { get; set; }

    public decimal? LtaReimbursementArrears { get; set; }

    public decimal? MedicalReimbursementArrears { get; set; }

    public decimal? FuelReimbursementArrears { get; set; }

    public decimal? DriverReimbursementArrears { get; set; }

    public decimal? TotalArrear { get; set; }

    public decimal? GrossSalary { get; set; }

    public decimal? CanteenDeduction2 { get; set; }

    public decimal? SalaryAdvance2 { get; set; }

    public decimal? OtherDeduction2 { get; set; }

    public decimal? PrevMonthRoundOffRecovery2 { get; set; }

    public decimal? ProvidentFund { get; set; }

    public decimal? ProvidentFundArrear { get; set; }

    public decimal? AbryBenefits { get; set; }

    public decimal? VoluntaryProvidentFund { get; set; }

    public decimal? VoluntaryProvidentFundArrear { get; set; }

    public decimal? EmployeeStateInsurance { get; set; }

    public decimal? EsiArrears { get; set; }

    public decimal? ProfTax { get; set; }

    public decimal? IncomeTax { get; set; }

    public decimal? TotalDeduction { get; set; }

    public decimal? RoundingOff { get; set; }

    public decimal? NetPay { get; set; }

    public decimal? EpsArrear { get; set; }

    public decimal? EmployeePensionScheme { get; set; }

    public decimal? CompanyProvidentFund { get; set; }

    public decimal? CompanyProvidentFundArrear { get; set; }

    public decimal? EmployerContributionEsi { get; set; }

    public decimal? EmployerContributionEsiArrears { get; set; }

    public decimal? EdliContribution { get; set; }

    public decimal? EdliAdminCharges { get; set; }

    public decimal? AC2PfAdminCharges { get; set; }

    public decimal? CessForThePeriod { get; set; }

    public decimal? RawtaxForThePeriod { get; set; }

    public decimal? SurchargeForThePeriod { get; set; }

    public decimal? BonusProvision2 { get; set; }

    public decimal? GratuityProvision2 { get; set; }

    public decimal? LwfEmployerContribution { get; set; }

    public decimal? ExgratiaProvision2 { get; set; }

    public string? AdhocRemark { get; set; }
}
