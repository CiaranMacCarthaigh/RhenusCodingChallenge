using MediatR;
using RhenusCodingChallenge.Application.Exceptions;
using RhenusCodingChallenge.Application.Repositories;
using RhenusCodingChallenge.Domain.Player.Events;
using RhenusCodingChallenge.Domain.Player;
using RhenusCodingChallenge.Application.Player.Queries.GetPlayerAccount.Contracts;
using RhenusCodingChallenge.Application.Player.Queries.GetPlayerAccount;

namespace RhenusCodingChallenge.Application.Player.Commands.PlayerWithdrawsFunds
{
    public class PlayerWithdrawsFundsCommandHandler : IRequestHandler<PlayerWithdrawsFundsCommand, PlayerAccountContract>
    {
        private readonly IMediator _mediator;
        private readonly IDomainEventRepository _domainEventRepository;

        public PlayerWithdrawsFundsCommandHandler(IMediator mediator, IDomainEventRepository domainEventRepository)
        {
            _mediator = mediator;
            _domainEventRepository = domainEventRepository;
        }

        public void Validate(PlayerWithdrawsFundsCommand command, PlayerAccount player, IReadOnlyCollection<PlayerEvent> playerEvents)
        {
            if (playerEvents.Count == 0)
            {
                throw new AggregateNotFoundException(command.PlayerId);
            }

            if (command.Amount <= 0)
            {
                throw new InvalidCommandException(nameof(command.Amount), "The amount to withdraw must be greater than zero");
            }
            if (player.Balance < command.Amount)
            {
                throw new InvalidCommandException(nameof(player.Balance), "The player balance is too low to withdraw the amount requested");
            }
        }

        public async Task<PlayerAccountContract> Handle(PlayerWithdrawsFundsCommand command, CancellationToken cancellationToken)
        {
            var playerEvents = await _domainEventRepository.GetDomainEventsAsync<PlayerEvent>(command.PlayerId);
            var player = PlayerAccount.CreateFromEvents(playerEvents);

            Validate(command, player, playerEvents);

            var nextVersionNumber = playerEvents.Last().AggregateVersion + 1;
            var events = new[]
            {
                new PlayerWithdrawsFundsEvent(nextVersionNumber, command.PlayerId, command.Amount)
            };

            await _domainEventRepository.AddEventsAsync(events);
            await _domainEventRepository.SaveAsync();

            var getPlayerAccountContractQuery = new GetPlayerAccountQuery(PlayerId: player.Id);

            return await _mediator.Send(getPlayerAccountContractQuery);
        }
    }
}
