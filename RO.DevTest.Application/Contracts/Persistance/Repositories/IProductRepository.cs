using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Enums;
using System.Linq.Expressions;

namespace RO.DevTest.Application.Contracts.Persistance.Repositories
{
    public interface IProductRepository : IBaseRepository<Product>
    {
        Task<List<Product>> GetAllActiveProducts(Expression<Func<Product, bool>> predicate = null!, params Expression<Func<Product, object>>[] includes);
        Task<Product?> GetByIdAsync(Guid id);
        Task<List<Product>> GetPagedAsync(int page, int pageSize, string? orderBy, bool ascending, string? search, float? minPrice, float? maxPrice, CategoriesProduct? categoryId);
        Task<int> GetTotalCountAsync(string? search, float? minPrice, float? maxPrice, CategoriesProduct? categoryId);


    }
}
