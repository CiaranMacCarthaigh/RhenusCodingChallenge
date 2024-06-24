using Microsoft.EntityFrameworkCore;
using NodaTime;
using RhenusCodingChallenge.Application;
using RhenusCodingChallenge.Application.Repositories;
using RhenusCodingChallenge.Domain;
using RhenusCodingChallenge.Domain.Player.Events;
using RhenusCodingChallenge.Infrastructure.Database.Context;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace RhenusCodingChallenge.Infrastructure.Database.Repositories
{
    public class DomainEventRepository : IDomainEventRepository
    {
        private readonly EventStorageDbContext _eventStorageDbContext;
        private readonly IClock _clock;
        private static readonly JsonSerializerOptions _defaultJsonSerialiserOptions;

        static DomainEventRepository()
        {
            _defaultJsonSerialiserOptions = new JsonSerializerOptions()
            {
                TypeInfoResolver = new DefaultJsonTypeInfoResolver()
                {
                    Modifiers =
                    {
                        PolymorphicTypeResolver.AddDerivedTypes(
                            typeof(DomainEvent),
                            typeof(PlayerAccountCreatedEvent),
                            typeof(PlayerAddsNewFundsEvent),
                            typeof(PlayerLosesEvent),
                            typeof(PlayerWinsEvent),
                            typeof(PlayerWithdrawsFundsEvent)
                        )
                    }
                }
            };
        }

        public DomainEventRepository(EventStorageDbContext eventStorageDbContext, IClock clock)
        {
            _eventStorageDbContext = eventStorageDbContext;
            _clock = clock;
        }

        public async Task AddEventsAsync<T>(IReadOnlyCollection<T> domainEvents)
            where T : DomainEvent
        {
            var storageObjects =
                domainEvents
                .Select(x => new DomainEventStorageObject(Version: x.AggregateVersion, AggregateId: x.AggregateId, EventData: JsonSerializer.Serialize(x, _defaultJsonSerialiserOptions), Timestamp: _clock.GetCurrentInstant()))
                .ToList();

            await _eventStorageDbContext.DomainEventStorageDbSet.AddRangeAsync(storageObjects);
        }

        public async Task<IReadOnlyCollection<T>> GetDomainEventsAsync<T>(Guid aggregateId) where T : DomainEvent
        {
            var events =
                await _eventStorageDbContext.DomainEventStorageDbSet
                .Where(x => x.AggregateId == aggregateId)
                .OrderBy(x => x.Version)
                .Select(x => x.EventData)
                .ToListAsync();

            return
                events
                .Select(x => JsonSerializer.Deserialize<T>(x, _defaultJsonSerialiserOptions))
                .ToList();
        }

        public async Task SaveAsync()
        {
            await _eventStorageDbContext.SaveChangesAsync();
        }

        public async Task<int> GetLatestVersionNumberAsync(Guid aggregateId)
        {
            var latestEvent =
                await _eventStorageDbContext.DomainEventStorageDbSet
                .Where(x => x.AggregateId == aggregateId)
                .OrderByDescending(x => x.Version)
                .FirstOrDefaultAsync();

            return latestEvent?.Version ?? 0;
        }
    }
}
