using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.API.APIModels
{
    public class EventModel/* : APIBaseModel*/
    {
        //public int Id { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int NumberOfViews { get; set; }
        public string Image { get; set; }
        public string Details { get; set; }

    }

    public class EventsAPIViewModel/* : APIBaseModel*/
    {
        //public int Id { get; set; }
        public List<EventModel> listOfShows { get; set; }

        public bool HasMore { get; set; }

    }

}
