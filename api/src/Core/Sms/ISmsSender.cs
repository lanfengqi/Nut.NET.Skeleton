using Foundatio.Skeleton.Core.Sms.Model;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Core.Sms {
    public interface ISmsSender {
        Task SendAsync(SmsMessage model);
    }
}
