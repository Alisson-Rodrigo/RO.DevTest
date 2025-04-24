using Moq;
using Xunit;
using RO.DevTest.Application.Features.Sale.Commands.CreatedSaleCommand;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Exception;
using Bogus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RO.DevTest.Application.Contracts.Application.Service;

namespace RO.DevTest.Tests.Application.Features.Sales.Commands
{
    public class CreateSaleCommandHandlerTests
    {
        private readonly Mock<ILogged> _mockLogged;
        private readonly Mock<ICartRepository> _mockCartRepository;
        private readonly Mock<ISaleRepository> _mockSaleRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly CreateSaleCommandHandler _handler;
        private readonly Faker _faker;

        public CreateSaleCommandHandlerTests()
        {
            _mockLogged = new Mock<ILogged>();
            _mockCartRepository = new Mock<ICartRepository>();
            _mockSaleRepository = new Mock<ISaleRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _handler = new CreateSaleCommandHandler(
                _mockLogged.Object,
                _mockCartRepository.Object,
                _mockSaleRepository.Object,
                _mockProductRepository.Object
            );
            _faker = new Faker(); 
        }

        [Fact]
        public async Task Handle_WhenUserIsNotAuthenticated_ShouldThrowBadRequestException()
        {
            _mockLogged.Setup(x => x.UserLogged()).ReturnsAsync((User)null!); // Usando 'User' no lugar de 'ApplicationUser'

            var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
                _handler.Handle(new CreatedSaleCommand(), CancellationToken.None)
            );
            Assert.Equal("Usuário não autenticado", exception.Message);
        }

        [Fact]
        public async Task Handle_WhenCartIsEmpty_ShouldThrowBadRequestException()
        {
            var user = new User { Id = _faker.Random.Guid().ToString() }; // Convertendo Guid para string
            _mockLogged.Setup(x => x.UserLogged()).ReturnsAsync(user);

            _mockCartRepository.Setup(x => x.GetListAsync(user.Id)) // Agora é uma string
                .ReturnsAsync(new List<CartItem>());

            var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
                _handler.Handle(new CreatedSaleCommand(), CancellationToken.None)
            );
            Assert.Equal("Carrinho vazio", exception.Message);
        }

        [Fact]
        public async Task Handle_WhenProductIsNotFound_ShouldThrowBadRequestException()
        {
            var user = new User { Id = _faker.Random.Guid().ToString() }; // Convertendo Guid para string
            var cartItems = new List<CartItem>
            {
                new CartItem { ProductId = _faker.Random.Guid(), Amount = 1, UnitPrice = (float)_faker.Finance.Amount() }
            };
            _mockLogged.Setup(x => x.UserLogged()).ReturnsAsync(user);
            _mockCartRepository.Setup(x => x.GetListAsync(user.Id)).ReturnsAsync(cartItems);
            _mockProductRepository.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))!.ReturnsAsync((Domain.Entities.Product?)null);

            var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
                _handler.Handle(new CreatedSaleCommand(), CancellationToken.None)
            );
            Assert.Equal($"Produto {cartItems[0].ProductId} não encontrado", exception.Message);
        }

        [Fact]
        public async Task Handle_WhenSaleIsCreated_ShouldReturnSaleId()
        {
            // Arrange
            var user = new User { Id = _faker.Random.Guid().ToString() }; 
            var cartItems = new List<CartItem>
            {
                new CartItem { ProductId = _faker.Random.Guid(), Amount = 2, UnitPrice = (float)_faker.Finance.Amount() }
            };
            var product = new Domain.Entities.Product { Id = cartItems[0].ProductId, Stock = 10, Name = _faker.Commerce.ProductName() };
            _mockLogged.Setup(x => x.UserLogged()).ReturnsAsync(user);
            _mockCartRepository.Setup(x => x.GetListAsync(user.Id)).ReturnsAsync(cartItems);
            _mockProductRepository.Setup(x => x.GetByIdAsync(cartItems[0].ProductId)).ReturnsAsync(product);
            _mockSaleRepository.Setup(x => x.CreateAsync(It.IsAny<Sale>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Sale { Id = _faker.Random.Guid() });
            _mockProductRepository.Setup(x => x.Update(It.IsAny<Domain.Entities.Product>())).Verifiable();

            var saleId = await _handler.Handle(new CreatedSaleCommand(), CancellationToken.None);

            Assert.NotEqual(Guid.Empty, saleId);
            _mockProductRepository.Verify(x => x.Update(It.IsAny<Domain.Entities.Product>()), Times.Once);
        }
    }
}
