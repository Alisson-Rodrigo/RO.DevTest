using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Domain.Entities
{
    public class Sale : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        public DateTime DateSale { get; set; } = DateTime.UtcNow;
        public float Total { get; set; } 
        public ICollection<SaleItem> Itens { get; set; } = new List<SaleItem>();
    }

}
