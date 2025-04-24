using Moq;
using Bogus;
using RO.DevTest.Application.Features.Cart.Commands;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Contracts.Application.Service;
using RO.DevTest.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;


namespace RO.DevTest.Tests.Unit.Application.Features.Cart.Commands
{
    public class CreatedCartCommandHandlerTests
    {
        private readonly Mock<ICartRepository> _mockCartRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ILogged> _mockLogged;
        private readonly CreatedCartCommandHandler _handler;

        public CreatedCartCommandHandlerTests()
        {
            _mockCartRepository = new Mock<ICartRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockLogged = new Mock<ILogged>();
            _handler = new CreatedCartCommandHandler(
                _mockCartRepository.Object,
                _mockProductRepository.Object,
                _mockLogged.Object
            );
        }

        [Fact]
        public async Task Handle_DeveAdicionarItemAoCarrinho_QuandoProdutoNaoExistirNoCarrinho()
        {
            var userFaker = new Faker<Domain.Entities.User>()
                .RuleFor(u => u.Id, f => f.Random.Guid().ToString())  // Gerar Guid
                .RuleFor(u => u.UserName, f => f.Internet.UserName());  // Gerar nome de usuário

            var user = userFaker.Generate();  // Gerar usuário

            Assert.NotNull(user);
            Assert.NotEmpty(user.UserName!);

            var productFaker = new Faker<Product>()
                .RuleFor(p => p.Id, f => f.Random.Guid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => f.Random.Float(10, 1000))
                .RuleFor(p => p.Stock, f => f.Random.Int(1, 100));

            var product = productFaker.Generate();

            _mockLogged.Setup(l => l.UserLogged()).ReturnsAsync(user);
            _mockProductRepository.Setup(pr => pr.Get(It.IsAny<Expression<Func<Product, bool>>>()))
                .Returns(product); 

            _mockCartRepository.Setup(c => c.GetItemAsync(It.IsAny<Guid>().ToString(), It.IsAny<Guid>())).ReturnsAsync((CartItem)null!);  // Produto não existe no carrinho

            var command = new CreatedCartCommand
            {
                ProductId = product.Id,
                Quantidade = 2
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockCartRepository.Verify(c => c.CreateAsync(It.IsAny<CartItem>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveAtualizarQuantidade_QuandoProdutoExistirNoCarrinho()
        {
            var userFaker = new Faker<Domain.Entities.User>()
                .RuleFor(u => u.Id, f => f.Random.Guid().ToString())  // Gerar Guid
                .RuleFor(u => u.UserName, f => f.Internet.UserName());  // Gerar nome de usuário

            var user = userFaker.Generate();  // Gerar usuário

            Assert.NotNull(user);
            Assert.NotEmpty(user.UserName!);

            var productFaker = new Faker<Product>()
                .RuleFor(p => p.Id, f => f.Random.Guid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => f.Random.Float(10, 1000))
                .RuleFor(p => p.Stock, f => f.Random.Int(1, 100));

            var product = productFaker.Generate();

            _mockLogged.Setup(l => l.UserLogged()).ReturnsAsync(user);
            _mockProductRepository.Setup(pr => pr.Get(It.IsAny<Expression<Func<Product, bool>>>()))
                .Returns(product); 

            var cartItem = new CartItem
            {
                UserId = user.Id,
                ProductId = product.Id,
                Quantidade = 1,
                PrecoUnitario = product.Price
            };

            _mockCartRepository.Setup(c => c.GetItemAsync(user.Id, product.Id)).ReturnsAsync(cartItem); // Produto já existe no carrinho

            var command = new CreatedCartCommand
            {
                ProductId = product.Id,
                Quantidade = 2
            };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockCartRepository.Verify(c => c.Update(It.IsAny<CartItem>()), Times.Once);
            Assert.Equal(3, cartItem.Quantidade); 
        }

        [Fact]
        public async Task Handle_DeveLancarExcecao_QuandoProdutoNaoEncontrado()
        {
            var userFaker = new Faker<Domain.Entities.User>()
                .RuleFor(u => u.Id, f => f.Random.Guid().ToString())  // Gerar Guid
                .RuleFor(u => u.UserName, f => f.Internet.UserName());  // Gerar nome de usuário

            var user = userFaker.Generate();  // Gerar usuário

            Assert.NotNull(user);
            Assert.NotEmpty(user.UserName!);

            _mockLogged.Setup(l => l.UserLogged()).ReturnsAsync(user);
            _mockProductRepository.Setup(pr => pr.Get(It.IsAny<Expression<Func<Product, bool>>>()))
                .Returns((Product)null!); 

            var command = new CreatedCartCommand
            {
                ProductId = Guid.NewGuid(),
                Quantidade = 1
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadHttpRequestException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}
