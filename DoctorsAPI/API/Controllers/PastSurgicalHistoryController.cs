using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Balanta.API.APIModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Seagull.API.APIHelper;
using Seagull.Core.Data;
using Seagull.Core.Helper.ExceptionLog;
using Seagull.Doctors.Data;
using Seagull.Doctors.Data.Model;
using Seagull.Doctors.Helper.ImageHelper;

namespace Doctors.API.Controllers
{
    [Route("Seagull")]
    public class PastSurgicalHistoryController : ControllerBase
    {
        private LibraryDbContext _context;
        private readonly DbSet<PastSurgicalHistory> _PastSurgicalHistory;
        private readonly DbSet<PastSurgicalHistoryImage> _PastSurgicalHistoryImage;
        public Paths _paths = new Paths();
        public static IConfiguration Configuration { get; set; }

        public PastSurgicalHistoryController(LibraryDbContext context)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            _context = context;
            _PastSurgicalHistory = _context.Set<PastSurgicalHistory>();
            _PastSurgicalHistoryImage = _context.Set<PastSurgicalHistoryImage>();
            Configuration = builder.Build();
        }
        public string AcceptLanguage
        {
            get
            {
                return Request.Headers["Accept-Language"].ToString();
            }
        }
        [HttpGet("GetAllPastSurgicalHistory")]
        public ActionResult GetAllPastSurgicalHistory()
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

            result.data = (from a in _PastSurgicalHistory.Where(a => a.UserId == UserId)
                           select new
                           {
                               a.Id,
                               a.Name,
                               SurgicalDate = PublicFunctions.ConvertToTimestamp(a.SurgicalDate),
                           }).ToList();
            //var data = _allergyRepository.GetAllByUserId(UserId);
            //result.data = data;
            return Ok(result);
        }

        [HttpGet("GetSurgicalHistoryById")]
        public ActionResult GetSurgicalHistoryById([FromBody] int Id)
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

            result.data = (from a in _PastSurgicalHistory.Where(a => a.UserId == UserId && a.Id == Id)
                           select new
                           {
                               a.Id,
                               a.Name,
                               SurgicalDate = PublicFunctions.ConvertToTimestamp(a.SurgicalDate),
                               a.UserId,
                               Images = (from image in _PastSurgicalHistoryImage.Where(f => f.SurgicalId == a.Id).ToList()
                                         select new
                                         {
                                             image = Configuration["Doctors:Url"] + "/" + "Upload/SurgicalHistory/" + UserId + "/" + image.Image,
                                         }).ToList()
                           }).FirstOrDefault();

            return Ok(result);
        }

        [HttpPost("UpdateSurgicalHistory")]
        [Produces("application/json")]
        [Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
        public ActionResult UpdateSurgicalHistory(SurgicalHistoryViewModel model)
        {
            var userId = User.Claims.SingleOrDefault(x => x.Type == "UserId") != null ? User.Claims.SingleOrDefault(x => x.Type == "UserId").Value : null;
            int UserId = 0;
            Int32.TryParse(userId, out UserId);
            //new ExceptionLog().WriteException(JsonConvert.SerializeObject(model).ToString());
            APIJsonResult result = new APIJsonResult();
            
            if (UserId == 0 || model == null)
            {
                result.success = false;
                result.Msg.Add("NoUser");
                return Ok(result);
            }
            try
            {
                if (model.Id < 1)
                {
                    //add mode 
                    PastSurgicalHistory entity = new PastSurgicalHistory();
                    entity.Name = model.Name;
                    entity.SurgicalDate = PublicFunctions.ConvertTimestampToDateTime(model.SurgicalDate);
                    entity.UserId = UserId;
                    entity.CreatedBy = UserId;
                    entity.CreatedDate = DateTime.Now;
                    _PastSurgicalHistory.Add(entity);
                    _context.SaveChanges();
                    if (model.Images != null && model.Images.Count() > 0)
                    {
                        model.Images.ToList().ForEach(a =>
                        {
                            var CoverImagetPath = _paths.SurgicalHistoryImages + UserId + "/";
                            if (!Directory.Exists(CoverImagetPath))
                            {
                                Directory.CreateDirectory(CoverImagetPath);
                            }
                            using (var fileStream = new FileStream(CoverImagetPath + a.FileName, FileMode.Create))
                            {
                                a.CopyToAsync(fileStream);
                            }
                            PastSurgicalHistoryImage images = new PastSurgicalHistoryImage();
                            images.Image = a.FileName;
                            images.SurgicalId = entity.Id;
                            _PastSurgicalHistoryImage.Add(images);
                            _context.SaveChanges();
                        });

                    }
                }
                else
                {
                    PastSurgicalHistory entity = _PastSurgicalHistory.Where(a => a.Id == model.Id).FirstOrDefault();
                    entity.Name = model.Name;
                    entity.SurgicalDate = PublicFunctions.ConvertTimestampToDateTime(model.SurgicalDate);
                    entity.UpdateBy = UserId;
                    entity.UserId = UserId;
                    entity.UpdateDate = DateTime.Now;
                    _PastSurgicalHistory.Update(entity);
                    _context.SaveChanges();

                    //Delete the Images
                    var oldImages = _PastSurgicalHistoryImage.Where(a => a.SurgicalId == model.Id).ToList();
                    if (oldImages != null && oldImages.Count() > 0)
                    {
                        oldImages.ForEach(a =>
                        {
                            _PastSurgicalHistoryImage.Remove(a);
                            _context.SaveChanges();
                        });
                    }
                    //Add new Images
                    if (model.Images != null && model.Images.Count() > 0)
                    {
                        model.Images.ToList().ForEach(a =>
                        {
                            var CoverImagetPath = _paths.SurgicalHistoryImages + UserId + "/";
                            if (!Directory.Exists(CoverImagetPath))
                            {
                                Directory.CreateDirectory(CoverImagetPath);
                            }
                            using (var fileStream = new FileStream(CoverImagetPath + a.FileName, FileMode.Create))
                            {
                                a.CopyToAsync(fileStream);
                            }
                            PastSurgicalHistoryImage images = new PastSurgicalHistoryImage();
                            images.Image = a.FileName;
                            images.SurgicalId = entity.Id;
                            _PastSurgicalHistoryImage.Add(images);
                            _context.SaveChanges();
                        });

                    }
                }
            }
            catch(Exception e)
            {
                result.data = e.Message;
            }
           
            return Ok(result);
        }
    }
}