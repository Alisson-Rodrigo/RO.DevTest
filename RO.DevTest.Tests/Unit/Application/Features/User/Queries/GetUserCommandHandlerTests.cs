using Bogus;
using Moq;
using RO.DevTest.Application.Features.User.Queries.GetUserCommand;
using RO.DevTest.Domain.Exception;
using Xunit;
using FluentAssertions;
using RO.DevTest.Application.Contracts.Application.Service;

namespace RO.DevTest.Tests.Unit.Application.Features.User.Queries
{
    public class GetUserCommandHandlerTests
    {
        private readonly Mock<ILogged> _loggedMock;
        private readonly GetUserCommandHandler _handler;
        private readonly Faker _faker;

        public GetUserCommandHandlerTests()
        {
            _loggedMock = new Mock<ILogged>();
            _handler = new GetUserCommandHandler(_loggedMock.Object);
            _faker = new Faker();  // Inicializando o Faker
        }

        [Fact(DisplayName = "Deve obter dados do usuário com sucesso")]
        public async Task Handle_DeveRetornarDadosUsuario_ComSucesso()
        {
            // Arrange
            var expectedUser = new Domain.Entities.User
            {
                Id = _faker.Random.Guid().ToString(),
                UserName = _faker.Internet.UserName(),
                Name = _faker.Name.FullName(),
                Email = _faker.Internet.Email()
            };

            // Setup para mock do ILogged
            _loggedMock.Setup(x => x.UserLogged())
                       .ReturnsAsync(expectedUser);

            var command = new GetUserCommand();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeNull();
            result.Id.Should().Be(expectedUser.Id);
            result.UserName.Should().Be(expectedUser.UserName);
            result.Name.Should().Be(expectedUser.Name);
            result.Email.Should().Be(expectedUser.Email);
        }

        [Fact(DisplayName = "Deve lançar exceção se houver falha ao obter dados do usuário")]
        public async Task Handle_DeveLancarExcecao_SeFalharAoObterUsuario()
        {
            // Arrange
            _loggedMock.Setup(x => x.UserLogged())
                       .ThrowsAsync(new Exception("Erro ao obter usuário"));

            var command = new GetUserCommand();

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                     .WithMessage("Falha ao obter dados do usuário");
        }
    }
}
