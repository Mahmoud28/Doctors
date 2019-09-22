
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Seagull.Core.Helpers_Extensions.Helpers;
using Seagull.Helpers.WhereOperation;
using Seagull.Core.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Seagull.Core.Areas.Admin.Controllers.LocalizationController;

namespace Seagull.Core.Helper.Localization
{
    public class JsonStringLocalizer : IStringLocalizer, ILocalizationRepository
    {
        List<JsonLocalization> localization = new List<JsonLocalization>();
        public JsonStringLocalizer()
        {
            JsonSerializer serializer = new JsonSerializer();
            //read all json file
            localization = JsonConvert.DeserializeObject<List<JsonLocalization>>(File.ReadAllText(@"Helper/Localization/localization.json"));
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
            return localization.Where(l => l.LocalizedValue.Keys.Any(lv => lv == CultureInfo.CurrentCulture.Name)).Select(l => new LocalizedString(l.Key, l.LocalizedValue[CultureInfo.CurrentCulture.Name], true));
        }
        public List<JsonLocalization> GetAll()
        {
            return localization;
        }

        public List<JsonLocalization> GetAll(pagination pagination, sort sort, string search, string search_operator, string filter)
        {
            dynamic searchFilter = string.Empty;
            var operater = string.IsNullOrEmpty(search_operator) ? null : JObject.Parse(search_operator);
            var templocalization = localization.AsEnumerable();
            if (!string.IsNullOrEmpty(search) && search.Length > 2)
            {
                searchFilter = JObject.Parse(search);
                foreach (var _search in searchFilter)
                {
                    if (!object.ReferenceEquals(null, _search.Value) && !string.IsNullOrEmpty(_search.Value.ToString()))
                    {
                        switch ((string)_search.Name)
                        {
                            case "Resource":
                                templocalization = templocalization.Where(a => a.Key.Contains(_search.Value.ToString()));
                                break;
                            //case "Arabic":
                            //    templocalization = templocalization.//Where(a => a.LocalizedValue.Contains(_search.Value.ToString()));//Contains(_search.Value.ToString())

                            //    break;
                            //case "English":
                            //    break;

                        }
                    }
                }
            }

            return templocalization.ToList();
        }
        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return new JsonStringLocalizer();
        }

        public string GetString(string name)
        {
            var query = localization.Where(l => l.LocalizedValue.Keys.Any(lv => lv == Thread.CurrentThread.CurrentCulture.Name));
            var value = query.FirstOrDefault(l => l.Key.ToLower() == name.ToLower());
            return value == null ? name : value.LocalizedValue[CultureInfo.CurrentCulture.Name];
        }
        public void AddOrUpdateLocalize(CustomLocalizeModel entity)
        {
            string json = File.ReadAllText(@"Helper/Localization/localization.json");
            bool isFound = localization.Where(s => s.Key.ToLower().Contains(entity.Resource.ToLower())).Count() > 0;
            if (isFound)
                localization.Remove(localization.Where(s => s.Key.ToLower().Contains(entity.Resource.ToLower())).FirstOrDefault());

            localization.Add(new JsonLocalization()
            {
                LocalizedValue = new Dictionary<string, string>()
                {
                    { "ar-AE", entity.Arabic} , { "en-US", entity.English},
                },
                Key = entity.Resource
            });
            string output = JsonConvert.SerializeObject(localization, Formatting.Indented);
            File.WriteAllText(@"Helper/Localization/localization.json", output);
        }
    }
}
