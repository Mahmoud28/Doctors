using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Seagull.API.APIHelper;
using Seagull.Core.Data;
using Seagull.Doctors.Data.Model;

namespace Doctors.API.Controllers
{
    [Route("Seagull")]
    public class ScheduleAppointmentAPIController : ControllerBase
    {
        private LibraryDbContext _context;
        public ScheduleAppointmentAPIController(LibraryDbContext context)
        {
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
                result.Access = false;
                return Ok(result);
            }
            if (model == null)
            {
                result.Msg.Add("Model Null");
                result.success = false;
                result.Access = false;
                return Ok(result);
            }

            if(model.Id == 0)
            {
                ScheduleAppointment scheduleAppointment = new ScheduleAppointment()
                {
                    //Status = model.s
                    Notes = model.Notes,
                };
            }
            else
            {

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