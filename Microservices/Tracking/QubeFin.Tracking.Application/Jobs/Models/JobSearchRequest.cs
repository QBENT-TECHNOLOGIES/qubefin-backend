using QubeFin.Tracking.Application.Enums;

namespace QubeFin.Tracking.Application.Jobs.Models;

public record JobSearchRequest(
    string? SearchText,
    JobStatus? Status,
    bool OpenOnly,
    Guid? OrganisationUnitId,
    Guid? AssigneeId,
    DateOnly? DueFrom,
    DateOnly? DueTo,
    DpdRange? DpdRange,
    int PageIndex,
    int PageSize,
    string? SortOn,
    SortDirection SortDirection);
