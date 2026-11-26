using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Global;
using QubeFin.Persistence.Models.Members;

namespace QubeFin.Global.Persistence.Repositories;

public interface IMemberRepository
{
    Task<Member?> GetByIdAsync(Guid id);
    Task AddAsync(Member member);
    Task UpdateAsync(Member member);
}

public class MemberRepository(QubeFinDataContext context) : IMemberRepository
{
    public async Task<Member?> GetByIdAsync(Guid id)
    {
        var memberEntity = await context.TblMembers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return memberEntity is null ? null : memberEntity.ToDomain();
    }

    public async Task AddAsync(Member member)
    {
        await context.TblMembers.AddAsync(member.ToEntity());
    }

    public async Task UpdateAsync(Member member)
    {
        var entity = await context.TblMembers.FirstOrDefaultAsync(x => x.Id == member.Id);
        if (entity is null)
        {
            return;
        }

        var updated = member.ToEntity();

        entity.BranchCode = updated.BranchCode;
        entity.Salutaion = updated.Salutaion;
        entity.FirstName = updated.FirstName;
        entity.MiddleName = updated.MiddleName;
        entity.LastName = updated.LastName;
        entity.FullName = updated.FullName;
        entity.GuardianName = updated.GuardianName;
        entity.MotherName = updated.MotherName;
        entity.SpouseName = updated.SpouseName;
        entity.DateOfBirth = updated.DateOfBirth;
        entity.Gender = updated.Gender;
        entity.Religion = updated.Religion;
        entity.Caste = updated.Caste;
        entity.Nationality = updated.Nationality;
        entity.MaritalStatus = updated.MaritalStatus;
        entity.Qualification = updated.Qualification;
        entity.MobileNo = updated.MobileNo;
        entity.AlternateMobileNo = updated.AlternateMobileNo;
        entity.LastModifiedBy = updated.LastModifiedBy;
        entity.LastModifiedOn = updated.LastModifiedOn;
    }
}