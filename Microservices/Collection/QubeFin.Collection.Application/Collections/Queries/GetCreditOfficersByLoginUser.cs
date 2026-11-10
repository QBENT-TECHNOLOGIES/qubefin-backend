using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Collection.Persistence.Repositories;

namespace QubeFin.Collection.Application.Collections.Queries;

#region --- QUERY ---
public record GetCreditOfficersByLoginUserQuery(Guid EmployeeId) : IRequest<Result<List<CreditOfficerResponse>>>;
#endregion

#region --- VALIDATOR ---
public class GetCreditOfficersByLoginUserQueryValidator : AbstractValidator<GetCreditOfficersByLoginUserQuery>
{
    public GetCreditOfficersByLoginUserQueryValidator()
    {
        RuleFor(v => v.EmployeeId).NotEmpty().WithMessage("Employee Id is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record CreditOfficerResponse(
    Guid EmployeeId,
    string EmployeeName,
    string? EmployeeCode,
    Guid DesignationId,
    string DesignationName,
    bool IsIncharge,
    Guid OrganizationUnitId,
    string OrganizationUnitName,
    int OrganizationCode,
    string OrganizationUnitTypeName);
#endregion

#region --- HANDLER ---
internal sealed class GetCreditOfficersByLoginUserQueryHandler(ICollectionRepository collectionRepository)
    : IRequestHandler<GetCreditOfficersByLoginUserQuery, Result<List<CreditOfficerResponse>>>
{
    public async Task<Result<List<CreditOfficerResponse>>> Handle(GetCreditOfficersByLoginUserQuery request, CancellationToken cancellationToken)
    {
        var creditOfficers = await collectionRepository.GetCreditOfficersAsync(request.EmployeeId, cancellationToken);

        return Result.Ok(creditOfficers.Select(m => new CreditOfficerResponse(
            m.EmployeeId,
            m.EmployeeName,
            m.EmployeeCode,
            m.DesignationId,
            m.DesignationName,
            m.IsIncharge,
            m.OrganizationUnitId,
            m.OrganizationUnitName,
            m.OrganizationCode,
            m.OrganizationUnitTypeName)).ToList());
    }
}
#endregion
