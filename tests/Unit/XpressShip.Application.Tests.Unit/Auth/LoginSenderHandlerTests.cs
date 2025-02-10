using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using XpressShip.Application.Abstractions.Services.Token;
using XpressShip.Application.DTOs.Token;
using XpressShip.Application.Features.Auth.Sender.Login;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities.Users;
using XpressShip.Tests.Common.Handlers;
using XpressShip.Tests.Common.Factories;
using DataConstants = XpressShip.Tests.Common.Constants.Constants;

namespace XpressShip.Application.Tests.Unit.Auth
{
    public class LoginSenderHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Handler.Auth.LoginSender _loginSenderHandler;
        public LoginSenderHandlerTests()
        {
            var auth = new Handler.Auth();
            _mockUserManager = auth.mockUserManager;
            _mockSignInManager = auth.mockSignInManager;
            _mockTokenService = auth.mockTokenService;

            _loginSenderHandler = new Handler.Auth.LoginSender(auth);
        }

        [Fact]
        public async Task Handle_WhenTokenGenerationFails_ReturnsFailure()
        {
            // Arrange
            var request = new LoginSenderCommand { UserName = DataConstants.Sender.UserName, Password = "password" };
            var sender = Factory.Sender.GenerateSender();

            _mockUserManager.Setup(x => x.FindByNameAsync(request.UserName))
                .ReturnsAsync(sender);

            _mockSignInManager.Setup(x => x.PasswordSignInAsync(sender, request.Password, false, false))
                .ReturnsAsync(SignInResult.Success);

            _mockTokenService.Setup(x => x.GetTokenAsync(sender))
                .ReturnsAsync(Result<TokenDTO>.Failure(Error.TokenError()));

            // Act
            var result = await _loginSenderHandler.Handle(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.Token);
        }

        [Fact]
        public async Task Handle_WhenValidCredentials_ReturnsToken()
        {
            // Arrange
            var request = new LoginSenderCommand { UserName = DataConstants.Sender.UserName, Password = "password" };
            var sender = Factory.Sender.GenerateSender();

            var token = new TokenDTO { AccessToken = "access-token", RefreshToken = "refresh-token", ExpiresAt = DateTime.UtcNow.AddHours(1) };

            _mockUserManager.Setup(x => x.FindByNameAsync(request.UserName))
                .ReturnsAsync(sender);

            _mockSignInManager.Setup(x => x.PasswordSignInAsync(sender, request.Password, false, false))
                .ReturnsAsync(SignInResult.Success);

            _mockTokenService.Setup(x => x.GetTokenAsync(sender))
                .ReturnsAsync(Result<TokenDTO>.Success(token));

            _mockTokenService.Setup(x => x.UpdateRefreshTokenAsync(token.RefreshToken, sender, token.ExpiresAt))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _loginSenderHandler.Handle(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(token);
        }

        [Fact]
        public async Task Handle_WhenWrongCredentials_ReturnsFailure()
        {
            // Arrange
            var request = new LoginSenderCommand { UserName = "wrong-username", Password = "wrong-password" };
            var sender = Factory.Sender.GenerateSender();

            _mockUserManager.Setup(x => x.FindByNameAsync(request.UserName))
                .ReturnsAsync((ApplicationUser)null);

            _mockSignInManager.Setup(x => x.PasswordSignInAsync(sender, request.Password, false, false))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _loginSenderHandler.Handle(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.Login);
            result.Error.Message.Should().Contain("Wrong credentials");
        }

        [Fact]
        public async Task Handle_WhenInvalidUsername_ReturnsFailure()
        {
            // Arrange
            var request = new LoginSenderCommand { UserName = "", Password = "password" };

            // Act
            var result = await _loginSenderHandler.Handle(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public async Task Handle_WhenInvalidPassword_ReturnsFailure()
        {
            // Arrange
            var request = new LoginSenderCommand { UserName = DataConstants.Sender.UserName, Password = "" };

            // Act
            var result = await _loginSenderHandler.Handle(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public async Task Handle_WhenInvalidUsernameAndPassword_ReturnsFailure()
        {
            // Arrange
            var request = new LoginSenderCommand { UserName = "", Password = "" };

            // Act
            var result = await _loginSenderHandler.Handle(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.Validation);
        }
    }
}
