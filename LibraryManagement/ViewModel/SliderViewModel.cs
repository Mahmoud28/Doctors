
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Core.ViewModel
{
    public class SliderViewModel
    {
       public SliderViewModel()
        {
            SliderImage = new Image();
            NewImage = new ImagePath();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string TitleAR { get; set; }
        public string Description { get; set; }
        public string DescriptionAR { get; set; }
        public string Image { get; set; }
        public string Link { get; set; }
        public int? SortOrder { get; set; }
        public bool? Status { get; set; }
        public string ImageName { get; set; }

        public int? CreatedBy { get; set; }
        public int? UpdateBy { get; set; }
        public int? DeletedBy { get; set; }
        public bool? IsDeleted { get; set; }
        //public Image ImageModel { get; set; }
        public string Type { get; set; }
        public string Token { get; set; }

        public Image SliderImage { get; set; }
        public ImagePath NewImage { get; set; }
    }
    public class Image
    {
        public int Id { get; set; }
        public string SRC { get; set; }
        public string Name { get; set; }
    }

    public class ImagePath
    {
        public string Name { get; set; }
    }
}
