using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeQualificationQuery(Guid Id) : IRequest<Result<GetQualificationResponse>>;
#endregion
#region --- RESPONSE ---
public record GetQualificationResponse(
    List<QualificationRequest> Qualifications
    );

#endregion
#region --- HANDLER ---
internal sealed class GetEmployeeQualificationQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetEmployeeQualificationQuery, Result<GetQualificationResponse>>
{
    public async Task<Result<GetQualificationResponse>> Handle(GetEmployeeQualificationQuery request, CancellationToken cancellationToken)
    {
        var employee = await context.TblEmployees.Include(m => m.TblEmployeeQualifications).Where(m => m.Id == request.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }
        var employeeQualification = employee.TblEmployeeQualifications.ToList();
        var qualifications = employeeQualification.Any() ? [.. employeeQualification.Select(d => new QualificationRequest()
        {
            Id = d.Id,
            EmployeeId = d.EmployeeId,
            AcademicStream = d.AcademicStream,
            Specialization = d.Specialization,
            YearOfPassing = d.YearOfPassing,
            UniversityOrBoard = d.UniversityOrBoard,
            SchoolOrCollege = d.SchoolOrCollege,
            GradeOrMarks = d.GradeOrMarks,
            DocFileName = d.DocFileName,
            DocFileNo = d.DocFileNo,
            Sequence = d.Sequence
        })]

                    : new List<QualificationRequest>();
        return Result.Ok(new GetQualificationResponse(qualifications.OrderBy(m => m.Sequence).ToList()));
    }
}
#endregion