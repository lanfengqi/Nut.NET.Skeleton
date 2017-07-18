using System;
using System.Web;

namespace Foundatio.Skeleton.Domain.Localization {
    public class LocalizedString : MarshalByRefObject {

        private readonly string _localized;
        private readonly string _textHint;
        private readonly object[] _args;

        public LocalizedString(string languageNeutra) {
            _localized = languageNeutra;
            _textHint = languageNeutra;
        }

        public LocalizedString(string localized,string textHint,object[] args) {
            _localized = localized;
            _textHint = textHint;
            _args = args;
        }

        public static LocalizedString TextOrDefault(string text, LocalizedString defaultValue) {
            if (string.IsNullOrEmpty(text))
                return defaultValue;
            return new LocalizedString(text);
        }

        public string TextHint {
            get { return _textHint; }
        }

        public string Text {
            get { return _localized; }
        }

        public override string ToString() {
            return _localized;
        }

        public override int GetHashCode() {
            var hashCode = 0;
            if (_localized != null)
                hashCode ^= _localized.GetHashCode();
            return hashCode;
        }

        public override bool Equals(object obj) {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var that = (LocalizedString)obj;
            return string.Equals(_localized, that._localized);
        }

        public override object InitializeLifetimeService() {
            return null;
        }
    }
}
