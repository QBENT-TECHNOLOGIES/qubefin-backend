using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.Holidays.Commands;

public record UpdateHolidayCommand(
    Guid Id,
    Guid OrgUnitId,
    DateOnly HolidayDate,
    string Description,
    Guid ModifiedBy) : IRequest<Result<string>>;

public class UpdateHolidayCommandValidator : AbstractValidator<UpdateHolidayCommand>
{
    public UpdateHolidayCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.OrgUnitId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ModifiedBy).NotEmpty();
    }
}
internal sealed class UpdateHolidayCommandHandler(IHolidayRepository holidayRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateHolidayCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UpdateHolidayCommand request, CancellationToken cancellationToken)
    {
        var holiday = await holidayRepository.GetByIdAsync(request.Id);
        if (holiday is null)
        {
            return new RecordNotFoundError("Holiday not found.");
        }

        if (await holidayRepository.ExistsAsync(request.OrgUnitId, request.HolidayDate, request.Id))
        {
            return new ValidationError("A holiday already exists for the selected organization unit and date.");
        }

        holiday.Update(request.OrgUnitId, request.HolidayDate, request.Description.Trim(), request.ModifiedBy);
        await holidayRepository.UpdateAsync(holiday);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok($"{request.HolidayDate.ToString("dd-MM-yyyy")} Holiday updated successfully.");
    }
}
