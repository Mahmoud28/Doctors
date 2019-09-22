using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.API.APIModels
{
    public class SearchModel
    {
        public string showType { get; set; }
        public string showName { get; set; }
        public int PageNumber { get; set; }
        public int Count { get; set; }
    }
}
