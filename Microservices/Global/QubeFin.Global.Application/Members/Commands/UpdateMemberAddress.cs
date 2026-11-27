using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Global.Application.Members.Models;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;

namespace QubeFin.Global.Application.Members.Commands;

#region --- COMMAND ---
public record UpdateMemberAddressCommand(
    Guid MemberId,
    List<MemberAddressRequest> Addresses,
    Guid LastModifiedBy
) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class UpdateMemberAddressCommandValidator : AbstractValidator<UpdateMemberAddressCommand>
{
    public UpdateMemberAddressCommandValidator()
    {
        RuleFor(x => x.MemberId)
            .NotEmpty()
            .WithMessage("Member Id is required.");

        RuleFor(x => x.Addresses)
            .NotNull()
            .NotEmpty()
            .WithMessage("At least one address is required.");

        RuleFor(x => x.Addresses)
            .Must(list => list.Count(a => a.IsDefault) == 1)
            .When(x => x.Addresses is not null && x.Addresses.Count > 0)
            .WithMessage("Exactly one address must be marked as default.");

        RuleForEach(x => x.Addresses).ChildRules(address =>
        {
            // Present Address (All Required)
            address.RuleFor(a => a.PresentHouseNo).NotEmpty().WithMessage("Present House/Plot No is required.");
            address.RuleFor(a => a.PresentStreetName).NotEmpty().WithMessage("Present Road/Street Name is required.");
            address.RuleFor(a => a.PresentCity).NotEmpty().WithMessage("Present City/Village is required.");
            address.RuleFor(a => a.PresentAdministrativeUnitId).NotEmpty().WithMessage("Present Administrative Unit is required.");
            address.RuleFor(a => a.PresentPoliceStationId).NotEmpty().WithMessage("Present Police Station is required.");
            address.RuleFor(a => a.PresentPostOffice).NotEmpty().WithMessage("Present Post Office is required.");
            address.RuleFor(a => a.PresentPinCode).NotEmpty().Matches("^[0-9]{6}$").WithMessage("Present Pin Code must be a valid 6-digit number.");
            address.RuleFor(a => a.PresentNearestLandmark).NotEmpty().WithMessage("Present Nearest Landmark is required.");
            address.RuleFor(a => a.PresentResidenceYear).NotNull().GreaterThanOrEqualTo(0).WithMessage("Present Residence Year is required.");

            // Permanent Address (All Required)
            address.RuleFor(a => a.ParmanentHouseNo).NotEmpty().WithMessage("Permanent House/Plot No is required.");
            address.RuleFor(a => a.ParmanentStreetName).NotEmpty().WithMessage("Permanent Road/Street Name is required.");
            address.RuleFor(a => a.ParmanentCity).NotEmpty().WithMessage("Permanent City/Village is required.");
            address.RuleFor(a => a.ParmanentAdministrativeUnitId).NotEmpty().WithMessage("Permanent Administrative Unit is required.");
            address.RuleFor(a => a.ParmanentPoliceStationId).NotEmpty().WithMessage("Permanent Police Station is required.");
            address.RuleFor(a => a.ParmanentPostOffice).NotEmpty().WithMessage("Permanent Post Office is required.");
            address.RuleFor(a => a.ParmanentPinCode).NotEmpty().Matches("^[0-9]{6}$").WithMessage("Permanent Pin Code must be a valid 6-digit number.");
            address.RuleFor(a => a.ParmanentNearestLandmark).NotEmpty().WithMessage("Permanent Nearest Landmark is required.");
            address.RuleFor(a => a.ParmanentResidenceYear).NotNull().GreaterThanOrEqualTo(0).WithMessage("Permanent Residence Year is required.");
        });
    }
}
#endregion

#region --- HANDLER ---
internal sealed class UpdateMemberAddressCommandHandler(
    IMemberRepository memberRepository,
    IUnitOfWork unitOfWork,
    QubeFinDataContext context)
    : IRequestHandler<UpdateMemberAddressCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        UpdateMemberAddressCommand request,
        CancellationToken cancellationToken)
    {
        var existingMember = await memberRepository.GetByIdAsync(request.MemberId);
        if (existingMember is null)
        {
            return new ValidationError("Member does not exist with the given id.");
        }

        var existingAddresses = await context.TblMemberAddresses
            .Where(a => a.MemberId == request.MemberId)
            .ToListAsync(cancellationToken);

        var incomingIds = request.Addresses
            .Where(a => a.Id.HasValue && a.Id.Value != Guid.Empty)
            .Select(a => a.Id!.Value)
            .ToHashSet();

        // Remove deleted rows
        var toRemove = existingAddresses
            .Where(a => !incomingIds.Contains(a.Id))
            .ToList();

        if (toRemove.Count > 0)
        {
            context.TblMemberAddresses.RemoveRange(toRemove);
        }

        // Kono address default na thakle prothom-take default kore dao
        var hasAnyDefault = request.Addresses.Any(a => a.IsDefault);

        for (int i = 0; i < request.Addresses.Count; i++)
        {
            var req = request.Addresses[i];
            bool isDefault = hasAnyDefault ? req.IsDefault : i == 0;

            var presentHouseNo = req.PresentPermanentAddressSame ? req.ParmanentHouseNo : req.PresentHouseNo;
            var presentStreetName = req.PresentPermanentAddressSame ? req.ParmanentStreetName : req.PresentStreetName;
            var presentCity = req.PresentPermanentAddressSame ? req.ParmanentCity : req.PresentCity;
            var presentAdminUnitId = req.PresentPermanentAddressSame ? req.ParmanentAdministrativeUnitId : req.PresentAdministrativeUnitId;
            var presentPoliceStationId = req.PresentPermanentAddressSame ? req.ParmanentPoliceStationId : req.PresentPoliceStationId;
            var presentPostOffice = req.PresentPermanentAddressSame ? req.ParmanentPostOffice : req.PresentPostOffice;
            var presentPinCode = req.PresentPermanentAddressSame ? req.ParmanentPinCode : req.PresentPinCode;
            var presentLandmark = req.PresentPermanentAddressSame ? req.ParmanentNearestLandmark : req.PresentNearestLandmark;
            var presentResidenceYear = req.PresentPermanentAddressSame ? req.ParmanentResidenceYear : req.PresentResidenceYear;

            var existing = (req.Id.HasValue && req.Id.Value != Guid.Empty)
                ? existingAddresses.FirstOrDefault(a => a.Id == req.Id.Value)
                : null;

            if (existing is not null)
            {
                existing.ParmanentHouseNo = req.ParmanentHouseNo?.Trim();
                existing.ParmanentStreetName = req.ParmanentStreetName?.Trim();
                existing.ParmanentCity = req.ParmanentCity?.Trim();
                existing.ParmanentAdministrativeUnitId = req.ParmanentAdministrativeUnitId;
                existing.ParmanentPoliceStationId = req.ParmanentPoliceStationId;
                existing.ParmanentPostOffice = req.ParmanentPostOffice?.Trim();
                existing.ParmanentPinCode = req.ParmanentPinCode?.Trim();
                existing.ParmanentNearestLandmark = req.ParmanentNearestLandmark?.Trim();
                existing.ParmanentResidenceYear = req.ParmanentResidenceYear;
                existing.PresentPermanentAddressSame = req.PresentPermanentAddressSame;
                existing.PresentHouseNo = presentHouseNo?.Trim();
                existing.PresentStreetName = presentStreetName?.Trim();
                existing.PresentCity = presentCity?.Trim();
                existing.PresentAdministrativeUnitId = presentAdminUnitId;
                existing.PresentPoliceStationId = presentPoliceStationId;
                existing.PresentPostOffice = presentPostOffice?.Trim();
                existing.PresentPinCode = presentPinCode?.Trim();
                existing.PresentNearestLandmark = presentLandmark?.Trim();
                existing.PresentResidenceYear = presentResidenceYear;
                existing.IsDefault = isDefault;
            }
            else
            {
                context.TblMemberAddresses.Add(new TblMemberAddress
                {
                    Id = Guid.NewGuid(),
                    MemberId = request.MemberId,
                    ParmanentHouseNo = req.ParmanentHouseNo?.Trim(),
                    ParmanentStreetName = req.ParmanentStreetName?.Trim(),
                    ParmanentCity = req.ParmanentCity?.Trim(),
                    ParmanentAdministrativeUnitId = req.ParmanentAdministrativeUnitId,
                    ParmanentPoliceStationId = req.ParmanentPoliceStationId,
                    ParmanentPostOffice = req.ParmanentPostOffice?.Trim(),
                    ParmanentPinCode = req.ParmanentPinCode?.Trim(),
                    ParmanentNearestLandmark = req.ParmanentNearestLandmark?.Trim(),
                    ParmanentResidenceYear = req.ParmanentResidenceYear,
                    PresentPermanentAddressSame = req.PresentPermanentAddressSame,
                    PresentHouseNo = presentHouseNo?.Trim(),
                    PresentStreetName = presentStreetName?.Trim(),
                    PresentCity = presentCity?.Trim(),
                    PresentAdministrativeUnitId = presentAdminUnitId,
                    PresentPoliceStationId = presentPoliceStationId,
                    PresentPostOffice = presentPostOffice?.Trim(),
                    PresentPinCode = presentPinCode?.Trim(),
                    PresentNearestLandmark = presentLandmark?.Trim(),
                    PresentResidenceYear = presentResidenceYear,
                    IsDefault = isDefault
                });
            }
        }

        existingMember.SetModified(request.LastModifiedBy);
        await memberRepository.UpdateAsync(existingMember);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var fullName = $"{existingMember.PersonalInfo.FirstName} {existingMember.PersonalInfo.LastName}".Trim();
        return Result.Ok($"Member address information updated successfully for Name : {fullName}");
    }
}
#endregion