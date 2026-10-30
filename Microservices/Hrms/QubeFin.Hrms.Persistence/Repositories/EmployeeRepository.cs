using System.Data;
using Microsoft.EntityFrameworkCore;
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
    Task<string?> GenerateEmployeeCodeAsync(Guid? companyId, CancellationToken cancellationToken);
    Task AddDesignationAsync(Guid employeeId, Guid designationId, DateOnly joiningDate);
    Task AddGrossSalaryAsync(Guid employeeId, decimal grossSalary, DateOnly joiningDate);
    Task SeparateAsync(Guid employeeId, DateOnly separationDate, Guid modifiedBy, CancellationToken cancellationToken);
    Task TransferEntry(Guid employeeId, Guid organisationUnitId, Guid designationId, Guid salaryGradeId, decimal grossSalary, CancellationToken cancellationToken);
    Task<AddressUnit?> GetAdressUnit(Guid administrativeUnitId);
    Task TransferEmployee(Guid EmployeeId, Guid OrganisationUnitId, DateOnly EffectiveFrom, Guid UserId, CancellationToken cancellationToken);
    Task SaveGrossSalary(Guid EmployeeId, Guid SalaryGradeId, decimal GrossSalary, decimal? PfAmount, DateOnly EffectiveFrom, CancellationToken cancellationToken);
    Task<List<EmployeeSalaryStructure>> GetSalaryStructureByGrossAsync(Guid employeeId, Guid salaryGradeId, decimal grossSalary, decimal? fixedPFAmount, CancellationToken cancellationToken);
    Task<string?> GetJoiningInfoReferenceNoAsync(Guid employeeId, CancellationToken cancellationToken);
    Task<int> CountJoiningInfoCreatedInYearAsync(CancellationToken cancellationToken);
    Task SaveJoiningInfoAsync(Guid employeeId, string referenceNo, DateOnly offerLetterDate, decimal monthlyCtc, Guid userId, CancellationToken cancellationToken);
    Task SetJoiningInfoApprovalAsync(Guid joiningInfoId, string approvalStatus, bool isEmployeeActive, Guid userId, CancellationToken cancellationToken);
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
            .Include(x => x.TblJoiningInfos)
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

    public async Task<string?> GenerateEmployeeCodeAsync(Guid? companyId, CancellationToken cancellationToken)
    {
        if (companyId is null)
        {
            return null;
        }

        var company = await context.TblCompanies.FirstOrDefaultAsync(c => c.Id == companyId.Value, cancellationToken);
        if (company is null || string.IsNullOrWhiteSpace(company.EmpCodeAlias))
        {
            return null;
        }

        var nextSeq = (company.Seq ?? 0) + 1;
        company.Seq = nextSeq;

        return $"{company.EmpCodeAlias.Trim()}{nextSeq:D4}";
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

    // Closes the employee's running designation, gross salary and transfer on the separation date and deactivates
    // the employee's login. Rows added earlier in the same unit of work are closed too.
    public async Task SeparateAsync(Guid employeeId, DateOnly separationDate, Guid modifiedBy, CancellationToken cancellationToken)
    {
        await context.TblEmployeeDesignations.Where(d => d.EmployeeId == employeeId && d.EffectiveTo == null).LoadAsync(cancellationToken);
        foreach (var designation in context.TblEmployeeDesignations.Local.Where(d => d.EmployeeId == employeeId && d.EffectiveTo == null))
        {
            designation.EffectiveTo = separationDate.ToDateTime(TimeOnly.MinValue);
        }

        await context.TblEmployeeGrossSalaries.Where(s => s.EmployeeId == employeeId && s.EffectiveTill == null).LoadAsync(cancellationToken);
        foreach (var grossSalary in context.TblEmployeeGrossSalaries.Local.Where(s => s.EmployeeId == employeeId && s.EffectiveTill == null))
        {
            grossSalary.EffectiveTill = separationDate;
        }

        await context.TblEmployeeTransfers.Where(t => t.EmployeeId == employeeId && t.ToDate == null).LoadAsync(cancellationToken);
        foreach (var transfer in context.TblEmployeeTransfers.Local.Where(t => t.EmployeeId == employeeId && t.ToDate == null))
        {
            transfer.ToDate = separationDate;
        }

        var users = await context.TblUsers.Where(u => u.EmployeeId == employeeId && u.IsActive).ToListAsync(cancellationToken);
        foreach (var user in users)
        {
            user.IsActive = false;
            user.LastModifiedBy = modifiedBy;
            user.LastModifiedOn = DateTime.Now;
        }

        await CloseUserSessionsAsync(employeeId, cancellationToken);
    }

    // Log out the employee's active sessions
    private async Task CloseUserSessionsAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        var activeSessions = await context.TblUserSessions
            .Where(s => s.User.EmployeeId == employeeId && s.LogoutTime == null)
            .ToListAsync(cancellationToken);

        foreach (var session in activeSessions)
        {
            session.LogoutTime = DateTime.Now;
        }
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
    // Moves the employee to another organisation unit. Only the designation changes: a vacant, active designation of
    // the same post in the target unit is reused, otherwise a new one is created there with the current designation's
    // name, post and roles. Either way it is mapped to the employee's current grade. The previous designation and its
    // grade mapping are deactivated. Grade and gross salary are carried over unchanged.
    public async Task TransferEmployee(Guid employeeId, Guid organisationUnitId, DateOnly effectiveFrom, Guid userId, CancellationToken cancellationToken)
    {
        // Running records are closed one day before the new effective date
        var effectiveTill = effectiveFrom.AddDays(-1);

        var employee = await context.TblEmployees.FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);

        if (employee is null)
            throw new Exception("Employee not found.");

        if (employee.OrganizationUnitId == organisationUnitId)
            throw new InvalidOperationException("No changes found. The employee is already assigned to the selected organisation unit.");

        var currentEmployeeDesignation = await context.TblEmployeeDesignations.FirstOrDefaultAsync(d => d.EmployeeId == employeeId && d.EffectiveTo == null, cancellationToken);

        if (currentEmployeeDesignation is null)
            throw new Exception("Designation is not mapped with the employee.");

        var currentDesignation = await context.TblDesignations
            .Include(d => d.TblDesignationRoles)
            .Include(d => d.TblDesignationGradeMappings)
            .FirstAsync(d => d.Id == currentEmployeeDesignation.DesignationId, cancellationToken);

        var currentGradeMapping = currentDesignation.TblDesignationGradeMappings.FirstOrDefault(g => g.IsActive);

        if (currentGradeMapping is null)
            throw new Exception("Employee designation is not mapped with salary grade.");

        var currentTransfer = await context.TblEmployeeTransfers.FirstOrDefaultAsync(t => t.EmployeeId == employeeId && t.ToDate == null, cancellationToken);

        if (currentTransfer is null)
            throw new Exception("Employee transfer history not found.");

        var currentGrossSalary = await context.TblEmployeeGrossSalaries.AsNoTracking().FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.EffectiveTill == null, cancellationToken);

        if (currentGrossSalary is null)
            throw new Exception("Employee gross salary is not mapped.");

        if (effectiveFrom <= currentTransfer.FromDate)
            throw new InvalidOperationException($"Effective From must be later than the current transfer from date ({currentTransfer.FromDate:dd-MM-yyyy}).");

        // Serializable so two concurrent transfers cannot both claim the same vacant designation
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken);

        try
        {
            var salaryGradeId = currentGradeMapping.GradeId;

            // Vacant designation of the same post in the target unit; one already on the employee's grade is preferred
            var vacantDesignation = await context.TblDesignations
                .Include(d => d.TblDesignationGradeMappings)
                .Where(d => d.OrganizationUnitId == organisationUnitId
                    && d.PostId == currentDesignation.PostId
                    && d.IsActive
                    && !d.TblEmployeeDesignations.Any(ed => ed.EffectiveTo == null))
                .OrderByDescending(d => d.TblDesignationGradeMappings.Any(g => g.IsActive && g.GradeId == salaryGradeId))
                .ThenBy(d => d.CreatedOn)
                .FirstOrDefaultAsync(cancellationToken);

            Guid designationId;

            if (vacantDesignation is not null)
            {
                designationId = vacantDesignation.Id;

                // Map the employee's grade to the vacant designation if it is on a different one
                if (!vacantDesignation.TblDesignationGradeMappings.Any(g => g.IsActive && g.GradeId == salaryGradeId))
                {
                    foreach (var gradeMapping in vacantDesignation.TblDesignationGradeMappings.Where(g => g.IsActive))
                    {
                        gradeMapping.IsActive = false;
                    }

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
            }
            else
            {
                designationId = Guid.NewGuid();

                await context.TblDesignations.AddAsync(
                    new TblDesignation
                    {
                        Id = designationId,
                        Name = currentDesignation.Name,
                        OrganizationUnitId = organisationUnitId,
                        PostId = currentDesignation.PostId,
                        IsActive = true,
                        IsIncharge = false,
                        CreatedBy = userId,
                        CreatedOn = DateTime.UtcNow,
                        TblDesignationRoles = currentDesignation.TblDesignationRoles
                            .Select(r => new TblDesignationRole
                            {
                                Id = Guid.NewGuid(),
                                RoleId = r.RoleId,
                                CreatedBy = userId,
                                CreatedOn = DateTime.UtcNow
                            })
                            .ToList(),
                        TblDesignationGradeMappings = new List<TblDesignationGradeMapping>
                        {
                            new TblDesignationGradeMapping
                            {
                                Id = Guid.NewGuid(),
                                GradeId = salaryGradeId,
                                IsActive = true
                            }
                        }
                    },
                    cancellationToken);
            }

            // Deactivate the previous designation and its grade mapping
            currentDesignation.IsActive = false;
            currentDesignation.LastModifiedBy = userId;
            currentDesignation.LastModifiedOn = DateTime.UtcNow;
            currentGradeMapping.IsActive = false;

            // Close current designation and map the resolved one
            currentEmployeeDesignation.EffectiveTo = effectiveTill.ToDateTime(TimeOnly.MinValue);

            await context.TblEmployeeDesignations.AddAsync(
                new TblEmployeeDesignation
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employeeId,
                    DesignationId = designationId,
                    EffectiveFrom = effectiveFrom.ToDateTime(TimeOnly.MinValue),
                    EffectiveTo = null
                },
                cancellationToken);

            // Close current transfer
            currentTransfer.ToDate = effectiveTill;

            // Create new transfer
            await context.TblEmployeeTransfers.AddAsync(
                new TblEmployeeTransfer
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employeeId,
                    OrganisationUnitId = organisationUnitId,
                    DesignationId = designationId,
                    SalaryGradeId = salaryGradeId,
                    GrossSalary = currentGrossSalary.GrossSalary,
                    FromDate = effectiveFrom,
                    ToDate = null,
                    IsApprove = false
                },
                cancellationToken);

            employee.OrganizationUnitId = organisationUnitId;

            await CloseUserSessionsAsync(employeeId, cancellationToken);

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

    public async Task SaveGrossSalary(Guid employeeId, Guid salaryGradeId, decimal grossSalary, decimal? pfAmount, DateOnly effectiveFrom, CancellationToken cancellationToken)
    {
        var employee = await context.TblEmployees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == employeeId, cancellationToken);

        if (employee is null)
            throw new Exception("Employee not found.");

        var currentGrossSalary = await context.TblEmployeeGrossSalaries.FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.EffectiveTill == null, cancellationToken);

        if (currentGrossSalary is null)
            throw new Exception("Employee gross salary is not mapped.");

        var employeeDesignations = await context.TblEmployeeDesignations.Where(d => d.EmployeeId == employeeId).ToListAsync(cancellationToken);

        if (!employeeDesignations.Any())
            throw new Exception("Designation is not mapped with the employee.");

        // Employee last designation
        var currentEmployeeDesignation = employeeDesignations.Any(d => d.EffectiveTo == null) ?
            employeeDesignations.First(d => d.EffectiveTo == null) :
            employeeDesignations.OrderByDescending(d => d.EffectiveFrom).First();

        var designationSalaryGrade = await context.TblDesignationGradeMappings.FirstOrDefaultAsync(d => d.DesignationId == currentEmployeeDesignation.DesignationId && d.IsActive, cancellationToken);

        if (designationSalaryGrade is null)
            throw new Exception("Employee designation is not mapped with salary grade.");

        if (currentGrossSalary.GrossSalary == grossSalary && designationSalaryGrade.GradeId == salaryGradeId)
            throw new InvalidOperationException("No changes found. The employee is already assigned to the selected gross salary and salary grade.");

        if (currentGrossSalary.GrossSalary != grossSalary && effectiveFrom <= currentGrossSalary.EffectiveFrom)
            throw new InvalidOperationException($"Effective From must be later than the current effective from date ({currentGrossSalary.EffectiveFrom:dd-MM-yyyy}).");

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            if (currentGrossSalary.GrossSalary != grossSalary)
            {
                // Close the running gross salary one day before the new effective date
                currentGrossSalary.EffectiveTill = effectiveFrom.AddDays(-1);

                // Create new gross salary
                await context.TblEmployeeGrossSalaries.AddAsync(
                    new TblEmployeeGrossSalary
                    {
                        Id = Guid.NewGuid(),
                        EmployeeId = employeeId,
                        GrossSalary = grossSalary,
                        PfAmount = pfAmount,
                        EffectiveFrom = effectiveFrom,
                        EffectiveTill = null
                    },
                    cancellationToken);
            }

            if (designationSalaryGrade.GradeId != salaryGradeId)
            {
                designationSalaryGrade.IsActive = false;

                await context.TblDesignationGradeMappings.AddAsync(
                    new TblDesignationGradeMapping
                    {
                        Id = Guid.NewGuid(),
                        DesignationId = currentEmployeeDesignation.DesignationId,
                        GradeId = salaryGradeId,
                        IsActive = true
                    },
                    cancellationToken);
            }

            await context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<List<EmployeeSalaryStructure>> GetSalaryStructureByGrossAsync(Guid employeeId, Guid salaryGradeId, decimal grossSalary, decimal? fixedPFAmount, CancellationToken cancellationToken)
    {
        var result = await context.Database.SqlQuery<EmployeeSalaryStructure>($"""
            EXEC [Payroll].[USP_GetSalaryStructure]
                @EmployeeId = {employeeId},
                @SalaryGradeId = {salaryGradeId},
                @GrossSalary = {grossSalary},
                @FixedPFamount = {fixedPFAmount}
            """).ToListAsync(cancellationToken);

        return result;
    }

    public Task<string?> GetJoiningInfoReferenceNoAsync(Guid employeeId, CancellationToken cancellationToken)
    {
        return context.TblJoiningInfos
            .AsNoTracking()
            .Where(j => j.EmployeeId == employeeId)
            .Select(j => j.ReferenceNo)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<int> CountJoiningInfoCreatedInYearAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        return context.TblJoiningInfos.AsNoTracking().CountAsync(j => j.CreatedOn.Year == year, cancellationToken);
    }

    public async Task SaveJoiningInfoAsync(Guid employeeId, string referenceNo, DateOnly offerLetterDate, decimal monthlyCtc, Guid userId, CancellationToken cancellationToken)
    {
        var employeeExists = await context.TblEmployees.AsNoTracking().AnyAsync(e => e.Id == employeeId, cancellationToken);
        if (!employeeExists)
            throw new Exception("Employee not found.");

        var referenceNoExists = await context.TblJoiningInfos.AsNoTracking().AnyAsync(j => j.ReferenceNo == referenceNo && j.EmployeeId != employeeId);
        if (referenceNoExists)
            throw new Exception("Reference number already exists.");


        var joiningInfo = await context.TblJoiningInfos.FirstOrDefaultAsync(j => j.EmployeeId == employeeId, cancellationToken);

        if (joiningInfo is null)
        {
            await context.TblJoiningInfos.AddAsync(
                new TblJoiningInfo
                {
                    Id = Guid.NewGuid(),
                    ReferenceNo = referenceNo,
                    EmployeeId = employeeId,
                    OfferLetterDate = offerLetterDate,
                    MonthlyCtc = monthlyCtc,
                    CreatedBy = userId,
                    CreatedOn = DateTime.UtcNow
                },
                cancellationToken);
            return;
        }

        joiningInfo.ReferenceNo = referenceNo;
        joiningInfo.OfferLetterDate = offerLetterDate;
        joiningInfo.MonthlyCtc = monthlyCtc;
        joiningInfo.LastModifiedBy = userId;
        joiningInfo.LastModifiedOn = DateTime.UtcNow;
    }

    public async Task SetJoiningInfoApprovalAsync(Guid joiningInfoId, string approvalStatus, bool isEmployeeActive, Guid userId, CancellationToken cancellationToken)
    {
        var joiningInfo = await context.TblJoiningInfos
            .Include(j => j.Employee)
            .FirstOrDefaultAsync(j => j.Id == joiningInfoId, cancellationToken);

        if (joiningInfo is null)
            throw new Exception("Joining information not found.");

        if (joiningInfo.ApprovalStatus is not null && joiningInfo.ApprovalStatus != "Pending")
            throw new InvalidOperationException($"Joining information is already {joiningInfo.ApprovalStatus.ToLowerInvariant()}.");

        var now = DateTime.UtcNow;

        joiningInfo.ApprovalStatus = approvalStatus;
        joiningInfo.ApprovedRejectedBy = userId;
        joiningInfo.LastModifiedBy = userId;
        joiningInfo.LastModifiedOn = now;

        joiningInfo.Employee.IsActive = isEmployeeActive;
        joiningInfo.Employee.IsPayrollActive = isEmployeeActive;
        joiningInfo.Employee.LastModifiedBy = userId;
        joiningInfo.Employee.LastModifiedOn = now;
    }

}

