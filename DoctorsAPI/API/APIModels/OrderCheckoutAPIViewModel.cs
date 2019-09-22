using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seagull.API.APIModels
{
    public class OrderCheckoutAPIViewModel
    {
        public OrderCheckoutAPIViewModel()
        {
            //CheckoutModel = new List<CheckoutModel>();
        }
        public int OrderId { get; set; }
        public decimal Price { get; set; }
        public int ShowId { get; set; }
        public string ShowName { get; set; }
        public string ShowImage { get; set; }
        public string SeatName{ get; set; }
        public string PackageName { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }

        public int NumberOfTicket { get; set; }
        //public List<CheckoutModel> CheckoutModel { get; set; }
    }

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

}
