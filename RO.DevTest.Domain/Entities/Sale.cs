using RO.DevTest.Domain.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO.DevTest.Domain.Entities
{
    public class Sale : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public DateTime DataVenda { get; set; } = DateTime.UtcNow;
        public float Total { get; set; } 
        public ICollection<SaleItem> Itens { get; set; } = new List<SaleItem>();
    }

}
