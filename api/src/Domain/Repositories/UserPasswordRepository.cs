using FluentValidation;
using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Repositories {
    public class UserPasswordRepository : EFRepositoryBase<UserPassword>, IUserPasswordRepository {

        public UserPasswordRepository(IEFRepositoryContext efRepositoryContext, IValidator<UserPassword> validators)
            : base(efRepositoryContext, validators) {
        }

        public async Task<UserPassword> GetByPasswordResetTokenAsync(string token) {
            if (String.IsNullOrEmpty(token))
                return null;

            var users = await this.FindAsync(x => x.PasswordResetToken.Equals(token));
            return users.FirstOrDefault();
        }

        public async Task<UserPassword> GetByUserIdAsync(string userId) {
            if (String.IsNullOrEmpty(userId))
                return null;

            var users = await this.FindAsync(x => x.UserId.Equals(userId));

            return users.OrderByDescending(d => d.CreatedUtc).FirstOrDefault();
        }
    }
}
