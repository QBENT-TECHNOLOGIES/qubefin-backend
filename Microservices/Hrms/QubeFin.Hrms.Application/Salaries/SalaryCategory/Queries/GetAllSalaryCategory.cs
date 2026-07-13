using FluentResults;
using MediatR;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Hrms.Application.Salaries.SalaryCategory.Queries
{
    public record GetAllSalaryComponentCategoriesQuery() : IRequest<Result<GetAllSalaryComponentCategoriesResponse>>;
    public record GetAllSalaryComponentCategoriesResponse(IEnumerable<SalaryComponentCategory> SalaryComponentCategories);
    internal sealed class GetAllSalaryComponentCategoriesQueryHandler(ISalaryComponentCategoryRepository categoryRepository) : IRequestHandler<GetAllSalaryComponentCategoriesQuery, Result<GetAllSalaryComponentCategoriesResponse>>
    {
        public async Task<Result<GetAllSalaryComponentCategoriesResponse>> Handle(GetAllSalaryComponentCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await categoryRepository.GetAllSalaryComponentCategories();
            return Result.Ok(new GetAllSalaryComponentCategoriesResponse(categories));
        }
    }
}
