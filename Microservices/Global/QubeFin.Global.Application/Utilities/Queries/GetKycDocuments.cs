using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using QubeFin.Persistence;

namespace QubeFin.Global.Application.OrganizationUnitTypes.Queries;

#region --- QUERY ---
public record GetKycDocumentsQuery : IRequest<Result<List<GetKycDocumentResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetKycDocumentResponse(Guid Id, string Name, bool IsMandatory, bool IsIdentityProof, bool IsAddressProof, bool IsDateValidate, int Sequence);
#endregion

#region --- HANDLER ---
internal sealed class GetKycDocumentsQueryHandler(QubeFinDataContext context)
    : IRequestHandler<GetKycDocumentsQuery, Result<List<GetKycDocumentResponse>>>
{
    public async Task<Result<List<GetKycDocumentResponse>>> Handle(GetKycDocumentsQuery request, CancellationToken cancellationToken)
    {
        var utilities = await context
            .TblKycDocuments
            .OrderBy(a => a.Sequence)
            .Select(a => new GetKycDocumentResponse(a.Id, a.Name,a.IsMandatory, a.IsIdentityProof, a.IsAddressProof, a.IsDateValidate, a.Sequence))
            .ToListAsync(cancellationToken);

        return Result.Ok(utilities);
    }
}
#endregion
