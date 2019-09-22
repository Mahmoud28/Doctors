using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Seagull.Doctors.Data.Model
{
    public class ScheduleAppointment
    {
        public int Id { get; set; }
        public DateTime? Appointment { get; set; }
        public int? UserId { get; set; }
        public int? Status { get; set; }
        public int? DoctorId { get; set; }
        public int? VisitType { get; set; }
        public string Notes { get; set; }
    }
}
