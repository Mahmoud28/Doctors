using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.API.APIModels
{
    public class CheckPromoCode
    {
        public int OrderId { get; set; }
        public string PromoCode { get; set; }
        //public int showId { get; set; }
    }
    public class PromoCodeModel
    {
        public int PromoCodeId { get; set; }
        public bool IsValid { get; set; }
        public decimal Price { get; set; }
        public string Message { get; set; }
    }
}
