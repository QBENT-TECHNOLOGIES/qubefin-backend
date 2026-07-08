using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.Salaries.Commands
{
    #region --- COMMAND ---
    public record UpdateSalaryComponentCommand(Guid Id,
        string Name,
        string Code,
        Guid CategoryId,
        bool IsTaxable,
        bool IsPfapplicable,
        bool IsEsiapplicable,
        bool IsCtccomponent,
        bool IsActive,
        int DisplayOrder,
        Guid? LastModifiedBy) : IRequest<Result<UpdateSalaryComponentResponse>>;
    #endregion

    public record UpdateSalaryComponentResponse(bool Updated);
    internal sealed class UpdateSalaryComponentCommandHandler(ISalaryComponentRepository salaryRepository, IUnitOfWork unitOfWork) : IRequestHandler<UpdateSalaryComponentCommand, Result<UpdateSalaryComponentResponse>>
    {
        public async Task<Result<UpdateSalaryComponentResponse>> Handle(UpdateSalaryComponentCommand request, CancellationToken cancellationToken)
        {   
            var existingSalaryComponent = await salaryRepository.GetSalaryComponentById(request.Id);
            if(existingSalaryComponent is null) return new RecordNotFoundError("Salary Component not found");
            existingSalaryComponent.Update(
                name: request.Name,
                code: request.Code,
                categoryId: request.CategoryId,
                isTaxable: request.IsTaxable,
                isPfapplicable: request.IsPfapplicable,
                isEsiapplicable: request.IsEsiapplicable,
                isCtccomponent: request.IsCtccomponent,
                isActive: request.IsActive,
                displayOrder: request.DisplayOrder,
                lastModifiedBy: request.LastModifiedBy
            );
            await salaryRepository.UpdateSalaryComponent(existingSalaryComponent);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new UpdateSalaryComponentResponse(true));
        }
    }
}
