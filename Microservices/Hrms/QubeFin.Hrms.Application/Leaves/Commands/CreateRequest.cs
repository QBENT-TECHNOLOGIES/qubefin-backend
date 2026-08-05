using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using QubeFin.Core.Results;
using QubeFin.Core.Settings;
using QubeFin.Hrms.Persistence.Repositories;

namespace QubeFin.Hrms.Application.Leaves.Commands;

#region --- QUERY --
public record CreateRequestCommand(Guid EmployeeId, Guid LeaveTypeId, DateOnly FromDate, DateOnly ToDate, string Address, string Reason, string EnclosedFileName, IFormFile EnclosedFile)
    : IRequest<Result<CreateRequestResponse>>;
#endregion

#region --- RESPONSE ---
public record CreateRequestResponse(string Message);
#endregion

#region --- HANDLER ---
internal sealed class CreateRequestCommandHandler(ILeaveRepository leaveRepository, IOptions<AppSettings> appSettings)
    : IRequestHandler<CreateRequestCommand, Result<CreateRequestResponse>>
{
    public async Task<Result<CreateRequestResponse>> Handle(CreateRequestCommand request, CancellationToken cancellationToken)
    {
        var fileNo = string.Empty;
        if (request.EnclosedFile != null)
        {
            fileNo = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(request.EnclosedFile.FileName);
            var directory = Path.Combine(@"C:\WeGrow", "LeaveRequests");

            Directory.CreateDirectory(directory);
            var filePath = Path.Combine(directory, fileNo);

            using var stream = File.Create(filePath);
            await request.EnclosedFile.CopyToAsync(stream, cancellationToken);
        }

        var message = await leaveRepository.AddAsync(request.EmployeeId, request.LeaveTypeId, request.FromDate, request.ToDate, request.Address, request.Reason, request.EnclosedFileName, fileNo);
        if (!string.IsNullOrEmpty(message))
        {
            return new ValidationError(message);
        }
        return Result.Ok(new CreateRequestResponse(string.IsNullOrEmpty(message) ? "Leave request created successfully.": message));
    }
}
#endregion