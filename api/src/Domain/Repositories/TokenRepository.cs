using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;
using Foundatio.Skeleton.Core.Extensions;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class TokenRepository : EFRepositoryBase<Token>, ITokenRepository {


        public TokenRepository(IEFRepositoryContext efRepositoryContext)
            : base(efRepositoryContext, null, null) {
        }


        public async Task<Token> GetByRefreshTokenAsync(string refreshToken) {
            if (string.IsNullOrEmpty(refreshToken))
                return null;

            var results = await FindAsync(x => x.Refresh == refreshToken);
            return results.FirstOrDefault();

        }

        public async Task<IReadOnlyCollection<Token>> GetByUserIdAsync(string userId) {

            if (userId == string.Empty)
                return null;

            return await FindAsync(x => x.UserId == userId);

        }

        public async Task<Token> GetOrCreateUserToken(string userId, string organizationId) {

            var existingToken = (await GetByUserIdAsync(userId)).FirstOrDefault();
            if (existingToken != null && existingToken.ExpiresUtc > DateTime.UtcNow)
                return existingToken;

            var token = new Token {
                Id = StringUtils.GetNewToken(),
                UserId = userId,
                OrganizationId = organizationId,
                CreatedBy = userId,
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow,
                TypeId = (int)Models.TokenType.Access,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
            };

            await AddAsync(token);

            return token;
        }

    }
}
