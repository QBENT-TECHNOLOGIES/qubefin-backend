using QubeFin.Persistence.Models.Global;

namespace QubeFin.Persistence.Models.Hrms;

public class EmployeeQualification
{
    public Guid Id { get; private set; }
    public string? AcademicStream { get; private set; }
    public string? Specialization { get; private set; }
    public int YearOfPassing { get; private set; }
    public string? UniversityOrBoard { get; private set; }
    public string? SchoolOrCollege { get; private set; }
    public string? GradeOrMarks { get; private set; }
    public string? DocFileName { get; private set; }
    public string? DocFileNo { get; private set; }
    public Guid EmployeeId { get; private set; }
    public int Sequence { get; private set; }

    private EmployeeQualification()
    {
    }

    public EmployeeQualification(
        Guid id,
        string? academicStream,
        string? specialization,
        int yearOfPassing,
        string? universityOrBoard,
        string? schoolOrCollege,
        string? gradeOrMarks,
        string? docFileName,
        string? docFileNo,
        Guid employeeId,
        int sequence)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        AcademicStream = academicStream?.Trim();
        Specialization = specialization?.Trim();
        YearOfPassing = yearOfPassing;
        UniversityOrBoard = universityOrBoard?.Trim();
        SchoolOrCollege = schoolOrCollege?.Trim();
        GradeOrMarks = gradeOrMarks?.Trim();
        DocFileName = docFileName?.Trim();
        DocFileNo = docFileNo?.Trim();
        EmployeeId = employeeId;
        Sequence = sequence;
    }
}
