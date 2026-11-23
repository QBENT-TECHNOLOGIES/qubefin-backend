using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyDocuments.Commands;

#region --- COMMAND ---
public record UploadSurveyDocumentCommand(
    Guid SurveyId,
    Stream FileStream,
    string FileName,
    string ContentType,
    long FileSize,
    Guid UserId,
    Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class UploadSurveyDocumentCommandValidator : AbstractValidator<UploadSurveyDocumentCommand>
{
    private const long MaxFileSizeInBytes = 5 * 1024 * 1024;   // 5 MB
    private static readonly string[] AllowedContentTypes = ["image/jpeg", "image/png"];

    public UploadSurveyDocumentCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
        RuleFor(v => v.FileName).NotEmpty().WithMessage("A file is required.");
        RuleFor(v => v.FileSize).GreaterThan(0).WithMessage("The file is empty.");
        RuleFor(v => v.FileSize).LessThanOrEqualTo(MaxFileSizeInBytes).WithMessage("The photograph can be at most 5 MB.");
        RuleFor(v => v.ContentType)
            .Must(type => AllowedContentTypes.Contains(type))
            .WithMessage("Only JPG and PNG photographs are allowed.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class UploadSurveyDocumentCommandHandler(
    ISurveyRepository surveyRepository,
    ISurveyDocumentRepository surveyDocumentRepository,
    IFileStorageRepository fileStorageRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UploadSurveyDocumentCommand, Result<string>>
{
    private const int MaxPhotographs = 10;

    public async Task<Result<string>> Handle(UploadSurveyDocumentCommand request, CancellationToken cancellationToken)
    {
        var survey = await surveyRepository.GetByIdAsync(request.SurveyId);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        // Photographs can be added only by the Lead Surveyor, and only until the survey is submitted.
        if (survey.Status != SurveyStatuses.Assigned && survey.Status != SurveyStatuses.Draft)
            return new ValidationError("Photographs cannot be changed after the survey is submitted.");

        var isLeadSurveyor = survey.SurveyAssigneds.Any(a => a.EmployeeId == request.EmployeeId && a.IsLead);
        if (!isLeadSurveyor)
            return new ForbiddenError("Only the Lead Surveyor can upload photographs.");

        if (await surveyDocumentRepository.CountAsync(request.SurveyId) >= MaxPhotographs)
            return new ValidationError($"You can upload at most {MaxPhotographs} photographs.");

        // Keep only the file name (no folders) as the name people will see.
        var displayName = Path.GetFileName(request.FileName);
        var storedName = await fileStorageRepository.UploadFileAsync(request.FileStream, displayName, $"SurveyPhotos/{request.SurveyId}", cancellationToken);

        await surveyDocumentRepository.AddAsync(new TblSurveyDocument
        {
            Id = Guid.NewGuid(),
            SurveyId = request.SurveyId,
            FileName = displayName,
            FileSequence = storedName,
            DocumentType = "Photograph",
            CreatedBy = request.UserId,
            CreatedOn = DateTime.Now
        });
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok("Photograph uploaded successfully.");
    }
}
#endregion
