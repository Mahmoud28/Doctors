using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Models
{
    public class OrderTiket
    {
        public int? TimeId { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public int ShowId { get; set; }
        public int SeatNumber { get; set; }
        public int AvialableTicketNumber { get; set; }
        public bool IsMobileOrder { get; set; }
        public int? WebViewId { get; set; }


    }
}
