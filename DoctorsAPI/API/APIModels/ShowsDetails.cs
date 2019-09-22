using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.API.APIModels
{
    public class ShowsDetails
    {
        public int Id { get; set; }
        public int NumberOfViews { get; set; }
        public string Image { get; set; }
        public int AvailableTicket { get; set; }
        public string ShowName { set; get; }
        public List<string> CategoryNames { set; get; }

        //public string MinDate { set; get; }
        //public string MaxDate { set; get; }
        public long MinDate { set; get; }
        public long MaxDate { set; get; }
        public string MinTime { set; get; }
        public string MaxTime { set; get; }
        public List<DateAndShowTime> DatesShows { set; get; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string VideoID { set; get; }

        public int Type { set; get; }
        public string PlayDetails { set; get; }

        public string PlayInstruction { set; get; }
        public string PlayCastInformation { set; get; }
        public string InstructionImage { get; set; }
        //public List<string> TimesOfShow { set; get; }
    }
    public class DateAndShowTime
    {
        public int Id { set; get; }
        //public string Date { set; get; }
        public long Date { set; get; }
        //public string Time { set; get; }

        //public List<string> Times { set; get; }
        //aya added 
        public List<TimesIds> Times { set; get; }
    }
    public class TimesIds
    {
        public int Id { set; get; }
        //public string Date { set; get; }
        public string Time { set; get; }
        //public string Time { set; get; }
    }
    public class DatesTimesOfShow
    {
        public int Id { set; get; }
        public List<long> Dates { set; get; }
        public List<string> Times { set; get; }
    }
}
