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
                private readonly RegisterAdminHandler handler;
                private readonly ValidationPipelineBehaviour<RegisterAdminCommand, Result<Unit>> validationBehaviour;
                private readonly RegisterAdminCommandValidator validator;
                private readonly Mock<IAdminNotificationService> mockAdminNotificationService;

                public RegisterAdmin(Auth auth)
                {
                    mockAdminNotificationService = new Mock<IAdminNotificationService>();

                    handler = new RegisterAdminHandler(
                    auth.mockUserManager.Object, mockAdminNotificationService.Object);

                    validator = new RegisterAdminCommandValidator(auth.mockUserManager.Object);

                    validationBehaviour = new ValidationPipelineBehaviour<RegisterAdminCommand, Result<Unit>>(
                        [validator]);
                }

                public async Task<Result<Unit>> Handle(RegisterAdminCommand request)
                {
                    return await validationBehaviour.Handle(request, () => handler.Handle(request, CancellationToken.None), CancellationToken.None);
                }
            }

            public class LoginAdmin
            {
                public readonly LoginAdminHandler handler;
                public readonly ValidationPipelineBehaviour<LoginAdminCommand, Result<TokenDTO>> validationBehaviour;
                public readonly LoginAdminCommandValidator validator;

                public LoginAdmin(Auth auth)
                {
                    handler = new LoginAdminHandler(
                    auth.mockUserManager.Object,
                    auth.mockSignInManager.Object,
                    auth.mockTokenService.Object);

                    validator = new LoginAdminCommandValidator();

                    validationBehaviour = new ValidationPipelineBehaviour<LoginAdminCommand, Result<TokenDTO>>(
                        [validator]);
                }

                public async Task<Result<TokenDTO>> Handle(LoginAdminCommand request)
                {
                    return await validationBehaviour.Handle(request, () => handler.Handle(request, CancellationToken.None), CancellationToken.None);
                }
            }

            public class LoginSender
            {
                public readonly LoginSenderHandler handler;
                public readonly ValidationPipelineBehaviour<LoginSenderCommand, Result<TokenDTO>> validationBehaviour;
                public readonly LoginSenderCommandValidator validator;

                public LoginSender(Auth auth)
                {
                    handler = new LoginSenderHandler(
                    auth.mockUserManager.Object,
                    auth.mockSignInManager.Object,
                    auth.mockTokenService.Object);

                    validator = new LoginSenderCommandValidator();

                    validationBehaviour = new ValidationPipelineBehaviour<LoginSenderCommand, Result<TokenDTO>>(
                        [validator]);
                }

                public async Task<Result<TokenDTO>> Handle(LoginSenderCommand request)
                {
                    return await validationBehaviour.Handle(request, () => handler.Handle(request, CancellationToken.None), CancellationToken.None);
                }
            }

        }
    }
}
