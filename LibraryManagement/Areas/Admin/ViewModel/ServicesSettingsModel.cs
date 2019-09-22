using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Areas.Admin.ViewModel
{
    public class ServicesSettingsModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TitleAR { get; set; }
        public string SubTitleHome { get; set; }
        public string SubTitleHomeAR { get; set; }
        public string Description { get; set; }
        public string DescriptionAR { get; set; }
        public string Image { get; set; }
    }
}
