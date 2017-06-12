using Foundatio.Skeleton.Domain.Models;
using Foundatio.Skeleton.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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
            if (await _userRepository.CountAsync() != 0)
                return;

            await CreateRoleAsync();

        }

        public async Task CreateRoleAsync() {
            var roles = new List<Role>();

            roles.Add(new Role() {
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString("N"),
                Name = "超级管理员",
                SystemName = AuthorizationRoles.GlobalAdmin
            });

            roles.Add(new Role() {
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString("N"),
                Name = "管理员",
                SystemName = AuthorizationRoles.Admin
            });

            roles.Add(new Role() {
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString("N"),
                Name = "用户",
                SystemName = AuthorizationRoles.User
            });

            roles.Add(new Role() {
                CreatedUtc = DateTime.UtcNow,
                UpdatedUtc = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString("N"),
                Name = "游客",
                SystemName = AuthorizationRoles.Client
            });


            await _roleRepository.AddAsync(roles);
        }

    }
}
