using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;

namespace QubeFin.Persistence.Entities;

public partial class TblGroupSurvey
{
    public Guid Id { get; set; }

    public Guid SurveyId { get; set; }

    public DateOnly SurveyDate { get; set; }

    public string? VillageLocality { get; set; }

    public string? GramPanchayatMunicipality { get; set; }

    public string? Block { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public Geometry? Location { get; set; }

    public string? NearestLandmark { get; set; }

    public int? ApproximateHouseholds { get; set; }

    public int? EstimatedPopulation { get; set; }

    public string? MainOccupation { get; set; }

    public string? MainOccupationOther { get; set; }

    public string? RoadAccessibility { get; set; }

    public string? AutoTotoAvailability { get; set; }

    public string? CommunityHarmony { get; set; }

    public string? WomenEconomicParticipation { get; set; }

    public string? SocialAcceptanceOfMicrofinance { get; set; }

    public string? PoliticalStability { get; set; }

    public string? PoliticalDisturbance { get; set; }

    public string? LocalInfluenceOnLending { get; set; }

    public string? ElectionRelatedIssues { get; set; }

    public string? FloodProne { get; set; }

    public string? WaterLogging { get; set; }

    public string? DroughtProne { get; set; }

    public string? FlashFloodRisk { get; set; }

    public string? CycloneRisk { get; set; }

    public string? RiverErosion { get; set; }

    public string? OtherNaturalRisks { get; set; }

    public int? NumberOfMfis { get; set; }

    public int? SmallFinanceBanks { get; set; }

    public int? ShgFinancingInstitutions { get; set; }

    public decimal? DistanceFromBranchKm { get; set; }

    public decimal? DistanceFromNearestWeGrowCentreKm { get; set; }

    public decimal? DistanceFromNearestCompetingCentreKm { get; set; }

    public int? AverageTravelTimeMinutes { get; set; }

    public int? GroupsThatCanBeFormed { get; set; }

    public int? EstimatedWomenBorrowers { get; set; }

    public string? SuitableMeetingPlaces { get; set; }

    public string? CommunityResponseTowardsWeGrow { get; set; }

    public string? OtherObservations { get; set; }

    public string? SurveyorRecommendation { get; set; }

    public string? SurveyorComments { get; set; }

    public bool IsSurveyorSubmit { get; set; }

    public DateTime? SurveyorSubmitOn { get; set; }

    public Guid? BranchManagerEmployeeId { get; set; }

    public string? BranchManagerRecommendation { get; set; }

    public string? BranchManagerComments { get; set; }

    public DateTime? BranchManagerActionOn { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public Guid? LastModifiedBy { get; set; }

    public DateTime? LastModifiedOn { get; set; }

    public virtual TblEmployee? BranchManagerEmployee { get; set; }

    public virtual TblSurvey Survey { get; set; } = null!;
}
