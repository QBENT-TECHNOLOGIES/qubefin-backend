using MediatR;
using QubeFin.Core.Endpoint;
using QubeFin.Hrms.Application.Salaries.Commands;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Api.Endpoints
{
    public class SalaryComponentEndpoints : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapPost("salary-components", async (CreateSalaryComponentCommand command, ISender sender) =>
            {
                var result = await sender.Send(command);
                return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
            }).WithSummary("Create a new salary component")
              .WithDescription("Creates a new salary component in the system.")
              .WithTags("Salary Components");
        }
    }
}
