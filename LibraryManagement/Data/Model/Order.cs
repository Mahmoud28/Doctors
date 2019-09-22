using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Data.Model
{
    public class Order
    {
        public int Id { get; set; }
        public int ShowId { get; set; }
        public int? TheaterId { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
        public string Time { get; set; }
        public decimal? Price { get; set; }
        public bool IsScanned { set; get; }
        public int? IsCancled { set; get; }
        public int? CancelledBy { set; get; }
        public int IsDone { set; get; }
        public bool? Paid { get; set; }
        public int? PromoCodeId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? OrderPaymintType { get; set; }
        public decimal? PromoCodeNewPrice { get; set; }
        public DateTime? CanceledDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public bool? IsDeleted { get; set; }
        public bool? PaymentTimeOut { get; set; }
        public string KnetPaymentId { get; set; }
        public string PaymentResult { get; set; }
        public int? PaymentStatus { get; set; }
        public string TrackId { get; set; }
        public bool? PaymentType { get; set; }
        public bool? IsMobileOrder { get; set; }
        public bool? OrderType { set; get; } // false online , true offline Ticketing user
    }
}
