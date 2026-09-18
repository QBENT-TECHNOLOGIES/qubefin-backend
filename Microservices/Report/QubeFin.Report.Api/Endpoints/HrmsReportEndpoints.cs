using MediatR;
using Microsoft.AspNetCore.Authorization;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Report.Application.Reports.Generate.SSRSReports;

namespace QubeFin.Report.Api.Endpoints
{
    public class HrmsReportEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            //
            #region SSRS Interview Proccess REPORTS
            app.MapGet("/interview/wegrow-interview-letter/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender) =>
            {
                var command = new GenerateSSRSReportsCommand(
                "Rpt_WegrowInterviewLetter",
                "PDF",
                new Dictionary<string, string>
                {
                    ["CandidateId"] = candidateId.ToString()
                });

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;
                return Results.File(file.FileStream, file.ContentType, file.FileName);
            }).WithSummary("Generate wegrow interview letter.");

            app.MapGet("/interview/weegrobc-interview-letter/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender) =>
            {
                var command = new GenerateSSRSReportsCommand(
                "Rpt_WegroBc_InterviewLetter",
                "PDF",
                new Dictionary<string, string>
                {
                    ["CandidateId"] = candidateId.ToString()
                });

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;
                return Results.File(file.FileStream, file.ContentType, file.FileName);
            }).WithSummary("Generate Wegro BC interview letter.");

            app.MapGet("/interview/wegrow-offer-letter/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender) =>
            {
                var command = new GenerateSSRSReportsCommand(
                "Rpt_Wegrow_OfferLetter",
                "PDF",
                new Dictionary<string, string>
                {
                    ["CandidateId"] = candidateId.ToString()
                });

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;
                return Results.File(file.FileStream, file.ContentType, file.FileName);
            }).WithSummary("Generate wegrow offer letter.");

            app.MapGet("/interview/weegrobc-offer-letter/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender) =>
            {
                var command = new GenerateSSRSReportsCommand(
                "Rpt_WegroBC_OfferLetter",
                "PDF",
                new Dictionary<string, string>
                {
                    ["CandidateId"] = candidateId.ToString()
                });

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;
                return Results.File(file.FileStream, file.ContentType, file.FileName);
            }).WithSummary("Generate Wegro BC offer letter.");

            app.MapGet("/interview/wegrow-appointment-letter/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender) =>
            {
                var command = new GenerateSSRSReportsCommand(
                "Rpt_Wegrow_AppointmentLetter",
                "PDF",
                new Dictionary<string, string>
                {
                    ["CandidateId"] = candidateId.ToString()
                });

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;
                return Results.File(file.FileStream, file.ContentType, file.FileName);
            }).WithSummary("Generate Wegrow appointment letter.");

            app.MapGet("/interview/weegrobc-appoinment-letter/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender) =>
            {
                var command = new GenerateSSRSReportsCommand(
                "Rpt_WegroBc_AppointmentLetter",
                "PDF",
                new Dictionary<string, string>
                {
                    ["CandidateId"] = candidateId.ToString()
                });

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;
                return Results.File(file.FileStream, file.ContentType, file.FileName);
            }).WithSummary("Generate Wegro BC appointment letter.");

            app.MapGet("/interview/wegrow-joining-letter/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender) =>
            {
                var command = new GenerateSSRSReportsCommand(
                "Rpt_Wegrow_JoiningLetter",
                "PDF",
                new Dictionary<string, string>
                {
                    ["CandidateId"] = candidateId.ToString()
                });

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;
                return Results.File(file.FileStream, file.ContentType, file.FileName);
            }).WithSummary("Generate Wegrow joining letter.");

            app.MapGet("/interview/weegrobc-joining-letter/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender) =>
            {
                var command = new GenerateSSRSReportsCommand(
                "Rpt_WeegroBc_JoiningLetter",
                "PDF",
                new Dictionary<string, string>
                {
                    ["CandidateId"] = candidateId.ToString()
                });

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;
                return Results.File(file.FileStream, file.ContentType, file.FileName);
            }).WithSummary("Generate Wegro BC joining letter.");

            app.MapGet("/interview/wegrow-personality-form/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender) =>
            {
                var command = new GenerateSSRSReportsCommand(
                "Rpt_Wegrow_PersonalityInventoryForm",
                "PDF",
                new Dictionary<string, string>
                {
                    ["CandidateId"] = candidateId.ToString()
                });

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;
                return Results.File(file.FileStream, file.ContentType, file.FileName);
            }).WithSummary("Generate Wegrow personality form.");

            app.MapGet("/interview/weegrobc-personality-form/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender) =>
            {
                var command = new GenerateSSRSReportsCommand(
                "Rpt_WeegroBC_PersonalityInventoryForm",
                "PDF",
                new Dictionary<string, string>
                {
                    ["CandidateId"] = candidateId.ToString()
                });

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;
                return Results.File(file.FileStream, file.ContentType, file.FileName);
            }).WithSummary("Generate Wegro BC personality form.");

            app.MapGet("/interview/wegrow-interviewpanel-acknowledgement/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender) =>
            {
                var command = new GenerateSSRSReportsCommand(
                "Rpt_Wegrow_InterviewPanelAcknowledgement",
                "PDF",
                new Dictionary<string, string>
                {
                    ["CandidateId"] = candidateId.ToString()
                });

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;
                return Results.File(file.FileStream, file.ContentType, file.FileName);
            }).WithSummary("Generate Wegrow interview panel acknowledgement.");

            app.MapGet("/interview/weegrobc-interviewpanel-acknowledgement/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender) =>
            {
                var command = new GenerateSSRSReportsCommand(
                "Rpt_WeegroBC_InterviewPanelAcknowledgement",
                "PDF",
                new Dictionary<string, string>
                {
                    ["CandidateId"] = candidateId.ToString()
                });

                var result = await sender.Send(command);

                if (result.IsFailed)
                    return result.ToHttpResult();

                var file = result.Value;
                return Results.File(file.FileStream, file.ContentType, file.FileName);
            }).WithSummary("Generate Wegro BC interview panel acknowledgement.");

            #endregion
        }
    }
}
