using RO.DevTest.Domain.Entities;


namespace RO.DevTest.Application.Contracts.Persistance.Repositories
{
    public interface ISaleRepository : IBaseRepository<Sale>
    {
        Task<IEnumerable<Sale>> GetAllAsync();
        Task<Sale?> GetByIdAsync(Guid id);
        Task<List<Sale>> GetSalesByPeriod(DateTime startDate, DateTime endDate);
    }
}
