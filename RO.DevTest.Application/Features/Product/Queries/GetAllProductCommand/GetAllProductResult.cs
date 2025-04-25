namespace RO.DevTest.Application.Features.Product.Queries.GetAllProductCommand
{
    public class GetAllProductResult
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Price { get; set; }
        public IList<string>? ImageUrl { get; set; } = new List<string>();
        public GetAllProductResult(Domain.Entities.Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            ProductId = product.Id;
            Name = product.Name ?? string.Empty;
            Price = product.Price;
            ImageUrl = product.ImageUrl;
        }

    }
}
