using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Hrms.Application.InterviewProcess.Commands;

/// <summary>
/// Uploads the candidate's signed/returned joining letter and stores the returned file reference on the
/// candidate record. Mirrors <see cref="UploadCandidateInterviewFormatCommand"/>.
/// </summary>
public record UploadJoiningLetterCommand(Guid CandidateId, CandidateJoiningLetterUploadRequest Upload, Guid ModifiedBy) : IRequest<Result<string>>;

public class UploadJoiningLetterCommandValidator : AbstractValidator<UploadJoiningLetterCommand>
{
    public UploadJoiningLetterCommandValidator()
    {
        RuleFor(x => x.CandidateId).NotEmpty().WithMessage("Candidate is required.");
        RuleFor(x => x.Upload.Attachment).NotNull().WithMessage("A file is required.");
    }
}

internal sealed class UploadJoiningLetterCommandHandler(
    ICandidateRepository candidateRepository,
    IFileStorageRepository fileStorageRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UploadJoiningLetterCommand, Result<string>>
{
    public async Task<Result<string>> Handle(UploadJoiningLetterCommand request, CancellationToken cancellationToken)
    {
        if (request.ModifiedBy == Guid.Empty)
        {
            return new ValidationError("Authenticated user is required.");
        }

        var candidate = await candidateRepository.GetByIdAsync(request.CandidateId);
        if (candidate is null)
        {
            return new RecordNotFoundError("Candidate not found.");
        }

        var file = request.Upload.Attachment;
        if (file is null || file.Length == 0)
        {
            return new ValidationError("A file is required.");
        }

        string fileKey;
        try
        {
            await using var stream = file.OpenReadStream();
            fileKey = await fileStorageRepository.UploadFileAsync(
                stream,
                file.FileName,
                file.ContentType ?? "application/octet-stream",
                cancellationToken);
        }
        catch (Exception)
        {
            return new ValidationError("Unable to upload the file. Please try again.");
        }

        candidate.SetSignedJoiningLetterFile(fileKey, request.ModifiedBy);

        await candidateRepository.UpdateAsync(candidate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(fileKey);
    }
}
