using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Models.Tracking;
using QubeFin.Tracking.Application.Enums;
using QubeFin.Tracking.Persistence.Repositories;

namespace QubeFin.Tracking.Application.Jobs.Commands;

#region --- COMMAND ---
// Sent as multipart/form-data from the mobile app: the completion photo, its GPS position and remarks.
// OD follow-up tasks also send the visit outcome: MemberAvailable, Paid, AmountCollected (when paid) and PtpDate (when not paid).
public class CompleteTaskRequest
{
    public IFormFile? Photo { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? Remarks { get; set; }
    public bool? MemberAvailable { get; set; }
    public bool? Paid { get; set; }
    public decimal? AmountCollected { get; set; }
    public DateOnly? PtpDate { get; set; }
}

public record CompleteTaskCommand(Guid UserId, Guid JobId, Guid TaskId, CompleteTaskRequest Request) : IRequest<Result<string>>;
#endregion

#region --- VALIDATOR ---
public class CompleteTaskCommandValidator : AbstractValidator<CompleteTaskCommand>
{
    private const long MaxPhotoBytes = 5 * 1024 * 1024;

    public CompleteTaskCommandValidator()
    {
        RuleFor(v => v.UserId).NotEmpty().WithMessage("User Id is required.");
        RuleFor(v => v.JobId).NotEmpty().WithMessage("Job Id is required.");
        RuleFor(v => v.TaskId).NotEmpty().WithMessage("Task Id is required.");
        RuleFor(v => v.Request.Photo).NotNull().WithMessage("Take a photo at the place.");
        RuleFor(v => v.Request.Photo!)
            .Must(f => f.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)).WithMessage("The photo must be an image.")
            .Must(f => f.Length > 0 && f.Length <= MaxPhotoBytes).WithMessage("The photo must be at most 5 MB.")
            .When(v => v.Request.Photo is not null);
        RuleFor(v => v.Request.Latitude).NotNull().InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");
        RuleFor(v => v.Request.Longitude).NotNull().InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
        RuleFor(v => v.Request.Remarks).MaximumLength(500).WithMessage("Remarks can be at most 500 characters.");
        RuleFor(v => v.Request.AmountCollected).GreaterThanOrEqualTo(0).When(v => v.Request.AmountCollected.HasValue)
            .WithMessage("Amount collected cannot be negative.");
    }
}
#endregion

#region --- HANDLER ---
// Only the job's assignee completes a Pending task of a job that is not cancelled. The server works out the distance from
// the task's target; IsLocationMatched = distance within the task's radius (a mismatch is saved and flagged, not refused).
// The task gets a TaskCompleted history row; if no pending task is left the job completes in the same save (no background job).
internal sealed class CompleteTaskCommandHandler(
    ITrackingScopeRepository scopeRepository,
    IJobRepository jobRepository,
    IFileStorageRepository fileStorage,
    IUnitOfWork unitOfWork) : IRequestHandler<CompleteTaskCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CompleteTaskCommand command, CancellationToken cancellationToken)
    {
        var viewer = await scopeRepository.GetViewerAsync(command.UserId, cancellationToken);
        if (viewer is null) return new ForbiddenError("You do not have access to Tracking.");

        var job = await jobRepository.GetJobForUpdateAsync(command.JobId, cancellationToken);
        if (job is null) return new RecordNotFoundError("Job not found");
        if (viewer.EmployeeId is null || job.AssignTo != viewer.EmployeeId) return new ForbiddenError("Only the job's assignee can complete its tasks.");
        if (job.IsCancel) return new ValidationError($"{job.JobCode} is cancelled.");

        var task = job.TblJobTasks.FirstOrDefault(t => t.Id == command.TaskId);
        if (task is null) return new RecordNotFoundError("Task not found");
        if (!JobRepository.IsPending(task)) return new ValidationError($"{task.TaskNo} is {JobRepository.StatusOf(task)}.");

        var request = command.Request;
        var isOdFollowUp = task.TaskType == nameof(TaskType.OdFollowUp);
        if (isOdFollowUp)
        {
            if (request.MemberAvailable is null) return new ValidationError("Say whether the member was available.");
            if (request.Paid is null) return new ValidationError("Say whether the member paid.");
            if (request.Paid == true && (request.AmountCollected is null || request.AmountCollected <= 0))
            {
                return new ValidationError("Enter the amount collected.");
            }
            if (request.Paid == false && request.PtpDate is null) return new ValidationError("Enter the promise-to-pay date.");
            if (request.Paid == false && request.PtpDate < TrackingClock.Today()) return new ValidationError("The promise-to-pay date cannot be in the past.");
        }

        var photo = request.Photo!;
        string photoKey;
        await using (var stream = photo.OpenReadStream())
        {
            photoKey = await fileStorage.UploadFileAsync(stream, "Tracking/Tasks", photo.FileName, photo.ContentType, cancellationToken);
        }

        var performedAt = DateTimeOffset.Now;
        var now = performedAt.DateTime;
        var location = new GeoPoint(request.Latitude!.Value, request.Longitude!.Value);
        var target = GeoCoordinates.ToGeoPoint(task.TargetLocation);
        decimal? distance = target is null ? null : Math.Round((decimal)DistanceInMeters(target, location), 2);

        task.Status = nameof(JobTaskStatus.Completed);
        task.IsCompleted = true;
        task.TaskCompletedBy = viewer.UserId;
        task.TaskCompletedOn = now;
        task.CompletedLocation = GeoCoordinates.ToGeometry(location);
        task.CompletedDistanceMeter = distance;
        task.IsLocationMatched = distance.HasValue && task.RadiusMeters.HasValue && distance.Value <= task.RadiusMeters.Value;
        task.PhotoUrl = photoKey;
        task.Remarks = string.IsNullOrWhiteSpace(request.Remarks) ? null : request.Remarks.Trim();
        if (isOdFollowUp)
        {
            task.IsOdMemberAvailable = request.MemberAvailable!.Value;
            task.IsEmireceived = request.Paid!.Value;
            task.ReceivedAmount = request.Paid == true ? request.AmountCollected : null;
            task.Odptpdate = request.Paid == true ? null : request.PtpDate;
        }
        task.ModifiedOn = now;
        job.ModifiedBy = viewer.UserId;
        job.ModifiedOn = now;

        await jobRepository.AddHistoryAsync(
            JobHistoryEntries.New(job.Id, JobHistoryAction.TaskCompleted, viewer.UserId, performedAt, [task], toAssigneeId: job.AssignTo, remarks: task.Remarks),
            cancellationToken);
        var completed = JobHistoryEntries.CompleteIfDone(job, viewer.UserId, performedAt);
        if (completed is not null) await jobRepository.AddHistoryAsync(completed, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var message = task.IsLocationMatched
            ? $"Task {task.TaskNo} completed."
            : $"Task {task.TaskNo} completed. The location does not match the place{(distance.HasValue ? $" ({distance.Value:0} m away)" : string.Empty)}.";
        return Result.Ok(completed is null ? message : $"{message} {job.JobCode} is now completed.");
    }

    // Great-circle distance (haversine) on the WGS-84 mean radius
    private static double DistanceInMeters(GeoPoint from, GeoPoint to)
    {
        const double earthRadius = 6371008.8;
        static double Radians(decimal degrees) => (double)degrees * Math.PI / 180;

        var dLat = Radians(to.Lat - from.Lat);
        var dLng = Radians(to.Lng - from.Lng);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
            + Math.Cos(Radians(from.Lat)) * Math.Cos(Radians(to.Lat)) * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        return 2 * earthRadius * Math.Asin(Math.Min(1, Math.Sqrt(a)));
    }
}
#endregion
