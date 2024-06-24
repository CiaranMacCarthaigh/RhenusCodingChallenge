using MediatR;
using RhenusCodingChallenge.Application.Exceptions;
using RhenusCodingChallenge.Application.Player.Commands.CreateNewPlayer.Contracts;
using RhenusCodingChallenge.Application.Repositories;
using RhenusCodingChallenge.Application.Services;
using RhenusCodingChallenge.Domain.Player.Events;

namespace RhenusCodingChallenge.Application.Player.Commands.CreateNewPlayer
{
    public class CreateNewPlayerCommandHandler : IRequestHandler<CreateNewPlayerCommand, CreateNewPlayerCommandResultContract>
    {
        private readonly IGuidProviderService _guidProviderService;
        private readonly IDomainEventRepository _domainEventRepository;

        public CreateNewPlayerCommandHandler(IGuidProviderService guidProviderService, IDomainEventRepository domainEventRepository)
        {
            _guidProviderService = guidProviderService;
            _domainEventRepository = domainEventRepository;
        }

        public void Validate(CreateNewPlayerCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.PlayerName))
            {
                throw new InvalidCommandException(nameof(command.PlayerName), "The player name was empty or whitespace");
            }

            if (command.InitialBalance <= 0)
            {
                throw new InvalidCommandException(nameof(command.InitialBalance), "The initial balance must be greater than 0");
            }
        }

        public async Task<CreateNewPlayerCommandResultContract> Handle(CreateNewPlayerCommand command, CancellationToken cancellationToken)
        {
            Validate(command);

            var playerId = _guidProviderService.GetNew();

            var events = new[]
            {
                new PlayerAccountCreatedEvent(0, playerId, command.PlayerName.Trim(), command.InitialBalance)
            };

            await _domainEventRepository.AddEventsAsync(events);
            await _domainEventRepository.SaveAsync();

            return new CreateNewPlayerCommandResultContract(PlayerId: playerId);
        }
    }
}
