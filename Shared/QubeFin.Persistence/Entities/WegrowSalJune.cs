using System;
using System.Collections.Generic;

namespace QubeFin.Persistence.Entities;

public partial class WegrowSalJune
{
    public byte S { get; set; }

    public string EmpCode { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string? FatherName { get; set; }

    public string Gender { get; set; } = null!;

    public string MaritalStatus { get; set; } = null!;

    public DateOnly DateOfJoining { get; set; }

    public string? TransferDate { get; set; }

    public DateOnly ConfirmationDate { get; set; }

    public DateOnly? DateOfLeaving { get; set; }

    public DateOnly? LastWorkingDate { get; set; }

    public string? RetirementDate { get; set; }

    public string EmailId { get; set; } = null!;

    public long MobileNo { get; set; }

    public string Status { get; set; } = null!;

    public string Location { get; set; } = null!;

    public string? PfNo { get; set; }

    public long? UanNo { get; set; }

    public long? EsicNo { get; set; }

    public string? Pran { get; set; }

    public string Pan { get; set; } = null!;

    public string AadharNo { get; set; } = null!;

    public string? City { get; set; }

    public string? SuperannuationId { get; set; }

    public string? StopPaymentFromDate { get; set; }

    public string? StopPaymentTillDate { get; set; }

    public string? BankName { get; set; }

    public string? BankBranchCode { get; set; }

    public string? IfscCode { get; set; }

    public long? BankAccountNo { get; set; }

    public string? ModeOfPayment { get; set; }

    public string Area { get; set; } = null!;

    public string Company { get; set; } = null!;

    public string Department { get; set; } = null!;

    public string Designation { get; set; } = null!;

    public string Grade { get; set; } = null!;

    public double BasicSalary { get; set; }

    public TimeOnly? BonusCtc { get; set; }

    public string? Bonus { get; set; }

    public string? BonusProvision { get; set; }

    public string? CanteenDeduction { get; set; }

    public string? ClaimExpensePayment { get; set; }

    public double ConveyanceAllowance { get; set; }

    public double CtcTotal { get; set; }

    public string? Da { get; set; }

    public string? DriverReimbursement { get; set; }

    public string? EarlyGoingDeduction { get; set; }

    public string? EducationAllowance { get; set; }

    public double EmployeeEsiCtc { get; set; }

    public double EmployeePfCtc { get; set; }

    public string? EmployerEdliCtc { get; set; }

    public double EmployerEsiCtc { get; set; }

    public string? EmployerPfAdminChargesCtc { get; set; }

    public double EmployerPfCtc { get; set; }

    public string? EpsContributionManual { get; set; }

    public string? EpsContributionArrearsManual { get; set; }

    public string? EsicEmployeeContributionManual { get; set; }

    public string? EsicEmployeeContributionArrearsManual { get; set; }

    public string? ExGratia { get; set; }

    public string? ExgratiaProvision { get; set; }

    public string? FuelReimbursement { get; set; }

    public TimeOnly? GratuityContributionCtc { get; set; }

    public string? GratuityAmountManual { get; set; }

    public string? GratuityProvision { get; set; }

    public double HouseRentAllowance { get; set; }

    public string? HraPercentage { get; set; }

    public string? LateComingDeduction { get; set; }

    public string? LeaveEncashment { get; set; }

    public string? LeaveTravelAllowance { get; set; }

    public string? LtaReimbursement { get; set; }

    public string? LwfManual { get; set; }

    public double MedicalAllowance { get; set; }

    public string? MedicalReimbursement { get; set; }

    public string? MinimumWage { get; set; }

    public double MonthlyGross { get; set; }

    public string? MpfContributionManual { get; set; }

    public string? MpfContributionArrearsManual { get; set; }

    public double NetPayCtc { get; set; }

    public string? OtherAllowance { get; set; }

    public string? OtherDeduction { get; set; }

    public string? OtherIncome { get; set; }

    public string? PrevMonthRoundOffRecovery { get; set; }

    public double ProfTaxCtc { get; set; }

    public string? ProfTaxManual { get; set; }

    public string? SalaryAdvance { get; set; }

    public string? SpecialAllowance { get; set; }

    public string? TdsManual { get; set; }

    public string? TravelExpensePayment { get; set; }

    public string? VpfContributionAmount { get; set; }

    public string? VpfContributionArrearsAmount { get; set; }

    public string? VpfContribution { get; set; }

    public byte DaysInMonth { get; set; }

    public byte WorkableDays { get; set; }

    public short DaysWorked { get; set; }

    public byte PresentDays { get; set; }

    public string? FullDays { get; set; }

    public string? HalfDays { get; set; }

    public byte WeekOffs { get; set; }

    public bool Holidays { get; set; }

    public byte HolidaysArrears { get; set; }

    public byte Lop { get; set; }

    public byte ArrearDays { get; set; }

    public byte LateLop { get; set; }

    public byte OvertimeHours { get; set; }

    public byte ArrearOvertimeHours { get; set; }

    public string Currency { get; set; } = null!;

    public TimeOnly? Gratuity { get; set; }

    public double BasicSalary2 { get; set; }

    public TimeOnly? Da2 { get; set; }

    public double HouseRentAllowance2 { get; set; }

    public double? ConveyanceAllowance2 { get; set; }

    public TimeOnly? SpecialAllowance2 { get; set; }

    public double? MedicalAllowance2 { get; set; }

    public TimeOnly? EducationAllowance2 { get; set; }

    public TimeOnly? LeaveTravelAllowance2 { get; set; }

    public TimeOnly? OtherAllowance2 { get; set; }

    public TimeOnly? ExGratia2 { get; set; }

    public TimeOnly? Bonus2 { get; set; }

    public TimeOnly? LeaveEncashment2 { get; set; }

    public TimeOnly? OtherIncome2 { get; set; }

    public TimeOnly? LateComingDeduction2 { get; set; }

    public TimeOnly? EarlyGoingDeduction2 { get; set; }

    public TimeOnly? LtaReimbursement2 { get; set; }

    public TimeOnly? MedicalReimbursement2 { get; set; }

    public TimeOnly? FuelReimbursement2 { get; set; }

    public TimeOnly? DriverReimbursement2 { get; set; }

    public TimeOnly? TravelExpensePayment2 { get; set; }

    public TimeOnly? ClaimExpensePayment2 { get; set; }

    public TimeOnly? BonusCurrentFy { get; set; }

    public TimeOnly? ExgratiaCurrentFy { get; set; }

    public TimeOnly? BonusPreviousFy { get; set; }

    public TimeOnly? ExgratiaPreviousFy { get; set; }

    public double TotalEarning { get; set; }

    public TimeOnly? ArrearEarlyGoingLop { get; set; }

    public TimeOnly? ArrearLateComingLop { get; set; }

    public TimeOnly? ArrearOther { get; set; }

    public TimeOnly? ArrearOvertime { get; set; }

    public TimeOnly? ArrearPaidHolidays { get; set; }

    public TimeOnly BasicSalaryArrears { get; set; }

    public TimeOnly? DaArrears { get; set; }

    public TimeOnly? HouseRentAllowanceArrears { get; set; }

    public TimeOnly? ConveyanceAllowanceArrears { get; set; }

    public TimeOnly? SpecialAllowanceArrears { get; set; }

    public TimeOnly? MedicalAllowanceArrears { get; set; }

    public TimeOnly? EducationAllowanceArrears { get; set; }

    public TimeOnly? LeaveTravelAllowanceArrears { get; set; }

    public TimeOnly? OtherAllowanceArrears { get; set; }

    public TimeOnly? ExGratiaArrears { get; set; }

    public TimeOnly? BonusArrears { get; set; }

    public TimeOnly? LeaveEncashmentArrears { get; set; }

    public TimeOnly? OtherIncomeArrears { get; set; }

    public TimeOnly? TravelExpensePaymentArrears { get; set; }

    public TimeOnly? ClaimExpensePaymentArrears { get; set; }

    public TimeOnly? LateComingDeductionArrears { get; set; }

    public TimeOnly? EarlyGoingDeductionArrears { get; set; }

    public TimeOnly? LtaReimbursementArrears { get; set; }

    public TimeOnly? MedicalReimbursementArrears { get; set; }

    public TimeOnly? FuelReimbursementArrears { get; set; }

    public TimeOnly? DriverReimbursementArrears { get; set; }

    public TimeOnly TotalArrear { get; set; }

    public double GrossSalary { get; set; }

    public TimeOnly? CanteenDeduction2 { get; set; }

    public TimeOnly? SalaryAdvance2 { get; set; }

    public TimeOnly? OtherDeduction2 { get; set; }

    public TimeOnly? PrevMonthRoundOffRecovery2 { get; set; }

    public double ProvidentFund { get; set; }

    public TimeOnly ProvidentFundArrear { get; set; }

    public TimeOnly? AbryBenefits { get; set; }

    public TimeOnly? VoluntaryProvidentFund { get; set; }

    public TimeOnly? VoluntaryProvidentFundArrear { get; set; }

    public double EmployeeStateInsurance { get; set; }

    public TimeOnly EsiArrears { get; set; }

    public double? ProfTax { get; set; }

    public double IncomeTax { get; set; }

    public double TotalDeduction { get; set; }

    public TimeOnly RoundingOff { get; set; }

    public double NetPay { get; set; }

    public TimeOnly EpsArrear { get; set; }

    public double EmployeePensionScheme { get; set; }

    public double CompanyProvidentFund { get; set; }

    public TimeOnly CompanyProvidentFundArrear { get; set; }

    public double EmployerContributionEsi { get; set; }

    public TimeOnly EmployerContributionEsiArrears { get; set; }

    public double EdliContribution { get; set; }

    public TimeOnly EdliAdminCharges { get; set; }

    public double AC2PfAdminCharges { get; set; }

    public double CessForThePeriod { get; set; }

    public double RawtaxForThePeriod { get; set; }

    public double BonusProvision2 { get; set; }

    public string? AdhocRemark { get; set; }
}
