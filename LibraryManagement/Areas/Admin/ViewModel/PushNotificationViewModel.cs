using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Areas.Admin.ViewModel
{
    public class PushNotificationViewModel
    {
        public int Id { get; set; }
        public string NotificationMessage { get; set; }
        public int? UsersCount { get; set; }
        public DateTime? SendDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        //
        public object Child { get; set; }
        public string strUser { get; set; }
    }
}
