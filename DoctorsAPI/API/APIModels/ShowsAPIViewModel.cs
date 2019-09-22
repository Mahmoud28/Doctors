using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.API.APIModels
{
    public class ShowsAPIViewModel/* : APIBaseModel*/
    {
        //public int Id { get; set; }
        
        public int ShowId { get; set; }
        public string Name { get; set; }
        public int  NumberOfViews { get; set; }
        public string Image { get; set; }
        public int  Type { get; set; }

       // public string ShowName { get; set; }
    }

    public  class PlaysAndViewsNumber
    {
        public int NumberOfPlays { get; set; }
        public int NumberOfEvents { get; set; }
        public bool HasMore { get; set; }
        public List<ShowsAPIViewModel> showsList { get; set; }
    }
}
