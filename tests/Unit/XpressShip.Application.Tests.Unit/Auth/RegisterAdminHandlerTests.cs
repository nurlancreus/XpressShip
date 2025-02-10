using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities.Users;
using XpressShip.Tests.Common.Factories;
using XpressShip.Tests.Common.Handlers;
using Moq.EntityFrameworkCore;

namespace XpressShip.Application.Tests.Unit.Auth
{
    public class RegisterAdminHandlerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Handler.Auth.RegisterAdmin _registerAdminHandler;

        public RegisterAdminHandlerTests()
        {
            var auth = new Handler.Auth();

            _mockUserManager = auth.mockUserManager;
            _registerAdminHandler = new Handler.Auth.RegisterAdmin(auth);
        }

        [Fact]
        public async Task Handle_WhenValidCredentials_ReturnsIsSuccessTrue()
        {
            // Arrange
            var request = Factory.Admin.GenerateValidRegisterRequest();

            var admin = Admin.Create(request.FirstName, request.LastName, request.UserName, request.Email, request.Phone);

            var existingUsers = new List<ApplicationUser>
            {
                Admin.Create(request.FirstName, request.LastName, "John", request.Email, request.Phone),
                Admin.Create(request.FirstName, request.LastName, request.UserName, "John@example.com", request.Phone),
               Admin.Create(request.FirstName, request.LastName, request.UserName, request.Email, "999999999")
            }.AsQueryable();

            _mockUserManager.Setup(x => x.Users).Returns(existingUsers);

            _mockUserManager.Setup(x => x.FindByNameAsync(request.UserName))
                .ReturnsAsync((ApplicationUser)null!);

            _mockUserManager.Setup(x => x.FindByEmailAsync(request.Email))
                .ReturnsAsync((ApplicationUser)null!);

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
                .ReturnsAsync(IdentityResult.Success);
            // Act
            var result = await _registerAdminHandler.Handle(request);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_WhenInValidCredentials_ReturnsFailure()
        {
            // Arrange
            var request = Factory.Admin.GenerateInValidRegisterRequest();

            // Act
            var result = await _registerAdminHandler.Handle(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.Validation);
        }


        [Fact]
        public async Task Handle_WhenUserCreationFails_ReturnsFailure()
        {
            // Arrange
            var request = Factory.Admin.GenerateValidRegisterRequest();

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), request.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed" }));

            // Act
            var result = await _registerAdminHandler.Handle(request);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error.Type.Should().Be(ErrorType.Register);
            result.Error.Message.Should().Contain("Wrong credentials");
        }
    }
}
