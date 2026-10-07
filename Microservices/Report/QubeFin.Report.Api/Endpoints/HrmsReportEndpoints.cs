using MediatR;
using Microsoft.AspNetCore.Authorization;
using QubeFin.Core.Endpoint;
using QubeFin.Core.Identity;
using QubeFin.Core.Results;
using QubeFin.Report.Application.Reports.Generate.SSRSReports;
using QubeFin.Report.Application.Reports.Queries;

namespace QubeFin.Report.Api.Endpoints
{
    public class HrmsReportEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            //
            #region SSRS Interview Proccess REPORTS

            app.MapGet("/interview/interview-letter/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender, IConfiguration configuration) =>
            {
                var company = await sender.Send(new GetCompanyByCandidateIdQuery(candidateId));

                if (company.IsFailed)
                    return company.ToHttpResult();
                else if (company.Value.companyId == Guid.Parse(configuration["Company:Wegrow"]))
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
                }
                else if (company.Value.companyId == Guid.Parse(configuration["Company:WegroBC"]))
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
                }
                else
                {
                    return Results.NotFound("Company not found for the given candidate.");
                }
            }).WithSummary("Generate interview letter.");

            app.MapGet("/interview/offer-letter/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender, IConfiguration configuration) =>
            {
                var company = await sender.Send(new GetCompanyByCandidateIdQuery(candidateId));

                if (company.IsFailed)
                    return company.ToHttpResult();
                else if (company.Value.companyId == Guid.Parse(configuration["Company:Wegrow"]))
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
                }
                else if (company.Value.companyId == Guid.Parse(configuration["Company:WegroBC"]))
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
                }
                else
                {
                    return Results.NotFound("Company not found for the given candidate.");
                }
            }).WithSummary("Generate offer letter.");

            app.MapGet("/interview/appointment-letter/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender, IConfiguration configuration) =>
            {
                var company = await sender.Send(new GetCompanyByCandidateIdQuery(candidateId));

                if (company.IsFailed)
                    return company.ToHttpResult();
                else if (company.Value.companyId == Guid.Parse(configuration["Company:Wegrow"]))
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
                }
                else if (company.Value.companyId == Guid.Parse(configuration["Company:WegroBC"]))
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
                }
                else
                {
                    return Results.NotFound("Company not found for the given candidate.");
                }
            }).WithSummary("Generate appointment letter.");

            app.MapGet("/interview/welcome-letter/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender, IConfiguration configuration) =>
            {
                var company = await sender.Send(new GetCompanyByCandidateIdQuery(candidateId));

                if (company.IsFailed)
                    return company.ToHttpResult();
                else if (company.Value.companyId == Guid.Parse(configuration["Company:Wegrow"]))
                {
                    var command = new GenerateSSRSReportsCommand(
                    "Rpt_Wegrow_WelcomeLetter",
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
                }
                else if (company.Value.companyId == Guid.Parse(configuration["Company:WegroBC"]))
                {
                    var command = new GenerateSSRSReportsCommand(
                    "Rpt_WeegroBC_WelcomeLetter",
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
                }
                else
                {
                    return Results.NotFound("Company not found for the given candidate.");
                }
            }).WithSummary("Generate welcome letter.");

            app.MapGet("/interview/joining-letter/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender, IConfiguration configuration) =>
            {
                var company = await sender.Send(new GetCompanyByCandidateIdQuery(candidateId));

                if (company.IsFailed)
                    return company.ToHttpResult();
                else if (company.Value.companyId == Guid.Parse(configuration["Company:Wegrow"]))
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
                }
                else if (company.Value.companyId == Guid.Parse(configuration["Company:WegroBC"]))
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
                }
                else
                {
                    return Results.NotFound("Company not found for the given candidate.");
                }
            }).WithSummary("Generate joining letter.");

            app.MapGet("/interview/personality-form/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender, IConfiguration configuration) =>
            {
                var company = await sender.Send(new GetCompanyByCandidateIdQuery(candidateId));

                if (company.IsFailed)
                    return company.ToHttpResult();
                else if (company.Value.companyId == Guid.Parse(configuration["Company:Wegrow"]))
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
                }
                else if (company.Value.companyId == Guid.Parse(configuration["Company:WegroBC"]))
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
                }
                else
                {
                    return Results.NotFound("Company not found for the given candidate.");
                }
            }).WithSummary("Generate personality form.");

            app.MapGet("/interview/interviewpanel-acknowledgement/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender, IConfiguration configuration) =>
            {
                var company = await sender.Send(new GetCompanyByCandidateIdQuery(candidateId));

                if (company.IsFailed)
                    return company.ToHttpResult();
                else if (company.Value.companyId == Guid.Parse(configuration["Company:Wegrow"]))
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
                }
                else if (company.Value.companyId == Guid.Parse(configuration["Company:WegroBC"]))
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
                }
                else
                {
                    return Results.NotFound("Company not found for the given candidate.");
                }
            }).WithSummary("Generate interview panel acknowledgement.");

            app.MapGet("/interview/jobapplication-form/{candidateId:guid}", [Authorize] async (Guid candidateId, ISender sender, IConfiguration configuration) =>
            {
                var company = await sender.Send(new GetCompanyByCandidateIdQuery(candidateId));

                if (company.IsFailed)
                    return company.ToHttpResult();
                else if (company.Value.companyId == Guid.Parse(configuration["Company:Wegrow"]))
                {
                    var command = new GenerateSSRSReportsCommand(
                    "Rpt_Wegrow_JobApplication",
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
                }
                else if (company.Value.companyId == Guid.Parse(configuration["Company:WegroBC"]))
                {
                    var command = new GenerateSSRSReportsCommand(
                    "Rpt_WeegroBC_JobApplication",
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
                }
                else
                {
                    return Results.NotFound("Company not found for the given candidate.");
                }
            }).WithSummary("Generate job application form.");

            // Blank job application form for a company - handed to a candidate before they are created, so there is
            // no candidate yet. The same report runs with an empty CandidateId.
            app.MapGet("/interview/jobapplication-form/blank/{companyId:guid}", [Authorize] async (Guid companyId, ISender sender, IConfiguration configuration) =>
            {
                string? reportName = null;
                if (companyId == Guid.Parse(configuration["Company:Wegrow"]!))
                    reportName = "Rpt_Wegrow_JobApplication";
                else if (companyId == Guid.Parse(configuration["Company:WegroBC"]!))
                    reportName = "Rpt_WeegroBC_JobApplication";

                if (reportName is null)
                    return Results.NotFound("No job application form is set up for this company.");

                var command = new GenerateSSRSReportsCommand(
                    reportName,
                    "PDF",
                    new Dictionary<string, string>
                    {
                        ["CandidateId"] = Guid.Empty.ToString()
                    });
                var result = await sender.Send(command);
                if (result.IsFailed)
                    return result.ToHttpResult();
                var file = result.Value;
                return Results.File(file.FileStream, file.ContentType, file.FileName);
            }).WithSummary("Generate a blank job application form for a company.");

            #endregion
        }
    }
}
