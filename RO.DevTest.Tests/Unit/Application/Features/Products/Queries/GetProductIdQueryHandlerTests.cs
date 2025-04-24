using Moq;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Application.Features.Product.Queries.GetProductIdCommand;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using System.Linq.Expressions;

namespace RO.DevTest.Tests.Unit.Application.Features.Products.Queries
{
    public class GetProductIdQueryHandlerTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly GetProductIdQueryHandler _handler;

        public GetProductIdQueryHandlerTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _handler = new GetProductIdQueryHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_DeveRetornarProduto_QuandoProdutoExistir()
        {
            // Arrange
            var fakeProduct = new Product
            {
                Id = Guid.NewGuid(),
                Name = "Produto Teste",
                Description = "Descrição do produto de teste",
                Price = 100.0f,
                Stock = 10,
                ImageUrl = new List<string> { "http://image.com/produto.jpg" }
            };

            // Configura o mock para retornar o produto fake quando a consulta for feita com o id
            _mockRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
                           .Returns(fakeProduct);

            var command = new GetProductIdQuery { Id = fakeProduct.Id };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(fakeProduct.Id, result.ProductId);
            Assert.Equal(fakeProduct.Name, result.Name);
        }

        [Fact]
        public async Task Handle_DeveLancarExcecao_QuandoProdutoNaoExistir()
        {
            // Arrange
            _mockRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>()))
                           .Returns((Product)null!); // Retorna null para simular produto não encontrado

            var command = new GetProductIdQuery { Id = Guid.NewGuid() };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _handler.Handle(command, CancellationToken.None)); // Espera que lance ArgumentNullException
        }
    }
}
