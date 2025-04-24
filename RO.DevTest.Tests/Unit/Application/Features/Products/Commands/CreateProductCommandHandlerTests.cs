using Bogus;
using FluentAssertions;
using Moq;
using RO.DevTest.Application.Contracts.Application.Service;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Features.Product.Commands.CreatedProductCommand;
using RO.DevTest.Domain.Exception;
using Xunit;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text;
using MediatR;
using RO.DevTest.Domain.Enums;

namespace RO.DevTest.Tests.Application.Features.Products.Commands
{
    public class CreateProductCommandHandlerTests
    {
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<ILogged> _loggedMock;
        private readonly CreateProductCommandHandler _handler;

        public CreateProductCommandHandlerTests()
        {
            _productRepositoryMock = new Mock<IProductRepository>();
            _loggedMock = new Mock<ILogged>();
            _handler = new CreateProductCommandHandler(_productRepositoryMock.Object, _loggedMock.Object);
        }

        [Fact(DisplayName = "Deve criar produto com sucesso quando usuário for admin")]
        public async Task Handle_DeveCriarProdutoComSucesso_QuandoUsuarioForAdmin()
        {
            // Arrange
            _loggedMock.Setup(x => x.IsInRole("Admin")).ReturnsAsync(true);

            var fakeCommand = new Faker<CreatedProductCommand>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Description, f => f.Lorem.Paragraph())
                .RuleFor(p => p.Price, f => float.Parse(f.Commerce.Price()))
                .RuleFor(p => p.Stock, f => f.Random.Int(1, 100))
                .RuleFor(p => p.Categories, f => f.PickRandom<CategoriesProduct>())
                .Generate();

            // Adiciona imagem fake simulando um IFormFile
            var fileMock = new Mock<IFormFile>();
            var content = "fake image content";
            var fileName = "imagem.jpg";
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(content));
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.Length).Returns(ms.Length);
            fileMock.Setup(f => f.OpenReadStream()).Returns(ms);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                    .Returns<Stream, CancellationToken>((stream, token) => ms.CopyToAsync(stream, token));

            fakeCommand.Imagens = new List<IFormFile> { fileMock.Object };

            // Act
            var result = await _handler.Handle(fakeCommand, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            _productRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Domain.Entities.Product>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
