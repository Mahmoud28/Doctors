using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Data.Model
{
    public class PastSurgicalHistoryImage
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public int SurgicalId { get; set; }
    }
}
