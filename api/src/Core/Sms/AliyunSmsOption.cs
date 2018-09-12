namespace Foundatio.Skeleton.Core.Sms {
    public class AliyunSmsOption {

        public AliyunSmsOption(string accessKeyId,string accessKeySecret, string signName) {
            AccessKeyId = accessKeyId;
            AccessKeySecret = accessKeySecret;
            SignName = signName;
        }

        public string AccessKeyId { get; private set; }

        public string AccessKeySecret { get; private set; }

        public string SignName { get; private set; }
    }
}
