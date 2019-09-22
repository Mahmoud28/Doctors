using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Seagull.Core.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Seagull.Core.Helper.Authentication
{
    public class SysAuthentication : BaseWebController
    {
        private readonly IHttpContextAccessor _accessor;

        public SysAuthentication(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }
        public bool SignIn(int UserId)
        {
            #region Login
            var claims = new List<Claim>
                    {
                        new Claim("UserId", UserId.ToString()),
                        new Claim("CustomCache", "0")
                    };
            ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "login");
            ClaimsPrincipal principal = new ClaimsPrincipal(userIdentity);
            try
            {
                _accessor.HttpContext.SignInAsync(principal);
                _accessor.HttpContext.User = principal;
            }
            catch(Exception e)
            {

            }
            #endregion
            return true;
        }
    }
}
