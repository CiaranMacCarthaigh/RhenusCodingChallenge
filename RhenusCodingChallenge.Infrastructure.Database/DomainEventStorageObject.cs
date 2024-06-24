using NodaTime;

namespace RhenusCodingChallenge.Infrastructure.Database
{
    public record DomainEventStorageObject(int Version, Guid AggregateId, string EventData, Instant? Timestamp);
}
