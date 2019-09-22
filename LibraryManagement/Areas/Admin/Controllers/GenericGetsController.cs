using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Seagull.Core.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Seagull.Core.Helpers_Extensions.Helpers;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Seagull.Core.Helper.Filters;
using Seagull.Core.Helper;
using Seagull.Doctors.Data.Interfaces;
using Seagull.Doctors.Areas.Admin.Models;
using AutoMapper;
using Seagull.Core.Helper.StaticVariables;

namespace Seagull.Core.Areas.Admin.Controllers
{
    //[Authorize, TypeFilter(typeof(CheckGuestUser))]
    [Area("Admin")]
    public class GenericGetsController : AdminCoreBusinessController
    {
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly IUserRepository _userRepository;
        private readonly IGlobalSettings _user;
        private readonly IGlobalSettings _globalSettings;

        public GenericGetsController(IUserRoleRepository userRoleRepository, 
            IStringLocalizer stringLocalizer,
            IGlobalSettings user,
            IUserRepository userRepository,
            IGlobalSettings globalSettings,
            IMapper mapper)
        {
            _userRoleRepository = userRoleRepository;
            _stringLocalizer= stringLocalizer;
            _user=user;
            _userRepository = userRepository;
            _globalSettings = globalSettings;
            _mapper = mapper;
        }
        
        #region class for AngularDropDown

        public JsonResultHelper ReturnData(List<dynamic> listData)
        {
            JsonResultHelper result = new JsonResultHelper();
            result.Access = true;
            result.data = listData.OrderBy(a => a.Id).ToList();
            result.success = true;
            return result;
        }
        #endregion

        #region GetAllUserRoles
        public ActionResult GetAllUserRoles([FromBody]PagingClass paging)
        {
            string type = _globalSettings.CurrentUser.UserRoleName.FirstOrDefault();
            if (type.Equals(UserRoleName.Producers))
            {
                 var data = (from a in _userRoleRepository.GetAll(paging.pagination, paging.sort, paging.search, paging.search_operator, paging.filter).Where(x => x.Name == UserRoleName.TicketingUser || x.Name == UserRoleName.OrganizersUser)
                            select new
                            {
                                Id = a.Id,
                                Name = string.IsNullOrEmpty(_stringLocalizer.GetString(a.Name).Value) ? a.Name : _stringLocalizer.GetString(a.Name).Value,
                            }).Cast<object>().ToList();
                return Json(ReturnData(data));
            }
            else
            {
                paging.filter = null;
               var  data = (from a in _userRoleRepository.GetAll(paging.pagination, paging.sort, paging.search, paging.search_operator, paging.filter)
                            select new
                            {
                                Id = a.Id,
                                Name = string.IsNullOrEmpty(_stringLocalizer.GetString(a.Name).Value) ? a.Name : _stringLocalizer.GetString(a.Name).Value,
                            }).Cast<object>().ToList();
                return Json(ReturnData(data));
            }
        }
        #endregion

        #region Helpers
        public class CustomList
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
        #endregion
        
        #region Get All User By UserRole
        public ActionResult GetAllUserByUserRole([FromBody]PagingClass paging)
        {
            var data = (from a in _userRepository.GetAll(paging.pagination, paging.sort, paging.search, paging.search_operator, paging.filter,UserRoleName.Producers)
                        select new
                        {
                            Id = a.Id,
                            Name =a.Name,
                        }).Cast<object>().ToList();

            return Json(ReturnData(data));

        }

        public ActionResult GetProducersUser([FromBody]PagingClass paging)
        {
           
            var data = (from a in _userRepository.GetAll(paging.pagination, paging.sort, paging.search, paging.search_operator, paging.filter, UserRoleName.Producers)
                        select new
                        {
                            Id = a.Id,
                            Name = a.Name,
                        }).Cast<object>().ToList();
            return Json(ReturnData(data));
        }


        public ActionResult GetAllUsersHasFCMToken([FromBody]PagingClass paging)
        {
            object all = new { Id = 1, Name = "All" };
            var data = (from a in _userRepository.GetAll(paging.pagination, paging.sort, paging.search, paging.search_operator, paging.filter, UserRoleName.User).Where(x => x.FCMToken != null)
                        select new
                        {
                            Id = a.Id,
                            Name = a.Name,
                        }).Cast<object>().ToList();
            data.Insert(0,all);
            return Json(ReturnData(data));
        }

        #endregion

        #region Get All Users 
        public ActionResult GetAllUsers([FromBody]PagingClass paging)
        {
            var data = (from a in _userRepository.GetAllUsers(paging.pagination, paging.sort, paging.search, paging.search_operator, paging.filter)
                        select new
                        {
                            Id = a.Id,
                            Name = a.Name,
                        }).Cast<object>().ToList();

            return Json(ReturnData(data));

        }
        #endregion
    }
}