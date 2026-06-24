using Microsoft.AspNetCore.Routing;

namespace QubeFin.Core.Endpoint;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
