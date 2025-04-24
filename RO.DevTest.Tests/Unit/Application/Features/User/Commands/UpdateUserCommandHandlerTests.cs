using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using RO.DevTest.Application.Contracts.Application.Service;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Features.User.Commands.UpdateUserCommand;
using RO.DevTest.Domain.Exception;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace RO.DevTest.Tests.Unit.Application.Features.User.Commands
{
    public class UpdateUserCommandHandlerTests
    {
        private readonly Mock<IIdentityAbstractor> _identityAbstractorMock;
        private readonly Mock<ILogged> _loggedMock;
        private readonly UpdateUserCommandHandler _handler;
        private readonly Faker _faker;

        public UpdateUserCommandHandlerTests()
        {
            _identityAbstractorMock = new Mock<IIdentityAbstractor>();
            _loggedMock = new Mock<ILogged>();
            _handler = new UpdateUserCommandHandler(_identityAbstractorMock.Object, _loggedMock.Object);
            _faker = new Faker();  // Inicializando o Faker
        }

        [Fact(DisplayName = "Deve atualizar o usuário com sucesso")]
        public async Task Handle_DeveAtualizarUsuario_ComSucesso()
        {
            // Arrange
            var existingUser = new Domain.Entities.User
            {
                Id = _faker.Random.Guid().ToString(),
                UserName = "originalUser", // Nome de usuário original
                Name = "Original Name",    // Nome original
                Email = "original@email.com"
            };

            var command = new UpdateUserCommand
            {
                UserName = "updatedUser",  // Novo nome de usuário
                Name = "Updated Name",     // Nome atualizado
                Email = "updated@email.com"
            };

            // Mocking
            _loggedMock.Setup(x => x.UserLogged())
                       .ReturnsAsync(existingUser);

            _identityAbstractorMock.Setup(x => x.FindByNameAsync(command.UserName))
                                    .ReturnsAsync((Domain.Entities.User)null); // Nenhum usuário com esse UserName

            _identityAbstractorMock.Setup(x => x.FindUserByEmailAsync(command.Email))
                                    .ReturnsAsync((Domain.Entities.User)null); // Nenhum usuário com esse email

            _identityAbstractorMock.Setup(x => x.UpdateUserAsync(It.IsAny<Domain.Entities.User>()))
                                    .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Updated Name");  // O nome deve ser atualizado
            result.UserName.Should().Be("updatedUser");  // O UserName deve ser atualizado
            result.Email.Should().Be("updated@email.com");  // O email deve ser atualizado

            _identityAbstractorMock.Verify(x => x.UpdateUserAsync(It.Is<Domain.Entities.User>(u =>
                u.UserName == "updatedUser" &&
                u.Email == "updated@email.com" &&
                u.Name == "Updated Name"  // Verifica se os dados foram atualizados corretamente
            )), Times.Once);
        }


        [Fact(DisplayName = "Deve lançar exceção se o e-mail já estiver em uso")]
        public async Task Handle_EmailEmUso_DeveLancarExcecao()
        {
            // Arrange
            var loggedUser = new Domain.Entities.User
            {
                Id = _faker.Random.Guid().ToString(),
                UserName = _faker.Internet.UserName(),
                Email = _faker.Internet.Email()
            };

            var command = new UpdateUserCommand
            {
                UserName = _faker.Internet.UserName(),
                Name = _faker.Name.FullName(),
                Email = _faker.Internet.Email()
            };

            var userComMesmoEmail = new Domain.Entities.User
            {
                Id = _faker.Random.Guid().ToString(),
                Email = command.Email
            };

            _loggedMock.Setup(x => x.UserLogged())
                       .ReturnsAsync(loggedUser);

            _identityAbstractorMock.Setup(x => x.FindByNameAsync(command.UserName))
                                   .ReturnsAsync((Domain.Entities.User)null);

            _identityAbstractorMock.Setup(x => x.FindUserByEmailAsync(command.Email))
                                   .ReturnsAsync(userComMesmoEmail);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                     .WithMessage("E-mail já está em uso por outra conta");
        }

        [Fact(DisplayName = "Deve lançar exceção se o nome de usuário já estiver em uso")]
        public async Task Handle_UsernameEmUso_DeveLancarExcecao()
        {
            // Arrange
            var loggedUser = new Domain.Entities.User
            {
                Id = _faker.Random.Guid().ToString(),
                UserName = _faker.Internet.UserName(),
                Email = _faker.Internet.Email()
            };

            var command = new UpdateUserCommand
            {
                UserName = _faker.Internet.UserName(),
                Name = _faker.Name.FullName(),
                Email = _faker.Internet.Email()
            };

            var userComMesmoNome = new Domain.Entities.User
            {
                Id = _faker.Random.Guid().ToString(),
                UserName = command.UserName
            };

            _loggedMock.Setup(x => x.UserLogged())
                       .ReturnsAsync(loggedUser);

            _identityAbstractorMock.Setup(x => x.FindByNameAsync(command.UserName))
                                   .ReturnsAsync(userComMesmoNome);

            // Act
            var act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<BadRequestException>()
                     .WithMessage("Nome de usuário já está em uso por outra conta");
        }
    }
}
