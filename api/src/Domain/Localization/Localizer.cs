using System.Threading.Tasks;

namespace Foundatio.Skeleton.Domain.Localization {
    /// <summary>
    /// localizes some text basedon the current work context culture
    /// </summary>
    /// <param name="text"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public delegate Task<LocalizedString> Localizer(string text, params object[] args);

}
