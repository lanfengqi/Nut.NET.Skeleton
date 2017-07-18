using Foundatio.Caching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Foundatio.Skeleton.Domain.Localization.Services {
    public class LocalizedStringManager : ILocalizedStringManager {

        protected readonly ICacheClient _cacheClient;

        const string LocalizaionFilePathFormat = "~/App_Data/Localizaion/{0}/";

        public LocalizedStringManager(ICacheClient cacheClient) {
            _cacheClient = cacheClient;
        }


        public async Task<string> GetLocalizedString(string text, string cultureName) {

            var culture = await LoadCulture(cultureName);
            string key = text.ToLower();
            if (culture.Translations.ContainsKey(key)) {
                return culture.Translations[key];
            }

            return text;
        }

        private async Task<CultureDictionary> LoadCulture(string cultureName) {

            string cacheKey = string.Format("Culture.{0}", cultureName);

            var isExist = await _cacheClient.ExistsAsync(cacheKey);
            if (isExist) {
                var cachedValue = await _cacheClient.GetAsync<CultureDictionary>(cacheKey);
                return cachedValue.Value;
            }

            var result = new CultureDictionary {
                CultureName = cultureName,
                Translations = LoadTranslationsForCulture(cultureName)
            };

            await _cacheClient.SetAsync(cacheKey, result, TimeSpan.FromMinutes(60));

            return result;
        }

        private IDictionary<string, string> LoadTranslationsForCulture(string cultureName) {
            IDictionary<string, string> translations = new Dictionary<string, string>();

            var fliePath = string.Format(LocalizaionFilePathFormat, cultureName);
            var languageResourcePath = HttpContext.Current.Server.MapPath(fliePath);

            if (!Directory.Exists(languageResourcePath))
                Directory.CreateDirectory(languageResourcePath);

            DirectoryInfo folder = new DirectoryInfo(languageResourcePath);
            var resourceXmlFiles = folder.GetFiles("*.res.xml");

            if (resourceXmlFiles == null || !resourceXmlFiles.Any())
                return translations;

            foreach (var resourceXmlFile in resourceXmlFiles) {
                try {
                    string xmlContext = File.ReadAllText(resourceXmlFile.FullName);
                    if (String.IsNullOrEmpty(xmlContext))
                        break;

                    //stored procedures aren't supported
                    var xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(xmlContext);

                    var nodes = xmlDoc.SelectNodes(@"//Language/LocaleResource");
                    foreach (XmlNode node in nodes) {
                        string name = node.Attributes["Name"].InnerText.ToLowerInvariant().Trim();
                        string value = "";
                        var valueNode = node.SelectSingleNode("Value");
                        if (valueNode != null)
                            value = valueNode.InnerText;

                        if (String.IsNullOrEmpty(name))
                            continue;

                        if (!translations.ContainsKey(name))
                            translations.Add(name, value);

                    }
                } catch {

                }

            }

            return translations;

        }

        class CultureDictionary {
            public string CultureName { get; set; }

            public IDictionary<string, string> Translations { get; set; }
        }
    }
}
