using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Seagull.Doctors.Areas.Admin.ViewModel;
using Seagull.Doctors.Data;
using Seagull.Doctors.Data.Interfaces;
using Seagull.Doctors.Data.Model;
using Seagull.Core.Areas.Admin.Controllers;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Helper;
using Seagull.Core.Helper.Filters;
using Seagull.Core.Helpers_Extensions.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Seagull.Doctors.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PushNotificationController : AdminCoreBusinessController
    {
        private readonly IGlobalSettings _globalSettings;
        private readonly IMapper _mapper;
        private readonly IPermissionRepository _permissionUser;
        private readonly IUserRepository _userRepository;
        private readonly IPushNotificationRepositry _pushNotificationRepositry;
        private readonly IUsersNotificationRepositry _usersNotificationRepositry;
        private readonly IStringLocalizer _localization;
        public PushNotificationController(IMapper mapper, IPermissionRepository permissionRepository,
            IGlobalSettings globalSettings, IUserRepository userRepository, IPushNotificationRepositry pushNotificationRepositry,
            IUsersNotificationRepositry usersNotificationRepositry, IStringLocalizer localization)
        {
            _localization = localization;
            _pushNotificationRepositry = pushNotificationRepositry;
            _usersNotificationRepositry = usersNotificationRepositry;
            _userRepository = userRepository;
            _globalSettings = globalSettings;
            _mapper = mapper;
            _permissionUser = permissionRepository;
        }
        [Route("PushNotification")]
        public IActionResult Index()
        {
            if (!_permissionUser.CheckAccess(PermissionsName.PushNotificationList))
                return AccessDeniedView();
            return View();
        }
        [HttpPost]
        public IActionResult List([FromBody]PagingClass paging)
        {
            if (!_permissionUser.CheckAccess(PermissionsName.PushNotificationList))
                return AccessDeniedJson();

            var notification = _pushNotificationRepositry.GetAll(paging.pagination, paging.sort, paging.search, paging.search_operator, paging.filter);
            var data = _mapper.Map<List<PushNotification>, List<PushNotificationViewModel>>(notification.ToList());
            data.ForEach(x =>
            {
                x.Child = (from a in _usersNotificationRepositry.GetByPushNotificationId(x.Id)
                           select new
                           {
                               UserName = a.UsersId != null ? _userRepository.GetById((int)a.UsersId).Name : _localization.GetString("Admin.UserNotFound"),
                           }).ToList();
            });
            var angularTable = new DataSourceAngular
            {
                data = data,
                data_count = notification.TotalCount,
                page_count = notification.TotalPages,
            };
            return Json(angularTable);
        }

        [HttpGet]
        public IActionResult PreparePushNotification(int Id)
        {
            if (!_permissionUser.CheckAccess(PermissionsName.PushNotificationAdd))
                return AccessDeniedView();
            return View(Id);
        }
        [HttpPost]
        public IActionResult CreateOrEditModel([FromBody]int Id)
        {
            JsonResultHelper result = new JsonResultHelper();
            result.Access = true;
            result.success = true;

            if (!_permissionUser.CheckAccess(PermissionsName.PushNotificationAdd))
                return AccessDeniedJson();

            //Add Mode
            result.data = _mapper.Map<PushNotificationViewModel, PushNotification>(new PushNotificationViewModel());

            //End result
            return Json(result);
        }


        [HttpPost, ParseParameterActionFilter(typeof(PushNotificationViewModel))]
        public IActionResult CreateOrEdit([FromBody]JsonData jsonData)
        {
            JsonResultHelper result = new JsonResultHelper();
            PushNotificationViewModel model = jsonData.model;

            PushNotification data = new PushNotification();
            result.Access = true;
            result.success = true;
            if (model.Id > 0)
            {
                //no Action in Edit Mode 
            }
            else
            {
                //Add Mode
                data = _mapper.Map<PushNotificationViewModel, PushNotification>(model);
                data.SendDate = DateTime.Now;
                data.UsersCount = model.strUser.Split(",").ToList().Count() - 1;

                _pushNotificationRepositry.Create(data);
                if (!string.IsNullOrEmpty(model.strUser))
                    if (model.strUser.Contains("1"))
                    {
                        var users = _userRepository.GetAllRegisterUser().Where(x => x.FCMToken != null).ToList();
                        users.ForEach(x =>
                        {
                            UsersNotification entity = new UsersNotification();
                            entity.UsersId = x.Id;
                            entity.PushNotificationId = data.Id;

                            PublicFunctions.SendNotification(model.NotificationMessage, x.FCMToken,true);

                            _usersNotificationRepositry.Create(entity);
                        });
                        data.UsersCount = users.Count();
                    }
                    else
                    {
                        model.strUser.Split(",").ToList().ForEach(e =>
                        {
                            if (!string.IsNullOrEmpty(e))
                            {
                                UsersNotification entity = new UsersNotification();
                                entity.UsersId = int.Parse(e);
                                entity.PushNotificationId = data.Id;

                                var user = _userRepository.GetById((int)entity.UsersId);
                                if (user != null)
                                {
                                    PublicFunctions.SendNotification(model.NotificationMessage, user.FCMToken,false);

                                    _usersNotificationRepositry.Create(entity);
                                }
                                //send Notificatin Function
                            }
                        });
                    }

            }
            result.Msg.Add("Succces");
            result.url = jsonData.continueEditing ? Url.Action("PreparePushNotification", "PushNotification", new { id = data.Id }) : Url.Action("Index", "PushNotification");
            return Json(result);
        }


        [HttpPost, ParseParameterDeleteAction(typeof(DeleteModel))]
        [Route("Admin/PushNotification/Delete")]
        public IActionResult Delete([FromBody]DeleteModel deleteModel)
        {
            if (!_permissionUser.CheckAccess(PermissionsName.PushNotificationDelete))
                return AccessDeniedJson();
            JsonResultHelper result = new JsonResultHelper();
            var pushNotification = _pushNotificationRepositry.GetById(deleteModel.Id);
            if (pushNotification != null)
            {
                var userNotification = _usersNotificationRepositry.GetAll().Where(x => x.PushNotificationId == pushNotification.Id).ToList();
                userNotification.ForEach(x =>
                {
                    x.IsDeleted = true;
                    _usersNotificationRepositry.Update(x);
                });
                pushNotification.IsDeleted = true;
                pushNotification.DeletedDate = DateTime.Now;
                pushNotification.DeletedBy = _globalSettings.CurrentUser.Id;
                _pushNotificationRepositry.Update(pushNotification);
            }

            result.success = true;
            return Json(result);
        }
    }
}