using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using XpressShip.Application.Abstractions.Services.Token;
using XpressShip.Application.DTOs.Token;
using XpressShip.Application.Features.Auth.Admin.Login;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities.Users;
using XpressShip.Tests.Common.Factories;
using XpressShip.Tests.Common.Handlers;
using DataConstants = XpressShip.Tests.Common.Constants.Constants;

namespace XpressShip.Application.Tests.Unit.Auth
{
    public class LoginAdminHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Handler.Auth.LoginAdmin _loginAdminHandler;
        public LoginAdminHandlerTests()
        {
            var auth = new Handler.Auth();
            _mockUserManager = auth.mockUserManager;
            _mockSignInManager = auth.mockSignInManager;
            _mockTokenService = auth.mockTokenService;

            _loginAdminHandler = new Handler.Auth.LoginAdmin(auth);
        }

        [Fact]
        public async Task Handle_WhenTokenGenerationFails_ReturnsFailure()
        {
            // Arrange
            var request = new LoginAdminCommand { UserName = DataConstants.Admin.UserName, Password = "password" };
            var adminUser = Factory.Admin.GenerateAdmin();

            _mockUserManager.Setup(x => x.FindByNameAsync(request.UserName))
                .ReturnsAsync(adminUser);

            _mockSignInManager.Setup(x => x.PasswordSignInAsync(adminUser, request.Password, false, false))
                .ReturnsAsync(SignInResult.Success);

            _mockTokenService.Setup(x => x.GetTokenAsync(adminUser))
                .ReturnsAsync(Result<TokenDTO>.Failure(Error.TokenError()));

            // Act
            var result = await _loginAdminHandler.Handle(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.Token);
        }

        [Fact]
        public async Task Handle_WhenValidCredentials_ReturnsToken()
        {
            // Arrange
            var request = new LoginAdminCommand { UserName = DataConstants.Admin.UserName, Password = "password" };
            var adminUser = Factory.Admin.GenerateAdmin();

            var token = new TokenDTO { AccessToken = "access-token", RefreshToken = "refresh-token", ExpiresAt = DateTime.UtcNow.AddHours(1) };

            _mockUserManager.Setup(x => x.FindByNameAsync(request.UserName))
                .ReturnsAsync(adminUser);

            _mockSignInManager.Setup(x => x.PasswordSignInAsync(adminUser, request.Password, false, false))
                .ReturnsAsync(SignInResult.Success);

            _mockTokenService.Setup(x => x.GetTokenAsync(adminUser))
                .ReturnsAsync(Result<TokenDTO>.Success(token));

            _mockTokenService.Setup(x => x.UpdateRefreshTokenAsync(token.RefreshToken, adminUser, token.ExpiresAt))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _loginAdminHandler.Handle(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeEquivalentTo(token);
        }

        [Fact]
        public async Task Handle_WhenWrongCredentials_ReturnsFailure()
        {
            // Arrange
            var request = new LoginAdminCommand { UserName = "wrong-username", Password = "wrong-password" };
            var adminUser = Factory.Admin.GenerateAdmin();

            _mockUserManager.Setup(x => x.FindByNameAsync(request.UserName))
                .ReturnsAsync((ApplicationUser)null);

            _mockSignInManager.Setup(x => x.PasswordSignInAsync(adminUser, request.Password, false, false))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _loginAdminHandler.Handle(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.Login);
            result.Error.Message.Should().Contain("Wrong credentials");
        }

        [Fact]
        public async Task Handle_WhenInvalidUsername_ReturnsFailure()
        {
            // Arrange
            var request = new LoginAdminCommand { UserName = "", Password = "password" };

            // Act
            var result = await _loginAdminHandler.Handle(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public async Task Handle_WhenInvalidPassword_ReturnsFailure()
        {
            // Arrange
            var request = new LoginAdminCommand { UserName = DataConstants.Admin.UserName, Password = "" };

            // Act
            var result = await _loginAdminHandler.Handle(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.Validation);
        }

        [Fact]
        public async Task Handle_WhenInvalidUsernameAndPassword_ReturnsFailure()
        {
            // Arrange
            var request = new LoginAdminCommand { UserName = "", Password = "" };

            // Act
            var result = await _loginAdminHandler.Handle(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.Validation);
        }
    }
}
