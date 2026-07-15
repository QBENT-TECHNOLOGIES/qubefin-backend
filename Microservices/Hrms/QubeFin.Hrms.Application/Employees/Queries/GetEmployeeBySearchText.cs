
using MediatR;
using FluentResults;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeBySearchTextQuery(string SearchText) : IRequest<Result<GetEmployeeBySearchTextResponse>>;
#endregion

#region --- RESPONSE ---
public record EmployeeBySearchTextResult(Guid Id, string? EmployeeName, bool HasSignaturePhoto);
public record GetEmployeeBySearchTextResponse(IReadOnlyList<EmployeeBySearchTextResult> Employees);
#endregion

#region --- HANDLER ---
internal sealed class GetEmployeeBySearchTextQueryHandler(QubeFinDataContext context) : IRequestHandler<GetEmployeeBySearchTextQuery, Result<GetEmployeeBySearchTextResponse>>
{
    public async Task<Result<GetEmployeeBySearchTextResponse>> Handle(GetEmployeeBySearchTextQuery request, CancellationToken cancellationToken)
    {

        var employeEntities = await context.TblEmployees.Include(m => m.TblEmployeeDocuments).Where(m => m.IsActive && (m.FullName.StartsWith(request.SearchText) || m.Code.StartsWith(request.SearchText))).OrderBy(m => m.FirstName).ToListAsync();
        var searchResult = employeEntities == null ? new List<EmployeeBySearchTextResult>() :
               employeEntities.Select(m => new EmployeeBySearchTextResult(m.Id, m.FullName+"(" + m.Code + ")", HasSignaturePhoto(m))).ToList();

        return new GetEmployeeBySearchTextResponse(searchResult);
    }
    private static bool HasSignaturePhoto(TblEmployee employee)
    {
        return employee.TblEmployeeDocuments.Any(d => d.DocumentName == "SIGNATURE" && !string.IsNullOrWhiteSpace(d.FileName))
               && employee.TblEmployeeDocuments.Any(d => d.DocumentName == "PHOTO" && !string.IsNullOrWhiteSpace(d.FileName));
    }
}
#endregion