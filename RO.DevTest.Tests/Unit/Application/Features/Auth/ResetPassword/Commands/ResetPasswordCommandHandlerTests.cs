using System.Security.Claims;
using FluentAssertions;
using Moq;
using RO.DevTest.Application.Contracts.Application.Service;
using RO.DevTest.Application.Contracts.Infrastructure;
using RO.DevTest.Application.Features.Auth.Commands.ResetPassword;
using Microsoft.AspNetCore.Identity;
using Bogus;

namespace RO.DevTest.Tests.Unit.Application.Features.Auth.Commands;

public class ResetPasswordCommandHandlerTests
{
    private readonly Mock<IIdentityAbstractor> _identityAbstractorMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly ResetPasswordCommandHandler _handler;
    private readonly Faker _faker;

    public ResetPasswordCommandHandlerTests()
    {
        _identityAbstractorMock = new Mock<IIdentityAbstractor>();
        _tokenServiceMock = new Mock<ITokenService>();
        _handler = new ResetPasswordCommandHandler(_identityAbstractorMock.Object, _tokenServiceMock.Object);
        _faker = new Faker("pt_BR");
    }

    [Fact(DisplayName = "Deve resetar a senha com sucesso usando dados fake")]
    public async Task Handle_DeveResetarSenhaComSucesso_UsandoBogus()
    {
        // Arrange
        var email = _faker.Internet.Email();
        var password = _faker.Internet.Password(10, true, "[A-Z]", "@1");
        var token = _faker.Random.Hash();
        var identityToken = _faker.Random.AlphaNumeric(32);

        var user = new Domain.Entities.User { Email = email };
        var command = new ResetPasswordCommand
        {
            Token = token,
            Password = password,
            ConfirmPassword = password // <-- Adicionado para passar na validação
        };

        var claims = new[] { new Claim(ClaimTypes.Email, email) };
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        _tokenServiceMock
            .Setup(ts => ts.ValidatePasswordResetToken(token, out It.Ref<ClaimsPrincipal>.IsAny))
            .Returns((string _, out ClaimsPrincipal output) =>
            {
                output = claimsPrincipal;
                return true;
            });

        _identityAbstractorMock
            .Setup(ia => ia.FindUserByEmailAsync(email))
            .ReturnsAsync(user);

        _tokenServiceMock
            .Setup(ts => ts.GetIdentityResetToken(email))
            .ReturnsAsync(identityToken);

        _identityAbstractorMock
            .Setup(ia => ia.ResetPasswordAsync(user, identityToken, password))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(MediatR.Unit.Value);
    }
}
