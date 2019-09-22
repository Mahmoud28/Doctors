using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Seagull.Core.Areas.Admin.ViewModel;
using Seagull.Core.Data;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Data.MappingExtension;
using Seagull.Core.Helper;
using Seagull.Core.Helper.Caching;
using Seagull.Core.Helper.Filters;
using Seagull.Core.Helpers_Extensions.Helpers;
using Seagull.Doctors.Helper;
using Seagull.Core.ViewModel;
using Seagull.Doctors.Helper.ImageHelper;

namespace Seagull.Core.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SystemSettingController : AdminCoreBusinessController
    {
        private readonly IUserRepository _userRepository;
        private readonly IGlobalSettings _globalSettings;
        private readonly ICacheServicecs _cache;
        private readonly IServiceProvider m_ServiceProvider;
        private readonly LibraryDbContext _context;
        private readonly IPermissionRepository _permissionUser;
        private UserViewModel _currentUser = null;
        private readonly Paths _paths = new Paths();

        public SystemSettingController(IUserRepository userRepository, IPermissionRepository permissionUser, IGlobalSettings globalSettings, ICacheServicecs cache, IServiceProvider serviceProvider)
        {
            _userRepository = userRepository;
            _globalSettings = globalSettings;
            _cache = cache;
            m_ServiceProvider = serviceProvider;
            _permissionUser = permissionUser;
            _currentUser = _globalSettings.CurrentUser;
            _context = m_ServiceProvider.CreateScope().ServiceProvider.GetService<LibraryDbContext>();
        }
        public IActionResult PrepareSystemSetting()
        {
            if (!_permissionUser.CheckAccess(PermissionsName.SystemSetting))
                return AccessDeniedJson();
            return View(0);
        }

        [HttpPost]
        public IActionResult CreateOrEditModel([FromBody]int Id)
        {
            if (!_permissionUser.CheckAccess(PermissionsName.SystemSetting))
                return AccessDeniedJson();
            JsonResultHelper result = new JsonResultHelper();
            result.Access = true;
            result.success = true;

            SystemSettingViewModel model = _globalSettings.CurrentSystemSetting;
            new SystemSettingMapping().ToEntity(_context, model, "");
            model.Token = DateTime.Now.Ticks.ToString();
            result.data = model;
            //End result
            return Json(result);
        }

        [HttpPost, ParseParameterActionFilter(typeof(SystemSettingViewModel))]
        public IActionResult CreateOrEdit([FromBody]JsonData jsonData)
        {
            if (!_permissionUser.CheckAccess(PermissionsName.SystemSetting))
                return AccessDeniedJson();
            JsonResultHelper result = new JsonResultHelper();
            SystemSettingViewModel model = jsonData.model;

            var systemlogo = _globalSettings.CurrentSystemSetting.SystemLogo;
            model.SystemLogo = systemlogo;

            var aboutHomeImage = _globalSettings.CurrentSystemSetting.AboutHomeImage;
            model.AboutHomeImage = aboutHomeImage;

            var promotionImage = _globalSettings.CurrentSystemSetting.PromotionImage;
            model.PromotionImage = promotionImage;
            new SystemSettingMapping().ToEntity(_context, model, "");


            //new ImageHelper().SaveByteArrayAsImage(@"\SystemLogo\", "SystemLogo.jpeg", model.SystemLogo);
            //new ImageHelper().SaveByteArrayAsImage(@"\AboutUs\", "AboutUs.jpeg", model.AboutHomeImage);
            //new SystemSettingMapping().ToEntity(_context, model, "");

            
            ////End result
            result.Access = true;
            result.success = true;
            //result.data = Model;
            result.url = jsonData.continueEditing ? Url.Action("PrepareSystemSetting") : Url.Action("Index", "Dashboard");
            return Json(result);
        }

        [HttpPost]
        public IActionResult UplodeFile(string token, int Id, bool IsMultiple, string type)
        {
            string imageType = "";
            if (IsMultiple)
                imageType = @"\Slider\";
            else
                imageType = @"\" + type + @"\";
            List<string> paths = new List<string>();
            var data = Request.Form.Files;
            if (data != null)
            {
                //if (Id > 0)
                //{
                var systemSetting = _globalSettings.CurrentSystemSetting;
                if (systemSetting != null)
                {
                    if (!IsMultiple)
                    {
                        var updatePath = _paths._mainDirectory + @"\SystemSetting\" + imageType + @"\";
                        DirectoryInfo di = new DirectoryInfo(updatePath);
                        if (Directory.Exists(updatePath))
                            foreach (FileInfo file in di.GetFiles())
                            {
                                if (file != null)
                                    file.Delete();
                            }
                        else
                        {
                            Directory.CreateDirectory(updatePath);
                        }

                        foreach (var image in data)
                        {
                            string extension = Path.GetExtension(image.FileName);
                            var fileName = image.FileName.Replace(extension, ".jpeg");
                            var fileStream = new FileStream(updatePath + fileName, FileMode.Create);
                            image.CopyTo(fileStream);
                            paths.Add(@"\Upload\SystemSetting\" + type + @"\" + fileName);
                            if (type == "SystemLogo")
                            {
                                systemSetting.SystemLogo = @"\Upload\SystemSetting\" + type + @"\" + fileName;
                                fileStream.Close();
                                fileStream.Dispose();
                            }
                            if (type == "AboutHomeImage")
                            {
                                systemSetting.AboutHomeImage = @"\Upload\SystemSetting\" + type + @"\" + fileName;
                                fileStream.Close();
                                fileStream.Dispose();
                            }
                            if (type == "PromotionImage")
                            {
                                systemSetting.PromotionImage = @"\Upload\SystemSetting\" + type + @"\" + fileName;
                                fileStream.Close();
                                fileStream.Dispose();
                            }

                        }
                        new SystemSettingMapping().ToEntity(_context, systemSetting, "");
                    }
                    return Json(paths);
                    //}
                }
             
            }
            return Json(paths);
        }

    }
}