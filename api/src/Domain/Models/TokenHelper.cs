using Foundatio.Skeleton.Domain.Repositories;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Models {
    public class TokenHelper
    {
        private readonly ITokenRepository _tokenRepository;
        public TokenHelper(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        public async Task<bool> ValidateToken(string tokenId)
        {
            var token = await _tokenRepository.GetByIdAsync(tokenId);
            return (token != null);
        }
    }
}
