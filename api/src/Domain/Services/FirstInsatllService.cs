using Foundatio.Skeleton.Domain.Repositories;
using System.Threading.Tasks;

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


        }

    }
}
