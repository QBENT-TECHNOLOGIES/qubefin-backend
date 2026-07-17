using QubeFin.Persistence.Models.Global;
using Entity = QubeFin.Persistence.Entities.TblCompany;

namespace QubeFin.Persistence.Mappers.Global
{
    public static class CompanyMapper
    {
        public static Company ToDomain(this Entity entity)
        {
            return new Company
            (entity.Id, entity.Name);
        }
    }
}
