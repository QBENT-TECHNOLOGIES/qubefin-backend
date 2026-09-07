namespace QubeFin.Persistence.Models.Global
{
    public class AddressUnit
    {
        public Guid? CountryId { get; set; }
        public string? CountryName { get; set; } = string.Empty;

        public Guid? StateId { get; set; }
        public string? StateName { get; set; } = string.Empty;

        public Guid? DistrictId { get; set; }
        public string? DistrictName { get; set; } = string.Empty;

        public Guid? BlockId { get; set; }
        public string? BlockName { get; set; } = string.Empty;

        public Guid? GramPanchayatId { get; set; }
        public string? GramPanchayatName { get; set; } = string.Empty;

        public Guid? VillageId { get; set; }
        public string? VillageName { get; set; } = string.Empty;

        public Guid? MunicipalityId { get; set; }
        public string? MunicipalityName { get; set; } = string.Empty;

        public Guid? WardId { get; set; }
        public string? WardName { get; set; } = string.Empty;
    }
}
