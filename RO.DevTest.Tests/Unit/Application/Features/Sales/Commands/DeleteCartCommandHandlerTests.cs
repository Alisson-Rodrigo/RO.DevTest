using Moq;
using RO.DevTest.Application.Features.Cart.Commands.DeleteCartCommand;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Contracts.Application.Service;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Exception;


namespace RO.DevTest.Tests.Unit.Application.Features.Cart.Commands
{
    public class DeleteCartCommandHandlerTests
    {
        private readonly Mock<ICartRepository> _mockCartRepository;
        private readonly Mock<ILogged> _mockLogged;
        private readonly DeleteCartCommandHandler _handler;

        public DeleteCartCommandHandlerTests()
        {
            _mockCartRepository = new Mock<ICartRepository>();
            _mockLogged = new Mock<ILogged>();
            _handler = new DeleteCartCommandHandler(
                _mockCartRepository.Object,
                _mockLogged.Object
            );
        }

        [Fact]
        public async Task Handle_DeveLancarExcecao_QuandoUsuarioNaoEstiverLogado()
        {
            // Arrange
            _mockLogged.Setup(l => l.UserLogged()).ReturnsAsync((Domain.Entities.User)null!); // Usuário não logado

            var command = new DeleteCartCommand { Id = Guid.NewGuid() };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Erro ao recuperar usuário", exception.Message);
        }

        [Fact]
        public async Task Handle_DeveLancarExcecao_QuandoProdutoNaoExistirNoCarrinho()
        {
            // Arrange
            var user = new Domain.Entities.User { Id = Guid.NewGuid().ToString() };
            _mockLogged.Setup(l => l.UserLogged()).ReturnsAsync(user);

            _mockCartRepository.Setup(c => c.GetItemAsync(user.Id, It.IsAny<Guid>())).ReturnsAsync((CartItem)null!); // Produto não encontrado no carrinho

            var command = new DeleteCartCommand { Id = Guid.NewGuid() };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
            Assert.Equal("Produto no carrinho não encontrado", exception.Message);
        }

        [Fact]
        public async Task Handle_DeveExcluirProduto_QuandoProdutoExistirNoCarrinho()
        {
            // Arrange
            var user = new Domain.Entities.User { Id = Guid.NewGuid().ToString() };
            _mockLogged.Setup(l => l.UserLogged()).ReturnsAsync(user);

            var cartItem = new CartItem { UserId = user.Id, ProductId = Guid.NewGuid(), Quantidade = 2, PrecoUnitario = 100 };

            _mockCartRepository.Setup(c => c.GetItemAsync(user.Id, It.IsAny<Guid>())).ReturnsAsync(cartItem); // Produto encontrado no carrinho
            _mockCartRepository.Setup(c => c.Delete(It.IsAny<CartItem>())).Verifiable(); // Espera-se que o Delete seja chamado

            var command = new DeleteCartCommand { Id = cartItem.ProductId };

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _mockCartRepository.Verify(c => c.Delete(It.IsAny<CartItem>()), Times.Once); // Verifica se o método Delete foi chamado uma vez
        }
    }
}
