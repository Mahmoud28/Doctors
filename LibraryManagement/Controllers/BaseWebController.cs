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

namespace Seagull.Core.Controllers
{
    //[Authorize, TypeFilter(typeof(CheckPlanCachKey))]
    public abstract partial class BaseWebController : Controller
    {
        /// <summary>
        /// Access denied view
        /// </summary>
        /// <returns>Access denied view</returns>
        protected ActionResult AccessDeniedView()
        {
            //return new HttpUnauthorizedResult();
            return RedirectToAction("AccessDenied", "Security", new { pageUrl = this.Request });
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
                url = Url.Action("AccessDenied", "")
            };
            return Json(data);
        }
    }
}
