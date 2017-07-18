using Foundatio.Skeleton.Domain.Localization.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Foundatio.Skeleton.Domain.Localization {
    public class Text : IText {

        private readonly ILocalizedStringManager _localizedStringManager;

        public Text(ILocalizedStringManager localizedStringManager) {
            _localizedStringManager = localizedStringManager;
        }

        public async Task<LocalizedString> Get(string textHint, params object[] args) {
            var currentCulture = Settings.Current.CurrentCulture;

            var localizedFormat = await _localizedStringManager.GetLocalizedString(textHint, currentCulture);

            return args.Length == 0
                ? new LocalizedString(localizedFormat, textHint, args)
                : new LocalizedString(
                    string.Format(GetFormatProvider(currentCulture), localizedFormat, args.Select(Encode).ToArray()),
                    textHint, args);
        }

        private static IFormatProvider GetFormatProvider(string currentCulture) {
            try {
                return CultureInfo.GetCultureInfo(currentCulture);
            } catch {
                return null;
            }
        }

        static object Encode(object arg) {
            if (arg is IFormatProvider)
                return arg;

            return HttpUtility.HtmlEncode(arg);
        }
    }
}
