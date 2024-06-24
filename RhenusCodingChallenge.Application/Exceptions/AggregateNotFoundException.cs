namespace RhenusCodingChallenge.Application.Exceptions
{
    public class AggregateNotFoundException : Exception
    {
        public AggregateNotFoundException(Guid aggregateId)
            : base($"A reference was made to aggregate with ID {aggregateId}, but no such aggregate existed")
        {
            AggregateId = aggregateId;
        }

        public Guid AggregateId { get; }
    }
}