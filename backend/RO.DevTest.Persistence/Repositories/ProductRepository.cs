using MediatR;
using Microsoft.EntityFrameworkCore;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Enums;
using System.Linq.Expressions;

namespace RO.DevTest.Persistence.Repositories
{
    public class ProductRepository(DefaultContext context)
        : BaseRepository<Product>(context), IProductRepository
    {
        public async Task<List<Product>> GetAllActiveProducts(Expression<Func<Product, bool>> predicate = null!, params Expression<Func<Product, object>>[] includes)
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

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await Context.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Product>> GetPagedAsync(int page, int pageSize, string? orderBy, bool ascending, string? search, float? minPrice, float? maxPrice, CategoriesProduct? categoryId)
        {
            var query = Context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search));

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            if (categoryId.HasValue)
                query = query.Where(p => p.Category == categoryId.Value);

            query = orderBy?.ToLower() switch
            {
                "name" => ascending ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                "price" => ascending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Name)
            };

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }


        public async Task<int> GetTotalCountAsync(string? search, float? minPrice, float? maxPrice, CategoriesProduct? categoryId)
        {
            var query = Context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(p => p.Name.Contains(search));

            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            if (categoryId.HasValue)
                query = query.Where(p => p.Category == categoryId.Value);

            return await query.CountAsync();
        }



    }
}
