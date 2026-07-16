using System;

namespace QubeFin.Persistence.Models.Hrms;

public class EmployeeEmployment
{
    public Guid Id { get; private set; }
    public string EmployerName { get; private set; } = null!;
    public string Designation { get; private set; } = null!;
    public DateOnly FromDate { get; private set; }
    public DateOnly ToDate { get; private set; }
    public decimal LastDrawnSalary { get; private set; }
    public string? JobTitle { get; private set; }
    public string? NocFileName { get; private set; }
    public string? NocFileNo { get; private set; }
    public string? ExpCertFileName { get; private set; }
    public string? ExpCertFileNo { get; private set; }
    public Guid EmployeeId { get; private set; }
    public int Sequence { get; private set; }
    public DateTime? CreatedOn { get; private set; }
    public Guid? CreatedBy { get; private set; }
    public Guid? LastModifiedBy { get; private set; }
    public DateTime? LastModifiedOn { get; private set; }

    // Required for ORM/EF Core hydration
    private EmployeeEmployment()
    {
    }

    public EmployeeEmployment(
        Guid id,
        string employerName,
        string designation,
        DateOnly fromDate,
        DateOnly toDate,
        decimal lastDrawnSalary,
        string? jobTitle,
        string? nocFileName,
        string? nocFileNo,
        string? expCertFileName,
        string? expCertFileNo,
        Guid employeeId,
        int sequence,
        Guid? createdBy)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        EmployerName = employerName.Trim();
        Designation = designation.Trim();
        FromDate = fromDate;
        ToDate = toDate;
        LastDrawnSalary = lastDrawnSalary;
        JobTitle = jobTitle?.Trim();
        NocFileName = nocFileName?.Trim();
        NocFileNo = nocFileNo?.Trim();
        ExpCertFileName = expCertFileName?.Trim();
        ExpCertFileNo = expCertFileNo?.Trim();
        EmployeeId = employeeId;
        Sequence = sequence;
        CreatedBy = createdBy;
        CreatedOn = DateTime.UtcNow;
    }
}
