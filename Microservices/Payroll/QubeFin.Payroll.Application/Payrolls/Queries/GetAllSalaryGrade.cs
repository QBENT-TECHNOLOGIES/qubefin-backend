using FluentResults;
using MediatR;
using QubeFin.Payroll.Persistence.Repositories;
using QubeFin.Persistence.Entities;

namespace QubeFin.Payroll.Application.Payrolls.Queries;

public record GetAllSalaryGradeQuery() : IRequest<Result<List<GetAllSalaryGradeResponse>>>;

public record GetAllSalaryGradeResponse(Guid Id, string Name, string Code, bool IsActive, decimal? GrossSalary);
internal sealed class GetAllSalaryGradeQueryHandler(IPayrollRepository payrollRepository) : IRequestHandler<GetAllSalaryGradeQuery, Result<List<GetAllSalaryGradeResponse>>>
{
    public async Task<Result<List<GetAllSalaryGradeResponse>>> Handle(GetAllSalaryGradeQuery request, CancellationToken cancellationToken)
    {
        var salaryGrade = await payrollRepository.GetAllSalaryGrade();
        return Result.Ok(salaryGrade.Select(m => new GetAllSalaryGradeResponse(m.Id, m.Name, m.Code, m.IsActive, GetLatestGrossSalary(m)))
            .OrderBy(m => GetSalaryGradeOrder(m.Code)).ThenBy(m => m.Code).ToList());
    }

    private static readonly Dictionary<char, int> RomanNumerals = new()
    {
        ['I'] = 1,
        ['V'] = 5,
        ['X'] = 10,
        ['L'] = 50,
        ['C'] = 100,
        ['D'] = 500,
        ['M'] = 1000
    };

    // Salary grade codes are roman numerals (I, II, IV, V, IX, X, XII), so they are ordered by value and not alphabetically.
    // A code that is not a roman numeral keeps its alphabetical position at the end of the list.
    private static int GetSalaryGradeOrder(string? code)
    {
        if (string.IsNullOrWhiteSpace(code))
            return int.MaxValue;

        var value = code.Trim().ToUpperInvariant();
        var total = 0;
        var highest = 0;

        for (var i = value.Length - 1; i >= 0; i--)
        {
            if (!RomanNumerals.TryGetValue(value[i], out var current))
                return int.MaxValue;

            total += current < highest ? -current : current;
            highest = Math.Max(highest, current);
        }

        return total;
    }

    // Latest gross amount from Tbl_SalaryStructure: the running structure (EffectiveToDate == null), else the newest one.
    private static decimal? GetLatestGrossSalary(TblSalaryGrade salaryGrade)
    {
        if (!salaryGrade.TblSalaryStructures.Any())
            return null;

        var latestSalaryStructure = salaryGrade.TblSalaryStructures.Any(ss => ss.EffectiveToDate == null) ?
            salaryGrade.TblSalaryStructures.Where(ss => ss.EffectiveToDate == null).First() :
            salaryGrade.TblSalaryStructures.OrderByDescending(ss => ss.EffectiveFromDate).First();

        return latestSalaryStructure.GrossAmount;
    }
}
