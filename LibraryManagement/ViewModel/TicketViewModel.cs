using Seagull.Doctors.Areas.Admin.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.ViewModel
{
    public class TicketViewModel
    {
        public TicketViewModel()
        {

        }
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int SeatNumber { get; set; }
        public string Package { get; set; }



    }

}
