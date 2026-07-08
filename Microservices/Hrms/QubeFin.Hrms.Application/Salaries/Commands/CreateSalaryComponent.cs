using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.Salaries.Commands
{
    #region --- COMMAND ---
    public record CreateSalaryComponentCommand(SalaryComponent salaryComponent) : IRequest<Result<CreateSalaryComponentResponse>>;
    #endregion
    #region --- RESPONSE ---
    public record CreateSalaryComponentResponse(bool Created);
    #endregion
    internal sealed class CreateSalaryComponentCommandHandler(ISalaryComponentRepository salaryRepository, IUnitOfWork unitOfWork) : IRequestHandler<CreateSalaryComponentCommand, Result<CreateSalaryComponentResponse>>
    {
        public async Task<Result<CreateSalaryComponentResponse>> Handle(CreateSalaryComponentCommand request, CancellationToken cancellationToken)
        {
            var createSalaryComponent = salaryRepository.CreateSalaryComponent(request.salaryComponent);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Ok(new CreateSalaryComponentResponse(true));
        }
    }
}
