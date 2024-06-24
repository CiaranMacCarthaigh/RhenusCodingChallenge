using MediatR;
using RhenusCodingChallenge.Application.Player.Commands.PlayerPlacesBet.Contracts;

namespace RhenusCodingChallenge.Application.Player.Commands.PlayerPlacesBet
{
    public class PlayerPlacesBetCommand : IRequest<PlayerBetResultContract>
    {
        public Guid PlayerId { get; set; }
        public int Number { get; set; }
        public decimal Stake { get; set; }
    }
}
