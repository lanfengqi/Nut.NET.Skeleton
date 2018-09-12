using Foundatio.Jobs;
using Foundatio.Logging;
using Foundatio.Queues;
using Foundatio.Skeleton.Core.Extensions;
using Foundatio.Skeleton.Core.Sms;
using Foundatio.Skeleton.Core.Sms.Model;
using System;
using System.Threading.Tasks;

namespace Foundatio.Skeleton.Core.Jobs {
    public class SmsMessageJob : QueueJobBase<SmsMessage> {
        private readonly ISmsSender _smsSender;

        public SmsMessageJob(IQueue<SmsMessage> queue, ISmsSender smsSender, ILoggerFactory loggerFactory)
            : base(queue, loggerFactory) {
            _smsSender = smsSender;
            AutoComplete = true;
        }

        protected override async Task<JobResult> ProcessQueueEntryAsync(QueueEntryContext<SmsMessage> context) {
            _logger.Trace("Processing sms message '{0}'", context.QueueEntry.Id);
            try {
                await _smsSender.SendAsync(context.QueueEntry.Value).ConfigureAwait(false);
                _logger.Info().
                    Message(() => $"Send  Sms messge: to={context.QueueEntry.Value.Phone} code={context.QueueEntry.Value.TemplateCode}")
                    .Write();
            } catch (Exception ex) {
                await context.QueueEntry.AbandonAsync().AnyContext();
                return JobResult.FromException(ex);
            }

            return JobResult.Success;
        }
    }
}
