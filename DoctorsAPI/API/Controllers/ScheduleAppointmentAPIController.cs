using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seagull.API.APIHelper;
using Seagull.Core.Data;
using Seagull.Doctors.Data;
using Seagull.Doctors.Data.Model;

namespace Doctors.API.Controllers
{
    [Route("Seagull")]
    public class ScheduleAppointmentAPIController : ControllerBase
    {
        private LibraryDbContext _context;
        private readonly DbSet<ScheduleAppointment> _scheduleAppointment;
        public ScheduleAppointmentAPIController(LibraryDbContext context)
        {
            _scheduleAppointment= _context.Set<ScheduleAppointment>();
            _context = context;
        }
        [HttpPost("ScheduleAppointmentAddOrEdit")]
        public IActionResult ScheduleAppointmentAddOrEdit([FromBody] ScheduleAppointmentModel model)
        {
            APIJsonResult result = new APIJsonResult();
            result.Access = true;
            result.success = true;
            var currentUserToken = User.Claims.SingleOrDefault(x => x.Type == "UserRole") != null ? User.Claims.SingleOrDefault(x => x.Type == "UserRole").Value : null;
            if (currentUserToken == null)
            {
                result.Msg.Add("Session Time Out");
                result.success = false;
                result.Access = true;
                return Ok(result);
            }
            if (model == null)
            {
                result.Msg.Add("Model Null");
                result.success = false;
                result.Access = true;
                return Ok(result);
            }

            if(model.Id == 0)
            {
                ScheduleAppointment scheduleAppointment = new ScheduleAppointment()
                {
                    Notes = model.Notes,
                    UserId = Convert.ToInt32(User.Claims.SingleOrDefault(x => x.Type == "UserId") != null ? User.Claims.SingleOrDefault(x => x.Type == "UserId").Value : null),
                    VisitType = model.VisitType,
                    Appointment = PublicFunctions.ConvertTimestampToDateTime(model.Appointment),
                    DoctorId = model.DoctorId,
                    Status = 1,
                };
                _scheduleAppointment.Add(scheduleAppointment);
                _context.SaveChanges();
                
            }
            else
            {
                var scheduleAppointment = _scheduleAppointment.Where(x => x.Id == model.Id).FirstOrDefault();
                if(scheduleAppointment == null)
                {
                    result.Msg.Add("Not found");
                    result.success = false;
                    result.Access = true;
                    return Ok(result);
                }
                scheduleAppointment.Notes = model.Notes;
                scheduleAppointment.VisitType = model.VisitType;
                scheduleAppointment.Appointment = PublicFunctions.ConvertTimestampToDateTime(model.Appointment);
                scheduleAppointment.DoctorId = model.DoctorId;
                _scheduleAppointment.Update(scheduleAppointment);
                _context.SaveChanges();
            }
            return Ok(result);
        }
        public class ScheduleAppointmentModel
        {
            public int Id { get; set; }
            public long Appointment { get; set; }
            public int DoctorId { get; set; }
            public int VisitType { get; set; }
            public string Notes { get; set; }
        }
    }
}