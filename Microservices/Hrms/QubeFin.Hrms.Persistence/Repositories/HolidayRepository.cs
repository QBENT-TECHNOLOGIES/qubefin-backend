using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Persistence.Repositories;

public interface IHolidayRepository
{
    Task AddAsync(Holiday holiday);
    Task UpdateAsync(Holiday holiday);
    Task<Holiday?> GetByIdAsync(Guid id);
    Task<IEnumerable<Holiday>> GetAllAsync();
    Task<IEnumerable<Holiday>> GetByOrgUnitIdAsync(Guid orgUnitId);
    Task<bool> ExistsAsync(Guid orgUnitId, DateOnly holidayDate, Guid? excludeId = null);
    Task<IEnumerable<Holiday>> GetAllByEmployeeIdAsync(Guid employeeId);
}

public class HolidayRepository(QubeFinDataContext context) : IHolidayRepository
{
    public async Task AddAsync(Holiday holiday)
    {
        await context.TblHolidays.AddAsync(holiday.ToEntity());
    }

    public Task UpdateAsync(Holiday holiday)
    {
        context.TblHolidays.Update(holiday.ToEntity());
        return Task.CompletedTask;
    }

    public async Task<Holiday?> GetByIdAsync(Guid id)
    {
        var entity = await context.TblHolidays
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return entity?.ToDomain();
    }

    public async Task<IEnumerable<Holiday>> GetAllAsync()
    {
        var entities = await context.TblHolidays
            .AsNoTracking()
            .OrderBy(x => x.HolidayDate)
            .ToListAsync();

        return entities.Select(x => x.ToDomain());
    }

    public async Task<IEnumerable<Holiday>> GetByOrgUnitIdAsync(Guid orgUnitId)
    {
        var entities = await context.TblHolidays
            .AsNoTracking()
            .Where(x => x.OrgUnitId == orgUnitId)
            .OrderBy(x => x.HolidayDate)
            .ToListAsync();

        return entities.Select(x => x.ToDomain());
    }

    public Task<bool> ExistsAsync(Guid orgUnitId, DateOnly holidayDate, Guid? excludeId = null)
    {
        return context.TblHolidays
            .AsNoTracking()
            .AnyAsync(x => x.OrgUnitId == orgUnitId
                && x.HolidayDate == holidayDate
                && (!excludeId.HasValue || x.Id != excludeId.Value));
    }

    public async Task<IEnumerable<Holiday>> GetAllByEmployeeIdAsync(Guid employeeId)
    {
        var employee = await context.TblEmployees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == employeeId);
        if (employee == null)
        {
            return Enumerable.Empty<Holiday>();
        }

        var entities = await context.TblHolidays.Where(x => x.OrgUnitId == employee.OrganizationUnitId)
            .AsNoTracking()
            .OrderBy(x => x.HolidayDate)
            .ToListAsync();
        return entities.Select(x => x.ToDomain());
    }
}
