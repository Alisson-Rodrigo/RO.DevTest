using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Domain.Entities
{
    public class SaleItem : BaseEntity
    {
        public Guid SaleId { get; set; }
        public Sale Sale { get; set; } = null!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Amount { get; set; }
        public float UnitPrice { get; set; }
    }

}
