using QubeFin.Persistence.Models.Members;
using Entity = QubeFin.Persistence.Entities.TblMember;

namespace QubeFin.Persistence.Mappers.Global;

public static class MemberMapper
{
    public static Member ToDomain(this Entity entity)
    {
        return new Member(
            entity.Id,
            entity.Code,
            MapPersonalInfo(entity),
            entity.CreatedBy,
            entity.CreatedOn,
            entity.LastModifiedBy,
            entity.LastModifiedOn,
            designationId: entity.DesignationId
        );
    }

    public static Entity ToEntity(this Member member)
    {
        var fullName = string.Join(" ",
            new[]
            {
                member.PersonalInfo.FirstName,
                member.PersonalInfo.MiddleName,
                member.PersonalInfo.LastName
            }.Where(x => !string.IsNullOrWhiteSpace(x)));

        return new Entity
        {
            Id = member.Id,
            Code = member.Code,
            DesignationId = member.DesignationId,
            BranchCode = member.PersonalInfo.BranchCode,
            GroupId = member.PersonalInfo.GroupId,
            Salutaion = member.PersonalInfo.Salutaion,
            FirstName = member.PersonalInfo.FirstName,
            MiddleName = member.PersonalInfo.MiddleName,
            LastName = member.PersonalInfo.LastName,
            FullName = fullName,
            GuardianName = member.PersonalInfo.GuardianName,
            MotherName = member.PersonalInfo.MotherName,
            SpouseName = member.PersonalInfo.SpouseName,
            DateOfBirth = member.PersonalInfo.DateOfBirth,
            Gender = member.PersonalInfo.Gender,
            Religion = member.PersonalInfo.Religion,
            Caste = member.PersonalInfo.Caste,
            Nationality = member.PersonalInfo.Nationality,
            MaritalStatus = member.PersonalInfo.MaritalStatus,
            Qualification = member.PersonalInfo.EducationalQualification,
            MobileNo = member.PersonalInfo.MobileNo,
            AlternateMobileNo = member.PersonalInfo.AlternateMobileNo,
            AadharNo = string.Empty,

            CreatedBy = member.CreatedBy,
            CreatedOn = member.CreatedDate,
            LastModifiedBy = member.LastModifiedBy,
            LastModifiedOn = member.LastModifiedOn
        };
    }

    private static PersonalInfo MapPersonalInfo(Entity entity)
    {
        return new PersonalInfo(
            
            entity.BranchCode,
            entity.GroupId,
            entity.Code,
            entity.Salutaion,
            entity.FirstName,
            entity.MiddleName,
            entity.LastName,
            entity.GuardianName,
            entity.MotherName,
            entity.SpouseName,
            entity.DateOfBirth,
            entity.Gender,
            entity.Religion,
            entity.Caste,
            entity.Nationality,
            entity.MaritalStatus,
            entity.Qualification,
            entity.MobileNo,
            entity.AlternateMobileNo
        );
    }
}