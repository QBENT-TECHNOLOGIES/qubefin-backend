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
                    "Rpt_WegroBC_WelcomeLetter",
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

            #endregion
        }
    }
}
