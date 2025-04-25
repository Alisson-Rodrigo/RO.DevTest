using Bogus;
using Moq;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Contracts.Application.Service;
using RO.DevTest.Application.Features.Product.Commands.DeleteProductCommand;
using System.Linq.Expressions;


namespace RO.DevTest.Tests.Application.Features.Products.Commands
{

    public class DeleteProductCommandHandlerTests
    {
        [Fact]
        public async Task Handle_DeveDeletarProdutoComSucesso_QuandoUsuarioForAdmin()
        {
            // Arrange
            var productId = Guid.NewGuid();

            var fakeProduct = new Faker<Domain.Entities.Product>()
                .RuleFor(p => p.Id, productId)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .Generate();

            var mockLogged = new Mock<ILogged>();
            mockLogged.Setup(x => x.IsInRole("Admin")).ReturnsAsync(true);

            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Domain.Entities.Product, bool>>>())).Returns(fakeProduct);
            mockRepository.Setup(r => r.Delete(It.IsAny<Domain.Entities.Product>()));

            var handler = new DeleteProductCommandHandler(mockLogged.Object, mockRepository.Object);
            var command = new DeleteProductCommand { Id = productId };

            // Act
            var result = await handler.Handle(command);

            // Assert
            mockRepository.Verify(r => r.Delete(fakeProduct), Times.Once);
            Assert.Equal(MediatR.Unit.Value, result);
        }
    }

}
