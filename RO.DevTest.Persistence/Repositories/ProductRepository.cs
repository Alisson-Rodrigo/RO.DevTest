using Microsoft.EntityFrameworkCore;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using System.Linq.Expressions;

namespace RO.DevTest.Persistence.Repositories
{
    public class ProductRepository(DefaultContext context)
        : BaseRepository<Product>(context), IProductRepository
    {
        public async Task<List<Product>> GetAllActiveProducts(Expression<Func<Product, bool>> predicate = null, params Expression<Func<Product, object>>[] includes)
        {
            IQueryable<Product> query = Context.Set<Product>().Where(x => x.IsActive);

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return await query.ToListAsync();
        }

    }
}
