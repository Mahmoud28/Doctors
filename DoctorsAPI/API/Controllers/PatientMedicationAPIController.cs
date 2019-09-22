using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seagull.API.APIHelper;
using Seagull.Core.Data;
using Seagull.Core.Data.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace Doctors.API.Controllers
{
    [Route("PatientMedication")]
    public class PatientMedicationAPIController : ControllerBase
    {
        private LibraryDbContext _context;
        private readonly DbSet<Medication> _medication;
        public PatientMedicationAPIController(LibraryDbContext context)
        {
            _context = context;
            _medication = _context.Set<Medication>();
        }
        [HttpPost("PatientMedication")]
        [SwaggerOperation(Summary = "Policy Api ", Description = "To Get Privacy & Policy Data Like Title and Description")]
        public ActionResult PatientMedicationData([FromBody] PatientMedicationModel model)
        {
            APIJsonResult result = new APIJsonResult();
            result.Access = true;
            result.success = true;
            if(model == null)
            {
                result.success = false;
                result.Msg.Add("Model is NULL");
            }
            var currentUserToken = User.Claims.SingleOrDefault(x => x.Type == "UserRole") != null ? User.Claims.SingleOrDefault(x => x.Type == "UserRole").Value : null;
            if (currentUserToken == null)
            {
                result.Msg.Add("Session Time Out");
                result.success = false;
                result.Access = false;
                return Ok(result);
            }
            if(model.Id == 0)
            {
                Medication m = new Medication()
                {
                    AfternoonCount = model.Afternoon,
                    Doctors = model.DoctorName,
                    MorningCount = model.Morning,
                    PharmacyName = model.PharmacyName,
                    Name = model.Medication.ToString(),
                    Sideeffect = model.Sideeffect,
                    UserId = Convert.ToInt32(User.Claims.SingleOrDefault(x => x.Type == "UserId") != null ? User.Claims.SingleOrDefault(x => x.Type == "UserId").Value : null)
                };
                m.Days = string.Join(",", model.SelectedIds);
                _medication.Add(m);
                _context.SaveChanges();
            }
            else
            {

            }
           
            return Ok(result);
        }
        [HttpGet("PatientMedicationGet")]
        public IActionResult PatientMedicationGet(int Id)
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
            if(Id == 0)
            {
                result.Msg.Add("Not found");
                result.success = false;
                result.Access = false;
                return Ok(result);
            }
           var medication =_medication.Where(x => x.Id == Id).FirstOrDefault();
            return null;
        }
    }
    

    public class PatientMedicationModel
    {
        public int Id { get; set; }
        public int Medication { get; set; }
        public string DoctorName { get; set; }
        public int Morning { get; set; }
        public int Afternoon { get; set; }
        public string PharmacyName { get; set; }
        public string Sideeffect { get; set; }
        public List<int> SelectedIds { get; set; }
    }

}