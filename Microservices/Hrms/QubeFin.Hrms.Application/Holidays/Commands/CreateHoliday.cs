using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Holidays.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Holidays.Commands;

public record CreateHolidayCommand(
    HolidayRequest request, Guid userId) : IRequest<Result<string>>;

public class CreateHolidayCommandValidator : AbstractValidator<CreateHolidayCommand>
{
    public CreateHolidayCommandValidator()
    {
        RuleFor(x => x.request.OrgUnitIds)
            .NotEmpty().WithMessage("Please select at least one organization unit.");

        RuleFor(x => x.request.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(50);

        RuleFor(x => x.request.HolidayDate)
            .NotEmpty().WithMessage("Holiday date is required.");
    }
}

internal sealed class CreateHolidayCommandHandler(IHolidayRepository holidayRepository, IUnitOfWork unitOfWork)
: IRequestHandler<CreateHolidayCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateHolidayCommand request, CancellationToken cancellationToken)
    {
        var addedCount = 0;

        foreach (var orgUnitId in request.request.OrgUnitIds)
        {
            if (await holidayRepository.ExistsAsync(orgUnitId, request.request.HolidayDate))
            {
                continue;
            }

            var holiday = Holiday.Create(
                Guid.NewGuid(),
                orgUnitId,
                request.request.HolidayDate,
                request.request.Description.Trim(),
                request.userId); 

            await holidayRepository.AddAsync(holiday);
            addedCount++;
        }

        if (addedCount == 0)
        {
            return new ValidationError("Holidays already exist for the selected organization units on this date.");
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok($"{request.request.HolidayDate.ToString("dd-MM-yyyy")} Marked as Holiday for {addedCount} unit(s).");
    }
}