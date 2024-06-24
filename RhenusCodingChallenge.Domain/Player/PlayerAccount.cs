using System.Diagnostics;
using System.Runtime.CompilerServices;
using RhenusCodingChallenge.Domain.Player.Events;

[assembly: InternalsVisibleTo("RhenusCodingChallenge.Domain.Test")]

namespace RhenusCodingChallenge.Domain.Player
{
    public class PlayerAccount
    {
        public Guid Id { get; }

        public string PlayerName { get; }

        public decimal Balance { get; private set; }

        internal PlayerAccount(Guid id, string playerName, decimal initialBalance)
        {
            Id = id;
            PlayerName = playerName;
            Balance = initialBalance;
        }

        [DebuggerStepThrough()]
        private static void AssertPlayerAccountInitialised(PlayerAccount? playerAccount, PlayerEvent playerEvent)
        {
            if (playerAccount == null)
            {
                throw new ArgumentNullException($"Attempted to apply an event of type {playerEvent} with ID {playerEvent.AggregateId} version {playerEvent.AggregateVersion} on an instance of {nameof(PlayerAccount)} that was no initialised", nameof(playerAccount));
            }
        }

        public static PlayerAccount CreateFromEvents(IReadOnlyCollection<PlayerEvent> events)
        {
            if (events == null || events.Count == 0)
            {
                throw new ArgumentException($"Attempted to create an instance of {nameof(PlayerAccount)} from no events.", nameof(events));
            }

            PlayerAccount playerAccount = null;
            foreach (var playerEvent in events.OrderBy(x => x.AggregateVersion))
            {
                switch (playerEvent)
                {
                    case PlayerAccountCreatedEvent playerAccountCreated:
                        if (playerAccount != null)
                        {
                            throw new InvalidOperationException($"Tried to apply a {nameof(PlayerAccountCreatedEvent)} event to an already initialised instance of {nameof(PlayerAccount)}");
                        }

                        playerAccount = new PlayerAccount(playerAccountCreated.PlayerId, playerAccountCreated.PlayerName, playerAccountCreated.InitialBalance);
                        break;

                    case PlayerLosesEvent playerLosesEvent:
                        AssertPlayerAccountInitialised(playerAccount, playerLosesEvent);
                        playerAccount.Balance -= playerLosesEvent.Stake;
                        break;

                    case PlayerWinsEvent playerWinsEvent:
                        AssertPlayerAccountInitialised(playerAccount, playerWinsEvent);
                        playerAccount.Balance += playerWinsEvent.AmountWon;
                        break;

                    case PlayerAddsNewFundsEvent playerAddsNewFundsEvent:
                        AssertPlayerAccountInitialised(playerAccount, playerAddsNewFundsEvent);
                        playerAccount.Balance += playerAddsNewFundsEvent.AmountAdded;
                        break;

                    case PlayerWithdrawsFundsEvent playerWithdrawsFundsEvent:
                        AssertPlayerAccountInitialised(playerAccount, playerWithdrawsFundsEvent);
                        playerAccount.Balance -= playerWithdrawsFundsEvent.AmountWithdrawn;
                        break;

                    default:
                        throw new InvalidDataException($"The event of type {playerEvent.GetType()} could not be applied to an instance of {nameof(PlayerAccount)}");
                }
            }

            return playerAccount;
        }
    }
}
