using FluentResults;
using MediatR;
using QubeFin.Hrms.Application.InterviewProcess.Models;
using QubeFin.Hrms.Persistence.Repositories;

namespace QubeFin.Hrms.Application.InterviewProcess.Queries;

public record GetCandidateByIdQuery(Guid Id) : IRequest<Result<CandidateDetailDto>>;

internal sealed class GetCandidateByIdQueryHandler(ICandidateRepository candidateRepository) : IRequestHandler<GetCandidateByIdQuery, Result<CandidateDetailDto>>
{
    public async Task<Result<CandidateDetailDto>> Handle(GetCandidateByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var candidate = await candidateRepository.GetByIdAsync(request.Id);

            if (candidate is null)
            {
                return Result.Fail("Candidate not found for the given Id.");
            }

            var result = new CandidateDetailDto
            {
                Id = candidate.Id,
                FirstName = candidate.FirstName,
                MiddleName = candidate.MiddleName,
                LastName = candidate.LastName,
                Gender = candidate.Gender,
                FatherName = candidate.FatherName,
                MobileNo = candidate.MobileNo,
                Email = candidate.Email,
                HouseNo = candidate.HouseNo,
                RoadName = candidate.RoadName,
                LandMark = candidate.LandMark,
                AdministrativeUnitId = candidate.AdministrativeUnitId,
                PoliceStationId = candidate.PoliceStationId,
                PostOfficeId = candidate.PostOfficeId,
                PinCode = candidate.PinCode,
                ReferenceNo = candidate.ReferenceNo,
                InterviewDate = candidate.InterviewDate,
                InterviewTime = candidate.InterviewTime,
                DepartmentId = candidate.DepartmentId,
                InterviewPost = candidate.InterviewPost,
                VenueOrganizationUnitId = candidate.VenueOrganizationUnitId,
                InterviewMode = candidate.InterviewMode,
                ReferedBy = candidate.ReferedBy,
                RecruitmentSource = candidate.RecruitmentSource,
                VacancyReference = candidate.VacancyReference,
                CurrentSalary = candidate.CurrentSalary,
                ExpectedSalary = candidate.ExpectedSalary,
                NoticePeriodInDays = candidate.NoticePeriodInDays,
                EarliestJoiningDate = candidate.EarliestJoiningDate,
                IsWillingRelocate = candidate.IsWillingRelocate,
                PreferredLocation = candidate.PreferredLocation,
                PostedOrganizationUnitId = candidate.PostedOrganizationUnitId,
                DateOfJoining = candidate.DateOfJoining,
                ReportingTime = candidate.ReportingTime,
                MonthlyCostCompany = candidate.MonthlyCostCompany,
                OverallPerformance = candidate.OverallPerformance,
                SuitableRoleDepartment = candidate.SuitableRoleDepartment,
                RecommendedGradeId = candidate.RecommendedGradeId,
                IsTrainingRequired = candidate.IsTrainingRequired,
                RecommendationStatus = candidate.RecommendationStatus,
                TotalRatingPoint = candidate.TotalRatingPoint,
                RatingStatus = candidate.RatingStatus,
                AadharNumber = candidate.AadharNumber,
                IsAadharValidated = candidate.IsAadharValidated,
                VoterNumber = candidate.VoterNumber,
                IsVoterValited = candidate.IsVoterValited,
                Pan = candidate.Pan,
                IsPanValidated = candidate.IsPanValidated,
                IsMobileValidated = candidate.IsMobileValidated,
                Uan = candidate.Uan,
                IsUanVerified = candidate.IsUanVerified,
                IsCreditBureauChecked = candidate.IsCreditBureauChecked,
                CreditBureauReportLink = candidate.CreditBureauReportLink
            };

            return Result.Ok(result);
        }
        catch(Exception ex)
        {
            return Result.Fail("Something went wrong while fetching candidate details.");
        }
    }
}
