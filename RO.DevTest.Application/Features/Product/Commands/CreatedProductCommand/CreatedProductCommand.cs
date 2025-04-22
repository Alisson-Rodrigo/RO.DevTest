using MediatR;
using Microsoft.AspNetCore.Http;
using RO.DevTest.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.Product.Commands.CreatedProductCommand
{
    public class CreatedProductCommand : IRequest<Unit>
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public float Price { get; set; }
        public int Stock { get; set; }
        public List<IFormFile>? Imagens { get; set; }
        public CategoriesProduct Categories { get; set; }

        public Domain.Entities.Product AssignToProduct()
        {
            return new Domain.Entities.Product
            {
                Name = Name,
                Description = Description,
                Price = Price,
                Stock = Stock,
                Category = Categories
            };
        }
    }
}
