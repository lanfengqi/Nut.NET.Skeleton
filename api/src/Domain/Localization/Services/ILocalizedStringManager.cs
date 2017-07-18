using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Localization.Services {
    public interface ILocalizedStringManager {

        Task<string> GetLocalizedString(string text, string cultureName);
    }
}
