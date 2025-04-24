using Moq;
using RO.DevTest.Application.Features.Cart.Commands.UpdateCartCommand;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Contracts.Application.Service;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Exception;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RO.DevTest.Tests.Unit.Application.Features.Cart.Commands
{
    public class UpdateCartCommandHandlerTests
    {
        private readonly Mock<ICartRepository> _mockCartRepository;
        private readonly Mock<ILogged> _mockLogged;
        private readonly UpdateCartCommandHandler _handler;

        public UpdateCartCommandHandlerTests()
        {
            _mockCartRepository = new Mock<ICartRepository>();
            _mockLogged = new Mock<ILogged>();
            _handler = new UpdateCartCommandHandler(
                _mockLogged.Object,
                _mockCartRepository.Object
            );
        }

        [Fact]
        public async Task Handle_DeveLancarExcecao_QuandoUsuarioNaoEstiverLogado()
        {
            // Arrange
            _mockLogged.Setup(l => l.UserLogged()).ReturnsAsync((Domain.Entities.User)null!); // Usuário não logado

            var command = new UpdateCartCommand { ProductId = Guid.NewGuid(), Quantidade = 1 };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Usuário não autenticado", exception.Message);
        }

        [Fact]
        public async Task Handle_DeveLancarExcecao_QuandoProdutoNaoExistirNoCarrinho()
        {
            // Arrange
            var user = new Domain.Entities.User { Id = Guid.NewGuid().ToString() };
            _mockLogged.Setup(l => l.UserLogged()).ReturnsAsync(user);

            _mockCartRepository.Setup(c => c.GetItemAsync(user.Id, It.IsAny<Guid>())).ReturnsAsync((CartItem)null!); // Produto não encontrado no carrinho

            var command = new UpdateCartCommand { ProductId = Guid.NewGuid(), Quantidade = 1 };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Produto não encontrado no carrinho", exception.Message);
        }

        [Fact]
        public async Task Handle_DeveAtualizarQuantidade_QuandoProdutoExistirNoCarrinhoComQuantidadePositiva()
        {
            // Arrange
            var user = new Domain.Entities.User { Id = Guid.NewGuid().ToString() };
            _mockLogged.Setup(l => l.UserLogged()).ReturnsAsync(user);

            var cartItem = new CartItem { UserId = user.Id, ProductId = Guid.NewGuid(), Quantidade = 2, PrecoUnitario = 100 };

            _mockCartRepository.Setup(c => c.GetItemAsync(user.Id, cartItem.ProductId)).ReturnsAsync(cartItem); // Produto encontrado no carrinho
            _mockCartRepository.Setup(c => c.Update(It.IsAny<CartItem>())).Verifiable(); // Espera-se que o Update seja chamado

            var command = new UpdateCartCommand { ProductId = cartItem.ProductId, Quantidade = 3 };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockCartRepository.Verify(c => c.Update(It.IsAny<CartItem>()), Times.Once); // Verifica se o método Update foi chamado uma vez
            Assert.Equal(3, cartItem.Quantidade); // A quantidade deve ser atualizada para 3
        }

        [Fact]
        public async Task Handle_DeveRemoverProduto_QuandoQuantidadeForZeroOuNegativa()
        {
            // Arrange
            var user = new Domain.Entities.User { Id = Guid.NewGuid().ToString() };
            _mockLogged.Setup(l => l.UserLogged()).ReturnsAsync(user);

            var cartItem = new CartItem { UserId = user.Id, ProductId = Guid.NewGuid(), Quantidade = 2, PrecoUnitario = 100 };

            _mockCartRepository.Setup(c => c.GetItemAsync(user.Id, cartItem.ProductId)).ReturnsAsync(cartItem); // Produto encontrado no carrinho
            _mockCartRepository.Setup(c => c.Delete(It.IsAny<CartItem>())).Verifiable(); // Espera-se que o Delete seja chamado

            var command = new UpdateCartCommand { ProductId = cartItem.ProductId, Quantidade = 0 };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockCartRepository.Verify(c => c.Delete(It.IsAny<CartItem>()), Times.Once); // Verifica se o método Delete foi chamado uma vez
        }
    }
}
