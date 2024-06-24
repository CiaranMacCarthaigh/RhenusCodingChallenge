using RhenusCodingChallenge.Application.Services;

namespace RhenusCodingChallenge.Services
{
    public class GuidProviderService : IGuidProviderService
    {
        public Guid GetNew()
        {
            return Guid.NewGuid();
        }
    }
}
