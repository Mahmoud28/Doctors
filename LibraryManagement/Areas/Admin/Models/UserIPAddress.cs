using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Areas.Admin.Models
{
    public class UserIPAddress
    {

        public int Id { get; set; }
        public int ShowId { get; set; }
        public string UserId { get; set; }
        public string IPAddress { get; set; }
    }
}
