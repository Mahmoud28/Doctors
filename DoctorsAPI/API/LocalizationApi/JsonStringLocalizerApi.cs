using Seagull.Core.Helper.Localization;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Seagull.API.LocalizationApi
{
    public class JsonStringLocalizerApi : IStringLocalizer
    {
        List<JsonLocalizationApi> localizationApi = new List<JsonLocalizationApi>();
        public string HeaderLanguage { get; set; }
        public JsonStringLocalizerApi()
        {
            JsonLocalizationApi serializer = new JsonLocalizationApi();
            //read all json file
            localizationApi = JsonConvert.DeserializeObject<List<JsonLocalizationApi>>(File.ReadAllText("LocalizationApi/localizationApi.json"));
        }
        public LocalizedString this[string name]
        {
            get
            {
                var value = GetString(name);
                return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
            }
        }

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var format = GetString(name);
                var value = string.Format(format ?? name, arguments);
                return new LocalizedString(name, value, resourceNotFound: format == null);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            throw new NotImplementedException();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            throw new NotImplementedException();
        }



        public string GetString(string name)
        {
            var query = localizationApi.Where(l => l.LocalizedValue.Keys.Any(lv => lv == HeaderLanguage));
            var value = query.FirstOrDefault(l => l.Key.ToLower() == name.ToLower());
            return value == null ? name : value.LocalizedValue[HeaderLanguage];
        }
    }
}
