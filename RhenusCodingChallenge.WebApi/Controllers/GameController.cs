using MediatR;
using Microsoft.AspNetCore.Mvc;
using RhenusCodingChallenge.Application.Player.Commands.CreateNewPlayer;
using RhenusCodingChallenge.Application.Player.Commands.CreateNewPlayer.Contracts;
using RhenusCodingChallenge.Application.Player.Commands.PlayerPlacesBet;
using RhenusCodingChallenge.Application.Player.Commands.PlayerPlacesBet.Contracts;
using RhenusCodingChallenge.Application.Player.Commands.PlayerWithdrawsFunds;
using RhenusCodingChallenge.Application.Player.Queries.GetPlayerAccount;
using RhenusCodingChallenge.Application.Player.Queries.GetPlayerAccount.Contracts;

namespace RhenusCodingChallenge.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GameController : Controller
    {
        private readonly ISender _sender;

        public GameController(ISender sender)
        {
            _sender = sender;
        }

        [HttpGet("player/{playerId}")]
        public async Task<PlayerAccountContract> GetPlayerAccount(Guid playerId)
        {
            var query = new GetPlayerAccountQuery(playerId);
            return await _sender.Send(query);
        }

        [HttpPut("player")]
        public async Task<CreateNewPlayerCommandResultContract> GetPlayerAccount(CreateNewPlayerCommand command)
        {
            return await _sender.Send(command);
        }

        [HttpPost("player/add-funds")]
        public async Task<PlayerAccountContract> PlayerAddsFunds(PlayerAddsFundsCommand command)
        {
            return await _sender.Send(command);
        }

        [HttpPost("player/withdraw-funds")]
        public async Task<PlayerAccountContract> PlayerWithdrawsFunds(PlayerWithdrawsFundsCommand command)
        {
            return await _sender.Send(command);
        }

        [HttpPost("bet")]
        public async Task<PlayerBetResultContract> PlaceBest(PlayerPlacesBetCommand command)
        {
            return await _sender.Send(command);
        }
    }
}
