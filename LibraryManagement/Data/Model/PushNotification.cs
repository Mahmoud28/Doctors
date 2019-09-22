using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Data.Model
{
    public class PushNotification
    {
        public int Id { get; set; }
        public string NotificationMessage { get; set; }
        public int? UsersCount { get; set; }
        public DateTime? SendDate { get; set; }

        public bool? IsDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
    public class UsersNotification
    {
        public int Id { get; set; }
        public int? PushNotificationId { get; set; }
        public int? UsersId { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
