using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Balanta.API.APIModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Seagull.API.APIHelper;
using Seagull.Core.Data;
using Seagull.Core.Data.Model;
using Seagull.Core.Models;
using Seagull.Doctors.Data.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace Doctors.API.Controllers
{
    [Route("Seagull")]
    public class PatientMedicationAPIController : ControllerBase
    {
        private LibraryDbContext _context;
        private readonly DbSet<MedicinesLookUp> _medicinesLookUp;
        private readonly DbSet<User> _user;
        private readonly DbSet<Days> _days;

        private readonly DbSet<Medication> _medication;
        public PatientMedicationAPIController(LibraryDbContext context)
        {
            _context = context;
            _user = _context.Set<User>();
            _medication = _context.Set<Medication>();
            _days = _context.Set<Days>();
            _medicinesLookUp = _context.Set<MedicinesLookUp>();
        }
        [HttpPost("PatientMedicationAddOrEdit")]
        public ActionResult PatientMedicationAddOrEdit([FromBody] PatientMedicationModel model)
        {
            APIJsonResult result = new APIJsonResult();
            result.Access = true;
            result.success = true;
            if (model == null)
            {
                result.success = false;
                result.Msg.Add("Model is NULL");
                return Ok(result);
            }
            var currentUserToken = User.Claims.SingleOrDefault(x => x.Type == "UserRole") != null ? User.Claims.SingleOrDefault(x => x.Type == "UserRole").Value : null;
            if (currentUserToken == null)
            {
                result.Msg.Add("Session Time Out");
                result.success = false;
                return Ok(result);
            }
            if (model.Id == 0)
            {
                Medication m = new Medication()
                {
                    AfternoonCount = model.Afternoon,
                    Doctors = model.DoctorName,
                    MorningCount = model.Morning,
                    PharmacyName = model.PharmacyName,
                    MedicationId = model.Medication,
                    Sideeffect = model.Sideeffect,
                    UserId = Convert.ToInt32(User.Claims.SingleOrDefault(x => x.Type == "UserId") != null ? User.Claims.SingleOrDefault(x => x.Type == "UserId").Value : null)
                };
                m.Days = string.Join(",", model.SelectedIds);
                _medication.Add(m);
                _context.SaveChanges();
            }
            else
            {
                var patientMedication = _medication.Where(x => x.Id == model.Id).FirstOrDefault();
                if (patientMedication == null)
                {
                    result.Msg.Add("Not found");
                    result.success = false;
                    return Ok(result);
                }
                patientMedication.MorningCount = model.Morning;
                patientMedication.MedicationId = model.Medication;
                patientMedication.PharmacyName = model.PharmacyName;
                patientMedication.Sideeffect = model.Sideeffect;
                patientMedication.UserId = Convert.ToInt32(User.Claims.SingleOrDefault(x => x.Type == "UserId") != null ? User.Claims.SingleOrDefault(x => x.Type == "UserId").Value : null);
                patientMedication.Days = string.Join(",", model.SelectedIds);
                _medication.Update(patientMedication);
                _context.SaveChanges();
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
                return Ok(result);
            }
            if (Id == 0)
            {
                result.Msg.Add("Not found");
                result.success = false;
                return Ok(result);
            }
            var medication = _medication.Where(x => x.Id == Id).FirstOrDefault();
            PatientMedicationModel model = new PatientMedicationModel()
            {
                Id = medication.Id,
                Afternoon = medication.AfternoonCount,
                DoctorName = medication.Doctors,
                Morning = medication.MorningCount,
                SelectedIds = medication.Days.Split(',').Select(Int32.Parse).ToList(),
                PharmacyName = medication.PharmacyName,
                Sideeffect = medication.Sideeffect,
                Medication = (medication.MedicationId)
            };
            result.data = model;
            return Ok(result);
        }
        [HttpGet("PatientMedicationList")]
        public IActionResult PatientMedicationList()
        {
            APIJsonResult result = new APIJsonResult();
            result.Access = true;
            result.success = true;
            var currentUserToken = User.Claims.SingleOrDefault(x => x.Type == "UserRole") != null ? User.Claims.SingleOrDefault(x => x.Type == "UserRole").Value : null;
            if (currentUserToken == null)
            {
                result.Msg.Add("Session Time Out");
                result.success = false;
                return Ok(result);
            }
            int userId = Convert.ToInt32(User.Claims.SingleOrDefault(x => x.Type == "UserId") != null ? User.Claims.SingleOrDefault(x => x.Type == "UserId").Value : null);
            List<PatientMedicationListModel> list = new List<PatientMedicationListModel>();
            _medication.Where(x => x.UserId == userId).ToList().ForEach(x => {
                var ids = x.Days.Split(',').Select(Int32.Parse).ToList();
                StringBuilder str = new StringBuilder();
                ids.ForEach(v => {
                    var day = _days.Where(id => id.Id == v).FirstOrDefault();
                    if (day != null)
                    {
                        str.Append(day.Name + "-");
                    }
                });
                string daysList = str.ToString();
                
                PatientMedicationListModel model = new PatientMedicationListModel()
                {
                    Days = daysList,
                    Doctor = x.Doctors,
                    Medicines = x.Doctors,
                    Id = x.Id
                };
                list.Add(model);
            });
            result.data = list;
            return Ok(result);
        }
     }
}