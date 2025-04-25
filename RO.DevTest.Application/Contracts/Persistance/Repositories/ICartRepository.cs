using RO.DevTest.Domain.Entities;


namespace RO.DevTest.Application.Contracts.Persistance.Repositories
{
    public interface ICartRepository : IBaseRepository<CartItem>
    {
        Task<IEnumerable<CartItem>> GetUserCartAsync(string userId);
        Task<CartItem?> GetItemAsync(string userId, Guid productId);
        Task<List<CartItem>> GetListAsync(string userId);

        Task DeleteAllAsync(string userId);

    }

}
