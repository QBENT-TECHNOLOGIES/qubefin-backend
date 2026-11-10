using MediatR;
using QubeFin.Collection.Application.Collections.Commands;
using QubeFin.Collection.Application.Collections.Queries;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Persistence.Models.Collection;
using System.Security.Claims;

namespace QubeFin.Collection.Api.Endpoints
{
    public class CollectionEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet("credit-officers", async (ISender sender, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            {
                if (principal.Identity is null || !principal.Identity.IsAuthenticated)
                {
                    return Results.Forbid();
                }

                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetCreditOfficersByLoginUserQuery(employeeId), cancellationToken);
                return result.ToHttpResult();
            }).WithSummary("Get credit officers for the logged-in user")
              .WithDescription("Active 'Credit Officer' designations currently held by an active employee, in the logged-in user's organization unit and every unit below it (HO -> Division -> Area -> Branch).")
              .WithTags("Credit Officers")
            .RequireAuthorization();

            app.MapGet("bc-groups/{designationId:guid}", async (Guid designationId, ISender sender, ClaimsPrincipal principal, CancellationToken cancellationToken) =>
            {
                if (principal.Identity is null || !principal.Identity.IsAuthenticated)
                {
                    return Results.Forbid();
                }

                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetGroupsByCreditOfficerQuery(employeeId, designationId), cancellationToken);
                return result.ToHttpResult();
            }).WithSummary("Get BC groups of a credit officer")
              .WithDescription("BC groups tagged to the credit officer's designation id (from the credit officer list), restricted to the logged-in user's organization unit and every unit below it.")
              .WithTags("Credit Officers")
            .RequireAuthorization();

            app.MapGet("bc-group-loans/{designationId:guid}", async (
                Guid designationId,
                DateOnly collectionDate,
                ISender sender,
                ClaimsPrincipal principal,
                CancellationToken cancellationToken,
                Guid? bcGroupId = null,
                string? searchText = null,
                bool? isClosed = null,
                string? sortOn = null,
                string? sortDirection = null,
                int pageIndex = 0,
                int pageSize = 10) =>
            {
                if (principal.Identity is null || !principal.Identity.IsAuthenticated)
                {
                    return Results.Forbid();
                }

                var searchParam = new BcGroupLoanSearchParam
                {
                    SearchText = searchText,
                    IsClosed = isClosed,
                    SortOn = sortOn ?? string.Empty,
                    SortDirection = sortDirection ?? string.Empty,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };

                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(new GetGroupWiseLoansQuery(employeeId, designationId, bcGroupId, collectionDate, searchParam), cancellationToken);
                return result.ToHttpResult();
            }).WithSummary("Search group wise loans of a credit officer for one collection date")
              .WithDescription("Paged search of the loans held by the members of the BC groups tagged to the credit officer's designation id (from the credit officer list), restricted to the logged-in user's organization unit and every unit below it. collectionDate (yyyy-MM-dd) is required and attaches that date's BC loan collection row to each loan - collectionId, collectionEmiAmount, collectedAmount, isDemandDate, isCollected and collectionRemarks are null / false when the sheet for that date has not been raised yet, so the loan still appears with nothing collected. bcGroupId is optional and narrows the search to one of the officer's groups. searchText matches loan no, member code, member name or mobile no. isClosed null returns both running and closed loans. sortOn accepts loanNo, groupName, memberName, memberCode, loanProductName, loanAmount, disburseDate, currentOutstandingAmount, overdueAmount or dpdDays; anything else falls back to group name then loan no. pageSize is capped at 200.")
              .WithTags("Group Loans")
            .RequireAuthorization();

            app.MapGet("bc-group-loans-payment/{designationId:guid}", async (
    Guid designationId,
    Guid bcGroupId,
    Guid loanId,
    ISender sender,
    ClaimsPrincipal principal,
    CancellationToken cancellationToken) =>
            {
                if (principal.Identity is null || !principal.Identity.IsAuthenticated)
                {
                    return Results.Forbid();
                }

                var employeeId = principal.Identity.GetEmployeeId();
                var result = await sender.Send(
                    new GetGroupWiseLoanDetailQuery(employeeId, designationId, bcGroupId, loanId),
                    cancellationToken);

                return result.ToHttpResult();
            }).WithSummary("Get one group wise loan of a credit officer for one collection date")
  .WithDescription("Returns a single loan (loanId) of the BC group (bcGroupId) tagged to the credit officer's designation id, restricted to the logged-in user's organization unit and every unit below it. collectionDate (yyyy-MM-dd) is required and attaches that date's BC loan collection row - collectionId, collectionEmiAmount, collectedAmount, isDemandDate, isCollected and collectionRemarks are null / false when the sheet for that date has not been raised yet. Returns not found when the loan does not belong to the given group / officer / unit.")
  .WithTags("Group Loans")
  .RequireAuthorization();

            app.MapPost("employees/search-loan-by-text", async (IMediator mediator, string searchText) =>
            {
                var result = await mediator.Send(new GetBcLoanBySearchTextQuery(searchText));
                return result.ToHttpResult();
            })
 .WithSummary("Search bc loan by Text")
 .RequireAuthorization();


            app.MapPost("save-bc-collection", async (
                SaveBcCollectionRequest request,
                ISender sender,
                ClaimsPrincipal principal,
                CancellationToken cancellationToken) =>
            {
                if (principal.Identity is null || !principal.Identity.IsAuthenticated)
                {
                    return Results.Forbid();
                }

                var employeeId = principal.Identity.GetEmployeeId();
                var userId = principal.Identity.GetUserId();
                var result = await sender.Send(new SaveBcCollectionCommand(employeeId, userId, request), cancellationToken);
                return result.ToHttpResult();
            }).WithSummary("Save the collection of one BC group for one collection date")
              .WithDescription("Saves the collected amount of each loan of a BC group (designationId and bcGroupId in the body) for collectionDate (yyyy-MM-dd) through [BC].[USP_BCLoanCollection]. Every loan must belong to the group, be running, have an EMI due on that date and not be collected already. Remarks are required when the amount is less than the EMI, and UTR No is required for a UPI collection.")
              .WithTags("Group Collections")
            .RequireAuthorization();
        }
    }
}
