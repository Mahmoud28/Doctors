using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.API.APIModels
{
    public class OrderAPIViewModel
    {

        public int ShowId { get; set; }
        public long Date { get; set; }
        //public int TheaterId { get; set; }
        //public string UserId { get; set; }
        public string Time { get; set; }
        public int SeatNumber { get; set; }
    }
}
