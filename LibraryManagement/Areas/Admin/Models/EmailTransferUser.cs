using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Areas.Admin.Models
{
    public class EmailTransferUser
    {
        public int Id { get; set; }
        public int? SubscriptionId { get; set; }
        public bool? EmailStatus { get; set; }
        public DateTime? Date { get; set; }
        public int? EmailId { get; set; }

    }
}
