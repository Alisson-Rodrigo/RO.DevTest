using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO.DevTest.Persistence.Repositories
{
    public class SaleRepository(DefaultContext context)
        : BaseRepository<Sale>(context), ISaleRepository
    {
        public async Task<IEnumerable<Sale>> GetAllAsync() { return await GetAllAsync(); }
        public async Task<Sale?> GetByIdAsync(Guid id) {  return await GetByIdAsync(id); }
    }
}
