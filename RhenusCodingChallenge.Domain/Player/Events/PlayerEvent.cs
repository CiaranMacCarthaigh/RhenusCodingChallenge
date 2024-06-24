namespace RhenusCodingChallenge.Domain.Player.Events
{
    public abstract class PlayerEvent : DomainEvent
    {
        protected PlayerEvent(int aggregateVersion, Guid playerId)
            : base(aggregateVersion, playerId)
        {
        }

        public Guid PlayerId { get { return AggregateId; } }
    }
}