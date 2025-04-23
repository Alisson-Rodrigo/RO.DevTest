using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO.DevTest.Application.Features.Cart.Queries.GetCartCommand
{
    public class CartItemResult
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public float PrecoUnitario { get; set; }
        public int Quantidade { get; set; }
        public float SubTotal => PrecoUnitario * Quantidade;
    }

}
