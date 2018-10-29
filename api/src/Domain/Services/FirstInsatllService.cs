using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Foundatio.Skeleton.Core.Extensions;

namespace Foundatio.Skeleton.Domain.Services {
    public class FirstInsatllService {

        private readonly IOrganizationRepository _organizationRepository;
        private readonly ITokenRepository _tokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;

        public FirstInsatllService(IOrganizationRepository organizationRepository, ITokenRepository tokenRepository,
           IUserRepository userRepository, IRoleRepository roleRepository) {

            this._organizationRepository = organizationRepository;
            this._roleRepository = roleRepository;
            this._tokenRepository = tokenRepository;
            this._userRepository = userRepository;
        }

        public async Task InstallData() {
            if (await _roleRepository.CountAsync() != 0)
                return;

            await CreateRoleAsync();

        }

        public async Task CreateRoleAsync() {

            var globalAdmin = new Role() {
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString("N"),
                Name = "超级管理员",
                SystemName = AuthorizationRoles.GlobalAdmin
            };
            await _roleRepository.AddAsync(globalAdmin).AnyContext();

            var admin = new Role() {
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString("N"),
                Name = "管理员",
                SystemName = AuthorizationRoles.Admin
            };
            await _roleRepository.AddAsync(admin).AnyContext();

            var user = new Role() {
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString("N"),
                Name = "用户",
                SystemName = AuthorizationRoles.User
            };
            await _roleRepository.AddAsync(user).AnyContext();

            var client = new Role() {
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString("N"),
                Name = "游客",
                SystemName = AuthorizationRoles.Client
            };
            await _roleRepository.AddAsync(client).AnyContext();

        }

    }
}
