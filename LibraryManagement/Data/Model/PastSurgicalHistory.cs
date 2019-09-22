using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Data.Model
{
    public class PastSurgicalHistory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime SurgicalDate { get; set; }
        public int UserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
