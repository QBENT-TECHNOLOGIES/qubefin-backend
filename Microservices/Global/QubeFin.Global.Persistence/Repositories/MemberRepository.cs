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
    public async Task AddAsync(Member member)
    {
        await context.TblMembers.AddAsync(member.ToEntity());
    }

    public async Task UpdateAsync(Member member)
    {
        var existing = await context.TblMembers
            .FirstOrDefaultAsync(x => x.Id == member.Id);

        if (existing is null)
        {
            return;
        }

        var updated = member.ToEntity();

        existing.BranchCode = updated.BranchCode;
        existing.GroupId = updated.GroupId;
        existing.Salutaion = updated.Salutaion;
        existing.FirstName = updated.FirstName;
        existing.MiddleName = updated.MiddleName;
        existing.LastName = updated.LastName;
        existing.FullName = updated.FullName;
        existing.GuardianName = updated.GuardianName;
        existing.MotherName = updated.MotherName;
        existing.SpouseName = updated.SpouseName;
        existing.DateOfBirth = updated.DateOfBirth;
        existing.Gender = updated.Gender;
        existing.Religion = updated.Religion;
        existing.Caste = updated.Caste;
        existing.Nationality = updated.Nationality;
        existing.MaritalStatus = updated.MaritalStatus;
        existing.Qualification = updated.Qualification;
        existing.MobileNo = updated.MobileNo;
        existing.AlternateMobileNo = updated.AlternateMobileNo;
        existing.LastModifiedBy = updated.LastModifiedBy;
        existing.LastModifiedOn = updated.LastModifiedOn;
    }

    public async Task<Member?> GetByIdAsync(Guid id)
    {
        var memberEntity = await context.TblMembers
            .Include(x => x.TblMemberAddresses)
            .Include(x => x.TblMemberDocuments)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return memberEntity is null ? null : memberEntity.ToDomain();
    }
}