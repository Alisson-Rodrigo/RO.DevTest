using RO.DevTest.Domain.Abstract;

namespace RO.DevTest.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Amount { get; set; }
        public float UnitPrice { get; set; }

    }
}
