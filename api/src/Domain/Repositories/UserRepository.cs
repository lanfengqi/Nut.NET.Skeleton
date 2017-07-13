﻿using Foundatio.Caching;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class UserRepository : EFRepositoryBase<User>, IUserRepository {


        public UserRepository(IEFRepositoryContext efRepositoryContext, ICacheClient cacheClient)
            : base(efRepositoryContext, cacheClient, null) {
        }

        public async Task<User> GetByEmailAddressAsync(string emailAddress) {
            if (String.IsNullOrEmpty(emailAddress))
                return null;

            emailAddress = emailAddress.ToLower();

            var users = await this.FindAsync(x => x.EmailAddress.Equals(emailAddress));
            return users.FirstOrDefault();
        }
    }
}
