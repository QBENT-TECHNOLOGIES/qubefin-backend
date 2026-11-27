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
public record UpdateMemberDocumentCommand(
    Guid MemberId,
    List<MemberDocumentRequest> Documents,
    Guid LastModifiedBy
) : IRequest<Result<string>>;
#endregion

#region --- VALIDATION ---
public class UpdateMemberDocumentCommandValidator : AbstractValidator<UpdateMemberDocumentCommand>
{
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024; // 5 MB
    private static readonly string[] AllowedContentTypes =
    [
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp",
        "application/pdf"
    ];

    public UpdateMemberDocumentCommandValidator()
    {
        RuleFor(x => x.MemberId)
            .NotEmpty()
            .WithMessage("Member Id is required.");

        RuleFor(x => x.Documents)
            .NotNull()
            .NotEmpty()
            .WithMessage("At least one KYC document is required.");

        RuleForEach(x => x.Documents).ChildRules(doc =>
        {
            doc.RuleFor(d => d.KycDocumentId)
                .NotEmpty()
                .WithMessage("KYC Document type is required.");

            doc.RuleFor(d => d.DocumentNo)
                .NotEmpty()
                .WithMessage("Document Number is required.");

            doc.RuleFor(d => d.ExpiryDate)
                .GreaterThanOrEqualTo(d => d.IssueDate)
                .When(d => d.IssueDate.HasValue && d.ExpiryDate.HasValue)
                .WithMessage("Expiry Date must be greater than or equal to Issue Date.");

            doc.When(d => d.FontSideFile is not null, () =>
            {
                doc.RuleFor(d => d.FontSideFile!.Length)
                    .GreaterThan(0).WithMessage("Front side file is empty.")
                    .LessThanOrEqualTo(MaxFileSizeInBytes).WithMessage("Front side file can be at most 5 MB.");

                doc.RuleFor(d => d.FontSideFile!.ContentType)
                    .Must(type => AllowedContentTypes.Contains(type))
                    .WithMessage("Only JPG, PNG, WEBP and PDF files are allowed for front side document.");
            });

            doc.When(d => d.BackSideFile is not null, () =>
            {
                doc.RuleFor(d => d.BackSideFile!.Length)
                    .GreaterThan(0).WithMessage("Back side file is empty.")
                    .LessThanOrEqualTo(MaxFileSizeInBytes).WithMessage("Back side file can be at most 5 MB.");

                doc.RuleFor(d => d.BackSideFile!.ContentType)
                    .Must(type => AllowedContentTypes.Contains(type))
                    .WithMessage("Only JPG, PNG, WEBP and PDF files are allowed for back side document.");
            });
        });
    }
}
#endregion

#region --- HANDLER ---
internal sealed class UpdateMemberDocumentCommandHandler(
    IMemberRepository memberRepository,
    IUnitOfWork unitOfWork,
    QubeFinDataContext context,
    IFileStorageRepository fileStorageRepository)
    : IRequestHandler<UpdateMemberDocumentCommand, Result<string>>
{
    private const string KycFolder = "MemberKyc";

    public async Task<Result<string>> Handle(
        UpdateMemberDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var existingMember = await memberRepository.GetByIdAsync(request.MemberId);
        if (existingMember is null)
        {
            return new ValidationError("Member does not exist with the given id.");
        }

        var existingDocs = await context.TblMemberDocuments
            .Where(d => d.MemberId == request.MemberId && d.CoBorrowerId == null)
            .ToListAsync(cancellationToken);

        var matchedExistingIds = new HashSet<Guid>();

        foreach (var req in request.Documents)
        {
            var existing = (req.Id.HasValue && req.Id.Value != Guid.Empty)
                ? existingDocs.FirstOrDefault(d => d.Id == req.Id.Value)
                : existingDocs.FirstOrDefault(d => d.KycDocumentId == req.KycDocumentId);

            // Front Side File Upload / Preserve / Clear
            string? fontFileName = string.IsNullOrWhiteSpace(req.FontSideFileName)
                ? null
                : Path.GetFileName(req.FontSideFileName.Trim());
            string? fontFileSeq = fontFileName is null
                ? null
                : (!string.IsNullOrWhiteSpace(req.FontSideFileSequence)
                    ? req.FontSideFileSequence
                    : existing?.FontSideFileSequence);

            if (req.FontSideFile is { Length: > 0 })
            {
                var displayName = Path.GetFileName(req.FontSideFile.FileName);
                await using var stream = req.FontSideFile.OpenReadStream();
                fontFileSeq = await fileStorageRepository.UploadFileAsync(
                    stream,
                    displayName,
                    KycFolder,
                    cancellationToken).ConfigureAwait(false);
                fontFileName = displayName;
            }

            // Back Side File Upload / Preserve / Clear
            string? backFileName = string.IsNullOrWhiteSpace(req.BackSideFileName)
                ? null
                : Path.GetFileName(req.BackSideFileName.Trim());
            string? backFileSeq = backFileName is null
                ? null
                : (!string.IsNullOrWhiteSpace(req.BackSideFileSequence)
                    ? req.BackSideFileSequence
                    : existing?.BackSideFileSequence);

            if (req.BackSideFile is { Length: > 0 })
            {
                var displayName = Path.GetFileName(req.BackSideFile.FileName);
                await using var stream = req.BackSideFile.OpenReadStream();
                backFileSeq = await fileStorageRepository.UploadFileAsync(
                    stream,
                    displayName,
                    KycFolder,
                    cancellationToken).ConfigureAwait(false);
                backFileName = displayName;
            }

            DateOnly? issueDate = req.IssueDate.HasValue
                ? DateOnly.FromDateTime(req.IssueDate.Value)
                : null;
            DateOnly? expiryDate = req.ExpiryDate.HasValue
                ? DateOnly.FromDateTime(req.ExpiryDate.Value)
                : null;

            if (existing is not null)
            {
                matchedExistingIds.Add(existing.Id);

                existing.KycDocumentId = req.KycDocumentId;
                existing.DocumentNo = req.DocumentNo.Trim();
                existing.IssueDate = issueDate;
                existing.ExpiryDate = expiryDate;
                existing.FontSideFileName = fontFileName;
                existing.FontSideFileSequence = fontFileSeq;
                existing.BackSideFileName = backFileName;
                existing.BackSideFileSequence = backFileSeq;
                existing.IsVerify = req.IsVerify;
                existing.Remarks = req.Remarks?.Trim();
                existing.LastModifiedBy = request.LastModifiedBy;
                existing.LastModifiedOn = DateTime.Now;
            }
            else
            {
                context.TblMemberDocuments.Add(new TblMemberDocument
                {
                    Id = Guid.NewGuid(),
                    MemberId = request.MemberId,
                    CoBorrowerId = null,
                    KycDocumentId = req.KycDocumentId,
                    DocumentNo = req.DocumentNo.Trim(),
                    IssueDate = issueDate,
                    ExpiryDate = expiryDate,
                    FontSideFileName = fontFileName,
                    FontSideFileSequence = fontFileSeq,
                    BackSideFileName = backFileName,
                    BackSideFileSequence = backFileSeq,
                    IsVerify = req.IsVerify,
                    Remarks = req.Remarks?.Trim(),
                    CreatedBy = request.LastModifiedBy,
                    CreatedOn = DateTime.Now
                });
            }
        }

        // Remove deleted documents (block if verified)
        var toRemove = existingDocs
            .Where(d => !matchedExistingIds.Contains(d.Id))
            .ToList();

        if (toRemove.Any(d => d.IsVerify))
        {
            return new ValidationError("Verified KYC documents cannot be deleted.");
        }

        if (toRemove.Count > 0)
        {
            context.TblMemberDocuments.RemoveRange(toRemove);
        }

        existingMember.SetModified(request.LastModifiedBy);
        await memberRepository.UpdateAsync(existingMember);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var fullName = $"{existingMember.PersonalInfo.FirstName} {existingMember.PersonalInfo.LastName}".Trim();
        return Result.Ok($"Member document information updated successfully for Name : {fullName}");
    }
}
#endregion