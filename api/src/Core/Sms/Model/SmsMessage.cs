using System;
using System.Collections.Generic;

namespace Foundatio.Skeleton.Core.Sms.Model {
    public class SmsMessage {

        public SmsMessage() {
            TemplateParams = new List<SmsToken>();
        }

        public string Title { get; set; }

        public string Context { get; set; }

        public string Phone { get; set; }

        public string TemplateCode { get; set; }

        public List<SmsToken> TemplateParams { get; set; }

        public string OutId { get; set; }
    }

    public class SmsToken {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
