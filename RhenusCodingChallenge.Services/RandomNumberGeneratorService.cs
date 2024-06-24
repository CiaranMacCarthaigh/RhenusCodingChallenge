using RhenusCodingChallenge.Application.Services;

namespace RhenusCodingChallenge.Services
{
    public class RandomNumberGeneratorService : IRandomNumberGeneratorService
    {
        private readonly Random _randomNumberGenerator = new Random();

        public int GetRandomNumber()
        {
            return _randomNumberGenerator.Next(0, 10);
        }
    }
}
