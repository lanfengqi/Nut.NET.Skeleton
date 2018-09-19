using Foundatio.Skeleton.Domain.Models;

namespace Foundatio.Skeleton.Domain.Services {
    public interface ITemplatedSmsService {

        void SendPhoneVerifyNotification(User user);

        void SendOrderCompletedNotification(Order order);

        void SendOrderCannelledNotification(Order order);
    }
}
