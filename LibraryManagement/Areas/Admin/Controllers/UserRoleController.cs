using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Seagull.Core.Helpers_Extensions.Helpers;
using Seagull.Core.Data;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Data.Model;
using Seagull.Core.Helper.Filters;
using Seagull.Core.Models;
using Seagull.Core.ViewModel;
using Seagull.Core.Helper;
using Seagull.Doctors.Helper;

namespace Seagull.Core.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserRoleController : AdminCoreBusinessController
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IMapper _mapper;
        private readonly IPermissionRepository _permissionUser;
        private readonly IGlobalSettings _globalSettings;
        private UserViewModel _currentUser = null;

        public UserRoleController( IUserRoleRepository userRoleRepository, IMapper mapper,IPermissionRepository permissionUser, IGlobalSettings globalSettings)
        {
          
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
            _permissionUser = permissionUser;
            _globalSettings = globalSettings;
            _currentUser = _globalSettings.CurrentUser;

        }

        [Route("UserRole")]
        public IActionResult Index()
        {
            if (!_permissionUser.CheckAccess(PermissionsName.UserRoleList))
                return AccessDeniedView();
            PreparePageViewModel model = new PreparePageViewModel();

            model.Add = _currentUser.PermissionName.Contains(PermissionsName.UserRoleAdd);
            model.Edit = _currentUser.PermissionName.Contains(PermissionsName.UserRoleEdit);
            model.Delete = _currentUser.PermissionName.Contains(PermissionsName.UserRoleDelete);

            return View(model);
        }

        [HttpPost]
        public IActionResult List([FromBody]PagingClass paging)
        {
            if (!_permissionUser.CheckAccess(PermissionsName.UserRoleList))
                return AccessDeniedJson();

            var userRole = _userRoleRepository.GetAll(paging.pagination, paging.sort, paging.search, paging.search_operator, paging.filter);
            var angularTable = new DataSourceAngular
            {

                data = userRole,
                data_count = userRole.TotalCount,
                page_count = userRole.TotalPages,
            };
            return Json(angularTable);
        }

        [HttpGet]
        public IActionResult PrepareUserRole(int Id)
        {
            if (Id > 0)
            {
                if (!_permissionUser.CheckAccess(PermissionsName.UserRoleEdit))
                    return AccessDeniedView();
            }
            else
            {
                if (!_permissionUser.CheckAccess(PermissionsName.UserRoleAdd))
                    return AccessDeniedView();
            }
            return View(Id);

        }

        [HttpPost]
        public IActionResult CreateOrEditModel([FromBody]int Id)
        {
            JsonResultHelper result = new JsonResultHelper();
            result.Access = true;
            result.success = true;
            if (Id > 0)
            {
                if (!_permissionUser.CheckAccess(PermissionsName.UserRoleEdit))
                    return AccessDeniedView();

                //Edit Mode
                result.data = _mapper.Map<UserRole, UserRoleViewModel>(_userRoleRepository.GetById(Id));
            }
            else
            {
                if (!_permissionUser.CheckAccess(PermissionsName.JobRoleAdd))
                    return AccessDeniedView();

                //Add Mode
                result.data = _mapper.Map<UserRole, UserRoleViewModel>(new UserRole());
            }
            //End result
            return Json(result);
        }
        [HttpPost, ParseParameterActionFilter(typeof(UserRoleViewModel))]
        public IActionResult CreateOrEdit([FromBody]JsonData jsonData)
        {
            JsonResultHelper result = new JsonResultHelper();
            UserRoleViewModel model = jsonData.model;
          
            UserRole data = null;
            if (model.Id > 0)
            {
                if (!_permissionUser.CheckAccess(PermissionsName.UserRoleEdit))
                    return AccessDeniedView();

                //Edit Mode  
                data = _mapper.Map<UserRoleViewModel, UserRole>(model, _userRoleRepository.GetById(model.Id));
              
                _userRoleRepository.Update(data);
            }
            else
            {
                if (!_permissionUser.CheckAccess(PermissionsName.UserRoleAdd))
                    return AccessDeniedView();

                //Add Mode
                data = _mapper.Map<UserRoleViewModel, UserRole>(model);
                _userRoleRepository.Create(data);
            }
            //End result
            result.Access = true;
            result.success = true;
            //result.data = Model;
            result.url = jsonData.continueEditing ? Url.Action("PrepareUserRole", "UserRole", new { id = data.Id }) : Url.Action("Index", "UserRole");
            return Json(result);
        }

    }
}