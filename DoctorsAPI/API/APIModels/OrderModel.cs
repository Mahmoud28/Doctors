using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.API.APIModels
{
    public class OrderModel
    {
        public bool HasMore { get; set; }
        public List<OrderBooking> listOfBooking { get; set; }
    }
    public class OrderBooking
    {
        public string ImagePath { get; set; }

        public String showName { get; set; }
        public string QrCodeImage { get; set; }

    }
}
