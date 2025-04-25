using Moq;
using RO.DevTest.Application.Features.Cart.Queries.GetCartCommand;
using RO.DevTest.Application.Contracts.Persistance.Repositories;
using RO.DevTest.Application.Contracts.Application.Service;
using RO.DevTest.Domain.Entities;
using RO.DevTest.Domain.Exception;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RO.DevTest.Tests.Unit.Application.Features.Cart.Queries
{
    public class GetCartQueryHandlerTests
    {
        private readonly Mock<ICartRepository> _mockCartRepository;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<ILogged> _mockLogged;
        private readonly GetCartQueryHandler _handler;

        public GetCartQueryHandlerTests()
        {
            _mockCartRepository = new Mock<ICartRepository>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockLogged = new Mock<ILogged>();
            _handler = new GetCartQueryHandler(
                _mockCartRepository.Object,
                _mockProductRepository.Object,
                _mockLogged.Object
            );
        }

        [Fact]
        public async Task Handle_DeveLancarExcecao_QuandoUsuarioNaoEstiverLogado()
        {
            // Arrange
            _mockLogged.Setup(l => l.UserLogged())!.ReturnsAsync((Domain.Entities.User?)null); // Usuário não logado

            var query = new GetCartQuery();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(query, CancellationToken.None));
            Assert.Equal("Usuário não autenticado", exception.Message);
        }

        [Fact]
        public async Task Handle_DeveRetornarListaVazia_QuandoCarrinhoNaoTiverItens()
        {
            // Arrange
            var user = new Domain.Entities.User { Id = Guid.NewGuid().ToString() };
            _mockLogged.Setup(l => l.UserLogged()).ReturnsAsync(user);

            _mockCartRepository.Setup(c => c.GetListAsync(user.Id.ToString())).ReturnsAsync(new List<CartItem>()); // Carrinho vazio

            var query = new GetCartQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Empty(result); // A lista deve estar vazia
        }

        [Fact]
        public async Task Handle_DeveRetornarItensDoCarrinho_QuandoCarrinhoTiverItensValidos()
        {
            // Arrange
            var user = new Domain.Entities.User { Id = Guid.NewGuid().ToString() };
            _mockLogged.Setup(l => l.UserLogged()).ReturnsAsync(user);

            var cartItem = new CartItem { ProductId = Guid.NewGuid(), UnitPrice = 100, Amount = 2, UserId = user.Id };
            var product = new Product { Id = cartItem.ProductId, Name = "Produto A" };

            _mockCartRepository.Setup(c => c.GetListAsync(user.Id.ToString())).ReturnsAsync(new List<CartItem> { cartItem }); // Carrinho com item
            _mockProductRepository.Setup(p => p.GetByIdAsync(cartItem.ProductId)).ReturnsAsync(product); // Produto encontrado

            var query = new GetCartQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Single(result); // Deve haver um item no resultado
            var resultItem = result[0];
            Assert.Equal(cartItem.ProductId, resultItem.ProductId); // Verifica o ID do produto
            Assert.Equal(product.Name, resultItem.ProductName); // Verifica o nome do produto
            Assert.Equal(cartItem.UnitPrice, resultItem.PrecoUnitario); // Verifica o preço unitário
            Assert.Equal(cartItem.Amount, resultItem.Quantidade); // Verifica a quantidade
        }

        [Fact]
        public async Task Handle_DeveIgnorarItensDoCarrinho_QuandoProdutoNaoForEncontrado()
        {
            // Arrange
            var user = new Domain.Entities.User { Id = Guid.NewGuid().ToString() };
            _mockLogged.Setup(l => l.UserLogged()).ReturnsAsync(user);

            var cartItem = new CartItem { ProductId = Guid.NewGuid(), UnitPrice = 100, Amount = 2, UserId = user.Id };

            _mockCartRepository.Setup(c => c.GetListAsync(user.Id.ToString())).ReturnsAsync(new List<CartItem> { cartItem }); // Carrinho com item
            _mockProductRepository.Setup(p => p.GetByIdAsync(cartItem.ProductId)).ReturnsAsync((Product)null!); // Produto não encontrado

            var query = new GetCartQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Empty(result); // Como o produto não foi encontrado, não deve retornar nada
        }
    }
}
