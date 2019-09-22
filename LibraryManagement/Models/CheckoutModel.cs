using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seagull.Doctors.Models
{
    public class CheckoutModel
    {
        public CheckoutModel()
        {
            SeatName = new StringBuilder();
        }
        public string Key { get; set; }
        public StringBuilder SeatName { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderCheckoutModel
    {
        public OrderCheckoutModel()
        {
            CheckoutModel = new List<CheckoutModel>();
        }
        public bool IsMobileOrder { get; set; }
        public int OrderId { get; set; }
        public decimal Price { get; set; }
        public int ShowId { get; set; }
        public List<CheckoutModel> CheckoutModel { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
    }
}
