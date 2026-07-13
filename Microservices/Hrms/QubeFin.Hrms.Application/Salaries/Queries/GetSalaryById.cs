using FluentResults;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Salaries.Queries
{
    public record GetSalaryComponentByIdQuery(Guid Id) : IRequest<Result<GetSalaryComponentByIdResponse>>;
    public record GetSalaryComponentByIdResponse(SalaryComponent SalaryComponent);
    internal sealed class GetSalaryByIdQueryHandler(ISalaryComponentRepository salaryRepository) : IRequestHandler<GetSalaryComponentByIdQuery, Result<GetSalaryComponentByIdResponse>>
    {
        public async Task<Result<GetSalaryComponentByIdResponse>> Handle(GetSalaryComponentByIdQuery request, CancellationToken cancellationToken)
        {
            var salaryComponent = await salaryRepository.GetSalaryComponentById(request.Id);
            if (salaryComponent is null) return new RecordNotFoundError("Salary Component not found");
            return Result.Ok(new GetSalaryComponentByIdResponse(salaryComponent));
        }
    }
}
