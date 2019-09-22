using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Seagull.Core.Helpers_Extensions.Helpers;
using Seagull.Core.Data;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Data.Model;
using Seagull.Core.Helper;
using Seagull.Core.Helper.Filters;
using Seagull.Core.Models;
using Seagull.Core.ViewModel;
using Seagull.Doctors.Helper;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Text;
using Seagull.Core.Helper.StaticVariables;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;

namespace Seagull.Core.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : AdminCoreBusinessController
    {
        private readonly IUserRepository _userRepository;
        private readonly IGlobalSettings _globalSettings;
        private readonly IMapper _mapper;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IUserRoleMapRepository _userRoleMapRepository;
        private readonly IPermissionRepository _permissionUser;
        private UserViewModel _currentUser = null;
        private IHttpContextAccessor _accessor;
        private readonly IStringLocalizer _localizer;

        //  private readonly DbContext _context;

        public UsersController(IUserRepository userRepository, IMapper mapper,
            IUserRoleRepository userRoleRepository, IUserRoleMapRepository userRoleMapRepository,
            IGlobalSettings globalSettings, IStringLocalizer localizer,
            IPermissionRepository permissionUser,
            IHttpContextAccessor accessor)
        {
            _localizer = localizer;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
            _userRoleMapRepository = userRoleMapRepository;
            _globalSettings = globalSettings;
            _permissionUser = permissionUser;
            _currentUser = _globalSettings.CurrentUser;
            _accessor = accessor;
            //  _context = context;
        }

        // GET: Users , TypeFilter(typeof(CheckPermissionFilter), Arguments = new[] { PermissionsName.UserList, "" })
        [Route("Users")]
        public IActionResult Index()
        {
            if (!_permissionUser.CheckAccess(PermissionsName.UserList))
                return AccessDeniedView();

            PreparePageViewModel model = new PreparePageViewModel();

            model.Add = _currentUser.PermissionName.Contains(PermissionsName.UserAdd);
            model.Edit = _currentUser.PermissionName.Contains(PermissionsName.UserEdit);
            model.Delete = _currentUser.PermissionName.Contains(PermissionsName.UserDelete);

            return View(model);
        }

        // , TypeFilter(typeof(CheckPermissionFilter), Arguments = new[] { PermissionsName.UserList, "" })

        [HttpPost]
        public IActionResult List([FromBody]PagingClass paging, string type)
        {
            if (!_permissionUser.CheckAccess(PermissionsName.UserList))
                return AccessDeniedJson();
            var users = _userRepository.GetAll(paging.pagination, paging.sort, paging.search, paging.search_operator, paging.filter, type);
            List<UserViewModel> model = _mapper.Map<List<User>, List<UserViewModel>>(users.ToList());
            if (_globalSettings.CurrentUser.UserRoleName.FirstOrDefault() == UserRoleName.Producers)
            {
                model = model.Where(x => x.Id == _globalSettings.CurrentUser.Id).ToList();
            }
            if (type == UserRoleName.Producers)
            {
                model.ForEach(c =>
                {
                    c.Child = (from a in _userRepository.GetUserByProducerId(c.Id)
                               let s = _userRoleRepository.GetById(_userRoleMapRepository.GetAll().Where(x => x.UserId == a.Id).FirstOrDefault().UserRoleId)
                               select new
                               {
                                   UserName = a.Name,
                                   UserEmail = a.Email,
                                   Status = a.Activation,
                                   UserRoleStr = s != null ? s.Name : "",
                                   UserId = a.Id,
                               }).ToList();
                });
            }
            if(type =="both")
            {
                model = model.Where(a=>a.ProducerId == _globalSettings.CurrentUser.Id).ToList();
            }
            var angularTable = new DataSourceAngular
            {
                data = model,
                data_count = users.TotalCount,
                page_count = users.TotalPages,
            };

            return Json(angularTable);
        }

        #region Permissions

        [HttpGet]
        public IActionResult Permissions()
        {
            if (!_permissionUser.CheckAccess(PermissionsName.PermissionList))
                return AccessDeniedView();
            return View();
        }

        [HttpPost]
        public virtual ActionResult PreparePermission()
        {
            return Json(_permissionUser.GetAllPermissionRecordForTree());
        }


        //Old List
        [HttpPost]
        public virtual ActionResult GetAllPermissionsForSelectedUserRole([FromBody] int UserRoleId)
        {
            if (!_permissionUser.CheckAccess(PermissionsName.PermissionList))
                return AccessDeniedJson();
            List<int> data = _userRoleRepository.GetAllUserRolePermissionByUserRoleId(UserRoleId);
            return Json(data);
        }


        public class Perms
        {
            public int UserRoleId { get; set; }
            public int[] Permissions { get; set; }
            public bool continueEditing { get; set; }
        }

        // [TypeFilter(typeof(CheckPermissionFilter), Arguments = new[] { PermissionsName.ManageACL, "" })]
        public virtual ActionResult PermissionsSave([FromBody]Perms perms)
        {

            JsonResultHelper result = new JsonResultHelper();
            List<int> removeList = new List<int>();
            List<int> addList = new List<int>();
            List<int> unionList = new List<int>();
            List<int> olddaa = _userRoleRepository.GetAllUserRolePermissionByUserRoleId(perms.UserRoleId);
            unionList = olddaa.Union(perms.Permissions.ToList()).ToList();
            removeList = unionList.Except(perms.Permissions.ToList()).ToList();
            addList = unionList.Except(olddaa).ToList();

            UserRole tempUserRole = _userRoleRepository.GetById(perms.UserRoleId);
            if (!_permissionUser.CheckAccess(PermissionsName.PermissionAdd))
                return AccessDeniedJson();

            //Add New Permission For Current User Role
            addList.ForEach(p =>
            {
                PermUserRoleMap entity = new PermUserRoleMap();
                entity.UserRole = tempUserRole;
                entity.Permission = _permissionUser.GetById(p);
                tempUserRole.fk_UserRolePermMap.Add(entity);

            });

            if (!_permissionUser.CheckAccess(PermissionsName.PermissionDelete))
                return AccessDeniedJson();
            //Remove Permission For Current User Role
            removeList.ForEach(p =>
            {
                var tempPerm = tempUserRole.fk_UserRolePermMap.Where(a => a.PermId == p && a.UserRoleId == tempUserRole.Id).FirstOrDefault();
                tempUserRole.fk_UserRolePermMap.Remove(tempPerm);
            });
            _userRoleRepository.Update(tempUserRole);
            //RemoveList = unionList.RemoveAll(i => perms.Contains(i));
            //AddList = unionList.RemoveAll(j => olddaa.Contains(j));
            result.Access = true;
            //result.Msg.Add(_localizationService.GetResource("Admin.Configuration.ACL.Updated"));
            result.success = true;
            result.url = perms.continueEditing ? string.Empty : Url.Action("Index", "Dashboard");
            return Json(result);

        }

        #endregion


        #region Users //,TypeFilter(typeof(CheckPermissionFilter), Arguments = new[] { PermissionsName.UserAdd, PermissionsName.UserEdit })


        [HttpGet]
        public IActionResult PrepareUser(int Id)
        {
            if (Id > 0)
            {
                var user = _userRepository.GetById(Id);
                var data = _mapper.Map<User, UserViewModel>(user);
                if (data.ProducerId != null)
                {
                    data.IsProducersUser = true;
                }
                else
                {
                    data.IsProducersUser = false;
                    if(data.UserRoleName.FirstOrDefault()==UserRoleName.Producers)
                    {

                    }
                
                }
                if (!_permissionUser.CheckAccess(PermissionsName.UserEdit))
                    return AccessDeniedView();
            }
            else
            {
                if(_globalSettings.CurrentUser.UserRoleName.FirstOrDefault() == UserRoleName.Producers)
                ViewBag.IsProducersUser = false;
                if (!_permissionUser.CheckAccess(PermissionsName.UserAdd))
                    return AccessDeniedView();
            }
            return View(Id);
        }

        //, TypeFilter(typeof(CheckPermissionFilter), Arguments = new[] { PermissionsName.UserAdd, PermissionsName.UserEdit })
        [HttpPost]
        public IActionResult CreateOrEditModel([FromBody]int Id)
        {
            JsonResultHelper result = new JsonResultHelper();
            result.Access = true;
            result.success = true;
            if (Id > 0)
            {
                if (!_permissionUser.CheckAccess(PermissionsName.UserEdit))
                    return AccessDeniedJson();
                //Edit Mode
                var user = _userRepository.GetById(Id);

                var data = _mapper.Map<User, UserViewModel>(user);
                data.SelectedProducersUser = user.ProducerId != null ? user.ProducerId.ToString() : null;
                if (data.ProducerId != null)
                {
                    data.SelectedUserRoles = user.fk_UserRoleMap.FirstOrDefault().UserRole.Name;
                    data.IsProducersUser = true;
                }
                else
                {
                    data.SelectedUserRoles = user.fk_UserRoleMap.FirstOrDefault().UserRole.Name;
                    data.IsProducersUser = false;
                }
                result.data = data;
            }
            else
            {
                if (!_permissionUser.CheckAccess(PermissionsName.UserAdd))
                    return AccessDeniedJson();
                //Add Mode
                var data = _mapper.Map<User, UserViewModel>(new User());
                data.SelectedProducersUser = null;


                result.data = data;

            }
            //End result
            return Json(result);
        }

        [HttpPost, ParseParameterActionFilter(typeof(UserViewModel))] // TypeFilter(typeof(CheckPermissionFilter), Arguments = new[] { PermissionsName.UserAdd }) ,
        public IActionResult CreateOrEdit([FromBody]JsonData jsonData)
        {
            JsonResultHelper result = new JsonResultHelper();
            UserViewModel model = jsonData.model;
            User data = null;
            //HashSet<int> newRole = new HashSet<int>(model.SelectedUserRoles.Split(',').ToList().Where(a => !string.IsNullOrEmpty(a)).Select(s => int.Parse(s)).ToList());
            //HashSet<string> newRoleName = new HashSet<string>(model.UserRoleName.ToList());
            //HashSet<string> newRole = new HashSet<string>(model.SelectedUserRoles);
            string newRoleName = model.SelectedUserRoles;
            HashSet<int> newProducerRole = model.SelectedProducersUser != null ? new HashSet<int>(model.SelectedProducersUser.Split(',').ToList().Where(a => !string.IsNullOrEmpty(a)).Select(s => int.Parse(s)).ToList()) : null;
            if ((newRoleName == UserRoleName.TicketingUser || newRoleName == UserRoleName.OrganizersUser) && newProducerRole !=null)
            {
                model.ProducerId = newProducerRole.FirstOrDefault();
            }
            else if ((newRoleName == UserRoleName.TicketingUser || newRoleName == UserRoleName.OrganizersUser) && newProducerRole == null)
            {
                model.ProducerId = _globalSettings.CurrentUser.Id;
            }
            else
            {
                model.ProducerId = null;
            }

            if (model.Id > 0)
            {
                //Edit Mode
                if (!_permissionUser.CheckAccess(PermissionsName.UserEdit))
                    return AccessDeniedJson();
                var checkUser = _userRepository.GetByEmail(model.Email);
                if(checkUser != null)
                {
                    if(checkUser.Email != _userRepository.GetById(model.Id).Email)
                    {
                        result.Access = false;
                        result.success = false;
                        result.Msg.Add("User E-mail exist");
                        return Json(result);
                    }
                }
                Regex mobilePattern = new Regex(RegexStrings.MobileRegex);
                if (!mobilePattern.IsMatch(model.Mobile))
                {
                    result.Access = false;
                    result.success = false;
                    result.Msg.Add("Unveiled mobile number - should be 8 digits");
                    return Json(result);
                }
                var oldPassword = _userRepository.GetById(model.Id).Password;
                data = _mapper.Map<UserViewModel, User>(model, _userRepository.GetById(model.Id));
                data.UpdatedBy = _globalSettings.CurrentUser.Id;
                if(data.Password != oldPassword)
                using (var algorithm = MD5.Create()) //or MD5 SHA256 etc.
                {
                    var hashedBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(data.Password));

                    data.Password = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                }
                _userRepository.Update(data);
                var getProducarUser = _userRepository.GetUserByProducerId(data.Id);
                var userRole = data.fk_UserRoleMap.Where(x => x.UserId == data.Id).FirstOrDefault().UserRole;
                if (userRole.Name == UserRoleName.Producers && userRole.Name != newRoleName) // get Old user Role and check if change hes role
                    if (getProducarUser.Count() > 0)//check if has users (Ticketing User , Organizers User) to rollback  if change his Role
                    {
                        result.Access = true;
                        result.success = false;
                        result.Msg.Add("This Producer Has Users");
                        result.url = Url.Action("PrepareUser", "Users", new { id = data.Id });
                        data.ProducerId = null;
                        _userRepository.Update(data);
                        return Json(result);
                    }
                //Remove UnSelected UserRole
                data.fk_UserRoleMap.Where(ur => !newRoleName.Contains(ur.UserRole.Name)).ToList().ForEach(d =>
                   {
                       data.fk_UserRoleMap.Remove(data.fk_UserRoleMap.Where(s => s.UserRoleId == d.UserRoleId).Select(r => r).FirstOrDefault());
                   });

                //Insert New Roles
                //newRoleName.ForEach(d =>
                //{
                //Check before Update
                if (data.fk_UserRoleMap.Where(a => a.UserRole.Name == newRoleName).Count() == 0)
                {
                    data.fk_UserRoleMap.Add(_userRoleRepository.MapUserRole(data, _userRoleRepository.GetByUserRoleName(newRoleName)));
                }
                //});
                _userRepository.Update(data);
            }
            else
            {
                if (!_permissionUser.CheckAccess(PermissionsName.UserAdd))
                    return AccessDeniedJson();

                //Add Mode
                var checkUser = _userRepository.GetByEmail(model.Email);
                if (checkUser != null)
                {
                    if (checkUser.Email.Equals(model.Email))
                    {
                        result.Access = false;
                        result.success = false;
                        result.Msg.Add(_localizer.GetString("User E-mail exist"));
                        return Json(result);
                    }
                }
                Regex mobilePattern = new Regex(RegexStrings.MobileRegex);
                if (!mobilePattern.IsMatch(model.Mobile))
                {
                    result.Access = false;
                    result.success = false;
                    result.Msg.Add(_localizer.GetString("Unveiled mobile number - should be 8 digits"));
                    return Json(result);
                }
                data = _mapper.Map<UserViewModel, User>(model);
                data.CreatedBy = _globalSettings.CurrentUser.Id;
                using (var algorithm = MD5.Create()) //or MD5 SHA256 etc.
                {
                    var hashedBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(data.Password));
                    data.Password = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                }
                _userRepository.Create(data);

                //Insert New Roles
                //newRoleName.ToList().ForEach(d =>
                //{
                if (newRoleName == UserRoleName.Producers)
                {
                    data.ProducerId = null;
                }
                data.fk_UserRoleMap.Add(_userRoleRepository.MapUserRole(data, _userRoleRepository.GetByUserRoleName(newRoleName))); // uncomment
                //});
                _userRepository.Update(data);
            }
            //End result
            result.Access = true;
            result.success = true;
            // result.data = Model;
            result.url = jsonData.continueEditing ? Url.Action("PrepareUser", "Users", new { id = data.Id }) : Url.Action("Index", "Users");
            return Json(result);
        }

        #endregion

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.User = null;
            var cookieName = _globalSettings.CurrentSystemSetting.CookieName;
            HttpContext.Response.Cookies.Delete(cookieName);
            return RedirectToAction("Index", "Home", new { Area = "" });
        }

        public ActionResult SetLang(int id)
        {
            if (_globalSettings.CurrentUser != null)
            {
                var data = _userRepository.GetById(_globalSettings.CurrentUser.Id);
                data.LangId = id;
                _userRepository.Update(data);
                //_globalSettings.RefreshUser();
                //#region Refresh Cookies 

                //var cookieName = "Seagull.Tam";

                //_accessor.HttpContext.Response.Cookies.Delete(cookieName);

                ////get date of cookie expiration
                //var cookieExpires = 24 * 365; //TODO make configurable
                //var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);

                ////set new cookie value
                //var options = new CookieOptions
                //{
                //    HttpOnly = true,
                //    //Secure = true,
                //    Expires = cookieExpiresDate
                //};
                //// write the cookie
                //var kvpCookie = new Dictionary<string, string>()
                //    {
                //        { "UserId", _globalSettings.CurrentUser.Id.ToString() },
                //        { "Lang", id.ToString() }
                //    };

                //var content = kvpCookie.ToLegacyCookieString();
                //_accessor.HttpContext.Response.Cookies.Append(cookieName, content, options);
                //#endregion
                return Json(true);
            }
            return Json(false);
        }

        [HttpPost, ParseParameterDeleteAction(typeof(DeleteModel))]
        [Route("Admin/Users/Delete")]
        public IActionResult Delete([FromBody]DeleteModel deleteModel)
        {
            JsonResultHelper result = new JsonResultHelper();
            var user = _userRepository.GetById(deleteModel.Id);
            if (user != null)
            {
                user.IsDeleted = true;
                user.DeletedBy = _globalSettings.CurrentUser.Id;

                #region Delete All Producer User if this User is Producer
                if (user.fk_UserRoleMap.Where(x => x.UserId == user.Id).FirstOrDefault().UserRole.Name == UserRoleName.Producers)
                {
                    var producerUser = _userRepository.GetUserByProducerId(user.Id).ToList();
                    if (producerUser.Count > 0)
                    {
                        producerUser.ForEach(x =>
                        {
                            var deleteProducerUser = _userRepository.GetById(x.Id);
                            if (deleteProducerUser != null)
                            {
                                deleteProducerUser.IsDeleted = true;
                                deleteProducerUser.DeletedBy = _globalSettings.CurrentUser.Id;
                                _userRepository.Update(deleteProducerUser);
                            }
                        });
                    }
                }

                #endregion

                _userRepository.Update(user);
                result.Msg.Add("web.User.DeletedSuccessful");
                result.success = true;
            }
            return Json(result);
        }
        public class GetProducersUserModel
        {
            public int UserId { get; set; }
            public int ProducerId { get; set; }
        }
    }
}
