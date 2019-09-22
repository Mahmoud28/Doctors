using Seagull.Core.Helpers_Extensions.Helpers;
using Seagull.API.APIHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.API.APIHelper
{
    public class APIJsonResult : JsonAPIResult
    {
        public APIJsonResult()
        {
            token = string.Empty;
        }
        public string token { get; set; }
    }
} 
