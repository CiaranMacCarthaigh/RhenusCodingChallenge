using MediatR;
using RhenusCodingChallenge.Application.Exceptions;
using RhenusCodingChallenge.Application.Repositories;
using RhenusCodingChallenge.Domain.Player.Events;
using RhenusCodingChallenge.Application.Services.Game;
using RhenusCodingChallenge.Application.Player.Commands.PlayerPlacesBet.Contracts;
using RhenusCodingChallenge.Domain.Player;

namespace RhenusCodingChallenge.Application.Player.Commands.PlayerPlacesBet
{
    public class PlayerPlacesBetCommandHandler : IRequestHandler<PlayerPlacesBetCommand, PlayerBetResultContract>
    {
        private readonly IGameService _gameService;
        private readonly IDomainEventRepository _domainEventRepository;

        public PlayerPlacesBetCommandHandler(IGameService gameService, IDomainEventRepository domainEventRepository)
        {
            _gameService = gameService;
            _domainEventRepository = domainEventRepository;
        }

        public void Validate(PlayerPlacesBetCommand command, PlayerAccount player, IReadOnlyCollection<PlayerEvent> playerEvents)
        {
            if (playerEvents.Count == 0)
            {
                throw new AggregateNotFoundException(command.PlayerId);
            }

            if (command.Stake <= 0)
            {
                throw new InvalidCommandException(nameof(command.Stake), "The bet stake must be greater than zero");
            }
            if (player.Balance < command.Stake)
            {
                throw new InvalidCommandException(nameof(command.Stake), "The player balance is too low to cover the stake");
            }
        }

        private static BetOutcome MapFromGameBetOutcome(GameBetOutcome gameBetOutcome)
        {
            switch (gameBetOutcome)
            {
                case GameBetOutcome.Win:
                    return BetOutcome.Win;

                case GameBetOutcome.Lose:
                    return BetOutcome.Lose;

                default:
                    throw new InvalidOperationException($"The value {gameBetOutcome} could not be mapped to a value of {nameof(BetOutcome)}");
            }
        }


        public async Task<PlayerBetResultContract> Handle(PlayerPlacesBetCommand command, CancellationToken cancellationToken)
        {
            var playerEvents = await _domainEventRepository.GetDomainEventsAsync<PlayerEvent>(command.PlayerId);
            var player = PlayerAccount.CreateFromEvents(playerEvents);

            Validate(command, player, playerEvents);

            var nextVersionNumber = playerEvents.Last().AggregateVersion + 1;

            var gameBet = new GameBet(PlayerId: command.PlayerId, NumberBet: command.Number, Stake: command.Stake);
            var result = _gameService.RunGame(gameBet);

            PlayerEvent resultEvent =
                result.Outcome == GameBetOutcome.Win
                ? new PlayerWinsEvent(aggregateVersion: nextVersionNumber, playerId: command.PlayerId, stake: command.Stake, winningNumber: result.WinningNumber, amountWon: result.Difference)
                : new PlayerLosesEvent(aggregateVersion: nextVersionNumber, playerId: command.PlayerId, stake: command.Stake, bettingNumber: command.Number, winningNumber: result.WinningNumber);

            var events = new[]
            {
                resultEvent
            };

            var resultContract = new PlayerBetResultContract(
                PlayerId: command.PlayerId,
                Stake: command.Stake,
                Outcome: MapFromGameBetOutcome(result.Outcome).ToString(),
                WinningNumber: result.WinningNumber,
                Difference: result.Difference,
                NewBalance: player.Balance + result.Difference
            );

            await _domainEventRepository.AddEventsAsync(events);
            await _domainEventRepository.SaveAsync();


            return resultContract;
        }
    }
}
