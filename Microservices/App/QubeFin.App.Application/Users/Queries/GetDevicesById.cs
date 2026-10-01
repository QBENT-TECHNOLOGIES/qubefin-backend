using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using QubeFin.App.Persistence.Repositories;
using QubeFin.Persistence;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.App.Application.Users.Queries;

#region --- QUERY ---
public record GetDevicesByIdQuery(Guid Id) : IRequest<Result<List<GetDevicesByIdResponse>>>;
#endregion

#region --- RESPONSE ---
public record GetDevicesByIdResponse(Guid Id, string DeviceId, DateTime? AssignDate, bool IsRelease, DateTime? ReleaseDate);
#endregion

#region --- HANDLER ---
internal sealed class GetDevicesByIdQueryHandler(IUserRepository userRepository, QubeFinDataContext context)
    : IRequestHandler<GetDevicesByIdQuery, Result<List<GetDevicesByIdResponse>>>
{
    public async Task<Result<List<GetDevicesByIdResponse>>> Handle(GetDevicesByIdQuery request, CancellationToken cancellationToken)
    {
        var devices = await context.TblUserDevices.Where(m => m.UserId == request.Id)
            .Select(m=> new GetDevicesByIdResponse(
                m.Id,
                m.DeviceId,
                m.AssignDate,
                m.IsReleased,
                m.ReleaseDate
                ))
            .ToListAsync(cancellationToken);
        return Result.Ok(devices);

    }
}
#endregion
