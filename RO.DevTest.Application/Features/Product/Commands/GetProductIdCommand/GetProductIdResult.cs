using RO.DevTest.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.Product.Commands.GetProductIdCommand
{
    public class GetProductIdResult
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public float Price { get; set; }
        public string Description { get; set; }
        public int Stock {get; set; }
        public CategoriesProduct CategoriesProduct { get; set; }
        public IList<string>? ImageUrl { get; set; } = new List<string>();
        public GetProductIdResult(Domain.Entities.Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            ProductId = product.Id;
            Description = product.Description;
            CategoriesProduct = product.Category;
            Stock = product.Stock;
            Name = product.Name;
            Price = product.Price;
            ImageUrl = product.ImageUrl;
        }

    }
}
