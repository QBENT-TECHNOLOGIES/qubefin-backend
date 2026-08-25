using Microsoft.EntityFrameworkCore;
using QubeFin.Persistence;
using QubeFin.Persistence.Mappers.Global;
using QubeFin.Persistence.Models.Global;

namespace QubeFin.Global.Persistence.Repositories
{
    public interface IBankRepository
    {
        Task<IEnumerable<Bank>> GetAllBanks();
    }

    public class BankRepository(QubeFinDataContext context) : IBankRepository
    {
        public async Task<IEnumerable<Bank>> GetAllBanks()
        {
            var entities = await context.TblFinancialInstitutes.AsNoTracking().OrderBy(b => b.Name).ToListAsync();
            return entities.Select(m => m.ToDomain());
        }
    }
}
