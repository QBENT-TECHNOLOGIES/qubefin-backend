using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Salaries.SalaryCategory.Queries
{
    public record GetAllSalaryComponentCategoriesQuery() : IRequest<Result<IEnumerable<SalaryComponentCategory>>>;
    internal sealed class GetAllSalaryComponentCategoriesQueryHandler(ISalaryComponentCategoryRepository categoryRepository) :
        IRequestHandler<GetAllSalaryComponentCategoriesQuery, Result<IEnumerable<SalaryComponentCategory>>>
    {
        public async Task<Result<IEnumerable<SalaryComponentCategory>>> Handle(GetAllSalaryComponentCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await categoryRepository.GetAllSalaryComponentCategories();
            return Result.Ok(categories);
        }
    }
}
