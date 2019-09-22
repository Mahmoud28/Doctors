using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Data.Model
{
    public class GustUserDevice
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string JwtToken { get; set; }
        public string FCMToken { get; set; }
    }
}
