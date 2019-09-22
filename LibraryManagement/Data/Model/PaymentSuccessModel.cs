using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Data.Model
{
    public class PaymentSuccessModel
    {
        public int OrderId { get; set; }
        public string PlayName { get; set; }
        public string OrderDate { get; set; }
        public string OrderTime { get; set; }
        public string PlayImage { get; set; }
        public string QrCodeImage { get; set; }
        public decimal Price { get; set; }
        public string TrackId { get; set; }
        public string PaymentId { get; set; }
        public string ShowLocation { get; set; }

        public string Seatsbooked { get; set; }
        public int SeatsQuantity { get; set; }
    }
}
