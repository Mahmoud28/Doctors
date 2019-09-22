using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Seagull.Core.Data;
using Seagull.API.APIModels;
using Seagull.API.APIHelper;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using Seagull.API.APIModels;
using Seagull.API.APIHelper.Mapping;
using static Seagull.API.APIModels.UserAPI;
using Seagull.API.APIHelper.Authorization;
using Seagull.Core.Models;
using System.Net.Mail;
using System.Text;
using Seagull.API.LocalizationApi;
using System.Text.RegularExpressions;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.IO;
using ShortURLService.Models;
using System.Security.Cryptography;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting;
using Seagull.Core.Helper.StaticVariables;
using System.Security.Claims;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using Seagull.Doctors.Areas.Admin.Models;
using Seagull.Doctors.Data.Model;
using Seagull.Doctors.Helper.ImageHelper;
using Seagull.Core.Data.Model;

namespace Seagull.API.Controllers
{
    [Route("Seagull")]
    public class PublicApiController : ControllerBase
    {
        string filePath = FilePath._Path;
        public string AcceptLanguage
        {
            get
            {
                return Request.Headers["Accept-Language"].ToString();
            }
        }
        public Paths _paths = new Paths();
        private LibraryDbContext _context;
        private readonly DbSet<BloodType> _bloodType;
        JsonStringLocalizerApi _localizer;
        public static IConfiguration Configuration { get; set; }
        public IHostingEnvironment _appEnvironment;
        private static string keySecret { get; set; } = @"3048 0241 00C9 18FA CF8D EB2D EFD5 FD37 89B9 E069 EA97 FC20 5E35 F5
                                            77 EE31 C4FB C6E4 4811 7D86 BC8F BAFA 362F 922B F01B 2F40 C744 2654 
                                            C0DD 2881 D673 CA2B 4003 C266 E2CD CB02 0301 0001 Z0R1 QT0L";
        //private readonly Paths _paths = new Paths();

        Paths _mainPath = new Paths();


        public PublicApiController(LibraryDbContext context, IHostingEnvironment appEnvironment)
        {
            _context = context;
            _bloodType = _context.Set<BloodType>();
        }
        #region Class's Models
        public class PolicyApiModel
        {
            public string Description { get; set; }
        }
        public class ConditionsApiModel
        {
            public string Description { get; set; }
        }
        public class AboutUsApiViewModel
        {
            public string AboutTitle { get; set; }
            public string AboutSubTitle { get; set; }
            public string AboutDescription { get; set; }
            public int WebVisitor { get; set; }
            public string Image { get; set; }
            public string AboutHomeFooterDescription { get; set; }
            //public string AboutHomeFooterDescriptionAr { get; set; }
        }
        public class FCMTokenModel
        {
            public int userId { get; set; }
            public string FCM { get; set; }
        }
        public class UserProfileModel
        {
            public int Id { get; set; }
            public string UserRoleName { get; set; }
            public long BirthDate { get; set; }
            public bool Gender { get; set; }
            public string InsuranceNo { get; set; }
            public string PersonalImage { get; set; }
            public string Address { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
            public string InsuranceImage { get; set; }
            public string Notes { get; set; }
            public int BloodType { get; set; }

        }
        public class ContactUsAPIViewModel
        {
            public string Name { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string Message { get; set; }

        }
        public class GenerateRegisterUserJWT
        {
            public int userId { get; set; }
        }
        public class Devices
        {
            public string deviceId { get; set; }
            public string fcmToken { get; set; }
        }
        public class UserEmail
        {
            public string Email { get; set; }
        }

        public class PromotionModel
        {
            public string PromotionTitle { get; set; }
            public string PromotionTitleAR { get; set; }
            public string PromotionDescription { get; set; }
        //public string PromotionDescriptionAR { get; set; 
            public string PromotionImage { get; set; }

        }
        #endregion


        [HttpGet("Policy")]
        [SwaggerOperation(Summary = "Policy Api ", Description = "To Get Privacy & Policy Data Like Title and Description")]
        public ActionResult Policy()
        {
            //HttpContext.Session.SetString("TestData", JsonConvert.SerializeObject(model));

            PolicyApiModel model = new PolicyApiModel();
            APIJsonResult result = new APIJsonResult();


            model.Description = AcceptLanguage == "ar"
                ? _context.SystemSettings.Where(x => x.Name == "PrivacyPolicyDescriptionAr").FirstOrDefault().Value
                : _context.SystemSettings.Where(x => x.Name == "PrivacyPolicyDescriptionEn").FirstOrDefault().Value;
            result.Access = true;
            result.success = true;
            result.data = model;
            return Ok(result);
        }

        [HttpGet("BloodType")]
        [SwaggerOperation(Summary = "BloodType Api ", Description = "To Get BloodType")]
        public ActionResult GetAllBloodType()
        {
            //HttpContext.Session.SetString("TestData", JsonConvert.SerializeObject(model));

            BloodTypeViewModel model = new BloodTypeViewModel();
            APIJsonResult result = new APIJsonResult();

            result.Access = true;
            result.success = true;
            result.data = _bloodType.ToList(); ;
            return Ok(result);
        }

        public class test
        {
            public IFormFile Image { get; set; }
            public string name { get; set; }
        }
        [HttpPost("TestImage")]
        [Produces("application/json")]
        [Consumes("application/json", "application/json-patch+json", "multipart/form-data")]
        [SwaggerOperation(Summary = "TestImage API ", Description = "IForm File")]
        public ActionResult TestImage(test model)
        {
            //HttpContext.Session.SetString("TestData", JsonConvert.SerializeObject(model));

            var name = string.Empty;

            if (model.Image != null || model.Image.Length > 0)
            {

                var file = model.Image;

                name = Path.GetFileNameWithoutExtension(model.Image.FileName) + ".jpeg";

                name = @"\Upload\PersonalImage\1\" + name;

                var CoverImagetPath = _paths._insuranceImage + @"\Upload\PersonalImage\1";
                if (!Directory.Exists(CoverImagetPath))
                {
                    Directory.CreateDirectory(CoverImagetPath);
                }

                file.CopyTo(new FileStream(CoverImagetPath + name, FileMode.Create));


            }
            APIJsonResult result = new APIJsonResult();

            result.Access = true;
            result.success = true;
            result.data = Configuration["Doctors:Url"] + name;
            return Ok(result);
        }
    }
}