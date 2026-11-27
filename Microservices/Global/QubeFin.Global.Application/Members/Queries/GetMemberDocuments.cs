using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Global.Persistence.Repositories;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.Members.Queries;

#region --- QUERY ---
public record GetMemberDocumentsQuery(Guid MemberId)
    : IRequest<Result<List<MemberDocumentResponse>>>;
#endregion

#region --- VALIDATOR ---
public class GetMemberDocumentsQueryValidator : AbstractValidator<GetMemberDocumentsQuery>
{
    public GetMemberDocumentsQueryValidator()
    {
        RuleFor(v => v.MemberId).NotEmpty().WithMessage("Member Id is required.");
    }
}
#endregion

#region --- RESPONSE ---
public record MemberDocumentResponse(
    Guid Id,
    Guid? MemberId,
    Guid KycDocumentId,
    string KycDocumentName,
    bool IsDateValidate,
    string DocumentNo,
    DateOnly? IssueDate,
    DateOnly? ExpiryDate,
    string? FontSideFileName,
    string? FontSideFileSequence,
    string? FontSideFileUrl,
    string? BackSideFileName,
    string? BackSideFileSequence,
    string? BackSideFileUrl,
    bool IsVerify,
    string? Remarks
);
#endregion

#region --- HANDLER ---
internal sealed class GetMemberDocumentsQueryHandler(
    QubeFinDataContext context,
    IFileStorageRepository fileStorageRepository)
    : IRequestHandler<GetMemberDocumentsQuery, Result<List<MemberDocumentResponse>>>
{
    private const string KycFolder = "MemberKyc";

    public async Task<Result<List<MemberDocumentResponse>>> Handle(
        GetMemberDocumentsQuery request,
        CancellationToken cancellationToken)
    {
        var docs = await context.TblMemberDocuments
            .AsNoTracking()
            .Include(d => d.KycDocument)
            .Where(d => d.MemberId == request.MemberId && d.CoBorrowerId == null)
            .OrderBy(d => d.KycDocument.Sequence)
            .ToListAsync(cancellationToken);

        var responses = new List<MemberDocumentResponse>(docs.Count);

        foreach (var d in docs)
        {
            var fontUrl = !string.IsNullOrWhiteSpace(d.FontSideFileSequence)
                ? await fileStorageRepository.GetFileUrlAsync(d.FontSideFileSequence, KycFolder)
                : null;

            var backUrl = !string.IsNullOrWhiteSpace(d.BackSideFileSequence)
                ? await fileStorageRepository.GetFileUrlAsync(d.BackSideFileSequence, KycFolder)
                : null;

            responses.Add(new MemberDocumentResponse(
                d.Id,
                d.MemberId,
                d.KycDocumentId,
                d.KycDocument.Name,
                d.KycDocument.IsDateValidate,
                d.DocumentNo,
                d.IssueDate,
                d.ExpiryDate,
                d.FontSideFileName,
                d.FontSideFileSequence,
                fontUrl,
                d.BackSideFileName,
                d.BackSideFileSequence,
                backUrl,
                d.IsVerify,
                d.Remarks
            ));
        }

        return Result.Ok(responses);
    }
}
#endregion