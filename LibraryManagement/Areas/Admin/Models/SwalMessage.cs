using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Areas.Admin.Models
{
    public class SwalMessage
    {
        public int Id { get; set; }
        public string Header { get; set; }
        public string Message { get; set; }
        public string SwalType { get; set; }
        public bool MessageType { get; set; }
        public string Url { get; set; }
        public string Data { get; set; }
    }
}
