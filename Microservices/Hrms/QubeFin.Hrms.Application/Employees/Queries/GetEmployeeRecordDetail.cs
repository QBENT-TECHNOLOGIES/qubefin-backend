using FluentResults;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Hrms.Application.Employees.Models;
using QubeFin.Hrms.Persistence.Repositories;
using QubeFin.Persistence;
using System.Text.Json;

namespace QubeFin.Hrms.Application.Employees.Queries;

#region --- QUERY ---
public record GetEmployeeRecordDetailQuery(string RecordType, Guid Id) : IRequest<Result<EmployeeRecordDetail>>;
#endregion

#region --- VALIDATOR ---
public class GetEmployeeRecordDetailQueryValidator : AbstractValidator<GetEmployeeRecordDetailQuery>
{
    public GetEmployeeRecordDetailQueryValidator()
    {
        RuleFor(v => v.Id).NotEmpty().WithMessage("Record Id is required.");
        RuleFor(v => v.RecordType)
            .Must(EmployeeRecordTypes.IsValid)
            .WithMessage($"Record Type must be one of: {string.Join(", ", EmployeeRecordTypes.All)}.");
    }
}
#endregion

#region --- HANDLER ---
internal sealed class GetEmployeeRecordDetailQueryHandler(
    QubeFinDataContext context,
    IFileStorageRepository fileStorageRepository)
    : IRequestHandler<GetEmployeeRecordDetailQuery, Result<EmployeeRecordDetail>>
{
    private const string DateFormat = "dd MMM yyyy";
    private const string TimeFormat = "h:mm tt";

    public async Task<Result<EmployeeRecordDetail>> Handle(GetEmployeeRecordDetailQuery request, CancellationToken cancellationToken)
    {
        var detail = EmployeeRecordTypes.Normalize(request.RecordType) switch
        {
            EmployeeRecordTypes.Regularization => await LoadRegularizationAsync(request.Id, cancellationToken),
            EmployeeRecordTypes.Prayer => await LoadPrayerAsync(request.Id, cancellationToken),
            EmployeeRecordTypes.Attendance => await LoadAttendanceAsync(request.Id, cancellationToken),
            EmployeeRecordTypes.Fitness => await LoadFitnessAsync(request.Id, cancellationToken),
            _ => await LoadLeaveAsync(request.Id, cancellationToken)
        };

        if (detail is null)
            return new RecordNotFoundError("Record not found");

        return Result.Ok(detail);
    }

    // ---- Leave request ------------------------------------------------------

    private async Task<EmployeeRecordDetail?> LoadLeaveAsync(Guid id, CancellationToken cancellationToken)
    {
        var row = await context.TblLeaveRequests
            .Include(m => m.Employee).ThenInclude(e => e.OrganizationUnit)
            .Include(m => m.Employee).ThenInclude(e => e.Company)
            .Include(m => m.LeaveType)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (row is null) return null;

        var detail = new EmployeeRecordDetail
        {
            Id = row.Id,
            RecordType = EmployeeRecordTypes.Leave,
            Category = row.LeaveType.Title,
            Period = FormatRange(row.FromDate, row.ToDate),
            FromDate = row.FromDate,
            ToDate = row.ToDate,
            Days = row.TotalDays,
            AppliedOn = DateOnly.FromDateTime(row.RequestDate),
            Status = row.CurrentStatus,
            Reason = row.Reason,
            Address = row.Address,
            Attachment = row.EnclosedDocName,
        };

        ApplyEmployee(detail, row.EmployeeId, row.Employee.FullName, row.Employee.Code,
            row.Employee.OrganizationUnit?.Name, row.Employee.Company?.Name);

        AddField(detail, "Leave Alias", row.LeaveType.Alias);
        AddField(detail, "Leave Year", row.LeaveYear.ToString());
        AddField(detail, "Document No", row.EnclosedDocNo);
        AddField(detail, "Submitted On", FormatDateTime(row.SubmittedOn));
        AddField(detail, "Actioned On", FormatDateTime(row.ApprovedOrRejectedOn));
        AddField(detail, "Rejected Reason", row.RejectedReason);

        await ApplyAttachmentUrlAsync(detail, row.EnclosedDocName, cancellationToken);
        detail.Events = await LoadEventsAsync(row.Id, row.RequestDate, cancellationToken);

        return detail;
    }

    // ---- Attendance regularization -------------------------------------------

    private async Task<EmployeeRecordDetail?> LoadRegularizationAsync(Guid id, CancellationToken cancellationToken)
    {
        var row = await context.TblAttendanceRegularizations
            .Include(m => m.Employee).ThenInclude(e => e.OrganizationUnit)
            .Include(m => m.Employee).ThenInclude(e => e.Company)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (row is null) return null;

        var detail = new EmployeeRecordDetail
        {
            Id = row.Id,
            RecordType = EmployeeRecordTypes.Regularization,
            Category = row.RegularizationType,
            Period = NormalizeRegularizationDates(row.RegularizationDates),
            AppliedOn = DateOnly.FromDateTime(row.CreatedOn),
            Status = row.CurrentStatus,
            Reason = row.Reason,
            Remarks = row.Remarks,
            Attachment = row.Attachment,
        };

        ApplyEmployee(detail, row.EmployeeId, row.Employee.FullName, row.Employee.Code,
            row.Employee.OrganizationUnit?.Name, row.Employee.Company?.Name);

        AddField(detail, "Regularization For", row.RegularizationFor);
        AddField(detail, "Actual In Time", FormatTime(row.ActualInTime));
        AddField(detail, "Actual Out Time", FormatTime(row.ActualOutTime));

        await ApplyAttachmentUrlAsync(detail, row.Attachment, cancellationToken);
        detail.Events = await LoadEventsAsync(row.Id, row.CreatedOn, cancellationToken);

        return detail;
    }

    // ---- Leave prayer ----------------------------------------------------------

    private async Task<EmployeeRecordDetail?> LoadPrayerAsync(Guid id, CancellationToken cancellationToken)
    {
        var row = await context.TblLeavePrayers
            .Include(m => m.Employee).ThenInclude(e => e.OrganizationUnit)
            .Include(m => m.Employee).ThenInclude(e => e.Company)
            .Include(m => m.LeaveType)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (row is null) return null;

        var detail = new EmployeeRecordDetail
        {
            Id = row.Id,
            RecordType = EmployeeRecordTypes.Prayer,
            Category = row.LeaveType.Title,
            Days = row.PrayerDays,
            AppliedOn = DateOnly.FromDateTime(row.CreatedOn),
            Status = row.CurrentStatus,
            Remarks = row.Remarks,
            Reason = row.Remarks,
            Attachment = row.Attachment,
        };

        ApplyEmployee(detail, row.EmployeeId, row.Employee.FullName, row.Employee.Code,
            row.Employee.OrganizationUnit?.Name, row.Employee.Company?.Name);

        AddField(detail, "Leave Alias", row.LeaveType.Alias);
        AddField(detail, "Prayer Days", row.PrayerDays.ToString());

        await ApplyAttachmentUrlAsync(detail, row.Attachment, cancellationToken);
        detail.Events = await LoadEventsAsync(row.Id, row.CreatedOn, cancellationToken);

        return detail;
    }

    // ---- Attendance -------------------------------------------------------------

    private async Task<EmployeeRecordDetail?> LoadAttendanceAsync(Guid id, CancellationToken cancellationToken)
    {
        var row = await context.TblAttendances
            .Include(m => m.Employee).ThenInclude(e => e.OrganizationUnit)
            .Include(m => m.Employee).ThenInclude(e => e.Company)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (row is null) return null;

        var detail = new EmployeeRecordDetail
        {
            Id = row.Id,
            RecordType = EmployeeRecordTypes.Attendance,
            Period = row.AttendanceDate.ToString(DateFormat),
            FromDate = row.AttendanceDate,
            ToDate = row.AttendanceDate,
            Status = DeriveAttendanceStatus(row.AttendanceDate, row.ActualInTime, row.ActualOutTime, row.IsLateEntry, row.IsEarlyLeave),
        };

        ApplyEmployee(detail, row.EmployeeId, row.Employee.FullName, row.Employee.Code,
            row.Employee.OrganizationUnit?.Name, row.Employee.Company?.Name);

        AddField(detail, "Expected In Time", FormatTime(row.ExpectedInTime));
        AddField(detail, "Expected Out Time", FormatTime(row.ExpectedOutTime));
        AddField(detail, "Actual In Time", FormatTime(row.ActualInTime));
        AddField(detail, "Actual Out Time", FormatTime(row.ActualOutTime));
        AddField(detail, "Working Hours", FormatWorkingHours(row.ActualInTime, row.ActualOutTime));
        AddField(detail, "Late Entry", row.IsLateEntry ? "Yes" : "No");
        AddField(detail, "Early Exit", row.IsEarlyLeave ? "Yes" : "No");
        AddField(detail, "Regularized", row.IsRegularization ? "Yes" : "No");
        AddField(detail, "On Duty", row.IsOnDuty ? "Yes" : "No");

        // Attendance is a punch record, so it carries no approval workflow.
        return detail;
    }

    // ---- Medical fitness -----------------------------------------------------------

    private async Task<EmployeeRecordDetail?> LoadFitnessAsync(Guid id, CancellationToken cancellationToken)
    {
        var row = await context.TblLeaveRequests
            .Include(m => m.Employee).ThenInclude(e => e.OrganizationUnit)
            .Include(m => m.Employee).ThenInclude(e => e.Company)
            .Include(m => m.LeaveType)
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (row is null) return null;

        var detail = new EmployeeRecordDetail
        {
            Id = row.Id,
            RecordType = EmployeeRecordTypes.Fitness,
            Category = row.LeaveType.Title,
            Period = FormatRange(row.FromDate, row.ToDate),
            FromDate = row.FromDate,
            ToDate = row.ToDate,
            Days = row.TotalDays,
            AppliedOn = row.FitnessReportUploadOn.HasValue ? DateOnly.FromDateTime(row.FitnessReportUploadOn.Value) : null,
            Status = row.IsFitnessReportApproved ? EmployeeRecordStatuses.Approved : EmployeeRecordStatuses.Pending,
            Reason = row.Reason,
            Attachment = row.FitnessReportAttachment,
        };

        ApplyEmployee(detail, row.EmployeeId, row.Employee.FullName, row.Employee.Code,
            row.Employee.OrganizationUnit?.Name, row.Employee.Company?.Name);

        AddField(detail, "Leave Period", FormatRange(row.FromDate, row.ToDate));
        AddField(detail, "Report Uploaded On", FormatDateTime(row.FitnessReportUploadOn));
        AddField(detail, "Report Approved On", FormatDateTime(row.FitnessReportApprovedOn));
        AddField(detail, "Leave Status", row.CurrentStatus);

        await ApplyAttachmentUrlAsync(detail, row.FitnessReportAttachment, cancellationToken);
        detail.Events = await LoadEventsAsync(row.Id, row.RequestDate, cancellationToken);

        return detail;
    }

    // ---- Approval trail ---------------------------------------------------------------

    private async Task<List<EmployeeRecordEvent>> LoadEventsAsync(Guid mappingId, DateTime requestedOn, CancellationToken cancellationToken)
    {
        var rows = await context.TblApprovalRequestEvents
            .AsNoTracking()
            .Where(e => e.MappingId == mappingId)
            .OrderBy(e => e.EventDate)
            .Select(e => new
            {
                e.EventDate,
                e.Remarks,
                e.ApprovalWorkflowStep.EventStatus,
                WorkflowCategory = e.ApprovalWorkflowStep.ApprovalWorkflow.Category,
                SenderDesignation = e.SenderDesignation.Name,
                SenderHolder = context.TblEmployeeDesignations
                    .Where(d => d.DesignationId == e.SenderDesignationId && d.EffectiveTo == null)
                    .Select(d => d.Employee.FullName + " (" + d.Employee.Code + ")")
                    .FirstOrDefault(),
                ReceiverDesignation = e.ReceiverDesignation.Name,
                ReceiverHolder = context.TblEmployeeDesignations
                    .Where(d => d.DesignationId == e.ReceiverDesignationId && d.EffectiveTo == null)
                    .Select(d => d.Employee.FullName + " (" + d.Employee.Code + ")")
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        var events = rows.Select(e => new EmployeeRecordEvent
        {
            Category = e.WorkflowCategory,
            EventStatus = e.EventStatus,
            EventDate = e.EventDate,
            Remarks = e.Remarks,
            SenderDesignation = FormatDesignation(e.SenderDesignation, e.SenderHolder),
            ReceiverDesignation = FormatDesignation(e.ReceiverDesignation, e.ReceiverHolder)
        }).ToList();

        // The submission itself is not stored as an event, so the trail starts with it.
        events.Insert(0, new EmployeeRecordEvent
        {
            EventStatus = "Requested",
            EventDate = requestedOn,
            SenderDesignation = events.FirstOrDefault()?.SenderDesignation
        });

        return events;
    }

    // ---- Helpers --------------------------------------------------------------------------

    private static void ApplyEmployee(
        EmployeeRecordDetail detail, Guid employeeId, string? name, string? code, string? organizationUnit, string? company)
    {
        detail.EmployeeId = employeeId;
        detail.EmployeeName = name;
        detail.EmployeeCode = code;
        detail.OrganizationUnit = organizationUnit;
        detail.Company = company;
    }

    private static void AddField(EmployeeRecordDetail detail, string label, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            detail.Fields.Add(new EmployeeRecordDetailField { Label = label, Value = value });
    }

    private async Task ApplyAttachmentUrlAsync(EmployeeRecordDetail detail, string? attachment, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(attachment))
            detail.AttachmentUrl = await fileStorageRepository.GetFileUrlAsync(attachment, cancellationToken);
    }

    private static string? FormatDesignation(string? designation, string? holder) =>
        string.IsNullOrWhiteSpace(holder) ? designation : $"{designation} - {holder}";

    private static string? FormatDate(DateOnly? value) => value?.ToString(DateFormat);

    private static string? FormatDateTime(DateTime? value) => value?.ToString(DateFormat);

    private static string? FormatTime(TimeOnly? value) => value?.ToString(TimeFormat);

    private static string? FormatRange(DateOnly? from, DateOnly? to)
    {
        var start = FormatDate(from);
        var end = FormatDate(to);

        if (start is null) return end;
        if (end is null || start == end) return start;

        return $"{start} – {end}";
    }

    private static string? FormatWorkingHours(TimeOnly? inTime, TimeOnly? outTime)
    {
        if (!inTime.HasValue || !outTime.HasValue) return null;

        var duration = outTime.Value.ToTimeSpan() - inTime.Value.ToTimeSpan();
        if (duration < TimeSpan.Zero) duration += TimeSpan.FromDays(1);

        return $"{duration.Hours} h {duration.Minutes} m";
    }

    private static string DeriveAttendanceStatus(
        DateOnly attendanceDate, TimeOnly? inTime, TimeOnly? outTime, bool isLateEntry, bool isEarlyLeave)
    {
        var isPastDay = attendanceDate < DateOnly.FromDateTime(DateTime.Now.Date);
        if (isPastDay && (inTime is null || outTime is null)) return "MSP";

        return (isLateEntry, isEarlyLeave) switch
        {
            (false, false) => "On Time",
            (true, false) => "Late Entry",
            (false, true) => "Early Exit",
            (true, true) => "Late Entry & Early Exit"
        };
    }

    private static string? NormalizeRegularizationDates(string? dates)
    {
        if (string.IsNullOrWhiteSpace(dates)) return null;

        try
        {
            var parsed = JsonSerializer.Deserialize<List<string>>(dates);
            return parsed is { Count: > 0 } ? string.Join(", ", parsed) : dates;
        }
        catch (JsonException)
        {
            return dates;
        }
    }
}
#endregion
