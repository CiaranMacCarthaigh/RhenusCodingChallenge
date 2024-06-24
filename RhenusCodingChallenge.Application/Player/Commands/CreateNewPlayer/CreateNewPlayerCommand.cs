using MediatR;
using RhenusCodingChallenge.Application.Player.Commands.CreateNewPlayer.Contracts;

namespace RhenusCodingChallenge.Application.Player.Commands.CreateNewPlayer
{
    public class CreateNewPlayerCommand : IRequest<CreateNewPlayerCommandResultContract>
    {
        public string PlayerName { get; set; }
        public decimal InitialBalance { get; set; }
    }
}
