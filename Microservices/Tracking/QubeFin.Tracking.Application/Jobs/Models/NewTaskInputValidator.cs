using FluentValidation;
using QubeFin.Tracking.Application.Enums;

namespace QubeFin.Tracking.Application.Jobs.Models;

public class NewTaskInputValidator : AbstractValidator<NewTaskInput>
{
    public NewTaskInputValidator()
    {
        RuleFor(t => t.TaskType).NotNull().WithMessage("Select the task type.");
        RuleFor(t => t.TaskTitle).NotEmpty().WithMessage("Task title is required.");
        RuleFor(t => t.TaskTitle).MaximumLength(200).WithMessage("Task title can be at most 200 characters.");
        RuleFor(t => t.Notes).MaximumLength(100).WithMessage("Instruction can be at most 100 characters.");
        RuleFor(t => t.Target).NotNull().WithMessage("Select where the task must be done.");
        RuleFor(t => t.Target!.Kind).NotNull().WithMessage("Select the kind of place.").When(t => t.Target is not null);
        RuleFor(t => t.Target!.TargetName).NotEmpty().WithMessage("Place name is required.").When(t => t.Target is not null);
        RuleFor(t => t.Target!.TargetName).MaximumLength(50).WithMessage("Place name can be at most 50 characters.").When(t => t.Target is not null);
        RuleFor(t => t.Target!.OriginRefId).NotEmpty().WithMessage("Select the group, member or branch.")
            .When(t => t.Target is not null && t.Target.Kind is not null && t.Target.Kind != TaskTargetKind.Custom);
        RuleFor(t => t.Target!.TargetLocation).NotNull().WithMessage("Pick the point on the map.")
            .When(t => t.Target?.Kind == TaskTargetKind.Custom);
        RuleFor(t => t.Target!.OrganisationUnitId).NotEmpty().WithMessage("Select the branch of the map point.")
            .When(t => t.Target?.Kind == TaskTargetKind.Custom);
    }
}
