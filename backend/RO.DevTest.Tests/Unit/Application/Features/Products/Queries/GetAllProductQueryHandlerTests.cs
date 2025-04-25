using Moq;
using Bogus;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Application.Features.Product.Queries.GetAllProductCommand;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;


namespace RO.DevTest.Tests.Unit.Application.Features.Products.Queries
{
    public class GetAllProductQueryHandlerTests
    {
        private readonly Mock<IProductRepository> _mockRepository;
        private readonly GetAllProductQueryHandler _handler;

        public GetAllProductQueryHandlerTests()
        {
            _mockRepository = new Mock<IProductRepository>();
            _handler = new GetAllProductQueryHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_DeveRetornarListaDeProdutos_QuandoProdutosExistirem()
        {
            // Arrange
            var faker = new Faker<Product>()
                .RuleFor(p => p.Id, f => f.Random.Guid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Commerce.ProductAdjective())
                .RuleFor(p => p.Price, f => float.Parse(f.Commerce.Price()))
                .RuleFor(p => p.Stock, f => f.Random.Int(1, 100))
                .RuleFor(p => p.ImageUrl, new List<string> { "http://image.com/produto.jpg" });

            var fakeProdutos = faker.Generate(3);

            _mockRepository.Setup(r => r.GetAllActiveProducts(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<Expression<Func<Product, object>>[]>()))
                           .ReturnsAsync(fakeProdutos); 

            var command = new GetAllProductCommand();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.All(result, p => Assert.False(string.IsNullOrWhiteSpace(p.Name)));
        }

        [Fact]
        public async Task Handle_DeveLancarExcecao_QuandoNaoExistiremProdutos()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAllActiveProducts(It.IsAny<Expression<Func<Product, bool>>>(), It.IsAny<Expression<Func<Product, object>>[]>()))
                           .ReturnsAsync(new List<Product>()); 

            var command = new GetAllProductCommand();

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(() =>
                _handler.Handle(command, CancellationToken.None)); 
        }
    }
}
