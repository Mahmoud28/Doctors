using Seagull.Core.Helper.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Seagull.Core.Data;
using Seagull.Core.Helpers_Extensions.Helpers;
using Seagull.Core.Data.Interfaces;
using Seagull.API.APIHelper;
using Seagull.Core.Data.Model;
using Microsoft.EntityFrameworkCore;

namespace Seagull.API.Controllers
{
    [Route("Seagull")]
    public class AllergyAPIController : ControllerBase
    {
        public string AcceptLanguage
        {
            get
            {
                return Request.Headers["Accept-Language"].ToString();
            }
        }
        #region Fileds
        private LibraryDbContext _context;
        private readonly DbSet<Allergy> _allergy;
        private readonly DbSet<MedicationAllergyMap> _medicationAllergyMap;

        public AllergyAPIController(LibraryDbContext context)
        {
            _context = context;
            _allergy = _context.Set<Allergy>();
            _medicationAllergyMap = _context.Set<MedicationAllergyMap>();
        }
        #endregion

        [HttpGet("GetPatientAllergy")]
        public ActionResult GetAllAllergyByUserId()
        {
            APIJsonResult result = new APIJsonResult();
            var userId = User.Claims.SingleOrDefault(x => x.Type == "UserId") != null ? User.Claims.SingleOrDefault(x => x.Type == "UserId").Value : null;
            int UserId = 0;
            Int32.TryParse(userId, out UserId);
            if (UserId == 0)
            {
                result.success = false;
                result.Msg.Add("NoUser");
                return Ok(result);
            }
            result.success = true;
            result.Access = true;
            List<AllergyViewModel> queryAllergy = (from a in _context.Set<Allergy>().ToList()
                                                   select new AllergyViewModel
                                                   {
                                                       Id =  a.Id,
                                                       Name = a.Name,
                                                       Selected = _context.Set<MedicationAllergyMap>().Where(s => s.UserId == UserId && s.AllergyId == a.Id).Count() > 0
                                                   }).ToList();

            var data = queryAllergy;
            result.data = data;
            return Ok(result);
        }

        [HttpPost("UpdatePatientAllergy")]
        public ActionResult SaveAllergyByUserId([FromBody]List<int> AllergyListIds)
        {
            APIJsonResult result = new APIJsonResult();
            //if (AllergyListIds.Count() < 1)
            //{
            //    result.success = false;
            //    result.Msg.Add("PatientAllergyEmpty");
            //    return Ok(result);
            //}
            var userId = User.Claims.SingleOrDefault(x => x.Type == "UserId") != null ? User.Claims.SingleOrDefault(x => x.Type == "UserId").Value : null;
            int UserId = 0;
            Int32.TryParse(userId, out UserId);
            if (UserId ==  0)
            {
                result.success = false;
                result.Msg.Add("NoUser");
                return Ok(result);
            }
            result.success = true;
            result.Access = true;

            _medicationAllergyMap.Where(a => a.UserId == UserId).ToList().ForEach(a =>
            {
                _medicationAllergyMap.Remove(a);
                _context.SaveChanges();
            });
            if (AllergyListIds != null )
            {
                AllergyListIds.ForEach(a =>
                {
                    MedicationAllergyMap model = new MedicationAllergyMap();
                    model.UserId = UserId;
                    model.AllergyId = a;
                    _medicationAllergyMap.Add(model);
                    _context.SaveChanges();
                });
            }
            
            //_allergyRepository.SaveAllaergytList(UserId, AllergyListIds);
            return Ok(result);
        }

    }
}
