using FluentResults;
using FluentValidation;
using MediatR;
using QubeFin.Core.Results;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Application.SurveyDocuments.Commands;

#region --- COMMAND ---
public record DeleteSurveyDocumentCommand(Guid SurveyId, Guid DocumentId, Guid EmployeeId) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class DeleteSurveyDocumentCommandValidator : AbstractValidator<DeleteSurveyDocumentCommand>
{
    public DeleteSurveyDocumentCommandValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
        RuleFor(v => v.DocumentId).NotEmpty().WithMessage("Document Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class DeleteSurveyDocumentCommandHandler(
    ISurveyRepository surveyRepository,
    ISurveyDocumentRepository surveyDocumentRepository,
    IFileStorageRepository fileStorageRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteSurveyDocumentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(DeleteSurveyDocumentCommand request, CancellationToken cancellationToken)
    {
        var survey = await surveyRepository.GetByIdAsync(request.SurveyId);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        if (survey.Status != SurveyStatuses.Assigned && survey.Status != SurveyStatuses.Draft)
            return new ValidationError("Photographs cannot be changed after the survey is submitted.");

        var isLeadSurveyor = survey.SurveyAssigneds.Any(a => a.EmployeeId == request.EmployeeId && a.IsLead);
        if (!isLeadSurveyor)
            return new ForbiddenError("Only the Lead Surveyor can remove photographs.");

        // The photograph must belong to this survey.
        var document = await surveyDocumentRepository.GetByIdAsync(request.DocumentId);
        if (document is null || document.SurveyId != request.SurveyId)
            return new RecordNotFoundError("Photograph not found.");

        var storedName = document.FileSequence;
        surveyDocumentRepository.Remove(document);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        // The database row is gone, now remove the stored file too.
        await fileStorageRepository.DeleteFileAsync(storedName, $"SurveyPhotos/{request.SurveyId}", cancellationToken);

        return Result.Ok("Photograph removed successfully.");
    }
}
#endregion
