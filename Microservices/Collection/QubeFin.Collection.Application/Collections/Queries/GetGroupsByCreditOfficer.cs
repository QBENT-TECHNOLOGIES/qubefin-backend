using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Collection.Persistence.Repositories;

namespace QubeFin.Collection.Application.Collections.Queries;

#region --- QUERY ---
// EmployeeId is the logged-in user; DesignationId is the credit officer's designation from the credit officer list.
public record GetGroupsByCreditOfficerQuery(Guid EmployeeId, Guid DesignationId) : IRequest<Result<List<CreditOfficerGroupResponse>>>;
#endregion

#region --- VALIDATOR ---
public class GetGroupsByCreditOfficerQueryValidator : AbstractValidator<GetGroupsByCreditOfficerQuery>
{
    public GetGroupsByCreditOfficerQueryValidator()
    {
        RuleFor(v => v.EmployeeId).NotEmpty().WithMessage("Employee Id is required.");
        RuleFor(v => v.DesignationId).NotEmpty().WithMessage("Designation Id is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record CreditOfficerGroupResponse(
    Guid Id,
    string Code,
    string Name,
    Guid DesignationId,
    string DesignationName,
    Guid? CreditOfficerId,
    string? CreditOfficerName,
    Guid OrganizationUnitId,
    string OrganizationUnitName,
    int OrganizationCode,
    DateOnly? FormationDate,
    string? CollectionDay,
    TimeOnly? CollectionTime,
    bool IsMonthlyGroup,
    DateOnly? BiWeeklyStartDate,
    string? ContactNo,
    string? AddressLine1,
    string? PinCode);
#endregion

#region --- HANDLER ---
internal sealed class GetGroupsByCreditOfficerQueryHandler(ICollectionRepository collectionRepository)
    : IRequestHandler<GetGroupsByCreditOfficerQuery, Result<List<CreditOfficerGroupResponse>>>
{
    public async Task<Result<List<CreditOfficerGroupResponse>>> Handle(GetGroupsByCreditOfficerQuery request, CancellationToken cancellationToken)
    {
        var groups = await collectionRepository.GetCreditOfficerGroupsAsync(request.EmployeeId, request.DesignationId, cancellationToken);

        return Result.Ok(groups.Select(m => new CreditOfficerGroupResponse(
            m.Id,
            m.Code,
            m.Name,
            m.DesignationId,
            m.DesignationName,
            m.CreditOfficerId,
            m.CreditOfficerName,
            m.OrganizationUnitId,
            m.OrganizationUnitName,
            m.OrganizationCode,
            m.FormationDate,
            m.CollectionDay,
            m.CollectionTime,
            m.IsMonthlyGroup,
            m.BiWeeklyStartDate,
            m.ContactNo,
            m.AddressLine1,
            m.PinCode)).ToList());
    }
}
#endregion
