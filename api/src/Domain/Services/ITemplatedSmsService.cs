using Foundatio.Skeleton.Domain.Models;

namespace Foundatio.Skeleton.Domain.Services {
    public interface ITemplatedSmsService {

        void SendPhoneVerifySms(User user);
    }
}
