using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.API.Controllers
{
    public class PagingModel
    {
        public int PageNumber { get; set; }
        public int Count { get; set; }
    }
}
