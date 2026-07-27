using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class WegrowConsolidateEmployee
{
    public string? Company { get; set; }

    public string? EmpCode { get; set; }

    public string? OldCode { get; set; }

    public string? Title { get; set; }

    public string? Name { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? FatherName { get; set; }

    public string? Gender { get; set; }

    public string? MaritalStatus { get; set; }

    public DateOnly? DateOfJoining { get; set; }

    public string? TransferDate { get; set; }

    public DateOnly? ConfirmationDate { get; set; }

    public int? DateOfLeaving { get; set; }

    public int? LastWorkingDate { get; set; }

    public int? RetirementDate { get; set; }

    public string? EmailId { get; set; }

    public long? MobileNo { get; set; }

    public string? Status { get; set; }

    public string? Location { get; set; }

    public string? PfNo { get; set; }

    public long? UanNo { get; set; }

    public long? EsicNo { get; set; }

    public string? Pran { get; set; }

    public string? Pan { get; set; }

    public string? AadharNo { get; set; }

    public string? City { get; set; }

    public string? SuperannuationId { get; set; }

    public string? StopPaymentFromDate { get; set; }

    public string? StopPaymentTillDate { get; set; }

    public string? BankName { get; set; }

    public string? BankBranchCode { get; set; }

    public string? IfscCode { get; set; }

    public long? BankAccountNo { get; set; }

    public string? ModeOfPayment { get; set; }

    public string? Area { get; set; }

    public string? Company2 { get; set; }

    public string? Department { get; set; }

    public string? Designation { get; set; }

    public string? Grade { get; set; }

    public string? Location2 { get; set; }

    public int? BasicSalary { get; set; }

    public int? BonusCtc { get; set; }

    public string? Bonus { get; set; }

    public string? BonusProvision { get; set; }

    public string? CanteenDeduction { get; set; }

    public string? ClaimExpensePayment { get; set; }

    public short? ConveyanceAllowance { get; set; }

    public int? CtcTotal { get; set; }

    public string? Da { get; set; }

    public string? DriverReimbursement { get; set; }

    public string? EarlyGoingDeduction { get; set; }

    public string? EducationAllowance { get; set; }

    public byte? EmployeeEsiCtc { get; set; }

    public double? EmployeePfCtc { get; set; }

    public byte? EmployerEdliCtc { get; set; }

    public short? EmployerEsiCtc { get; set; }

    public byte? EmployerPfAdminChargesCtc { get; set; }

    public double? EmployerPfCtc { get; set; }

    public string? EpsContributionManual { get; set; }

    public byte? EpsContributionArrearsManual { get; set; }

    public string? EsicEmployeeContributionManual { get; set; }

    public string? EsicEmployeeContributionArrearsManual { get; set; }

    public string? ExGratia { get; set; }

    public string? ExgratiaProvision { get; set; }

    public string? FuelReimbursement { get; set; }

    public short? GratuityContributionCtc { get; set; }

    public string? GratuityAmountManual { get; set; }

    public string? GratuityProvision { get; set; }

    public int? HouseRentAllowance { get; set; }

    public string? HraPercentage { get; set; }

    public string? LateComingDeduction { get; set; }

    public string? LeaveEncashment { get; set; }

    public string? LeaveTravelAllowance { get; set; }

    public string? LtaReimbursement { get; set; }

    public string? LwfManual { get; set; }

    public short? MedicalAllowance { get; set; }

    public string? MedicalReimbursement { get; set; }

    public string? MinimumWage { get; set; }

    public int? MonthlyGross { get; set; }

    public string? MpfContributionManual { get; set; }

    public string? MpfContributionArrearsManual { get; set; }

    public int? NetPayCtc { get; set; }

    public byte? OtherAllowance { get; set; }

    public string? OtherDeduction { get; set; }

    public string? OtherIncome { get; set; }

    public string? PrevMonthRoundOffRecovery { get; set; }

    public byte? ProfTaxCtc { get; set; }

    public string? ProfTaxManual { get; set; }

    public string? SalaryAdvance { get; set; }

    public string? SpecialAllowance { get; set; }

    public string? TdsManual { get; set; }

    public string? TravelExpensePayment { get; set; }

    public string? VpfContributionAmount { get; set; }

    public string? VpfContributionArrearsAmount { get; set; }

    public string? VpfContribution { get; set; }

    public byte? DaysInMonth { get; set; }

    public byte? WorkableDays { get; set; }

    public short? DaysWorked { get; set; }

    public byte? PresentDays { get; set; }

    public string? FullDays { get; set; }

    public string? HalfDays { get; set; }

    public byte? WeekOffs { get; set; }

    public bool? Holidays { get; set; }

    public byte? HolidaysArrears { get; set; }

    public byte? Lop { get; set; }

    public byte? ArrearDays { get; set; }

    public byte? LateLop { get; set; }

    public byte? OvertimeHours { get; set; }

    public byte? ArrearOvertimeHours { get; set; }

    public string? Currency { get; set; }

    public byte? Gratuity { get; set; }

    public int? BasicSalary2 { get; set; }

    public byte? Da2 { get; set; }

    public int? HouseRentAllowance2 { get; set; }

    public short? ConveyanceAllowance2 { get; set; }

    public byte? SpecialAllowance2 { get; set; }

    public short? MedicalAllowance2 { get; set; }

    public byte? EducationAllowance2 { get; set; }

    public byte? LeaveTravelAllowance2 { get; set; }

    public byte? OtherAllowance2 { get; set; }

    public byte? ExGratia2 { get; set; }

    public byte? Bonus2 { get; set; }

    public byte? LeaveEncashment2 { get; set; }

    public byte? OtherIncome2 { get; set; }

    public byte? LateComingDeduction2 { get; set; }

    public byte? EarlyGoingDeduction2 { get; set; }

    public byte? LtaReimbursement2 { get; set; }

    public byte? MedicalReimbursement2 { get; set; }

    public byte? FuelReimbursement2 { get; set; }

    public byte? DriverReimbursement2 { get; set; }

    public byte? TravelExpensePayment2 { get; set; }

    public byte? ClaimExpensePayment2 { get; set; }

    public byte? BonusCurrentFy { get; set; }

    public byte? ExgratiaCurrentFy { get; set; }

    public byte? BonusPreviousFy { get; set; }

    public byte? ExgratiaPreviousFy { get; set; }

    public int? TotalEarning { get; set; }

    public byte? ArrearEarlyGoingLop { get; set; }

    public byte? ArrearLateComingLop { get; set; }

    public byte? ArrearOther { get; set; }

    public byte? ArrearOvertime { get; set; }

    public byte? ArrearPaidHolidays { get; set; }

    public byte? BasicSalaryArrears { get; set; }

    public byte? DaArrears { get; set; }

    public byte? HouseRentAllowanceArrears { get; set; }

    public byte? ConveyanceAllowanceArrears { get; set; }

    public byte? SpecialAllowanceArrears { get; set; }

    public byte? MedicalAllowanceArrears { get; set; }

    public byte? EducationAllowanceArrears { get; set; }

    public byte? LeaveTravelAllowanceArrears { get; set; }

    public byte? OtherAllowanceArrears { get; set; }

    public byte? ExGratiaArrears { get; set; }

    public byte? BonusArrears { get; set; }

    public byte? LeaveEncashmentArrears { get; set; }

    public byte? OtherIncomeArrears { get; set; }

    public byte? TravelExpensePaymentArrears { get; set; }

    public byte? ClaimExpensePaymentArrears { get; set; }

    public byte? LateComingDeductionArrears { get; set; }

    public byte? EarlyGoingDeductionArrears { get; set; }

    public byte? LtaReimbursementArrears { get; set; }

    public byte? MedicalReimbursementArrears { get; set; }

    public byte? FuelReimbursementArrears { get; set; }

    public byte? DriverReimbursementArrears { get; set; }

    public byte? TotalArrear { get; set; }

    public int? GrossSalary { get; set; }

    public byte? CanteenDeduction2 { get; set; }

    public byte? SalaryAdvance2 { get; set; }

    public byte? OtherDeduction2 { get; set; }

    public byte? PrevMonthRoundOffRecovery2 { get; set; }

    public short? ProvidentFund { get; set; }

    public byte? ProvidentFundArrear { get; set; }

    public byte? AbryBenefits { get; set; }

    public byte? VoluntaryProvidentFund { get; set; }

    public byte? VoluntaryProvidentFundArrear { get; set; }

    public byte? EmployeeStateInsurance { get; set; }

    public byte? EsiArrears { get; set; }

    public byte? ProfTax { get; set; }

    public int? IncomeTax { get; set; }

    public int? TotalDeduction { get; set; }

    public byte? RoundingOff { get; set; }

    public int? NetPay { get; set; }

    public byte? EpsArrear { get; set; }

    public short? EmployeePensionScheme { get; set; }

    public short? CompanyProvidentFund { get; set; }

    public byte? CompanyProvidentFundArrear { get; set; }

    public short? EmployerContributionEsi { get; set; }

    public byte? EmployerContributionEsiArrears { get; set; }

    public byte? EdliContribution { get; set; }

    public byte? EdliAdminCharges { get; set; }

    public double? AC2PfAdminCharges { get; set; }

    public short? CessForThePeriod { get; set; }

    public int RawtaxForThePeriod { get; set; }

    public double BonusProvision2 { get; set; }

    public string? AdhocRemark { get; set; }
}
