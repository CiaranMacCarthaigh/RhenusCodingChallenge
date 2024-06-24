namespace RhenusCodingChallenge.Application.Services.Game
{
    public class GameService : IGameService
    {
        private const decimal WinningStakeMultiplier = 9;

        private readonly IRandomNumberGeneratorService _randomNumberGenerator;

        public GameService(IRandomNumberGeneratorService randomNumberGenerator)
        {
            _randomNumberGenerator = randomNumberGenerator;
        }

        public GameResult RunGame(GameBet gameBet)
        {
            var winningNumber = _randomNumberGenerator.GetRandomNumber();

            if (gameBet.NumberBet == winningNumber)
            {
                var amountWon = gameBet.Stake * WinningStakeMultiplier;
                return new GameResult(Outcome: GameBetOutcome.Win, WinningNumber: winningNumber, Stake: gameBet.Stake, Multiplier: WinningStakeMultiplier, Difference: amountWon);
            }

            return new GameResult(Outcome: GameBetOutcome.Lose, WinningNumber: winningNumber, Stake: gameBet.Stake, Multiplier: WinningStakeMultiplier, Difference: -gameBet.Stake);
        }
    }
}