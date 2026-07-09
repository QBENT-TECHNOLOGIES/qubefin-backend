using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Salaries.Commands
{
    #region --- COMMAND ---
    public record CreateSalaryComponentCommand(string Name,
        string Code,
        Guid CategoryId,
        bool IsTaxable,
        bool IsPfapplicable,
        bool IsEsiapplicable,
        bool IsCtccomponent,
        bool IsActive,
        int DisplayOrder,
        Guid CreatedBy) : IRequest<Result<CreateSalaryComponentResponse>>;
    #endregion
    #region --- RESPONSE ---
    public record CreateSalaryComponentResponse(bool Created);
    #endregion
    internal sealed class CreateSalaryComponentCommandHandler(ISalaryComponentRepository salaryRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateSalaryComponentCommand, Result<CreateSalaryComponentResponse>>
    {
        public async Task<Result<CreateSalaryComponentResponse>> Handle(CreateSalaryComponentCommand request, CancellationToken cancellationToken)
        {   
            var duplicateName = await salaryRepository.ExistsByNameAsync(request.Name, request.CategoryId);
            if(duplicateName) return new ValidationError("A salary component with this name already exists in the selected category.");
            var createSalaryComponent = SalaryComponent.Create(
                id: Guid.NewGuid(),
                name: request.Name,
                code: request.Code,
                categoryId: request.CategoryId,
                isTaxable: request.IsTaxable,
                isPfapplicable: request.IsPfapplicable,
                isEsiapplicable: request.IsEsiapplicable,
                isCtccomponent: request.IsCtccomponent,
                isActive: request.IsActive,
                displayOrder: request.DisplayOrder,
                createdBy: request.CreatedBy
            );
            await salaryRepository.CreateSalaryComponent(createSalaryComponent);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new CreateSalaryComponentResponse(true));
        }
    }
}
