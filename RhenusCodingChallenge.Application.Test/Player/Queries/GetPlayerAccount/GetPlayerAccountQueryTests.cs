using FluentAssertions;
using NSubstitute;
using RhenusCodingChallenge.Application.Exceptions;
using RhenusCodingChallenge.Application.Player.Queries.GetPlayerAccount;
using RhenusCodingChallenge.Application.Player.Queries.GetPlayerAccount.Contracts;
using RhenusCodingChallenge.Application.Repositories;
using RhenusCodingChallenge.Domain.Player.Events;

namespace RhenusCodingChallenge.Application.Test.Player.Queries.GetPlayerAccount
{
    [TestClass]
    public class GetPlayerAccountQueryTests
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

        [TestMethod]
        public async Task When_getting_a_player_aggregate_that_does_not_exist_throw_an_Exception()
        {
            // Arrange
            var domainEventRepository = MockDomainEventRepositoryWith();
            var playerId = Guid.NewGuid();
            
            var query = new GetPlayerAccountQuery(playerId);
            var queryHandler = new GetPlayerAccountQueryHandler(domainEventRepository);

            // Act
            var action = async () => await queryHandler.Handle(query, CancellationToken.None);

            // Assert
            (await action.Should().ThrowExactlyAsync<AggregateNotFoundException>())
                .And.AggregateId.Should().Be(playerId);
        }

        [TestMethod]
        public async Task When_getting_a_player_aggregate_that_exists_return_the_contract_with_the_correct_values()
        {
            // Arrange
            var playerName = "John Doe";
            var playerId = Guid.NewGuid();
            var events = new PlayerEvent[]
            {
                new PlayerAccountCreatedEvent(0, playerId, playerName, 1000m),
                new PlayerLosesEvent(1, playerId, 150m, 1, 8),
                new PlayerLosesEvent(2, playerId, 200m, 7, 3),
                new PlayerLosesEvent(3, playerId, 400m, 8, 2),
                new PlayerAddsNewFundsEvent(4, playerId, 650m),
                new PlayerLosesEvent(5, playerId, 200m, 3, 4),
                new PlayerLosesEvent(6, playerId, 150m, 1, 7),
                new PlayerWinsEvent(7, playerId, 200m, 6, 1800m),
                new PlayerWinsEvent(8, playerId, 300m, 5, 2700m),
                new PlayerWithdrawsFundsEvent(9, playerId, 3500m),
            };

            var domainEventRepository = MockDomainEventRepositoryWith(events);

            var query = new GetPlayerAccountQuery(playerId);
            var queryHandler = new GetPlayerAccountQueryHandler(domainEventRepository);

            var expectedResult = new PlayerAccountContract(Id: playerId, PlayerName: playerName, Balance: 1550m);

            // Act
            var actualResult = await queryHandler.Handle(query, CancellationToken.None);

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }
    }
}
