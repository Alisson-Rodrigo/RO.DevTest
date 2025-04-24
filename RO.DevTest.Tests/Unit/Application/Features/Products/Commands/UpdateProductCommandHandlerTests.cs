using Bogus;
using Moq;
using System.Linq.Expressions;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Application.Contracts.Application.Service;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Product.Commands.UpdateProductCommand;
using MediatR;
using RO.DevTest.Domain.Exception;

namespace RO.DevTest.Tests.Application.Features.Products.Commands
{

    public class UpdateProductCommandHandlerTests
    {
        [Fact]
        public async Task Handle_DeveAtualizarProdutoComSucesso_QuandoUsuarioForAdmin()
        {
            // Arrange
            var produtoId = Guid.NewGuid();

            var fakeProdutoExistente = new Faker<Product>()
                .RuleFor(p => p.Id, produtoId)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Lorem.Sentence())
                .RuleFor(p => p.Price, f => float.Parse(f.Commerce.Price()))
                .RuleFor(p => p.Stock, f => f.Random.Int(1, 100))
                .RuleFor(p => p.Category, f => f.PickRandom<RO.DevTest.Domain.Enums.CategoriesProduct>())
                .Generate();

            var fakeCommand = new Faker<UpdateProductCommand>()
                .RuleFor(p => p.Id, produtoId)
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
                .RuleFor(p => p.Price, f => float.Parse(f.Commerce.Price()))
                .RuleFor(p => p.Stock, f => f.Random.Int(1, 100))
                .RuleFor(p => p.Category, f => f.PickRandom<RO.DevTest.Domain.Enums.CategoriesProduct>())
                .RuleFor(p => p.IsActive, true)
                .RuleFor(p => p.Imagens, new List<Microsoft.AspNetCore.Http.IFormFile>()) // Simples: sem imagem
                .Generate();

            var mockLogged = new Mock<ILogged>();
            mockLogged.Setup(x => x.IsInRole("Admin")).ReturnsAsync(true);

            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>())).Returns(fakeProdutoExistente);
            mockRepository.Setup(r => r.Update(It.IsAny<Product>()));

            var handler = new UpdateProductCommandHandler(mockRepository.Object, mockLogged.Object);

            // Act
            var result = await handler.Handle(fakeCommand, CancellationToken.None);

            // Assert
            mockRepository.Verify(r => r.Update(It.Is<Product>(p => p.Id == produtoId)), Times.Once);
            Assert.Equal(MediatR.Unit.Value, result);
        }

        [Fact]
        public async Task Handle_DeveLancarBadRequestException_QuandoProdutoNaoEncontrado()
        {
            // Arrange
            var fakeCommand = new Faker<UpdateProductCommand>()
                .RuleFor(p => p.Id, Guid.NewGuid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Lorem.Sentence())
                .RuleFor(p => p.Price, f => float.Parse(f.Commerce.Price()))
                .RuleFor(p => p.Stock, f => f.Random.Int(1, 100))
                .RuleFor(p => p.Category, f => f.PickRandom<RO.DevTest.Domain.Enums.CategoriesProduct>())
                .RuleFor(p => p.IsActive, true)
                .Generate();

            var mockLogged = new Mock<ILogged>();
            mockLogged.Setup(x => x.IsInRole("Admin")).ReturnsAsync(true);

            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>())).Returns((Product)null!);

            var handler = new UpdateProductCommandHandler(mockRepository.Object, mockLogged.Object);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                handler.Handle(fakeCommand, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_DeveLancarBadRequestException_QuandoValidacaoFalhar()
        {
            // Arrange
            var produtoId = Guid.NewGuid();

            var fakeProdutoExistente = new Product { Id = produtoId };

            var comandoInvalido = new UpdateProductCommand
            {
                Id = produtoId,
                Name = "", // Nome inválido
                Description = "Teste",
                Price = 10,
                Stock = 5,
                Category = RO.DevTest.Domain.Enums.CategoriesProduct.toys,
                IsActive = true
            };

            var mockLogged = new Mock<ILogged>();
            mockLogged.Setup(x => x.IsInRole("Admin")).ReturnsAsync(true);

            var mockRepository = new Mock<IProductRepository>();
            mockRepository.Setup(r => r.Get(It.IsAny<Expression<Func<Product, bool>>>())).Returns(fakeProdutoExistente);

            var handler = new UpdateProductCommandHandler(mockRepository.Object, mockLogged.Object);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                handler.Handle(comandoInvalido, CancellationToken.None));
        }


        [Fact]
        public async Task Handle_DeveLancarForbiddenException_QuandoUsuarioNaoForAdmin()
        {
            // Arrange
            var fakeCommand = new Faker<UpdateProductCommand>()
                .RuleFor(p => p.Id, Guid.NewGuid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
                .RuleFor(p => p.Price, f => float.Parse(f.Commerce.Price()))
                .RuleFor(p => p.Stock, f => f.Random.Int(1, 100))
                .RuleFor(p => p.Category, f => f.PickRandom<RO.DevTest.Domain.Enums.CategoriesProduct>())
                .RuleFor(p => p.IsActive, true)
                .Generate();

            var mockLogged = new Mock<ILogged>();
            mockLogged.Setup(x => x.IsInRole("Admin")).ReturnsAsync(false); // Não é admin

            var mockRepository = new Mock<IProductRepository>();

            var handler = new UpdateProductCommandHandler(mockRepository.Object, mockLogged.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(() =>
                handler.Handle(fakeCommand, CancellationToken.None));
        }

    }

}
