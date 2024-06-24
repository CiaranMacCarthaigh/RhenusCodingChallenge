using RhenusCodingChallenge.Domain.Player.Events;
using FluentAssertions;
using RhenusCodingChallenge.Domain.Player;

namespace RhenusCodingChallenge.Domain.Test.Player
{
    [TestClass]
    public class PlayerAccountTests
    {
        [TestMethod]
        public void When_creating_a_new_PlayerAccount_with_a_corresponding_PlayerAccountCreatedEvent_the_PlayerAccount_is_correctly_initialised()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            var playerName = "Jonathan Doe";
            var initialBalance = 600m;

            var events = new[]
            {
                new PlayerAccountCreatedEvent(0, playerId, playerName, initialBalance)
            };

            var expectedResult = new PlayerAccount(playerId, playerName, initialBalance);

            // Act
            var actualResult = PlayerAccount.CreateFromEvents(events);

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void When_replaying_a_set_of_PlayerEvents_the_correct_PlayerAccount_aggregate_is_created()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            var playerName = "Jonathan Doe";
            var initialBalance = 1000m;

            var events = new PlayerEvent[]
            {
                new PlayerAccountCreatedEvent(0, playerId, playerName, initialBalance),
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

            var expectedResult = new PlayerAccount(playerId, playerName, 1550m);

            // Act
            var actualResult = PlayerAccount.CreateFromEvents(events);

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }
    }
}
