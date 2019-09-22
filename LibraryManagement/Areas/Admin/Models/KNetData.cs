using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Areas.Admin.Models
{
    public class KNetData
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string PaymentId { get; set; }
        public string TrackId { get; set; }
        public string RawResponse { get; set; }
        public string StatusKnet { get; set; }
        public int? Status { get; set; }
    }
}
