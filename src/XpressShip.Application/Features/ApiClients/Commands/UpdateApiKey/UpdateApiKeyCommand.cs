using XpressShip.Application.Abstractions;

namespace XpressShip.Application.Features.ApiClients.Commands.UpdateApiKey
{
    public record UpdateApiKeyCommand : ICommand<string>
    {
        public Guid Id { get; set; }
    }
}
