using RhenusCodingChallenge.Domain;

namespace RhenusCodingChallenge.Application.Repositories
{
    public interface IDomainEventRepository
    {
        Task AddEventsAsync<T>(IReadOnlyCollection<T> domainEvents) where T : DomainEvent;
        public Task<IReadOnlyCollection<T>> GetDomainEventsAsync<T>(Guid aggregateId) where T : DomainEvent;
        Task<int> GetLatestVersionNumberAsync(Guid aggregateId);
        Task SaveAsync();
    }
}
