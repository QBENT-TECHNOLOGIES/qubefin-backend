using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;

namespace QubeFin.Global.Persistence.Repositories
{
    public interface IGroupSurveyRepository
    {
        Task<TblGroupSurvey?> GetBySurveyIdAsync(Guid surveyId);
        Task AddAsync(TblGroupSurvey groupSurvey);
        Task<PlaceNames> GetPlaceNamesAsync(Guid administrativeUnitId);
        Task<SignOffInfo?> GetSignOffAsync(Guid employeeId);
    }

    // Village / Locality, Gram Panchayat / Municipality and Block above an administrative unit.
    public record PlaceNames(string? VillageLocality, string? GramPanchayatMunicipality, string? Block);

    // Who signs a block of the form: name, current designation, PIN (= employee code) and the signature file.
    public record SignOffInfo(string EmployeeName, string? Designation, string? Pin, string? SignatureFileNo);

    // The Group Formation Survey form has about 40 plain fields, so (like the Branch Survey) this works with the entity directly.
    public class GroupSurveyRepository(QubeFinDataContext context) : IGroupSurveyRepository
    {
        public async Task<TblGroupSurvey?> GetBySurveyIdAsync(Guid surveyId)
        {
            return await context.TblGroupSurveys.FirstOrDefaultAsync(x => x.SurveyId == surveyId);
        }

        public async Task AddAsync(TblGroupSurvey groupSurvey)
        {
            await context.TblGroupSurveys.AddAsync(groupSurvey);
        }

        // Walks up from the survey area: Village or Ward = Village / Locality, Gram Panchayat or Municipality, and Block.
        public async Task<PlaceNames> GetPlaceNamesAsync(Guid administrativeUnitId)
        {
            var units = await context.TblAdministrativeUnits.AsNoTracking()
                .Select(m => new { m.Id, m.Name, m.ParentId, TypeName = m.AdministrativeUnitType.Name })
                .ToDictionaryAsync(m => m.Id);

            string? village = null, panchayat = null, block = null;
            var visitedIds = new HashSet<Guid>();
            var currentId = (Guid?)administrativeUnitId;
            while (currentId.HasValue && visitedIds.Add(currentId.Value) && units.TryGetValue(currentId.Value, out var unit))
            {
                switch (unit.TypeName)
                {
                    case "Village" or "Ward": village ??= unit.Name; break;
                    case "Gram Panchayat" or "Municipality": panchayat ??= unit.Name; break;
                    case "Block": block ??= unit.Name; break;
                }
                currentId = unit.ParentId;
            }
            return new PlaceNames(village, panchayat, block);
        }

        // The signature is the employee document with DocumentCategory = 'SIGNATURE'.
        public async Task<SignOffInfo?> GetSignOffAsync(Guid employeeId)
        {
            var employee = await context.TblEmployees.AsNoTracking()
                .Where(e => e.Id == employeeId)
                .Select(e => new
                {
                    e.FullName,
                    e.Code,
                    Designation = e.TblEmployeeDesignations
                        .Where(ed => ed.EffectiveTo == null)
                        .Select(ed => ed.Designation.Name)
                        .FirstOrDefault(),
                    SignatureFileNo = e.TblEmployeeDocuments
                        .Where(d => d.DocumentCategory == "SIGNATURE" && d.FileNo != null)
                        .Select(d => d.FileNo)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();

            return employee is null ? null : new SignOffInfo(employee.FullName, employee.Designation, employee.Code, employee.SignatureFileNo);
        }
    }
}
