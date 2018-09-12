using System;
using System.Collections.Generic;
using System.Text;

namespace Foundatio.Skeleton.Core.Sms {
    public static class TokenExtensions {
        public static string ToJson(this IEnumerable<Sms.Model.SmsToken> tokens) {
            var sb = new StringBuilder("{");
            foreach (var token in tokens) {
                sb.Append(String.Format("\"{0}\": \"{1}\"", token.Key, token.Value)).Append(',');
            }
            if (sb.Length > 1)
                sb.Remove(sb.Length - 1, 1);
            return sb.Append('}').ToString();
        }
    }
}
