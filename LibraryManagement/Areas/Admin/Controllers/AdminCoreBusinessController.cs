using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Seagull.Core.Helpers_Extensions.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Seagull.Core.Data;
using Seagull.Core.Helper.Filters;

namespace Seagull.Core.Areas.Admin.Controllers
{
    [Authorize, TypeFilter(typeof(CheckPlanCachKey)) , TypeFilter(typeof(CheckGuestUser))]
    public abstract partial class AdminCoreBusinessController : Controller
    {
        /// <summary>
        /// Access denied view
        /// </summary>
        /// <returns>Access denied view</returns>
        protected ActionResult AccessDeniedView()
        {
            //return new HttpUnauthorizedResult();
             return RedirectToAction("Index", "AccessDenied", new { pageUrl = this.Request });
        }

        /// <summary>
        /// Access denied view as json
        /// </summary>
        /// <returns>Access denied view as json</returns>
        protected JsonResult AccessDeniedJson(string msg = "")
        {
            JsonResultHelper data = new JsonResultHelper
            {
                Access = false,
                success = false,
                Msg = new List<string> { string.IsNullOrEmpty(msg) ? "AccessDenied.Description" : msg },
                 url = Url.Action("Index", "AccessDenied", new { area = "Admin" })
                // url = Url.Action("AccessDenied",  new { area = "Admin" })

            };
            return Json(data);
        }
    }
}
