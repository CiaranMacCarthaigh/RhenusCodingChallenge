namespace RhenusCodingChallenge.Domain
{
    public abstract class DomainEvent
    {
        protected DomainEvent(int aggregateVersion, Guid aggregateId)
        {
            AggregateVersion = aggregateVersion;
            AggregateId = aggregateId;
        }

        public int AggregateVersion { get; }
        public Guid AggregateId { get; }
    }
}
