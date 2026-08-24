using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Global.Persistence.Repositories;

namespace QubeFin.Global.Application.AdministrativeUnits.Queries;

public record GetAllPostofficeQuery(string Pincode) : IRequest<Result<List<GetAllPostofficeResponse>>>;
public class GetAllPostofficeValidator : AbstractValidator<GetAllPostofficeQuery>
{
    public GetAllPostofficeValidator()
    {
        RuleFor(v => v.Pincode)
        .NotEmpty()
        .WithMessage("Pincode is required.")
        .Matches(@"^\d{6}$")
        .WithMessage("Pincode must be exactly 6 digits.");
    }
}
public record GetAllPostofficeResponse(Guid Id, string Name, string pincode);

internal sealed class GetAllPostofficeQueryHandler(IAdministrativeUnitRepository administrativeUnit) : IRequestHandler<GetAllPostofficeQuery, Result<List<GetAllPostofficeResponse>>>
{
    public async Task<Result<List<GetAllPostofficeResponse>>> Handle(GetAllPostofficeQuery request, CancellationToken cancellationToken)
    {
        var postoffices = await administrativeUnit.GetAllPostofficeAsync(request.Pincode);
        return Result.Ok(postoffices.Select(m => new GetAllPostofficeResponse(m.Id, m.Name, m.Pincode)).ToList());
    }
}
