namespace RhenusCodingChallenge.Domain.Player.Events
{
    public class PlayerAccountCreatedEvent : PlayerEvent
    {
        public string PlayerName { get; }
        public decimal InitialBalance { get; }

        public PlayerAccountCreatedEvent(int aggregateVersion, Guid playerId, string playerName, decimal initialBalance)
            : base(aggregateVersion, playerId)
        {
            PlayerName = playerName;
            InitialBalance = initialBalance;
        }
    }
}