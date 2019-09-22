using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Seagull.Admin.GridBuild
{
    public class GridSettings
    {
        public bool CanPrint { get; set; }
        public bool _search { get; set; }
        public int rows { get; set; }
        public int page { get; set; }
        public string sidx { get; set; }
        public string sord { get; set; }
    }
}