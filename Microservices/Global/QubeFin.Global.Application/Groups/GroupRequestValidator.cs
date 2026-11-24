using FluentValidation;
using QubeFin.Global.Application.Groups.Models;

namespace QubeFin.Global.Application.Groups
{
    // The rules of the group fields (Tbl_Group column sizes), used by Add and Edit.
    public class GroupRequestValidator : AbstractValidator<GroupRequest>
    {
        public GroupRequestValidator()
        {
            RuleFor(v => v.DesignationId).NotEmpty().WithMessage("Credit Officer is required.");
            RuleFor(v => v.Name).NotEmpty().WithMessage("Group Name is required.").MaximumLength(50).WithMessage("Group Name can be 50 characters at most.");
            RuleFor(v => v.CollectionDay).InclusiveBetween(1, 7).WithMessage("Collection Day is required.");
            RuleFor(v => v.FormationDate).NotEmpty().WithMessage("Formation Date is required.");
            RuleFor(v => v.MobileNo).NotEmpty().WithMessage("Mobile No is required.").Matches(@"^\d{10}$").WithMessage("Mobile No must be 10 digits.");
            RuleFor(v => v.AdministrativeUnitId).NotEmpty().WithMessage("Area is required.");
            RuleFor(v => v.PinCode).Matches(@"^\d{6}$").When(v => !string.IsNullOrWhiteSpace(v.PinCode)).WithMessage("PIN Code must be 6 digits.");
            RuleFor(v => v.Address).MaximumLength(500).WithMessage("Address can be 500 characters at most.");
            RuleFor(v => v.LandMark).MaximumLength(100).WithMessage("Landmark can be 100 characters at most.");
            RuleFor(v => v.Latitude).InclusiveBetween(-90m, 90m).When(v => v.Latitude.HasValue).WithMessage("Latitude must be between -90 and 90.");
            RuleFor(v => v.Longitude).InclusiveBetween(-180m, 180m).When(v => v.Longitude.HasValue).WithMessage("Longitude must be between -180 and 180.");
            RuleFor(v => v).Must(v => v.Latitude.HasValue == v.Longitude.HasValue).WithMessage("Enter both Latitude and Longitude, or neither.");
        }
    }
}
