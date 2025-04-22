using Microsoft.EntityFrameworkCore;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;

namespace RO.DevTest.Persistence.Repositories
{
    public class CartRepository(DefaultContext context)
        : BaseRepository<CartItem>(context), ICartRepository
    {
        public async Task<IEnumerable<CartItem>> GetUserCartAsync(string userId)
        {
            return Context.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Product);
        }

        public async Task<CartItem?> GetItemAsync(string userId, Guid productId)
        {
            return await Context.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductId == productId);
        }
    }

}
