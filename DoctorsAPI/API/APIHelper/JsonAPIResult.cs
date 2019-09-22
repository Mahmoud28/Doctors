using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.API.APIHelper
{
    public class JsonAPIResult
    {
        public int Id { get; set; }
        public bool success { get; set; }
        public bool Access { get; set; }
        public List<string> Msg { get; set; }
        public object data { get; set; }
        public string url { get; set; }
        public JsonAPIResult()
        {
            Access = false;
            success = true;
            Msg = new List<string>();
            data = new object();
            url = string.Empty;
        }
    }
}
