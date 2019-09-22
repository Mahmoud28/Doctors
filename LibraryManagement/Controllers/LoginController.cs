using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Seagull.Core.Data.Interfaces;
using Seagull.Core.Helper;
using Seagull.Core.Models;
using Seagull.Core.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Seagull.Core.Helper.Filters;
using Seagull.Core.Helpers_Extensions.Helpers;
using AutoMapper;
using Microsoft.Extensions.Localization;
using Seagull.Core.Helper.StaticVariables;
using Seagull.Doctors.Data.Interfaces;
using Seagull.Doctors.Areas.Admin.Models;
using Seagull.Core.Helper.SignalR.SeagullHub;
using Microsoft.AspNetCore.SignalR;
using Seagull.Doctors.Models;
using Facebook;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Seagull.Core.Controllers
{
    public class LoginController : BaseWebController
    {
        private IUserRepository _userRepository;
        private IGlobalSettings _currentUser;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer _localizer;
        private readonly IUserRoleRepository _userRoleRepository;
        private readonly IHubContext<SeagullHub> _lineHubContext;
        private readonly IGlobalSettings _globalSettings;
        private readonly IStringLocalizer _localization;

        public LoginController(IGlobalSettings currentUser, IUserRepository userRepository, IMapper mapper, IStringLocalizer localizer, IUserRoleRepository userRoleRepository, IHubContext<SeagullHub> lineHubContext, IGlobalSettings globalSettings,
             IStringLocalizer localization)
        {
            _currentUser = currentUser;
            _userRepository = userRepository;
            _mapper = mapper;
            _localizer = localizer;
            _userRoleRepository = userRoleRepository;
            _lineHubContext = lineHubContext;
            _globalSettings = globalSettings;
            _localization = localization;

        }
        [HttpGet]
        public IActionResult RegisterUser(string returnUrl = null)
        {
            this.ViewData["ReturnUrl"] = returnUrl;
            if (_currentUser.CurrentUser.UserRoleName.FirstOrDefault() == "Register User")
                return RedirectToAction("Profile", "Profile");

            UserRegisterModel model = new UserRegisterModel();
            model.returnUrl = returnUrl;
            return View(model);
        }

        //[HttpPost]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult RegisterUser([FromBody]UserRegisterModel model)
        {
            //this.ViewData["ReturnUrl"] = model.returnUrl;

            SwalMessage swal = new SwalMessage();

            List<string> errors = new List<string>();
            if (ModelState.IsValid)
            {
                if (!model.AcceptCondition)
                {
                    swal.MessageType = false;
                    swal.Message = _localizer.GetString("web.Please Accept Terms and Conditions");
                    swal.Header = _localizer.GetString("web.Error");
                    swal.SwalType = "error";
                    return Json(swal);
                }
                errors = _userRepository.GetByUserNameOrEmail(model.Email, model.Email);
                if (errors.Count > 0)
                {
                    swal.MessageType = false;
                    swal.Message = _localizer.GetString("web.Duplicate Email");
                    swal.Header = _localizer.GetString("web.Error");
                    swal.SwalType = "error";
                    return Json(swal);
                }

                if(model.Password.Length <8 )
                {
                    swal.MessageType = false;
                    swal.Message = _localizer.GetString("web.Password Must At least 8 Charachters");
                    swal.Header = _localizer.GetString("web.Error");
                    swal.SwalType = "error";
                    return Json(swal);
                }
                if (model.Password != model.ConfirmPassword)
                {
                    //swal.MessageType = false;
                    swal.MessageType = false;
                    swal.Message = _localizer.GetString("web.Password does not match");
                    swal.Header = _localizer.GetString("web.Error");
                    swal.SwalType = "error";
                    return Json(swal);
                }

                ////Regex pattern = new Regex(@"^([0]|[00]|[+] *\)?)[0-9]{11,13}$");
                Regex mobilePattern = new Regex(RegexStrings.MobileRegex);
                if (!mobilePattern.IsMatch(model.Mobile))
                {
                    swal.MessageType = false;
                    swal.Header = _localization.GetString("web.Failed");
                    swal.Message = _localization.GetString("web.Incorrect phone number must be 8 digits : 99999999");
                    swal.SwalType = "error";
                    return Json(swal);
                }

                bool isEmail = Regex.IsMatch(model.Email, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
                if (!isEmail)
                {
                    swal.MessageType = false;
                    swal.Header = _localization.GetString("web.Failed");
                    swal.Message = _localization.GetString("web.Incorrect Email Format");
                    swal.SwalType = "error";
                    return Json(swal);
                }

                if (string.IsNullOrEmpty(model.Email) & string.IsNullOrEmpty(model.Mobile) & string.IsNullOrEmpty(model.Name) & string.IsNullOrEmpty(model.Password))
                {
                    swal.MessageType = false;
                    swal.Header = _localization.GetString("web.Failed");
                    swal.Message = _localization.GetString("web.all fields  mandatory");
                    swal.SwalType = "error";
                    return Json(swal);
                }

                swal.MessageType = true;

                //model.returnUrl = "/Profile/profile";

                User currentUser = new User();
                currentUser = _mapper.Map<UserRegisterModel, User>(model);
                currentUser.Activation = true;
                currentUser.IsDeleted = false;
                currentUser.LangId = _currentUser.CurrentUser.LangId;


                using (var algorithm = MD5.Create()) //or MD5 SHA256 etc.
                {
                    var hashedBytes = algorithm.ComputeHash(Encoding.UTF8.GetBytes(currentUser.Password));

                    currentUser.Password = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
                }



                _userRepository.Create(currentUser);


                currentUser.fk_UserRoleMap.Add(_userRoleRepository.MapUserRole(currentUser, _userRoleRepository.GetById(7)));
                _userRepository.Update(currentUser);

                #region Login
                var claims = new List<Claim>
                    {
                        new Claim("UserId", currentUser.Id.ToString()),
                        new Claim("CustomCache", "0")
                    };
                ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "login");
                ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
                HttpContext.SignInAsync(principal);
                #endregion

                //#region Redirect To Profile
                if (!string.IsNullOrEmpty(model.returnUrl))
                // if (Url.IsLocalUrl(model.returnUrl))
                {
                    return Redirect("/" + model.returnUrl);
                }
                //#endregion
                //return RedirectToAction("Profile", "Profile", new { Area = "", returnUrl = model.returnUrl });
                return Json(swal);
            }
            swal.MessageType = false;
            swal.Header = _localization.GetString("web.Failed");
            swal.Message = _localization.GetString("web.all fields are mandatory");
            swal.SwalType = "error";
            return Json(swal);

        }

        [HttpPost]
        public IActionResult UserLogin([FromBody]UserLoginView model)
        {
            this.ViewData["ReturnUrl"] = model.returnUrl;   
            User currentUser = null;

            SwalMessage swal = new SwalMessage();

            CheckUserAccountModel data = _userRepository.CheckUserAccount(model.Email, model.Password);

            currentUser = data.user != null ? _userRepository.GetById(data.user.Id) : null;
            UserViewModel _UserModel = _mapper.Map<User, UserViewModel>(currentUser);

            if (currentUser != null)
            {
                if (currentUser.Activation == true)
                {
                    var userrole = _mapper.Map<User, UserViewModel>(currentUser);

                    if (userrole.UserRoleName.FirstOrDefault() == "Register User")
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

                        _UserModel = _mapper.Map<User, UserViewModel>(currentUser);
                    }
                    else if (userrole.UserRoleName.FirstOrDefault() == UserRoleName.TicketingUser)
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

                        _UserModel = _mapper.Map<User, UserViewModel>(currentUser);

                    }
                    else
                    {
                        swal.MessageType = false;
                        swal.Header = _localization.GetString("web.Failed");
                        swal.Message = _localization.GetString("web.Go to Admin Login");
                        swal.SwalType = "error";
                        return Json(swal);
                    }

                }
                else
                {
                    swal.MessageType = false;
                    swal.Header = _localization.GetString("web.Failed");
                    swal.Message = _localization.GetString("web.InactiveAccount");
                    swal.SwalType = "error";
                    return Json(swal);
                }
            }
            else
            {
                ////ModelState.AddModelError("Login Failed.Please enter correct credentials")
                //ModelState.AddModelError("", data.ErrorMessage);
                swal.MessageType = false;
                swal.Header = _localization.GetString("web.Failed");
                swal.Message = _localization.GetString("web.Incorrect credentials");
                swal.SwalType = "error";
                return Json(swal);
            }

            if (_UserModel != null)
                if (_UserModel.Id > 0)
                    if (_UserModel.UserRoleName.FirstOrDefault() == UserRoleName.User)
                    {
                        if (!string.IsNullOrEmpty(model.returnUrl))
                            if (Url.IsLocalUrl(model.returnUrl))
                            {
                                swal.Url = model.returnUrl;
                                swal.MessageType = true;
                                return Json(swal);
                            }
                        // return Redirect("/" + model.returnUrl);
                    }
                    else if (_UserModel.UserRoleName.FirstOrDefault() == UserRoleName.TicketingUser)
                    {
                        if (!string.IsNullOrEmpty(model.returnUrl))
                            if (Url.IsLocalUrl(model.returnUrl))
                            {
                                swal.Url = model.returnUrl;
                                swal.MessageType = true;
                                return Json(swal);
                            }

                    }

            if (!string.IsNullOrEmpty(model.returnUrl))
                if (Url.IsLocalUrl(model.returnUrl))
                {
                    return Redirect(model.returnUrl);
                }
            swal.MessageType = true;
            return Json(swal);
        }


        [HttpGet]
        //[AllowAnonymous]
        public IActionResult UserLogin(string returnUrl = null)
        {
            if (_currentUser.CurrentUser.UserRoleName.FirstOrDefault() == "User" && _currentUser.CurrentUser.Activation==true)
                return RedirectToAction("profile", "Profile");

            if (_currentUser.CurrentUser != null)
                if (_currentUser.CurrentUser.UserRoleName.FirstOrDefault() != "Guest")
                    if (Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);
            if (_currentUser.CurrentUser.UserRoleName.FirstOrDefault() == UserRoleName.TicketingUser && _currentUser.CurrentUser.Activation == true )
                return RedirectToAction("TicketingView", "Home");


            //  else
            //  return RedirectToAction("Index", "Dashboard", new { Area = "Admin" });


            this.ViewData["ReturnUrl"] = returnUrl;
            UserLoginView model = new UserLoginView();
            model.returnUrl = returnUrl;
            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.User = null;
            var cookieName = _globalSettings.CurrentSystemSetting.CookieName;
            HttpContext.Response.Cookies.Delete(cookieName);
            return RedirectToAction("Index", "Home", new { Area = "" });
        }

        #region Log In  Using Facebook
        private Uri RediredtUri
        {
            get
            {
                var uriBuilder = new UriBuilder(new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}"));
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallback");
                return uriBuilder.Uri;
            }
        }
        [AllowAnonymous]
        public ActionResult Facebook()
        {
            var fb = new FacebookClient();
            var loginUrl = fb.GetLoginUrl(new
            {
                client_id = "148384642407735",
                client_secret = "25b570ffac6aae99ab8729f63ea3c757",
                redirect_uri = RediredtUri.AbsoluteUri,
                response_type = "code",
                scope = "email"
            });
            return Redirect(loginUrl.AbsoluteUri);
        }
        public ActionResult FacebookCallback(string code)
        {
            var fb = new FacebookClient();
            try
            {
                dynamic result = fb.Post("oauth/access_token", new
                {
                    client_id = "148384642407735",
                    client_secret = "25b570ffac6aae99ab8729f63ea3c757",
                    redirect_uri = RediredtUri.AbsoluteUri,
                    code = code
                });
                var accessToken = result.access_token;
                //Session["AccessToken"] = accessToken;
                fb.AccessToken = accessToken;
                // to get all user data from url
                dynamic me = fb.Get("me?fields=link,first_name,currency,last_name,email,gender,locale,timezone,verified,picture,age_range");
                string email = me.email;
                TempData["email"] = me.email;
                TempData["first_name"] = me.first_name;
                TempData["lastname"] = me.last_name;
                //TempData["picture"] = me.picture.data.url;
                //FormsAuthentication.SetAuthCookie(email, false);

                #region chick if User email is exist or not

                var user = _userRepository.GetUserByEmail(email);
                if (user != null)
                {
                    UserLoginView userLogIn = new UserLoginView()
                    {
                        Email = email,
                    };
                    return View("UserLogin", userLogIn);
                }
                else
                {
                    UserRegisterModel userRegister = new UserRegisterModel()
                    {
                        Email = me.email,
                        Name = me.first_name,
                    };
                    return View("RegisterUser", userRegister);
                }
                #endregion
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
        #endregion
    }
}
