using FluentValidation;

namespace QubeFin.Global.Application.BranchSurveys.Models
{
    // What the Committee Lead sends from the Create Branch window.
    public class CreateBranchRequest
    {
        public string Name { get; set; } = string.Empty;
        public Guid ParentId { get; set; }
        public Guid CompanyId { get; set; }
        public Guid DistrictId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public TimeOnly? AttendanceInTime { get; set; }
        public TimeOnly? AttendanceOutTime { get; set; }
        public int? CheckRadiusInMeter { get; set; }
    }

    public class CreateBranchRequestValidator : AbstractValidator<CreateBranchRequest>
    {
        public CreateBranchRequestValidator()
        {
            RuleFor(r => r.Name).NotEmpty().WithMessage("Branch name is required.")
                .MaximumLength(50).WithMessage("Branch name cannot exceed 50 characters.");
            RuleFor(r => r.ParentId).NotEmpty().WithMessage("Parent unit is required.");
            RuleFor(r => r.CompanyId).NotEmpty().WithMessage("Company is required.");
            RuleFor(r => r.DistrictId).NotEmpty().WithMessage("District is required.");
            RuleFor(r => r.CheckRadiusInMeter).GreaterThan(0).When(r => r.CheckRadiusInMeter.HasValue).WithMessage("Check radius must be greater than 0.");
            RuleFor(r => r.Latitude).InclusiveBetween(-90, 90).When(r => r.Latitude.HasValue).WithMessage("Latitude must be between -90 and 90.");
            RuleFor(r => r.Longitude).InclusiveBetween(-180, 180).When(r => r.Longitude.HasValue).WithMessage("Longitude must be between -180 and 180.");
        }
    }

    // What the Create Branch window needs, and what the survey page shows after the branch exists.
    public class BranchCreateInfoResponse
    {
        public Guid SurveyId { get; set; }
        // The Committee Lead, while the survey is Approved and no branch exists yet.
        public bool CanCreate { get; set; }
        // Pre-filled values for the window.
        public string SuggestedName { get; set; } = string.Empty;
        public Guid? StateId { get; set; }
        public string? StateName { get; set; }
        public Guid? DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        // Filled after the branch is created.
        public Guid? BranchId { get; set; }
        public string? BranchName { get; set; }
        public int? BranchCode { get; set; }
    }
}
