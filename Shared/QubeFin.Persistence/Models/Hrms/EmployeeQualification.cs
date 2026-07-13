namespace QubeFin.Persistence.Models.Hrms;

public class EmployeeQualification
{
    public Guid Id { get; set; }
    public string AcademicStream { get; set; } = null!;
    public string? Specialization { get; set; }
    public int YearOfPassing { get; set; }
    public string? UniversityOrBoard { get; set; }
    public string? SchoolOrCollege { get; set; }
    public string? GradeOrMarks { get; set; }
    public string? DocFileName { get; set; }
    public string? DocFileNo { get; set; }
    public Guid EmployeeId { get; set; }
    public int Sequence { get; set; }
}
