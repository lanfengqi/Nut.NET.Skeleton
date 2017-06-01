using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using Foundatio.Skeleton.Repositories.Model;
using Foundatio.Skeleton.Repositories.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public interface ITokenRepository : IRepository<Token> {

        Task<Token> GetByRefreshTokenAsync(string refreshToken);

        //Task<PagedList<Token>> GetApiTokensAsync(string organizationId, IPagingOptions paging = null, bool useCache = false, TimeSpan? expireIn = null);

        Task<IReadOnlyCollection<Token>> GetByUserIdAsync(string userId);

        Task<Token> GetOrCreateUserToken(string userId, string organizationId);

    }
}
