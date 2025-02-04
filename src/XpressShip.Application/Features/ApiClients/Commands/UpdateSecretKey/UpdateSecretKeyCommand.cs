using XpressShip.Application.Abstractions;

namespace XpressShip.Application.Features.ApiClients.Commands.UpdateSecretKey
{
    public record UpdateSecretKeyCommand : ICommand<string>
    {
        public Guid Id { get; set; }
    }
}
