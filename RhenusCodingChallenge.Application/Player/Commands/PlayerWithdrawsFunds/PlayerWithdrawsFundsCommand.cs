using MediatR;
using RhenusCodingChallenge.Application.Player.Queries.GetPlayerAccount.Contracts;

namespace RhenusCodingChallenge.Application.Player.Commands.PlayerWithdrawsFunds
{
    public class PlayerWithdrawsFundsCommand : IRequest<PlayerAccountContract>
    {
        public Guid PlayerId { get; set; }
        public decimal Amount { get; set; }
    }
}
