using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.Helper.Localization
{
    public class JsonLocalizationApi
    {
        public string Key { get; set; }
        public object Keys { get; internal set; }

        public Dictionary<string, string> LocalizedValue = new Dictionary<string, string>();

    }
}
