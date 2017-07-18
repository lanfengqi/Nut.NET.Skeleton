using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Localization {
    public interface IText {
        Task<LocalizedString> Get(string textHint, params object[] args);
    }
}
