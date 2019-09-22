using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Data.Model
{
    public class SeatsOrder
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public string PackagName { get; set; }
        public int PackagId { get; set; }
        public int? SeatId { get; set; }
        public string SeatNumber { get; set; }
        public string SeatName { get; set; }
        public bool? Confirmed { get; set; }
    }
}
