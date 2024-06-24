namespace RhenusCodingChallenge.Domain.Player.Events
{
    public class PlayerAddsNewFundsEvent: PlayerEvent
    {
        public decimal AmountAdded { get; }

        public PlayerAddsNewFundsEvent(int aggregateVersion, Guid playerId, decimal amountAdded)
            : base(aggregateVersion, playerId)
        {
            AmountAdded = amountAdded;
        }
    }
}