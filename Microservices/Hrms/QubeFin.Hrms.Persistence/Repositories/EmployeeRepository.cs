using FluentResults;
using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QubeFin.Core.Results;
using QubeFin.Persistence;
using QubeFin.Persistence.Entities;
using QubeFin.Persistence.Mappers.App;
using QubeFin.Persistence.Mappers.Hrms;
using QubeFin.Persistence.Models.Global;
using QubeFin.Persistence.Models.Hrms;

namespace QubeFin.Hrms.Persistence.Repositories;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id);
    Task AddAsync(Employee employee);
    Task UpdateAsync(Employee employee);
    Task<Employee?> GetEmloyeeOrganization(Guid id);
    Task<bool> GetExsitingEmployeeByCode(Guid? id, string code);
    Task AddDesignationAsync(Guid employeeId, Guid designationId, DateOnly joiningDate);
    Task AddGrossSalaryAsync(Guid employeeId, decimal grossSalary, DateOnly joiningDate);
    Task TransferEntry(Guid employeeId, Guid organisationUnitId, Guid designationId, Guid salaryGradeId, decimal grossSalary, CancellationToken cancellationToken);
    Task<AddressUnit?> GetAdressUnit(Guid administrativeUnitId);
    Task TransferEmployee(Guid EmployeeId, Guid OrganisationUnitId, Guid DesignationId, Guid SalaryGradeId, decimal GrossSalary, CancellationToken cancellationToken);
}
public class EmployeeRepository(QubeFinDataContext context) : IEmployeeRepository
{
    public async Task AddAsync(Employee employee)
    {
        await context.TblEmployees.AddAsync(employee.ToEntity());
    }

    public async Task UpdateAsync(Employee employee)
    {
        context.TblEmployees.Update(employee.ToEntity());
    }

    //public async Task<EmployeeOrganizationTiming?> GetEmployeeOrganization(Guid employeeId)
    //{
    //    var employee = await context.TblEmployees.Include(x => x.OrganizationUnit).FirstOrDefaultAsync(x => x.Id == employeeId);

    //    if (employee == null)
    //        return null;

    //    return new EmployeeOrganizationTiming
    //    {
    //        AttendanceInTime = employee.OrganizationUnit?.AttendanceInTime,
    //        AttendanceOutTime = employee.OrganizationUnit?.AttendanceOutTime,
    //        OrganizationUnitId = employee.OrganizationUnitId
    //    };
    //}
    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        var employeeEntity = await context.TblEmployees
            .Include(x => x.TblEmployeeDesignations)
            .Include(x => x.TblEmployeeQualifications)
            .Include(x => x.TblEmployeeEmployments)
            .Include(x => x.TblEmployeeDocuments)
            .Include(x => x.TblEmployeeReferences)
            .Include(x => x.TblEmployeeGrossSalaries)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return employeeEntity is null ? null : employeeEntity.ToDomain();
    }

    public async Task<Employee?> GetEmloyeeOrganization(Guid id)
    {
        var employeeEntity = await context.TblEmployees.Include(m => m.OrganizationUnit).FirstOrDefaultAsync(x => x.Id == id);

        return employeeEntity is null ? null : employeeEntity.ToDomain();
    }

    public async Task<bool> GetExsitingEmployeeByCode(Guid? id, string code)
    {
        return await context.TblEmployees.AsNoTracking().AnyAsync(x => x.Code.ToLower().Trim() == code.ToLower().Trim() && (id == null || id == Guid.Empty || x.Id != id));
    }
    public async Task AddDesignationAsync(Guid employeeId, Guid designationId, DateOnly joiningDate)
    {
        var existingDesignation = await context.TblEmployeeDesignations
            .Where(ed => ed.EmployeeId != employeeId && ed.DesignationId == designationId && ed.EffectiveTo == null)
            .FirstOrDefaultAsync();
        if (existingDesignation != null)
        {
            throw new InvalidOperationException("The designation is already assigned to another employee.");
        }

        var employeeDesignation = new TblEmployeeDesignation
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            DesignationId = designationId,
            EffectiveFrom = joiningDate.ToDateTime(TimeOnly.MinValue),
            EffectiveTo = null
        };
        await context.TblEmployeeDesignations.AddAsync(employeeDesignation);
    }
    public async Task AddGrossSalaryAsync(Guid employeeId, decimal grossSalary, DateOnly joiningDate)
    {
        var employeeGrossSalary = new TblEmployeeGrossSalary
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            GrossSalary = grossSalary,
            EffectiveFrom = joiningDate,
            EffectiveTill = null
        };
        await context.TblEmployeeGrossSalaries.AddAsync(employeeGrossSalary);
    }

    public async Task<AddressUnit?> GetAdressUnit(Guid administrativeUnitId)
    {
        var administrativeTypes = await context.Set<TblAdministrativeUnitType>().AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.Name);
        var current = await context.Set<TblAdministrativeUnit>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == administrativeUnitId);

        if (current == null)
            return null;

        var result = new AddressUnit();

        while (current != null)
        {
            if (!administrativeTypes.TryGetValue(current.AdministrativeUnitTypeId, out var typeName))
            {
                break;
            }

            switch (typeName)
            {
                case "Country":
                    result.CountryId = current.Id;
                    result.CountryName = current.Name;
                    break;

                case "State":
                    result.StateId = current.Id;
                    result.StateName = current.Name;
                    break;

                case "District":
                    result.DistrictId = current.Id;
                    result.DistrictName = current.Name;
                    break;

                case "Block":
                    result.BlockId = current.Id;
                    result.BlockName = current.Name;
                    break;

                case "Gram Panchayat":
                    result.GramPanchayatId = current.Id;
                    result.GramPanchayatName = current.Name;
                    break;

                case "Village":
                    result.VillageId = current.Id;
                    result.VillageName = current.Name;
                    break;

                case "Municipality":
                    result.MunicipalityId = current.Id;
                    result.MunicipalityName = current.Name;
                    break;

                case "Ward":
                    result.WardId = current.Id;
                    result.WardName = current.Name;
                    break;
            }

            if (current.ParentId == null)
                break;

            current = await context.Set<TblAdministrativeUnit>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == current.ParentId);
        }

        return result;
    }
    public async Task TransferEmployee(Guid employeeId, Guid organisationUnitId, Guid designationId, Guid salaryGradeId, decimal grossSalary, CancellationToken cancellationToken)
    {
        var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);

        var employee = await context.TblEmployees.FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);

        if (employee is null)
            throw new Exception("Employee not found.");

        var existingDesignation = await context.TblEmployeeDesignations.Where(ed => ed.EmployeeId != employeeId && ed.DesignationId == designationId && ed.EffectiveTo == null).AsNoTracking().FirstOrDefaultAsync();
        if (existingDesignation != null)
        {
            throw new InvalidOperationException("The designation is already assigned to another employee.");
        }

        var currentEmployeeDesignation = await context.TblEmployeeDesignations.FirstOrDefaultAsync(d => d.EmployeeId == employeeId && d.EffectiveTo == null, cancellationToken);

        if (currentEmployeeDesignation is null)
            throw new Exception("Designation is not mapped with the employee.");

        var currentTransfer = await context.TblEmployeeTransfers.FirstOrDefaultAsync(t => t.EmployeeId == employeeId && t.ToDate == null, cancellationToken);

        if (currentTransfer is null)
            throw new Exception("Employee transfer history not found.");

        var designationSalaryGrade = await context.TblDesignationGradeMappings.FirstOrDefaultAsync(d => d.DesignationId == designationId && d.IsActive, cancellationToken);

        if (designationSalaryGrade is null)
            throw new Exception("Employee designation is not mapped with salary grade.");

        var currentGrossSalary = await context.TblEmployeeGrossSalaries.FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.EffectiveTill == null, cancellationToken);

        if (currentGrossSalary is null)
            throw new Exception("Employee gross salary is not mapped.");

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            if (currentEmployeeDesignation.DesignationId != designationId)
            {
                currentEmployeeDesignation.EffectiveTo = DateTime.Now;

                await context.TblEmployeeDesignations.AddAsync(
                    new TblEmployeeDesignation
                    {
                        Id = Guid.NewGuid(),
                        EmployeeId = employeeId,
                        DesignationId = designationId,
                        EffectiveFrom = DateTime.Now,
                        EffectiveTo = null
                    },
                    cancellationToken);
            }

            if (currentEmployeeDesignation.DesignationId != designationId || designationSalaryGrade.GradeId != salaryGradeId)
            {
                designationSalaryGrade.IsActive = false;

                await context.TblDesignationGradeMappings.AddAsync(
                    new TblDesignationGradeMapping
                    {
                        Id = Guid.NewGuid(),
                        DesignationId = designationId,
                        GradeId = salaryGradeId,
                        IsActive = true
                    },
                    cancellationToken);
            }

            if (currentGrossSalary.GrossSalary != grossSalary)
            {
                currentGrossSalary.EffectiveTill = currentDate;

                await context.TblEmployeeGrossSalaries.AddAsync(
                    new TblEmployeeGrossSalary
                    {
                        Id = Guid.NewGuid(),
                        EmployeeId = employeeId,
                        GrossSalary = grossSalary,
                        EffectiveFrom = currentDate,
                        EffectiveTill = null
                    },
                    cancellationToken);
            }

            // Close current transfer
            currentTransfer.ToDate = currentDate;

            // Create new transfer
            await context.TblEmployeeTransfers.AddAsync(
                new TblEmployeeTransfer
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employeeId,
                    OrganisationUnitId = organisationUnitId,
                    DesignationId = designationId,
                    SalaryGradeId = salaryGradeId,
                    GrossSalary = grossSalary,
                    FromDate = currentDate,
                    ToDate = null,
                    IsApprove = false
                },
                cancellationToken);

            employee.OrganizationUnitId = organisationUnitId;

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task TransferEntry(Guid employeeId, Guid organisationUnitId, Guid designationId, Guid salaryGradeId, decimal grossSalary, CancellationToken cancellationToken)
    {
        await context.TblEmployeeTransfers.AddAsync(
            new TblEmployeeTransfer
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                OrganisationUnitId = organisationUnitId,
                DesignationId = designationId,
                SalaryGradeId = salaryGradeId,
                GrossSalary = grossSalary,
                FromDate = DateOnly.FromDateTime(DateTime.Now),
                ToDate = null,
                IsApprove = false
            },
            cancellationToken);
    }

}

