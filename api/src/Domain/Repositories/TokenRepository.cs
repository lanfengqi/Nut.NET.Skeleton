using FluentValidation;
using Foundatio.Skeleton.Core.Extensions;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class TokenRepository : EFRepositoryBase<Token>, ITokenRepository {

        public TokenRepository(IEFRepositoryContext efRepositoryContext, IValidator<Token> validators)
            : base(efRepositoryContext, validators) {
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

            return await FindAsync(x => x.UserId == userId, o => o.ExpiresUtc, SortOrder.Descending);
        }

        public async Task<Token> GetOrCreateUserToken(string userId, string organizationId) {

            var currentDateTime = DateTime.UtcNow;

            var existingToken = (await GetByUserIdAsync(userId))?.FirstOrDefault();
            if (existingToken != null && existingToken.ExpiresUtc > currentDateTime)
                return existingToken;

            var token = new Token {
                Id = StringUtils.GetNewToken(),
                UserId = userId,
                OrganizationId = organizationId,
                CreatedBy = userId,
                CreatedUtc = currentDateTime,
                UpdatedUtc = currentDateTime,
                Type = TokenType.Access,
                ExpiresUtc = currentDateTime.AddMinutes(Settings.Current.TokenExpiressMinutes)
            };

            await AddAsync(token);

            return token;
        }

    }
}
