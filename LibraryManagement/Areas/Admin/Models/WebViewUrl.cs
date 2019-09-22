using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Areas.Admin.Models
{
    public class WebViewUrl
    {
        public int Id { get; set; }
        public int? TimeId { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public int ShowId { get; set; }
        public int? UserId { get; set; }
        public bool? IsGustUser { get; set; }
        public int? DeviceId { get; set; }
        public int? OrderId { get; set; }
    }
}
