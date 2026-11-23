using FluentValidation;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyUnit.Models
{
    // Rules that only need the request itself. Shared by the Create and Update commands.
    public class SurveyRequestValidator : AbstractValidator<SurveyRequest>
    {
        public SurveyRequestValidator()
        {
            RuleFor(v => v.SurveyType).NotEmpty().WithMessage("Survey type is required.");
            RuleFor(v => v.SurveyType)
                .Must(type => type == SurveyTypes.Branch || type == SurveyTypes.Group)
                .WithMessage("Survey type must be 'Branch Survey' or 'Group Survey'.")
                .When(v => !string.IsNullOrEmpty(v.SurveyType));
            RuleFor(v => v.AssignmentDate).NotEmpty().WithMessage("Assignment date is required.");
            RuleFor(v => v.ProposedArea).NotEmpty().WithMessage("Proposed area is required.");
            RuleFor(v => v.AdministrativeUnitId).NotEmpty().WithMessage("Administrative unit ID is required.");
            RuleFor(v => v.TentativeSubmissionDate).NotEmpty().WithMessage("Tentative submission date is required.");
            RuleFor(v => v.BranchOrganizationUnitId).NotNull().WithMessage("Branch is required for a Group Survey.")
                .When(v => v.SurveyType == SurveyTypes.Group);

            RuleFor(v => v.SurveyAssigneds).NotEmpty().WithMessage("At least one employee must be assigned.");
            RuleFor(v => v.SurveyAssigneds)
                .Must(assigneds => assigneds.Count(a => a.IsLead) == 1)
                .WithMessage("Exactly one assigned employee must be the lead.")
                .When(v => v.SurveyAssigneds.Count > 0);
            RuleFor(v => v.SurveyAssigneds)
                .Must(assigneds => assigneds.Select(a => a.EmployeeId).Distinct().Count() == assigneds.Count)
                .WithMessage("The same employee cannot be assigned twice.");
        }
    }
}
