using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull.Core.Helpers_Extensions.Helpers
{
    public class JsonResultHelper
    {
        public int Id { get; set; }
        public bool success { get; set; }
        public bool Access { get; set; }
        public List<string> Msg { get; set; }
        public Dictionary<string, object> FormErrors { get; set; }
        public object data { get; set; }
        public string url { get; set; }
        public JsonResultHelper()
        {
            Access = false;
            success = true;
            Msg = new List<string>();
            FormErrors = new Dictionary<string, object>();
            data = new object();
            url = string.Empty;
        }
    }
    public class JsonData
    {
        public dynamic model { get; set; }
        public bool continueEditing { get; set; }
        public bool savedraft { get; set; }
        public string token { get; set; }
        public string status { get; set; }
        public JsonData()
        {
           // model = new JObject();
        }
    }
    public class DeleteModel
    {
        public int Id { get; set; }
        public int token { get; set; }
    }
    public class JsonDataModel
    {
        public dynamic model { get; set; }
        public JsonDataModel()
        {
            // model = new JObject();
        }
    }
}