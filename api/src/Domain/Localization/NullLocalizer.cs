using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Localization {
    public static class NullLocalizer {

        static NullLocalizer() {
            _instance = (format, args) => new Task<LocalizedString>(() => {
                return new LocalizedString((args == null || args.Length == 0) ? format : string.Format(format, args));
            });
        }

        static readonly Localizer _instance;

        public static Localizer Instance { get { return _instance; } }
    }
}
