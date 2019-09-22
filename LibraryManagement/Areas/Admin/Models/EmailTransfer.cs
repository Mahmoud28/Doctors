using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Areas.Admin.Models
{
    public class EmailTransfer
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime? SendDate { get; set; }
        public int? UserCounts { get; set; }
    }
}
