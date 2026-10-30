using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Global.Application.OrganizationUnits.Commands;

#region --- COMMAND ---
public record DesignationInchargeUpdateCommand(Guid DesignationId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class DesignationInchargeUpdateValidator : AbstractValidator<DesignationInchargeUpdateCommand>
{
    public DesignationInchargeUpdateValidator()
    {
        RuleFor(x => x.DesignationId ).NotEmpty().WithMessage("Designation Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class DesignationInchargeUpdateCommandHandler(IUnitOfWork unitOfWork, IOrganizationUnitRepository organizationUnitRepository) :IRequestHandler<DesignationInchargeUpdateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(DesignationInchargeUpdateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await organizationUnitRepository.DesignationInchargeUpdateAsync(request.DesignationId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok($"Designation incharge updated successfully.");
        }
        catch(Exception ex)
        {
            return Result.Fail($"Failed to update incharge: {ex.Message}");
        }
    }
}
#endregion