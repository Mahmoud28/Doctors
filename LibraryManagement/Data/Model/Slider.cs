using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.Data.Model
{
    public class Slider
    {
        public int Id { get;set; }
        public string Title { get; set; }
        public string TitleAR { get; set; }
        public string Description { get; set; }
        public string DescriptionAR { get; set; }
        public string Image { get; set; }
        public string ImageName { get; set; }
        public string Link { get; set; }
        public int? SortOrder { get; set; }
        public bool? Status { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdateBy { get; set; }
        public int? DeletedBy { get; set; }
        public bool? IsDeleted { get; set; }
        public string Type { get; set; }
    }
}
