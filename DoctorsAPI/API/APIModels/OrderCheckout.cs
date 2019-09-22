using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.API.APIModels
{
    public class OrderCheckout
    {
        public int OrderId { get; set; }
        public int ShowId { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string mobile { get; set; }
        public int chickPromoCode { get; set; }
        public decimal price { get; set; }
        public string promoCode { get; set; }
        public int promoCodeId { get; set; }
    }
}
