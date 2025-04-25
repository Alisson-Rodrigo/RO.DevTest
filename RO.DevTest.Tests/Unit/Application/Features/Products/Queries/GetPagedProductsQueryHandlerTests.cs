using Moq;
using RO.DevTest.Application.Features.Product.Queries.Pages;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RO.DevTest.Tests.Unit.Application.Features.Products.Queries
{
    public class GetPagedProductsQueryHandlerTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly GetPagedProductsQueryHandler _handler;

        public GetPagedProductsQueryHandlerTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _handler = new GetPagedProductsQueryHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_DeveRetornarPaginaDeProdutos_QuandoExistiremProdutos()
        {
            // Arrange
            var fakeProducts = new List<Product>
            {
                new Product { Id = Guid.NewGuid(), Name = "Produto 1", Price = 100.0f, Stock = 10 },
                new Product { Id = Guid.NewGuid(), Name = "Produto 2", Price = 150.0f, Stock = 20 }
            };

            var pagedRequest = new PagedRequest
            {
                Page = 1,
                PageSize = 2,
                OrderBy = "Name",
                Ascending = true,
                Search = "Produto"
            };

            _mockRepository.Setup(r => r.GetPagedAsync(pagedRequest.Page, pagedRequest.PageSize, pagedRequest.OrderBy, pagedRequest.Ascending, pagedRequest.Search))
                           .ReturnsAsync(fakeProducts);

            _mockRepository.Setup(r => r.GetTotalCountAsync(pagedRequest.Search))
                           .ReturnsAsync(2); // Total de produtos encontrados

            // Act
            var result = await _handler.Handle(pagedRequest, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pagedRequest.Page, result.Page);
            Assert.Equal(pagedRequest.PageSize, result.PageSize);
            Assert.Equal(2, result.TotalItems); // Total de produtos encontrados
            Assert.Equal(2, result.Items.Count); // Número de produtos retornados
            Assert.All(result.Items, item => Assert.False(string.IsNullOrWhiteSpace(item.Name))); // Verificar que os nomes não são nulos ou vazios
        }

        [Fact]
        public async Task Handle_DeveRetornarPaginaVazia_QuandoNaoExistiremProdutos()
        {
            // Arrange
            var pagedRequest = new PagedRequest
            {
                Page = 1,
                PageSize = 2,
                OrderBy = "Name",
                Ascending = true,
                Search = "ProdutoInexistente"
            };

            _mockRepository.Setup(r => r.GetPagedAsync(pagedRequest.Page, pagedRequest.PageSize, pagedRequest.OrderBy, pagedRequest.Ascending, pagedRequest.Search))
                           .ReturnsAsync(new List<Product>());

            _mockRepository.Setup(r => r.GetTotalCountAsync(pagedRequest.Search))
                           .ReturnsAsync(0); // Nenhum produto encontrado

            // Act
            var result = await _handler.Handle(pagedRequest, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pagedRequest.Page, result.Page);
            Assert.Equal(pagedRequest.PageSize, result.PageSize);
            Assert.Equal(0, result.TotalItems); // Nenhum produto encontrado
            Assert.Empty(result.Items); // Nenhuma página de produtos
        }
    }
}
