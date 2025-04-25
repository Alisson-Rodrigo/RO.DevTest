using Bogus;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using RO.DevTest.Application.Contracts.Application.Service;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Features.User.Commands.DeleteUserCommand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RO.DevTest.Tests.Unit.Application.Features.User.Commands
{
    public class DeleteUserCommandHandlerTests
    {
        private readonly Mock<ILogged> _loggedMock;
        private readonly Mock<IIdentityAbstractor> _identityAbstractorMock;
        private readonly DeleteUserCommandHandler _handler;
        private readonly Faker _faker;

        public DeleteUserCommandHandlerTests()
        {
            _loggedMock = new Mock<ILogged>();
            _identityAbstractorMock = new Mock<IIdentityAbstractor>();
            _handler = new DeleteUserCommandHandler(_loggedMock.Object, _identityAbstractorMock.Object);
            _faker = new Faker("pt_BR");
        }

        [Fact(DisplayName = "Deve deletar o usuário com sucesso")]
        public async Task Handle_DeveDeletarUsuario_ComSucesso()
        {
            // Arrange
            var user = new Domain.Entities.User
            {
                Id = Guid.NewGuid().ToString(),
                Email = _faker.Internet.Email(),
                UserName = _faker.Internet.UserName()
            };

            _loggedMock.Setup(l => l.UserLogged())
                       .ReturnsAsync(user);

            _identityAbstractorMock.Setup(i => i.DeleteUser(user))
                .ReturnsAsync(IdentityResult.Success);


            var command = new DeleteUserCommand();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(MediatR.Unit.Value);
            _identityAbstractorMock.Verify(i => i.DeleteUser(user), Times.Once);
        }

        [Fact(DisplayName = "Deve lançar exceção se o usuário não estiver logado")]
        public async Task Handle_DeveLancarExcecao_SeUsuarioNaoLogado()
        {
            // Arrange
            _loggedMock.Setup(l => l.UserLogged())
                       .ReturnsAsync((Domain.Entities.User)null);

            var command = new DeleteUserCommand();

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should()
                     .ThrowAsync<UnauthorizedAccessException>()
                     .WithMessage("Usuário não encontrado");

            _identityAbstractorMock.Verify(i => i.DeleteUser(It.IsAny<Domain.Entities.User>()), Times.Never);
        }
    }
}
