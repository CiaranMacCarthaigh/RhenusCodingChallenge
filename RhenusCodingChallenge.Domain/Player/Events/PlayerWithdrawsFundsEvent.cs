namespace RhenusCodingChallenge.Domain.Player.Events
{
    public class PlayerWithdrawsFundsEvent : PlayerEvent
    {
        public decimal AmountWithdrawn { get; }

        public PlayerWithdrawsFundsEvent(int aggregateVersion, Guid playerId, decimal amountWithdrawn)
            : base(aggregateVersion, playerId)
        {
            AmountWithdrawn = amountWithdrawn;
        }
    }
}
