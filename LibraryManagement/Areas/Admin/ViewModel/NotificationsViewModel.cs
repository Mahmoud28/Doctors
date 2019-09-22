using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Areas.Admin.Models
{
    public class NotificationsViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string URL { get; set; }
        public int Type { get; set; }
        public string ProgressId { get; set; }
        //just for notification
        public bool? IsRead { get; set; }
        public DateTime Date { get; set; }
    }
}
