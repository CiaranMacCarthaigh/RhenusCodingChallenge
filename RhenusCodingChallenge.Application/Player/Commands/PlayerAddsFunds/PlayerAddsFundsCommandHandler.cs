using MediatR;
using RhenusCodingChallenge.Application.Exceptions;
using RhenusCodingChallenge.Application.Repositories;
using RhenusCodingChallenge.Domain.Player.Events;
using RhenusCodingChallenge.Domain.Player;
using RhenusCodingChallenge.Application.Player.Queries.GetPlayerAccount.Contracts;
using RhenusCodingChallenge.Application.Player.Queries.GetPlayerAccount;

namespace RhenusCodingChallenge.Application.Player.Commands.PlayerWithdrawsFunds
{
    public class PlayerAddsFundsCommandHandler : IRequestHandler<PlayerAddsFundsCommand, PlayerAccountContract>
    {
        private readonly IMediator _mediator;
        private readonly IDomainEventRepository _domainEventRepository;

        public PlayerAddsFundsCommandHandler(IMediator mediator, IDomainEventRepository domainEventRepository)
        {
            _mediator = mediator;
            _domainEventRepository = domainEventRepository;
        }

        public void Validate(PlayerAddsFundsCommand command, PlayerAccount player, IReadOnlyCollection<PlayerEvent> playerEvents)
        {
            if (playerEvents.Count == 0)
            {
                throw new AggregateNotFoundException(command.PlayerId);
            }

            if (command.Amount <= 0)
            {
                throw new InvalidCommandException(nameof(command.Amount), "The amount to add must be greater than zero");
            }
        }

        public async Task<PlayerAccountContract> Handle(PlayerAddsFundsCommand command, CancellationToken cancellationToken)
        {
            var playerEvents = await _domainEventRepository.GetDomainEventsAsync<PlayerEvent>(command.PlayerId);
            var player = PlayerAccount.CreateFromEvents(playerEvents);

            Validate(command, player, playerEvents);

            var nextVersionNumber = playerEvents.Last().AggregateVersion + 1;
            var events = new[]
            {
                new PlayerAddsNewFundsEvent(nextVersionNumber, command.PlayerId, command.Amount)
            };

            await _domainEventRepository.AddEventsAsync(events);
            await _domainEventRepository.SaveAsync();

            var getPlayerAccountContractQuery = new GetPlayerAccountQuery(PlayerId: player.Id);

            return await _mediator.Send(getPlayerAccountContractQuery);
        }
    }
}
