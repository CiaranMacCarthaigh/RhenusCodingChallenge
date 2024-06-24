namespace RhenusCodingChallenge.Application.Services.Game
{
    public enum GameBetOutcome
    {
        Win,
        Lose
    }

    public record GameResult(GameBetOutcome Outcome, int WinningNumber, decimal Stake, decimal Multiplier, decimal Difference);
}
