using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Models
{
    public class ServicesWebModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TitleAR { get; set; }
        public string Icon { get; set; }
        public string ShortDesciption { get; set; }
        public string ShortDesciptionAR { get; set; }
        public string FullDescription { get; set; }
        public string FullDescriptionAR { get; set; }
        public string Image { get; set; }
        public int SortOrder { get; set; }
    }
}
