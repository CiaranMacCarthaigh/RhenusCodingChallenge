using NSubstitute;
using RhenusCodingChallenge.Application.Exceptions;
using RhenusCodingChallenge.Application.Repositories;
using RhenusCodingChallenge.Application.Services.Game;
using RhenusCodingChallenge.Domain.Player.Events;
using RhenusCodingChallenge.Application.Player.Commands.PlayerPlacesBet;
using FluentAssertions;

namespace RhenusCodingChallenge.Application.Test.Player.PlayerPlacesBet
{
    [TestClass]
    public class PlayerPlacesBetCommandHandlerTests
    {
        private List<PlayerEvent> _storedPlayerEvents = new List<PlayerEvent>();

        [TestInitialize]
        public void TestInitialise()
        {
            _storedPlayerEvents = new List<PlayerEvent>();
        }

        private IDomainEventRepository MockDomainEventRepositoryWith(params PlayerEvent[] events)
        {
            _storedPlayerEvents = new List<PlayerEvent>(events);

            var substitute = Substitute.For<IDomainEventRepository>();
            substitute.GetDomainEventsAsync<PlayerEvent>(Arg.Any<Guid>()).Returns(Task.FromResult<IReadOnlyCollection<PlayerEvent>>(_storedPlayerEvents));
            substitute.AddEventsAsync<PlayerEvent>(Arg.Any<IReadOnlyCollection<PlayerEvent>>()).ReturnsForAnyArgs(callInfo =>
            {
                var newEvents = (IReadOnlyCollection<PlayerEvent>)callInfo[0];
                _storedPlayerEvents.AddRange(newEvents);
                return Task.CompletedTask;
            });
            return substitute;
        }

        private static IGameService MockLosingGameService(decimal amountLost)
        {
            var substitute = Substitute.For<IGameService>();
            substitute.RunGame(Arg.Any<GameBet>()).Returns(new GameResult(Outcome: GameBetOutcome.Lose, WinningNumber: 0, Stake: amountLost, Multiplier: 1, Difference: -amountLost));
            return substitute;
        }

        private static IGameService MockWinningGameService(decimal amountWon)
        {
            var substitute = Substitute.For<IGameService>();
            substitute.RunGame(Arg.Any<GameBet>()).Returns(new GameResult(Outcome: GameBetOutcome.Win, WinningNumber: 0, Stake: amountWon, Multiplier: 1, Difference: amountWon));
            return substitute;
        }

        [TestMethod]
        public async Task If_the_player_attempts_to_place_a_bet_that_they_do_not_have_enough_funds_for_throw_an_Exception_with_the_expected_information()
        {
            // Arrange
            var playerId = Guid.NewGuid();

            var playerEvents = new PlayerEvent[]
            {
                new PlayerAccountCreatedEvent(0, playerId, "John Doe", 1000),
                new PlayerLosesEvent(1, playerId, 200m, 1, 8),
            };
            var domainEventRepository = MockDomainEventRepositoryWith(playerEvents);
            var gameService = MockLosingGameService(100);

            var command = new PlayerPlacesBetCommand()
            {
                PlayerId = playerId,
                Number = 1,
                Stake = 900
            };
            var commandHandler = new PlayerPlacesBetCommandHandler(gameService, domainEventRepository);


            // Act
            var action = async () =>
            {
                await commandHandler.Handle(command, CancellationToken.None);
            };

            // Assert
            var thrownException =
                (await action.Should().ThrowExactlyAsync<InvalidCommandException>())
                .And.PropertyName.Should().Be(nameof(command.Stake));
        }

        [TestMethod]
        public async Task If_the_player_places_a_bet_and_wins_the_player_events_are_updated_with_new_event()
        {
            // Arrange
            var playerId = Guid.NewGuid();

            var playerEvents = new PlayerEvent[]
            {
                new PlayerAccountCreatedEvent(0, playerId, "John Doe", 1200),
                new PlayerLosesEvent(1, playerId, 200m, 1, 8),
            };
            var stake = 500m;
            var domainEventRepository = MockDomainEventRepositoryWith(playerEvents);
            var gameService = MockWinningGameService(stake);

            var command = new PlayerPlacesBetCommand()
            {
                PlayerId = playerId,
                Number = 1,
                Stake = stake
            };
            var commandHandler = new PlayerPlacesBetCommandHandler(gameService, domainEventRepository);


            var expectedPlayerEvents = playerEvents.ToList();
            expectedPlayerEvents.Add(new PlayerWinsEvent(2, playerId, stake, 1, stake));

            // Act
            await commandHandler.Handle(command, CancellationToken.None);

            // Assert
            _storedPlayerEvents.Should().BeEquivalentTo(expectedPlayerEvents);
        }

        [TestMethod]
        public async Task If_the_player_places_a_bet_and_loses_the_player_events_are_updated_with_new_event()
        {
            // Arrange
            var playerId = Guid.NewGuid();

            var playerEvents = new PlayerEvent[]
            {
                new PlayerAccountCreatedEvent(0, playerId, "John Doe", 1200),
                new PlayerLosesEvent(1, playerId, 200m, 1, 8),
            };
            var stake = 500m;
            var domainEventRepository = MockDomainEventRepositoryWith(playerEvents);
            var gameService = MockLosingGameService(stake);

            var command = new PlayerPlacesBetCommand()
            {
                PlayerId = playerId,
                Number = 1,
                Stake = stake
            };
            var commandHandler = new PlayerPlacesBetCommandHandler(gameService, domainEventRepository);


            var expectedPlayerEvents = playerEvents.ToList();
            expectedPlayerEvents.Add(new PlayerLosesEvent(2, playerId, stake, 1, 0));

            // Act
            await commandHandler.Handle(command, CancellationToken.None);

            // Assert
            _storedPlayerEvents.Should().BeEquivalentTo(expectedPlayerEvents);
        }
    }
}
