using MediatR;
using RhenusCodingChallenge.Application.Exceptions;
using RhenusCodingChallenge.Application.Player.Queries.GetPlayerAccount.Contracts;
using RhenusCodingChallenge.Application.Repositories;
using RhenusCodingChallenge.Domain.Player;
using RhenusCodingChallenge.Domain.Player.Events;

namespace RhenusCodingChallenge.Application.Player.Queries.GetPlayerAccount
{
    public class GetPlayerAccountQueryHandler : IRequestHandler<GetPlayerAccountQuery, PlayerAccountContract>
    {
        private readonly IDomainEventRepository _domainEventRepository;

        public GetPlayerAccountQueryHandler(IDomainEventRepository domainEventRepository)
        {
            _domainEventRepository = domainEventRepository;
        }

        public async Task<PlayerAccountContract> Handle(GetPlayerAccountQuery request, CancellationToken cancellationToken)
        {
            var events = await _domainEventRepository.GetDomainEventsAsync<PlayerEvent>(request.PlayerId);
            if (events == null || events.Count == 0)
            {
                throw new AggregateNotFoundException(request.PlayerId);
            }

            var player = PlayerAccount.CreateFromEvents(events);

            return new PlayerAccountContract(Id: player.Id, PlayerName: player.PlayerName, Balance: player.Balance);
        }
    }
}
