using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using QubeFin.Core.Results;
using QubeFin.Hrms.Persistence.Repositories;
using System.Net.Mail;

namespace QubeFin.Hrms.Application.Leaves.Commands;

#region --- QUERY --
public record SaveRequestCommand(Guid? Id, Guid EmployeeId, Guid LeaveTypeId, DateOnly FromDate, DateOnly ToDate, string Address, string Reason, string EnclosedFileName, IFormFile EnclosedFile)
    : IRequest<Result<SaveRequestResponse>>;
#endregion

#region --- RESPONSE ---
public record SaveRequestResponse(string Message);
#endregion

#region --- HANDLER ---
internal sealed class SaveRequestCommandHandler(ILeaveRepository leaveRepository, IFileStorageRepository fileStorageRepository) : IRequestHandler<SaveRequestCommand, Result<SaveRequestResponse>>
{
    public async Task<Result<SaveRequestResponse>> Handle(SaveRequestCommand request, CancellationToken cancellationToken)
    {
        var fileNo = string.Empty;
        if (request.EnclosedFile != null && request.EnclosedFile.Length > 0)
        {
            var file = request.EnclosedFile;
            await using var stream = file.OpenReadStream();
            fileNo = await fileStorageRepository.UploadFileAsync(stream, file.FileName, file.ContentType ?? "application/octet-stream", cancellationToken).ConfigureAwait(false);


            //fileNo = DateTime.Now.ToString("yyyyMMddHHmmssfff") + Path.GetExtension(request.EnclosedFile.FileName);
            //var directory = Path.Combine(@"C:\WeGrow", "LeaveRequests");

            //Directory.CreateDirectory(directory);
            //var filePath = Path.Combine(directory, fileNo);

            //using var stream = File.Create(filePath);
            //await request.EnclosedFile.CopyToAsync(stream, cancellationToken);
        }

        var message = await leaveRepository.AddAsync(request.Id == null ? Guid.Empty : request.Id.Value, request.EmployeeId, request.LeaveTypeId, request.FromDate, request.ToDate, request.Address, request.Reason, request.EnclosedFileName, fileNo);
        if (!string.IsNullOrEmpty(message))
        {
            return new ValidationError(message);
        }
        return Result.Ok(new SaveRequestResponse(string.IsNullOrEmpty(message) ? $"Leave request {(request.Id == null ? "created" : "updated")} successfully.": message));
    }
}
#endregion