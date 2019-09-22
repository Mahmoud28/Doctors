using Seagull.Core.Data.Interfaces;
using Seagull.Core.Data.Repository;
using Seagull.Core.Helper;
using Seagull.Core.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Linq;
using Seagull.Core.Data;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Threading;
using Seagull.Core.ViewModel;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Seagull.Core.Helper.Caching;
using Seagull.Core.Helper.Caching;
using Seagull.Core.Areas.Admin.ViewModel;
using Seagull.Core.Data.MappingExtension;
using Seagull.Core.Data.Model;
using Seagull.Doctors.Helper.UserHelper;
using Seagull.Core.Helper.Authentication;

namespace Seagull.Core.Helper
{
    public class GlobalSettings : IGlobalSettings
    {
        private IHttpContextAccessor _accessor;
        private UserViewModel _UserCach;
        private SystemSettingViewModel _SystemSettingCach;
        private readonly IServiceProvider m_ServiceProvider;
        private readonly ICacheServicecs _cacheServicecs;
        private readonly LibraryDbContext context;
        private readonly IServiceScope _scope;
        private IConfigurationBuilder _builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
        public static IConfiguration Configuration { get; set; }
        private string _cookieName = string.Empty;
        public GlobalSettings(IHttpContextAccessor accessor, IServiceProvider serviceProvider, ICacheServicecs cacheServicecs)//, IMapper mapper)
        {
            _accessor = accessor;
            m_ServiceProvider = serviceProvider;
            _cacheServicecs = cacheServicecs;
            //_mapper = mapper;
            _scope = m_ServiceProvider.CreateScope();
            context = _scope.ServiceProvider.GetService<LibraryDbContext>();
            Configuration = _builder.Build();
            _cookieName = Configuration["System:CookieName"];
        }



        private string GetValue(ClaimsPrincipal principal, string key)
        {
            if (principal == null)
                return string.Empty;

            return principal.FindFirstValue(key);
        }

        //To Read ConnectionString from appsettings.json file
        private static string GetConnectionString()
        {
            string connectionString = Configuration["ConnectionStrings:myConString"];
            return connectionString;
        }
        public UserViewModel CurrentUser
        {
            get
            {
                int userId = 0;
                UserViewModel _user = null; 
                try
                {
                    userId = string.IsNullOrEmpty(GetValue(_accessor.HttpContext.User, "UserId")) ? 0 : int.Parse(GetValue(_accessor.HttpContext.User, "UserId"));
                    if (userId > 0)
                    {
                        _user = GetUser(userId);
                        SetCustomerCookie(userId, _user.LangId.Value);
                    }
                    else if (GetCustomerCookie().Count() > 0)
                    {
                        CookiesModel model = GetCustomerCookie().ToCookiesModel();
                        _user = GetUser(model.UserId);
                        _user.LangId = model.Lang;
                        _user.IsRtl = model.Lang == 2 ? true : false;
                        SetCustomerCookie(_user.Id, model.Lang);
                    }
                    else
                    {
                        _user = GetUser(0);
                        SetCustomerCookie(0, _user.LangId.Value);
                    }
                }
                catch (Exception e)
                {

                }
                //set EN 
                //_UserCach.IsRtl = false;
                if(_user == null)
                {

                }
                return _user;
            }
        }


        public UserViewModel GetUser(int userId)
        {
            using (var serviceScope = m_ServiceProvider.CreateScope())
            {
                using (var _tempContext = serviceScope.ServiceProvider.GetService<LibraryDbContext>())
                {
                    var _s = _scope.ServiceProvider.GetService<IUserRepository>();
                    var mapper = serviceScope.ServiceProvider.GetService<IMapper>();

                    if (userId > 0 && _tempContext.Users.Where(a => a.Id == userId).Count() > 0)
                    {
                        new SysAuthentication(_accessor).SignIn(userId);

                        return mapper.Map<User, UserViewModel>(_tempContext.Users.Where(a => a.Id == userId).Include(f => f.fk_UserRoleMap).ThenInclude(g => g.UserRole).ThenInclude(p => p.fk_UserRolePermMap).ThenInclude(h => h.Permission).FirstOrDefault());
                    }
                    else return
                            mapper.Map <User, UserViewModel> (_s.InsertGuestUser(_accessor, _scope.ServiceProvider.GetService<IUserRoleRepository>()));
                }
            }
        }
        public SystemSettingViewModel CurrentSystemSetting
        {
            get
            {
                return MappinSystemSystting();
            }
        }

        public void RefreshUser()
        {
            int userId = string.IsNullOrEmpty(GetValue(_accessor.HttpContext.User, "UserId")) ? 0 : int.Parse(GetValue(_accessor.HttpContext.User, "UserId"));
            using (var serviceScope = m_ServiceProvider.CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<LibraryDbContext>())
                {
                    // you can access your DBContext instance
                    var mapper = serviceScope.ServiceProvider.GetService<IMapper>();
                    // you can access your mapper instance
                    _UserCach = userId > 0 ? mapper.Map<User, UserViewModel>(context.Users.Where(a => a.Id == userId).FirstOrDefault()) : null;
                }
            }
        }
        public SystemSettingViewModel MappinSystemSystting()
        {
            if (_SystemSettingCach == null)
                using (var serviceScope = m_ServiceProvider.CreateScope())
                {
                    using (var context = serviceScope.ServiceProvider.GetService<LibraryDbContext>())
                    {
                        // you can access your DBContext instance
                        _SystemSettingCach = new SystemSettingMapping().ToModel(context.SystemSettings, "");
                    }
                }
            return _SystemSettingCach;
        }

        protected virtual void SetCustomerCookie(int userId, int langId = 1 , int type = 0)
        {
            if (_accessor.HttpContext?.Response == null)
                return;

            //delete current cookie value
            _accessor.HttpContext.Response.Cookies.Delete(_cookieName);

            //get date of cookie expiration
            var cookieExpires = 24 * 365; //TODO make configurable
            var cookieExpiresDate = DateTime.Now.AddHours(cookieExpires);

            //if passed guid is empty set cookie as expired
            if (userId == -1 || type > 0)
                cookieExpiresDate = DateTime.Now.AddMonths(-1);

            //set new cookie value
            var options = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                //Secure = true,
                Expires = cookieExpiresDate
            };
            // write the cookie
            var kvpCookie = new Dictionary<string, string>()
            {
                { "UserId", userId.ToString() },
                { "Lang", langId.ToString() }
            };
            var content = kvpCookie.ToLegacyCookieString();
            _accessor.HttpContext.Response.Cookies.Append(_cookieName, content, options);
            //_accessor.HttpContext.Response.Cookies.Append(cookieName, userId.ToString(), options);
        }
        protected virtual IDictionary<string, string> GetCustomerCookie()
        {
            //return .FromLegacyCookieString();
            //return _accessor.HttpContext?.Request?.Cookies[cookieName];
            var u = _accessor.HttpContext?.Request?.Cookies[_cookieName];
            var y = _accessor.HttpContext.Request.Cookies[_cookieName];
            using (var serviceScope = m_ServiceProvider.CreateScope())
            {
                return serviceScope.ServiceProvider.GetService<IHttpContextAccessor>().HttpContext?.Request?.Cookies[_cookieName].FromLegacyCookieString();
            }
        }


    }
    public static class LegacyCookieExtensions
    {
        public static IDictionary<string, string> FromLegacyCookieString(this string legacyCookie)
        {
            if (!string.IsNullOrEmpty(legacyCookie))
            {
                return legacyCookie.Split('&').Select(s => s.Split('=')).ToDictionary(kvp => kvp[0], kvp => kvp[1]);
            }

            return new Dictionary<string, string>();
        }

        public static string ToLegacyCookieString(this IDictionary<string, string> dict)
        {
            return string.Join("&", dict.Select(kvp => string.Join("=", kvp.Key, kvp.Value)));
        }
        public static CookiesModel ToCookiesModel(this IDictionary<string, string> dict)
        {
            Type type = typeof(CookiesModel);
            var obj = Activator.CreateInstance(type);

            foreach (var kv in dict)
            {
                try
                {
                    type.GetProperty(kv.Key).SetValue(obj, int.Parse(kv.Value));
                }
                catch (Exception e)
                {

                }
            }
            return (CookiesModel)obj;
        }
    }

}