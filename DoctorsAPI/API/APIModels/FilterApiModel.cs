using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.API.APIModels
{
    public class FilterApiModel
    {
        public int? MaxPrice { get; set; }
        public int? MinPrice { get; set; }

        public int? MaxAttendance { get; set; }
        public int? MinAttendance { get; set; }

        public bool Male { get; set; }
        public bool Female { get; set; }
        public bool Both { get; set; }
    }
}
