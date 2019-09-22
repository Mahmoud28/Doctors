using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Data.Model
{
    public class PromoCodes
    {
        public int Id { get; set; }
        public string PromoCode { get; set;}
        public int? PromoCodeTypeId { get; set; }
        public decimal Value { get; set; }
        public DateTime? CretedDate { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdateBy { get; set; }
        public int? DeletedBy { get; set; }
        public bool? IsDeleted { get; set; }
        public bool Status { get; set; }
    }

    public class PromoCodeType
    {
        public int Id { get; set; }
        public string NameAr { get; set; }
        public string NameEn { get; set; }
    }
}
