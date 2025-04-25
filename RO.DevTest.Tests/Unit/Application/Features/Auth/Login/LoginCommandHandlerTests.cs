using Microsoft.AspNetCore.Identity;
using Moq;
using RO.DevTest.Application.Contracts.Application.Service;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Features.Auth.Commands.LoginCommand;
using RO.DevTest.Domain.Exception;


namespace RO.DevTest.Tests.Unit.Application.Features.Auth.Login
{
    public class LoginCommandHandlerTests
    {
        private readonly Mock<IIdentityAbstractor> _mockIdentityAbstractor;
        private readonly Mock<ITokenService> _mockTokenService;

        public LoginCommandHandlerTests()
        {
            _mockIdentityAbstractor = new Mock<IIdentityAbstractor>();
            _mockTokenService = new Mock<ITokenService>();
        }

        [Fact]
        public async Task Handle_UserNotFound_ThrowsBadRequestException()
        {
            // Arrange
            var request = new LoginCommand { Username = "user", Password = "password" };
            _mockIdentityAbstractor.Setup(x => x.FindByNameAsync(request.Username))
                .ReturnsAsync((Domain.Entities.User)null);

            var handler = new LoginCommandHandler(_mockIdentityAbstractor.Object, _mockTokenService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_IncorrectPassword_ThrowsBadRequestException()
        {
            // Arrange
            var request = new LoginCommand { Username = "user", Password = "wrongpassword" };
            var user = new Domain.Entities.User { Id = Guid.NewGuid().ToString(), UserName = "user" };

            _mockIdentityAbstractor.Setup(x => x.FindByNameAsync(request.Username)).ReturnsAsync(user);
            _mockIdentityAbstractor.Setup(x => x.CheckPasswordSignInAsync(user, request.Password))
                .ReturnsAsync(SignInResult.Failed);

            var handler = new LoginCommandHandler(_mockIdentityAbstractor.Object, _mockTokenService.Object);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_SuccessfulLogin_ReturnsLoginResponse()
        {
            // Arrange
            var request = new LoginCommand { Username = "user", Password = "password" };
            var user = new Domain.Entities.User { Id = Guid.NewGuid().ToString(), UserName = "user" };
            var roles = new[] { "Admin", "User" };

            _mockIdentityAbstractor.Setup(x => x.FindByNameAsync(request.Username)).ReturnsAsync(user);
            _mockIdentityAbstractor.Setup(x => x.CheckPasswordSignInAsync(user, request.Password)).ReturnsAsync(SignInResult.Success);
            _mockIdentityAbstractor.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);

            _mockTokenService.Setup(x => x.GenerateJwtToken(user, roles)).Returns("jwt_token");

            var handler = new LoginCommandHandler(_mockIdentityAbstractor.Object, _mockTokenService.Object);

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Equal("jwt_token", response.AccessToken);
            Assert.Equal(roles, response.Roles);
        }

        [Fact]
        public async Task Handle_UserFound_GeneratesJwtToken()
        {
            // Arrange
            var request = new LoginCommand { Username = "user", Password = "password" };
            var user = new Domain.Entities.User { Id = Guid.NewGuid().ToString(), UserName = "user" };
            var roles = new[] { "Admin" };

            _mockIdentityAbstractor.Setup(x => x.FindByNameAsync(request.Username)).ReturnsAsync(user);
            _mockIdentityAbstractor.Setup(x => x.CheckPasswordSignInAsync(user, request.Password)).ReturnsAsync(SignInResult.Success);
            _mockIdentityAbstractor.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);

            _mockTokenService.Setup(x => x.GenerateJwtToken(user, roles)).Returns("generated_jwt_token");

            var handler = new LoginCommandHandler(_mockIdentityAbstractor.Object, _mockTokenService.Object);

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.Equal("generated_jwt_token", response.AccessToken);
            Assert.Equal(roles, response.Roles);
        }
    }
}
