using RO.DevTest.Domain.Entities;
using System.Linq.Expressions;

namespace RO.DevTest.Application.Contracts.Persistance.Repositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<List<Product>> GetAllActiveProducts(Expression<Func<Product, bool>> predicate = null, params Expression<Func<Product, object>>[] includes);
        Task<Product?> GetByIdAsync(Guid id);

    }
}
