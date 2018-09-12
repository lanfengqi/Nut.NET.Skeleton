using Foundatio.Queues;
using Foundatio.Skeleton.Core.Extensions;
using Foundatio.Skeleton.Core.Sms.Model;
using Foundatio.Skeleton.Domain.Models;
using System;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Services {
    public class TemplatedSmsService : ITemplatedSmsService {
        private readonly IQueue<SmsMessage> _queue;


        public TemplatedSmsService(IQueue<SmsMessage> queue) {
            _queue = queue;
        }

        public void SendWellcomeSms(User user) {
            if (string.IsNullOrEmpty(user?.Phone))
                return;
            Task.Run(async () => {
                var message = new SmsMessage() {
                    OutId = Guid.NewGuid().ToString("N"),
                    Phone = user?.Phone,
                    TemplateCode = "SMS_142621751",
                    Title = "WellcomeSms"
                };
                message.TemplateParams.Add(new SmsToken {
                    Key = "BuyerName",
                    Value = "测试"
                });
                message.TemplateParams.Add(new SmsToken {
                    Key = "OrderNo",
                    Value = "123455"
                });
                await QueueMessageAsync(message).AnyContext();
            });

        }

        private Task QueueMessageAsync(SmsMessage message) {
            return _queue.EnqueueAsync(message);

        }
    }
}
