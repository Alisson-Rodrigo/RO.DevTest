using Bogus;
using Moq;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Features.User.Queries.GetIdUserCommand;
using RO.DevTest.Domain.Exception;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace RO.DevTest.Tests.Unit.Application.Features.User.Queries
{
    public class GetUserIdCommandHandlerTests
    {
        private readonly Mock<IIdentityAbstractor> _identityAbstractorMock;
        private readonly GetUserIdCommandHandler _handler;
        private readonly Faker _faker;

        public GetUserIdCommandHandlerTests()
        {
            _identityAbstractorMock = new Mock<IIdentityAbstractor>();
            _handler = new GetUserIdCommandHandler(_identityAbstractorMock.Object);
            _faker = new Faker();  // Inicializando o Faker
        }

        [Fact(DisplayName = "Deve retornar usuário encontrado com sucesso")]
        public async Task Handle_DeveRetornarUsuario_ComSucesso()
        {
            // Arrange
            var expectedUser = new Domain.Entities.User
            {
                Id = _faker.Random.Guid().ToString(),
                UserName = _faker.Internet.UserName(),
                Name = _faker.Name.FullName(),
                Email = _faker.Internet.Email()
            };

            var command = new GetUserIdCommand
            {
                Id = Guid.Parse(expectedUser.Id)
            };

            _identityAbstractorMock.Setup(x => x.FindUserByIdAsync(command.Id.ToString()))
                                   .ReturnsAsync(expectedUser);

            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.UserName.Should().Be(expectedUser.UserName);
            result.Name.Should().Be(expectedUser.Name);
            result.Email.Should().Be(expectedUser.Email);
        }



        [Fact(DisplayName = "Deve lançar exceção se o usuário não for encontrado")]
        public async Task Handle_DeveLancarExcecao_UsuarioNaoEncontrado()
        {
            // Arrange
            var command = new GetUserIdCommand
            {
                Id = Guid.NewGuid()
            };

            _identityAbstractorMock.Setup(x => x.FindUserByIdAsync(command.Id.ToString()))
                                   .ReturnsAsync((Domain.Entities.User)null);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                     .WithMessage("Usuário não encontrado");
        }


        [Fact(DisplayName = "Deve lançar exceção se ocorrer erro ao buscar usuário")]
        public async Task Handle_DeveLancarExcecao_AoBuscarUsuario()
        {
            // Arrange
            var command = new GetUserIdCommand
            {
                Id = Guid.NewGuid()
            };

            _identityAbstractorMock.Setup(x => x.FindUserByIdAsync(command.Id.ToString()))
                                   .ThrowsAsync(new Exception("Erro ao buscar usuário"));

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                     .WithMessage("Erro ao buscar usuário");
        }
    }
}
