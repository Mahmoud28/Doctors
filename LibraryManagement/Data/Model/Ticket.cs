using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Data.Model
{
    public class Ticket
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int SeatNumber { get; set; }
        public string Package { get; set; }

    }
}
