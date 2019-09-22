using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Seagull.Core.Controllers;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Helper;
using Seagull.Core.Helper.Filters;
using Seagull.Core.Helper.StaticVariables;
using Seagull.Core.Helpers_Extensions.Helpers;
using Seagull.Core.Models;
using Seagull.Core.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Seagull.Doctors.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LoginController : BaseWebController
    {
        private IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer _stringLocalizer;
        private readonly IGlobalSettings _globalSettings;
        public LoginController(IUserRepository userRepository, IMapper mapper, IStringLocalizer stringLocalizer, IGlobalSettings globalSettings)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _stringLocalizer = stringLocalizer;
            _globalSettings = globalSettings;
        }
        [HttpGet]
        public IActionResult UserLogin(string returnUrl = null)
        {
            if (_globalSettings.CurrentUser != null)
            {
                if (_globalSettings.CurrentUser.UserRoleName.FirstOrDefault() == UserRoleName.Producers || _globalSettings.CurrentUser.UserRoleName.FirstOrDefault() == UserRoleName.Admin)
                    return RedirectToAction("Index", "Dashboard");
            }
            this.ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost, ParseParameterActionFilterModel(typeof(UserLoginView))]
        //[ValidateAntiForgeryToken]
        public IActionResult UserLogin([FromBody]JsonDataModel data)
        {
            JsonResultHelper result = new JsonResultHelper();
            UserLoginView model = data.model;
            User currentUser = _userRepository.CheckLogin(model.Email, model.Password);
            UserViewModel _UserModel = null;// _mapper.Map<User, UserViewModel>(currentUser);
            if (currentUser != null)
            {
                if (currentUser.Activation == true)
                {
                    UserViewModel userrole = _mapper.Map<User, UserViewModel>(currentUser);
                    if (userrole.UserRoleName.FirstOrDefault() == UserRoleName.Admin || userrole.UserRoleName.FirstOrDefault() == UserRoleName.Producers)
                    {
                        var claims = new List<Claim>
                    {
                        new Claim("UserId", currentUser.Id.ToString()),
                        new Claim("CustomCache", "0")
                    };
                        ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "login");
                        ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                        HttpContext.SignInAsync(principal);
                        HttpContext.User = principal;
                        result.success = true;
                        result.Msg.Add(_stringLocalizer.GetString("Admin.Login Success"));
                        _UserModel = _mapper.Map<User, UserViewModel>(currentUser);
                    }
                    else
                    {
                        result.success = false;
                        result.Msg.Add(_stringLocalizer.GetString("Admin.YouAreNotAdmin"));
                        return Json(result);
                    }
                }
                else
                {
                    result.success = false;
                    result.Msg.Add(_stringLocalizer.GetString("Admin.InactiveAccount"));
                    return Json(result);
                }
            }
            else
            {
                result.success = false;
                result.Msg.Add(_stringLocalizer.GetString("Admin.LoginFailed.Pleaseentercorrectcredentials"));
                return Json(result);
            }

            if (_UserModel != null)
                if (_UserModel.Id > 0)
                    if (_UserModel.UserRoleName.FirstOrDefault() == "Admin" || _UserModel.UserRoleName.FirstOrDefault() == "Producers")
                    {
                        result.url = Url.Action("Index", "Dashboard");
                        return Json(result);
                    }

            if (!string.IsNullOrEmpty(model.returnUrl))
                if (Url.IsLocalUrl(model.returnUrl))
                {
                    result.url = model.returnUrl;
                    return Json(result);
                }

            //Defualt Rout
            //result.url = Url.Action("Index", "Dashboard", new { Area = "Admin" });
            //return Json(result);
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.User = null;
            var cookieName = "Seagull.Tam";
            HttpContext.Response.Cookies.Delete(cookieName);
            return RedirectToAction("Index", "Home", new { Area = "" });
        }
    }
}