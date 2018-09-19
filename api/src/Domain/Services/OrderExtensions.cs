using Foundatio.Skeleton.Core.Extensions;
using Foundatio.Skeleton.Domain.Models;

namespace Foundatio.Skeleton.Domain.Services {
    public static class OrderExtensions {
        public static string GrenerteOrderCustomNumber(this Order order) {
            if (order == null)
                return "";

            var customNumber = Settings.Current.CustomOrderNumberMask
                .Replace("{ID}", StringUtils.GetRandomString(6, "0123456789"))
                .Replace("{YYYY}", order.CreatedUtc.ToString("yyyy"))
                .Replace("{YY}", order.CreatedUtc.ToString("yy"))
                .Replace("{MM}", order.CreatedUtc.ToString("mm"))
                .Replace("{DD}", order.CreatedUtc.ToString("dd")).Trim();

            return customNumber;
        }
    }
}
