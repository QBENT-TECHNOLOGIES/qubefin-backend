using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Global.Application.SurveyDocuments.Models;
using QubeFin.Global.Application.SurveyUnit;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.SurveyDocuments.Queries;

#region --- QUERY ---
public record GetSurveyDocumentsQuery(Guid SurveyId, Guid UserId, Guid EmployeeId) : IRequest<Result<List<SurveyDocumentResponse>>>;
#endregion

#region --- VALIDATOR ---
public class GetSurveyDocumentsQueryValidator : AbstractValidator<GetSurveyDocumentsQuery>
{
    public GetSurveyDocumentsQueryValidator()
    {
        RuleFor(v => v.SurveyId).NotEmpty().WithMessage("Survey Id is required.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetSurveyDocumentsQueryHandler(
    QubeFinDataContext context,
    ISurveyDocumentRepository surveyDocumentRepository,
    IFileStorageRepository fileStorageRepository) : IRequestHandler<GetSurveyDocumentsQuery, Result<List<SurveyDocumentResponse>>>
{
    public async Task<Result<List<SurveyDocumentResponse>>> Handle(GetSurveyDocumentsQuery request, CancellationToken cancellationToken)
    {
        var survey = await context.TblSurveys
            .Include(s => s.TblSurveyAssigneds)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.SurveyId, cancellationToken);
        if (survey is null)
            return new RecordNotFoundError("Survey not found.");

        // The photographs can be seen by the same people who can see the survey.
        if (!await SurveyAccess.CanViewAsync(context, survey, request.UserId, request.EmployeeId, cancellationToken))
            return new ForbiddenError("You do not have access to this survey.");

        var documents = await surveyDocumentRepository.GetBySurveyIdAsync(request.SurveyId);

        var responses = new List<SurveyDocumentResponse>();
        foreach (var document in documents)
        {
            responses.Add(new SurveyDocumentResponse
            {
                Id = document.Id,
                FileName = document.FileName,
                DocumentType = document.DocumentType,
                Url = await fileStorageRepository.GetFileUrlAsync(document.FileSequence, $"SurveyPhotos/{request.SurveyId}"),
                CreatedOn = document.CreatedOn
            });
        }
        return responses;
    }
}
#endregion
