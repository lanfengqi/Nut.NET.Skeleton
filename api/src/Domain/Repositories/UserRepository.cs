using FluentValidation;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class UserRepository : EFRepositoryBase<User>, IUserRepository {


        public UserRepository(IEFRepositoryContext efRepositoryContext, IValidator<User> validators)
            : base(efRepositoryContext, validators) {
        }

        public async Task<User> GetByPhoneAsync(string phone) {
            if (String.IsNullOrEmpty(phone))
                return null;

            phone = phone.ToLower();

            var users = await this.FindAsync(x => x.Phone.Equals(phone));
            return users.FirstOrDefault();
        }

        public async Task<User> GetByVerifyPhoneTokenAsync(string token) {
            if (String.IsNullOrEmpty(token))
                return null;


            var users = await this.FindAsync(x => x.VerifyPhoneToken.Equals(token));
            return users.FirstOrDefault();

        }
    }
}
