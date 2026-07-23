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
public record GetEmployeeReferenceQuery(Guid Id) : IRequest<Result<GetReferenceResponse>>;
#endregion
#region --- RESPONSE ---
public record GetReferenceResponse(
    List<ReferenceDetailRequest> References
    );

#endregion
#region --- HANDLER ---
internal sealed class GetEmployeeReferenceQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetEmployeeReferenceQuery, Result<GetReferenceResponse>>
{
    public async Task<Result<GetReferenceResponse>> Handle(GetEmployeeReferenceQuery request, CancellationToken cancellationToken)
    {
        var employee = await context.TblEmployees.Include(m => m.TblEmployeeReferences).Where(m => m.Id == request.Id).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (employee is null)
        {
            return new RecordNotFoundError($"Employee not found for the given Id");
        }
        var employeeReferences = employee.TblEmployeeReferences.ToList();
        var docs = employeeReferences.Any() ? [.. employeeReferences.Select(d => new ReferenceDetailRequest()
        {
            Id = d.Id,
            EmployeeId = d.EmployeeId,
            PersonName = d.PersonName,
            Mobile = d.Mobile,
            Email = d.Email,
            Address = d.Address,
            Occupation = d.Occupation,
            HowDoYouKnow = d.HowDoYouKnow,
        })]

                    : new List<ReferenceDetailRequest>();
        return Result.Ok(new GetReferenceResponse(docs));
    }
}
#endregion