using RO.DevTest.Domain.Abstract;
using RO.DevTest.Domain.Enums;


namespace RO.DevTest.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float Price { get; set; }
        public int Stock {  get; set; }
        public CategoriesProduct Category { get; set; }
        public ICollection<string>? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
