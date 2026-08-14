using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Application.Holidays.Commands;

public record CreateHolidayCommand(
    Guid OrgUnitId,
    DateOnly HolidayDate,
    string Description,
    Guid CreatedBy) : IRequest<Result<string>>;

public class CreateHolidayCommandValidator : AbstractValidator<CreateHolidayCommand>
{
    public CreateHolidayCommandValidator()
    {
        RuleFor(x => x.OrgUnitId).NotEmpty();
        RuleFor(x => x.Description).NotEmpty().MaximumLength(50);
        RuleFor(x => x.CreatedBy).NotEmpty();
    }
}

internal sealed class CreateHolidayCommandHandler(IHolidayRepository holidayRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateHolidayCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateHolidayCommand request, CancellationToken cancellationToken)
    {
        if (await holidayRepository.ExistsAsync(request.OrgUnitId, request.HolidayDate))
        {
            return new ValidationError("A holiday already exists for the selected organization unit and date.");
        }

        var holiday = Holiday.Create(
            Guid.NewGuid(),
            request.OrgUnitId,
            request.HolidayDate,
            request.Description.Trim(),
            request.CreatedBy);

        await holidayRepository.AddAsync(holiday);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok($"{request.HolidayDate.ToString("dd-MM-yyyy")} Marked as Holiday");
    }
}
