using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Departments.Commands
{
    #region --- COMMAND ---
    public record CreateDepartmentCommand(string Name, bool IsActive, Guid CreatedBy) : IRequest<Result<string>>;
    #endregion
    #region --- VALIDATION ---
    internal sealed class CreateDepartmentCommandValidator : AbstractValidator<CreateDepartmentCommand>
    {
        public CreateDepartmentCommandValidator() 
        { 
            
        }
    }
    #endregion

    #region --- HANDLER ---
    internal sealed class CreateDepartmentCommandHandler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateDepartmentCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
           if(await departmentRepository.ExistsAsync(request.Name))
            {
                return new ValidationError("A department with the same name already exists.");
            }
           var department = Department.Create(
               id:Guid.NewGuid(),
               name:request.Name,
               isActive:request.IsActive,
               createdBy:request.CreatedBy);
            await departmentRepository.AddAsync(department);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok($"Department '{request.Name}' created successfully.");
        }
    }

    #endregion
}
