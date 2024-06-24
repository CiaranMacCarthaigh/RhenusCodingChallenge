namespace RhenusCodingChallenge.Application.Player.Commands.PlayerPlacesBet.Contracts
{
    public enum BetOutcome
    {
        Win,
        Lose
    }

    public record PlayerBetResultContract(Guid PlayerId, decimal Stake, string Outcome, int WinningNumber, decimal Difference, decimal NewBalance);
}
