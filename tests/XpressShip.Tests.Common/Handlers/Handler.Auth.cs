using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using XpressShip.Application.Abstractions.Services.Notification;
using XpressShip.Application.Abstractions.Services.Token;
using XpressShip.Application.Behaviours;
using XpressShip.Application.DTOs.Token;
using XpressShip.Application.Features.Auth.Admin.Login;
using XpressShip.Application.Features.Auth.Admin.Register;
using XpressShip.Application.Features.Auth.Sender.Login;
using XpressShip.Domain.Abstractions;
using XpressShip.Domain.Entities.Users;

namespace XpressShip.Tests.Common.Handlers
{
    public static partial class Handler
    {
        public class Auth
        {
            public readonly Mock<UserManager<ApplicationUser>> mockUserManager;
            public readonly Mock<SignInManager<ApplicationUser>> mockSignInManager;
            public readonly Mock<ITokenService> mockTokenService;

            public Auth()
            {
                var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
                mockUserManager = new Mock<UserManager<ApplicationUser>>(
                    userStoreMock.Object, null, null, null, null, null, null, null, null);

                mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                    mockUserManager.Object,
                    Mock.Of<IHttpContextAccessor>(),
                    Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                    null, null, null, null);

                mockTokenService = new Mock<ITokenService>();
            }

            public class RegisterAdmin
            {
                private readonly RegisterAdminHandler _handler;
                private readonly ValidationPipelineBehaviour<RegisterAdminCommand, Result<Unit>> _validationBehaviour;
                private readonly ExceptionHandlingPipelineBehavior<RegisterAdminCommand, Result<Unit>> _exceptionHandlingBehaviour;
                private readonly RegisterAdminCommandValidator _validator;
                private readonly Mock<IAdminNotificationService> _mockAdminNotificationService;

                public RegisterAdmin(Auth auth)
                {
                    _mockAdminNotificationService = new Mock<IAdminNotificationService>();

                    _handler = new RegisterAdminHandler(
                    auth.mockUserManager.Object, _mockAdminNotificationService.Object);

                    _validator = new RegisterAdminCommandValidator(auth.mockUserManager.Object);

                    _validationBehaviour = new ValidationPipelineBehaviour<RegisterAdminCommand, Result<Unit>>(
                        [_validator]);
                    _exceptionHandlingBehaviour = new ExceptionHandlingPipelineBehavior<RegisterAdminCommand, Result<Unit>>();
                }

                public async Task<Result<Unit>> Handle(RegisterAdminCommand request)
                {

                    return await _exceptionHandlingBehaviour.Handle(request, () => _validationBehaviour.Handle(request, () => _handler.Handle(request, CancellationToken.None), CancellationToken.None), CancellationToken.None);
                }
            }

            public class LoginAdmin
            {
                private readonly LoginAdminHandler _handler;
                private readonly ExceptionHandlingPipelineBehavior<LoginAdminCommand, Result<TokenDTO>> _exceptionHandlingBehaviour;
                private readonly ValidationPipelineBehaviour<LoginAdminCommand, Result<TokenDTO>> _validationBehaviour;
                private readonly LoginAdminCommandValidator _validator;

                public LoginAdmin(Auth auth)
                {
                    _handler = new LoginAdminHandler(
                    auth.mockUserManager.Object,
                    auth.mockSignInManager.Object,
                    auth.mockTokenService.Object);

                    _validator = new LoginAdminCommandValidator();

                    _validationBehaviour = new ValidationPipelineBehaviour<LoginAdminCommand, Result<TokenDTO>>(
                        [_validator]);
                    _exceptionHandlingBehaviour = new ExceptionHandlingPipelineBehavior<LoginAdminCommand, Result<TokenDTO>>();
                }

                public async Task<Result<TokenDTO>> Handle(LoginAdminCommand request)
                {
                    return await _exceptionHandlingBehaviour.Handle(request, () => _validationBehaviour.Handle(request, () => _handler.Handle(request, CancellationToken.None), CancellationToken.None), CancellationToken.None);
                }
            }

            public class LoginSender
            {
                private readonly LoginSenderHandler _handler;
                private readonly ExceptionHandlingPipelineBehavior<LoginSenderCommand, Result<TokenDTO>> _exceptionHandlingBehaviour;
                private readonly ValidationPipelineBehaviour<LoginSenderCommand, Result<TokenDTO>> _validationBehaviour;
                private readonly LoginSenderCommandValidator _validator;

                public LoginSender(Auth auth)
                {
                    _handler = new LoginSenderHandler(
                    auth.mockUserManager.Object,
                    auth.mockSignInManager.Object,
                    auth.mockTokenService.Object);

                    _validator = new LoginSenderCommandValidator();

                    _validationBehaviour = new ValidationPipelineBehaviour<LoginSenderCommand, Result<TokenDTO>>(
                        [_validator]);

                    _exceptionHandlingBehaviour = new ExceptionHandlingPipelineBehavior<LoginSenderCommand, Result<TokenDTO>>();
                }

                public async Task<Result<TokenDTO>> Handle(LoginSenderCommand request)
                {
                    return await _exceptionHandlingBehaviour.Handle(request, () => _validationBehaviour.Handle(request, () => _handler.Handle(request, CancellationToken.None), CancellationToken.None), CancellationToken.None);
                }
            }

        }
    }
}
