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
public record DesignationStatusUpdateCommand(Guid DesignationId) : IRequest<Result<string>>;
#endregion
#region --- VALIDATION ---
public class DesignationStatusUpdateValidator : AbstractValidator<DesignationStatusUpdateCommand>
{
    public DesignationStatusUpdateValidator()
    {
        RuleFor(x => x.DesignationId).NotEmpty().WithMessage("Designation Id is required.");
    }
}
#endregion
#region --- HANDLER ---
internal sealed class DesignationStatusUpdateCommandHandler(IUnitOfWork unitOfWork, IOrganizationUnitRepository organizationUnitRepository) : IRequestHandler<DesignationStatusUpdateCommand, Result<string>>
{
    public async Task<Result<string>> Handle(DesignationStatusUpdateCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await organizationUnitRepository.DesignationStatusUpdateAsync(request.DesignationId);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok($"Designation disabled successfully.");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to disable designation: {ex.Message}");
        }
    }
}
#endregion
