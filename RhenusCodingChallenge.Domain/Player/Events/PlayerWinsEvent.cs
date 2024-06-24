namespace RhenusCodingChallenge.Domain.Player.Events
{
    public class PlayerWinsEvent : PlayerEvent
    {
        public decimal Stake { get; }
        public int WinningNumber { get; }
        public decimal AmountWon { get; }

        public PlayerWinsEvent(int aggregateVersion, Guid playerId, decimal stake, int winningNumber, decimal amountWon)
            : base(aggregateVersion, playerId)
        {
            Stake = stake;
            WinningNumber = winningNumber;
            AmountWon = amountWon;
        }
    }
}
