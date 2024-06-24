using MediatR;
using RhenusCodingChallenge.Application.Player.Queries.GetPlayerAccount.Contracts;

namespace RhenusCodingChallenge.Application.Player.Queries.GetPlayerAccount
{
    public record GetPlayerAccountQuery(Guid PlayerId) : IRequest<PlayerAccountContract>
    {
    }
}
