namespace RhenusCodingChallenge.Domain.Player.Events
{
    public class PlayerLosesEvent : PlayerEvent
    {
        public decimal Stake { get; }
        public int BettingNumber { get; }
        public int WinningNumber { get; }

        public PlayerLosesEvent(int aggregateVersion, Guid playerId, decimal stake, int bettingNumber, int winningNumber)
            : base(aggregateVersion, playerId)
        {
            Stake = stake;
            BettingNumber = bettingNumber;
            WinningNumber = winningNumber;
        }
    }
}
